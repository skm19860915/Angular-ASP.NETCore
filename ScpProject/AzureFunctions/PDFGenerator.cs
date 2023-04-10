using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Api2PdfLibrary;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Data.SqlClient;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.DTOs.Program;
using Models.Athlete;

namespace AzureFunctions
{
    public class PDFGenerator
    {
        protected const string TokenJoin = " INNER JOIN userTokens AS ut ON ut.UserId = ";
        public static SqlConnection SqlConn => new SqlConnection(Config.SqlConn);
        public static SendGridClient Client => new SendGridClient(Config.EmailAPIKey);
        public static EmailAddress FromEmail => new EmailAddress("noreply@strengthcoachpro.com", "Strength Coach Pro");
        public static Dictionary<string, string> Geta2pOptions(string Key, string Value)
        {
            return new Dictionary<string, string>() {
                { "marginTop", "1" }            ,
            { "pageSize", "Letter" },
            { "orientation", "landscape" },
            { "marginLeft", "1" }, { "marginRight", "1" },
            { "marginBottom", "1" },
                {Key,Value }
        };
        }
        [FunctionName("GeneratePDF")]
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            PrintPDFOptions data = JsonConvert.DeserializeObject<PrintPDFOptions>(requestBody);
            var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
            var userRepo = new DAL.Repositories.UserRepo(Config.SqlConn);
            var targetCoach = userRepo.Get(data.CreatedUserToken);
            if (Config.logEverything)
            {
                logRepo.LogShit(new Models.LogMessage() { Message = $"Enter old pdf", UserId = targetCoach.Id, LoggedDate = DateTime.Now, StackTrace = new System.Diagnostics.StackTrace().ToString() });
            }
            try
            {

                var programRepo = new DAL.Repositories.ProgramRepo(Config.SqlConn);
                var targetProgram = programRepo.GetProgram(data.ProgramId, data.CreatedUserToken);
                var athleteRepo = new DAL.Repositories.AthleteRepo(Config.SqlConn);
                var a2pClient = new Api2Pdf("c1db6271-1c8c-4050-be6f-1e7d3bd6410a");
                var options = Geta2pOptions("title", $"{targetProgram.Name}.pdf");
                var mergedPdfs = new List<String>();
                var html = string.Empty;


                if (data.PrintMasterPdf)
                {
                    if (!data.UseNewPdf)
                    {
                        html = GenerateMasterPDFHTML(targetProgram.Id, data.CreatedUserToken);
                        mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(html, options: options).Pdf);
                    }
                    else
                    {
                        if (targetProgram.Days.Count < 1)
                        {
                            html = GenerateMasterNewPDFHTML(targetProgram.Days, targetProgram.WeekCount, targetProgram.Name, context);
                            mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(html, options: options).Pdf);
                        }
                        else
                        {
                            var apiResponse = a2pClient.Merge(SendMultiPageMasterHTMLToAPI2Pdf(targetProgram, a2pClient, options, 1, context));
                            mergedPdfs.Add(apiResponse.Pdf);
                        }
                    }
                }
                //todo: this will no longer work as snapshots do not record what they were saved from
                var assignedProgramId = programRepo.GetLatestAssignedProgramID(data.ProgramId);
                var allAssignedAtheletes = data.PrintSelectedAthletes ? data.PrintOnlyTheseAthletes : programRepo.GetAllAthleteIdsForAssignedProgram(assignedProgramId, data.CreatedUserToken);
                if (allAssignedAtheletes.Any() && allAssignedAtheletes.Count == 1)
                {
                    var targetAthlete = athleteRepo.GetAthlete(allAssignedAtheletes.FirstOrDefault());

                    var currentAthleteProgram = new DAL.DTOs.AthleteAssignedPrograms.AssignedProgram();
                    if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
                    {
                        currentAthleteProgram = programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, data.CreatedUserToken, targetAthlete.Id);
                    }
                    else
                    {
                        currentAthleteProgram = programRepo.GetAssignedProgram(targetAthlete);
                    }

                    var athleteHtml = String.Empty;
                    if (!data.UseNewPdf)
                    {
                        athleteHtml = GenerateAthletePDFHTML(data.ProgramId, data.CreatedUserToken, targetAthlete,  programRepo);
                        mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(athleteHtml, options: options).Pdf);
                        var apiResponse = a2pClient.Merge(mergedPdfs);

                        await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetAthlete.FirstName, context);
                    }
                    else
                    {
                        if (currentAthleteProgram.Days.Count <= 1)
                        {
                            athleteHtml = GenerateAthleteNewPDFHTML(currentAthleteProgram.Days, currentAthleteProgram.WeekCount, currentAthleteProgram.Name, context);
                            mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(athleteHtml, options: options).Pdf);
                            var apiResponse = a2pClient.Merge(mergedPdfs);
                            await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetAthlete.FirstName, context);
                        }
                        else
                        {
                            var athletePFS = SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, 1, context);
                            athletePFS.AddRange(mergedPdfs);
                            var apiResponse = a2pClient.Merge(athletePFS);
                            await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetAthlete.FirstName, context);
                        }
                    }
                }
                else if (allAssignedAtheletes.Any() && allAssignedAtheletes.Count > 1)
                {

                    if (!data.UseNewPdf)
                    {
                        var tasks = new List<Task>();
                        allAssignedAtheletes.ForEach(a =>
                        {
                            var targetAthlete = athleteRepo.GetAthlete(a);
                            var athleteHtml = GenerateAthletePDFHTML(data.ProgramId, data.CreatedUserToken, targetAthlete, programRepo);
                            tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(athleteHtml, options: options).Pdf)));
                        });
                        Task.WaitAll(tasks.ToArray());

                        var apiResponse = a2pClient.Merge(mergedPdfs);

                        await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetCoach.FirstName, context);
                    }
                    else
                    {
                        var tasks = new List<Task>();
                        allAssignedAtheletes.ForEach(a =>
                        {
                            var targetAthlete = athleteRepo.GetAthlete(a);
                            var currentAthleteProgram = programRepo.GetAssignedProgram(targetAthlete);
                            if (currentAthleteProgram.Days.Count <= 1)
                            {
                                var ahtml = GenerateAthleteNewPDFHTML(currentAthleteProgram.Days, currentAthleteProgram.WeekCount, currentAthleteProgram.Name, context, targetAthlete.FirstName ?? string.Empty, targetAthlete.LastName ?? string.Empty);
                                tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(a2pClient.WkHtmlToPdf.FromHtml(ahtml, options: options).Pdf)));
                            }
                            else
                            {

                                var meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, 1, context));
                                if (string.IsNullOrEmpty(meregedAthletePDF.Pdf))//retry a failed attempt
                                {
                                    meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, 1, context));
                                }
                                if (string.IsNullOrEmpty(meregedAthletePDF.Pdf))//retry a failed attempt twice
                                {
                                    meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, 1, context));
                                }
                                tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(meregedAthletePDF.Pdf)));
                            }
                        });


                        Task.WaitAll(tasks.ToArray());

                        var apiResponse = a2pClient.Merge(mergedPdfs.Where(x => !string.IsNullOrEmpty(x)));//for some reason the api randomly doesnt return info. If any string is null
                                                                                                           //then the download link wont work.

                        await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetCoach.FirstName, context);
                    }
                }
                else
                {
                    var apiResponse = a2pClient.Merge(mergedPdfs);
                    await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetCoach.FirstName, context);
                }
            }
            catch (Exception ex)
            {
                if (Config.logEverything)
                {
                    logRepo.LogShit(new Models.LogMessage() { Message = "Fucking error " + ex.ToString(), UserId = targetCoach.Id, LoggedDate = DateTime.Now, StackTrace = new System.Diagnostics.StackTrace().ToString() });
                }
                var u = ex;
                throw;
            }
        }
        [FunctionName("GenerateAthletePDF")]
        public static async void GenerateAthletePDF([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            PrintPDFOptionsAthleteOnly data = JsonConvert.DeserializeObject<PrintPDFOptionsAthleteOnly>(requestBody);

            var programRepo = new DAL.Repositories.ProgramRepo(Config.SqlConn);
            var targetProgram = programRepo.GetProgram(data.ProgramId, data.CreatedUserToken);
            var athleteRepo = new DAL.Repositories.AthleteRepo(Config.SqlConn);
            var a2pClient = new Api2Pdf("c1db6271-1c8c-4050-be6f-1e7d3bd6410a");

            var options = new Dictionary<string, string>();
            options.Add("pageSize", "Letter");
            options.Add("orientation", "landscape");
            options.Add("marginLeft", "3");
            options.Add("marginRight", "3");
            options.Add("marginBottom", "3");
            options.Add("marginTop", "3");
            options.Add("title", $"{targetProgram.Name}.pdf");

            var userRepo = new DAL.Repositories.UserRepo(Config.SqlConn);

            var targetAthlete = athleteRepo.GetAthlete(data.AthleteId);
            var athleteHtml = GenerateAthletePDFHTML(data.ProgramId, data.CreatedUserToken, targetAthlete, programRepo);
            var pdfURL = a2pClient.WkHtmlToPdf.FromHtml(athleteHtml, options: options).Pdf;
            var targetUser = userRepo.Get(targetAthlete.AthleteUserId);

            await SendEmailAsync(pdfURL, targetProgram.Name, targetUser.Email, targetAthlete.FirstName, context);
        }
        public static async Task<Response> SendEmailAsync(string pdfURL, string programName, string toEmail, string headCoachFirstName, ExecutionContext context)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "newPDFEmail.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{HeadCoachFirstName}", headCoachFirstName).Replace("{pdfURL}", pdfURL);

            var subject = $"Your Program, {programName} Is Ready To Print";
            var to = new EmailAddress(toEmail, string.Empty);
            var plainTextContent = $"To download your Custom Strengh And Conditioning program click on the following link,  {pdfURL} . For Your safety and ours the workink link will expire in 24 hours. This is to ensure that your data spends the minimum amount of time exposed. ";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, body);
            var steve = await Client.SendEmailAsync(msg);
            if (Config.logEverything)
            {
                var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
                logRepo.LogShit(new Models.LogMessage() { Message = $"toEmail: {toEmail},fromEmail : {FromEmail}, sendEmailAsyncRepsone: {steve.StatusCode} " });
            }
            return await Task.FromResult<Response>(steve);
        }

        public static string GenerateAthleteNewPDFHTML(List<AssignedProgramDays> daysToPrint, int WeekCount, string programName, ExecutionContext context, string firstName = "", string lastName = "")
        {
            var allHtml = new List<HtmlAndItemDayPosition>();
            var finalHtml = new StringBuilder();

            var weekCount = WeekCount;

            daysToPrint.OrderBy(p => p.Position).ToList().ForEach(d =>
            {

                if (d.AssignedNotes.Any())
                {
                    //var newNoteHtml = $"<div style='width:100%;padding:5px 0px 5px 0px;border-bottom:1px solid black;'>{n.Note}</div>";

                    d.AssignedNotes.Where(w => w.ProgramDayId == d.Id).GroupBy(
                    g => g.Position,
                    g => g,
                    (key, notes) => new { key = key, notes = notes }).ToList().ForEach(n =>
                    {
                        if (n.notes.Any())
                        {
                            var noteHtml = $@"<div class='exercise - details - container'>
                                            <div class='exercise-notes'>
                                                <h4>Notes</h4>
                                                <p>{n.notes.First().NoteText}</p>
                                            </div>  
                                        </div>";

                            allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = noteHtml, ItemDayPosition = n.key });
                        }
                    });

                }
                if (d.AssignedMetrics.Any())
                {
                    d.AssignedMetrics.GroupBy(m => m.ProgramDayItemMetricId).ToList().ForEach(m =>
                    {
                        var newMetricHtml = new StringBuilder();

                        newMetricHtml.Append($@"<div class='exercise-metrics'>
                            <div class='metrics-row'>
                                <div class='metrics-exercise-name'>
                                    <h4>Metrics</h4>
                                    <strong>{m.FirstOrDefault().MetricName}</strong>
                                </div>");

                        var dispalyWeeks = m.Select(wi => wi.DisplayWeekId);

                        newMetricHtml.Append("<div class='metrics-week-values'>");

                        for (var i = 1; i <= weekCount; i++)
                        {
                            var emptySet = !dispalyWeeks.Contains(i);
                            if (emptySet)
                            {
                                newMetricHtml.Append($"<div class='metrics-dummy-space col-{weekCount}'></div>");
                            }
                            else
                            {
                                newMetricHtml.Append($@"<div class='metrics-value col-{weekCount}'><p> &nbsp; &nbsp;</p></div>");
                            }
                        }
                        newMetricHtml.Append(@"</div></div></div>");
                        allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = newMetricHtml.ToString(), ItemDayPosition = m.FirstOrDefault().Position });
                    });
                }

                d.AssignedSuperSets.ForEach(ss =>
                {
                    var newSuperSetHtml = new StringBuilder();
                    newSuperSetHtml.Append($@"<div class='exercise-info-row'>");
                    newSuperSetHtml.Append($@"<div class='exercise-title'>{ss.SuperSetDisplayTitle}</div>");
                    newSuperSetHtml.Append($@"<div class='weekly-parameter-container'>");
                    for (var i = 0; i < weekCount; i++)
                    {

                        newSuperSetHtml.Append($@"<div class='weekly-parameter col-{weekCount}'>
                                                           <div class='exercise-week-details col-6'>S</div>
                                                           <div class='exercise-week-details col-6'>R</div>
                                                           <div class='exercise-week-details col-6'>L</div>
                                                           <div class='exercise-week-details col-6'>T</div>
                                                           <div class='exercise-week-details col-6'>D</div>
                                                           <div class='exercise-week-details col-6'>RA</div>
                                                    </div>
                                                ");
                    }
                    newSuperSetHtml.Append("</div>");//closes out weekly-param,eter-container
                    newSuperSetHtml.Append("</div>");//closes out exercise-info-row

                    var orderedSet = ss.Exercises.OrderBy(sse_o => sse_o.PositionInSuperSet).ToArray();
                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append($@"<div class='exercise-notes'>
                                                <h4>Notes</h4>");
                        ss.Notes.Skip(1).ToList().ForEach(n =>
                        {
                            newSuperSetHtml.Append($@"<p>{n.Note}</p>");
                        });

                        newSuperSetHtml.Append("</div>");//closes out exercise-notes
                    }

                    for (int i = 0; i < orderedSet.Length; i++)
                    {
                        var sse = orderedSet[i];

                        if (sse.SetsAndReps.Any())
                        {

                            newSuperSetHtml.Append("<div class='exercise-details-container'>");
                            newSuperSetHtml.Append($"    <div class='exercise-names'><strong>{sse.SetsAndReps.FirstOrDefault().ExerciseName}</strong><p>{sse.SetsAndReps.FirstOrDefault().AssignedRest}</p></div>");
                            //if (sse.SetsAndReps.Count > 0 && i != orderedSet.Length - 1)
                            //{
                            //    newSuperSetHtml.Append(@"<br/>
                            //                            <div class='paired' style='clear:both'>Paired With</div>
                            //                            <br/>");
                            //}

                            newSuperSetHtml.Append(" <div class='week-values-container'>");
                            //ordering by supersetweekId because it is gaurneteed to be in order. It is an Identity column that is inserted at insert time
                            //so it is guarnteed to be smallest to largest in order of insert which is the order the user sees it in

                            newSuperSetHtml.Append($"<div class='week-values-division col-{weekCount}'>");
                            var orderedWeeks = sse.SetsAndReps.Select(s => s.SuperSetWeekId).Distinct().OrderBy(o => o).ToList();


                            var maxSetCount = 0;

                            for (int setIndex = 0; setIndex < orderedWeeks.Count; setIndex++)
                            {
                                var targetCount = sse.SetsAndReps.Where(x => x.SuperSetWeekId == orderedWeeks[setIndex]).Count();
                                if (maxSetCount < targetCount)
                                {
                                    maxSetCount = targetCount;
                                }
                            }

                            //I am doing count because the position in week and the Id could be anything. by doing count I am guarentting that i do at minimum
                            //all sets and reps. The targetSetRep != null will save us when we start running into nulls because we are skipping to much
                            //I hve no clue what I was thinking when I wrote the above
                            for (int count = 0; count <= maxSetCount; count++)
                            {
                                //so mother fucker says what!!! There can be more weeks in a program than there are weeks for sets and reps. IE
                                //I can make a 5 week program and only have testing on week 5, and weeks 1-4 are lifting weeks. So we need to go off 
                                //total weeks instead of just weeks with sets and reps

                                for (int setsRepsWeekCount = 0; setsRepsWeekCount < weekCount; setsRepsWeekCount++)
                                {
                                    newSuperSetHtml.Append($"<div class='week-values-division col-{weekCount}'>");
                                    //Guranteed to return non 0 integer. Because the ID is an identity column that starts at 1, not doing that -int max to increase space.
                                    //we might have to in the future, but we will just go to 64 bit integer
                                    var targetWeek = orderedWeeks.Skip(setsRepsWeekCount).Take(1).FirstOrDefault();

                                    if (targetWeek != 0)
                                    {
                                        //we only need to make items or empty items if there are items for that week. This if statement will stop us
                                        //from creating too many empty sets/reps
                                        //if (sse.SetsAndReps.Where(w => w.SuperSetWeekId == targetWeek).Skip(count).ToList().Count > 0)
                                        //{
                                        var targetSetRep = sse.SetsAndReps.Where(w => w.SuperSetWeekId == targetWeek).OrderBy(oss => oss.PositionInSet).Skip(count).Take(1).FirstOrDefault();
                                        if (targetSetRep != null)
                                        {
                                            newSuperSetHtml.Append(RenderOneSetRepRow(targetSetRep, false));
                                        }

                                    }
                                    else
                                    {
                                        newSuperSetHtml.Append(RenderOneSetRepRow(new AssignedSuperSetSetRep(), true));
                                    }
                                    //}
                                    //this forces the line to reset to the next line
                                    if (setsRepsWeekCount == WeekCount - 1)
                                    {
                                    }
                                    newSuperSetHtml.Append("</div>");
                                }
                            }
                            newSuperSetHtml.Append("</div>");
                        }
                    };
                    newSuperSetHtml.Append("</div>");
                    allHtml.Add(new HtmlAndItemDayPosition() { Html = newSuperSetHtml.ToString(), ItemDayPosition = ss.PositionInProgramDay, DayPosition = d.Position });
                });
            });
            var headerHtml = renderPdfHeader(firstName, lastName, programName.ToUpper(), string.Empty);// org.ProfilePicture.URL);
            finalHtml.Append($"<html><head></head><body><div class='pdf-container'>{headerHtml}<div class='pdf-body'>");
            var dayId = 1;
            daysToPrint.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                finalHtml.Append($@"<div class='pdf-day'> 
                                   <div class='pdf-exercises'>
                                   <div class='days-info-row'>
                                   <div class='day-number'>Day {dayId}</div>");
                for (var i = 1; i <= weekCount; i++)
                {

                    finalHtml.Append($@"<div class='week4a2c week-number'>Wk {i}</div>");
                }
                finalHtml.Append("</div>");

                allHtml.Where(x => x.DayPosition == d.Position).OrderBy(o => o.ItemDayPosition).ToList().ForEach(s =>
                {
                    finalHtml.Append(s.Html.ToString());
                });
                finalHtml.Append("</div>");
                finalHtml.Append("</div>");
                dayId++;
            });
            finalHtml.Append("</body></html>");
            finalHtml.Append(CSS(context));
            return finalHtml.ToString();
            //return apiResponse.Pdf;
        }
        public static string GenerateMasterNewPDFHTML(List<ProgramDay> daysToPrint, int WeekCount, string programName, ExecutionContext context)
        {
            var allHtml = new List<HtmlAndItemDayPosition>();
            var finalHtml = new StringBuilder();

            var weekCount = WeekCount;
            daysToPrint.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                d.Notes.ForEach(n =>
                {
                    var noteHtml = $"<div class='left-day-notes'><h4>Notes</h4><p>{n.Note}</p></div>";
                    allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = noteHtml, ItemDayPosition = n.Position });
                });
                d.Metrics.ForEach(m =>
                {
                    var newMetricHtml = new StringBuilder();
                    newMetricHtml.Append($@"<div class='left-day-metrics'><h4>Metrics</h4>");
                    newMetricHtml.Append($@"<div class='exercise-metrics'><div class='metrics-names'><p>{m.Metric.Name}</p></div>");
                    for (var i = 1; i < weekCount; i++)
                    {
                        var emptySet = !m.DisplayWeeks.Contains(i);
                        if (emptySet)
                        {
                            newMetricHtml.Append("<div class='week4a2c metrics-values' style='border:1px solid white'>");
                        }
                        else
                        {
                            newMetricHtml.Append("<div class='week4a2c metrics-values'>");
                        }

                        newMetricHtml.Append($@"<p>&nbsp;&nbsp;</p>");
                        newMetricHtml.Append("</div>");
                    }
                    newMetricHtml.Append("</div>");
                    newMetricHtml.Append("</div>");
                    allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = newMetricHtml.ToString(), ItemDayPosition = m.Position });
                });

                d.SuperSets.ForEach(ss =>
                {
                    var newSuperSetHtml = new StringBuilder();
                    newSuperSetHtml.Append($@"<div class='exercise-info-row'><div class='exercise-title'>{ss.SuperSetDisplayTitle}</div>");
                    for (var i = 0; i < weekCount; i++)
                    {
                        newSuperSetHtml.Append(@"<div class='week4a2c'>
                            <div class='a week-details'>S</div>
                            <div class='a week-details'>R</div>
                            <div class='a week-details'>L</div>
                            <div class='b week-details'>T</div>
                            <div class='c week-details'>D</div>
                            <div class='a week-details'>RA</div>
                        </div>");
                    }
                    newSuperSetHtml.Append("</div>");
                    newSuperSetHtml.Append("<div class='mockTable'>");

                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append($"<div class='left-day-notes'> <div class='exercise-name'>Notes</div><p>{ss.Notes.First().Note}</p>");
                        ss.Notes.Skip(1).ToList().ForEach(n => { newSuperSetHtml.Append($@"<div class='exercise-name'>&nbsp;</div><p>{n.Note}</p>"); });
                        newSuperSetHtml.Append("</div>");
                    }
                    var orderedSet = ss.Exercises.OrderBy(sse_o => sse_o.Position).ToArray();
                    for (int i = 0; i < orderedSet.Length; i++)
                    {
                        var sse = orderedSet[i];

                        newSuperSetHtml.Append("<div style='clear: both'></div>");
                        newSuperSetHtml.Append("<div>");
                        newSuperSetHtml.Append($"<div class='exercise-name'>{sse.Name}");
                        if (i >= 0 && i != orderedSet.Length - 1)
                        {
                            newSuperSetHtml.Append(@"<br/>
                                                        <div class='paired' style='clear:both'>Paired With</div>
                                                        <br/>");
                        }
                        newSuperSetHtml.Append("</div>");
                        newSuperSetHtml.Append("</div>");

                        newSuperSetHtml.Append("<div class='setsAndRepsWeekContainer'>");
                        var max = 0;
                        sse.Weeks.ForEach(s =>
                        {
                            if (s.SetsAndReps.Count > max)
                            {
                                max = s.SetsAndReps.Count;
                            }
                        });

                        for (int count = 0; count <= max; count++)
                        {

                            sse.Weeks.OrderBy(w => w.Position).ToList().ForEach(we =>
                           {
                               var targetSetRep = we.SetsAndReps.OrderBy(se => se.Position).Skip(count).Take(1).FirstOrDefault();
                               if (targetSetRep != null)
                               {
                                   newSuperSetHtml.Append(RenderOneMasterSetRepRow(targetSetRep));
                               }
                           });
                        }
                        newSuperSetHtml.Append("</div>");
                    }
                    newSuperSetHtml.Append("</div>");
                    allHtml.Add(new HtmlAndItemDayPosition() { Html = newSuperSetHtml.ToString(), ItemDayPosition = ss.Position, DayPosition = d.Position });
                });
            });

            var headerHtml = renderPdfHeader("Master", "Master", programName.ToUpper(), string.Empty);// org.ProfilePicture.URL);
            finalHtml.Append($"<html><head></head><body><div class='pdf-container'>{headerHtml}<div class='pdf-body'>");
            var dayId = 1;
            daysToPrint.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                finalHtml.Append($@"<div class='pdf-day'> 
                                   <div class='pdf-exercises'>
                                   <div class='days-info-row'>
                                   <div class='day-number'>Day {dayId}</div>");
                for (var i = 1; i <= weekCount; i++)
                {
                    finalHtml.Append($@"<div class='week4a2c week-number'>Wk {i}</div>");
                }
                finalHtml.Append("</div>");
                allHtml.Where(x => x.DayPosition == d.Position).OrderBy(o => o.ItemDayPosition).ToList().ForEach(s =>
                {
                    finalHtml.Append(s.Html.ToString());
                });
                finalHtml.Append("</div>");
                finalHtml.Append("</div>");
                dayId++;
            });
            finalHtml.Append("</body></html>");
            finalHtml.Append(CSS(context));
            return finalHtml.ToString();
            //return apiResponse.Pdf;
        }
        public static string GenerateMasterPDFHTML(int programId, Guid createdUserToken)
        {

            var programRepo = new DAL.Repositories.ProgramRepo(Config.SqlConn);
            var allExercise = new DAL.Repositories.ExerciseRepo(Config.SqlConn).GetAllExercises(createdUserToken);
            var program = programRepo.GetProgram(programId, createdUserToken);
            var totalWeeks = program.WeekCount;
            var allHtml = new List<HtmlAndItemDayPosition>();
            var finalHtml = new StringBuilder();
            var assignedProgramId = programRepo.GetLatestAssignedProgramID(programId);
            var allAssignedAtheltes = new DAL.Repositories.ProgramRepo(Config.SqlConn).GetAllAthleteIdsForAssignedProgram(assignedProgramId, createdUserToken);




            program.Days.OrderBy(p => p.Position).ToList().ForEach(d =>
            {

                d.Notes.ForEach(n =>
                {
                    var newNoteHtml = $"<div style='width:100%;padding:5px 0px 5px 0px;border-bottom:2px solid black;border-top:1px solid black;'>{n.Note}</div>";
                    allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = newNoteHtml, ItemDayPosition = n.Position });
                });
                d.Metrics.ForEach(m =>
                {
                    var newMetricHtml = new StringBuilder();
                    newMetricHtml.Append(" <div class='c'>");
                    newMetricHtml.Append(GetExerciseColumnHeader(m.Metric.Name));

                    for (var i = 1; i < program.WeekCount; i++)
                    {
                        newMetricHtml.Append(" <div class='g'>");
                        if (m.DisplayWeeks.Contains(i))
                        {
                            newMetricHtml.Append($@"<div class='r'>&nbsp;</div>");
                        }
                        else
                        {
                            newMetricHtml.Append($@"<div class='f'>&nbsp;</div>");
                        }
                        newMetricHtml.Append(" </div> ");
                    }
                    newMetricHtml.Append(" </div>");
                    allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = newMetricHtml.ToString(), ItemDayPosition = m.Position });
                });
                d.Exercises.ForEach(e =>
                {
                    var newExerciseHtml = new StringBuilder();
                    newExerciseHtml.Append(" <div class='c'>");
                    newExerciseHtml.Append(GetExerciseColumnHeader(e.Exercise.Name));
                    e.Weeks.OrderBy(s => s.Position).ToList().ForEach(w =>
                    {

                        newExerciseHtml.Append($@"<div class='g'>");
                        w.SetsAndReps.OrderBy(sOrder => sOrder.Position).ToList().ForEach(setRep =>
                        {
                            newExerciseHtml.Append(GetSetsRepsHtml(setRep.Sets, setRep.Reps, setRep.Weight));
                        });
                        newExerciseHtml.Append("</div>");
                    });
                    newExerciseHtml.Append("</div>");
                    allHtml.Add(new HtmlAndItemDayPosition() { Html = newExerciseHtml.ToString(), ItemDayPosition = e.Position, DayPosition = d.Position });
                });
                d.SuperSets.ForEach(ss =>
                {
                    var newSuperSetHtml = new StringBuilder();
                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append("  <div> ");
                    }
                    ss.Notes.ForEach(n => { newSuperSetHtml.Append($@"<div style='margin-left:10px;'>{n.Note} </div>"); });
                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append("  </div> ");
                    }
                    var orderedSet = ss.Exercises.OrderBy(sse_o => sse_o.Position).ToArray();
                    for (int i = 0; i < orderedSet.Length; i++)
                    {
                        var sse = orderedSet[i];
                        if (i != 0)
                        {
                            newSuperSetHtml.Append($@"<div class='d'>{ss.SuperSetDisplayTitle}</div>");
                        }

                        if (i == orderedSet.Length - 1)
                        {
                            newSuperSetHtml.Append(" <div class='c'>");
                        }
                        else
                        {
                            newSuperSetHtml.Append(" <div class='c k'>");
                        }

                        var targetExercise = allExercise.FirstOrDefault(ex => ex.Id == sse.ExerciseId);
                        newSuperSetHtml.Append(GetExerciseColumnHeader(targetExercise.Name));
                        sse.Weeks.OrderBy(w => w.Position).ToList().ForEach(we =>
                        {
                            newSuperSetHtml.Append($@"<div class='g'>");

                            we.SetsAndReps.OrderBy(se => se.Position).ToList().ForEach(wes =>
                            {

                                newSuperSetHtml.Append(GetSetsRepsHtml(
                                  wes.Sets.HasValue ? wes.Sets.Value : 0,
                                  wes.Reps.HasValue ? wes.Reps.Value : 0,
                                  wes.Weight.HasValue ? wes.Weight.Value : 0.0));

                            });

                            newSuperSetHtml.Append("</div>");
                        });
                        newSuperSetHtml.Append("</div>");
                    }
                    allHtml.Add(new HtmlAndItemDayPosition() { Html = newSuperSetHtml.ToString(), ItemDayPosition = ss.Position, DayPosition = d.Position });
                });
            });

            finalHtml.Append($"<html><head></head><body><div class='m'>MASTER</div><div class='pn'>{program.Name.ToUpper()}</div><div class='a'>");
            program.Days.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                finalHtml.Append($@"<div class='e'><div class='i'>Day {d.Position }</div>");
                finalHtml.Append("<div class='b' style='height:30px'>");
                finalHtml.Append($@"<div class='f'>&nbsp;</div>");
                for (var i = 1; i <= program.WeekCount; i++)
                {
                    finalHtml.Append($@"<div class='f'>WK {i}<br/>{GenerateSRWHeader()} </div>");
                }
                finalHtml.Append("</div>");
                allHtml.Where(x => x.DayPosition == d.Position).OrderBy(o => o.ItemDayPosition).ToList().ForEach(s =>
                {
                    finalHtml.Append(s.Html.ToString());
                });
                finalHtml.Append("</div>");
            });
            finalHtml.Append("</div></body></html>");
            finalHtml.Append(OldCSS(program.WeekCount, program.Days.Count));

            return finalHtml.ToString();
            //return apiResponse.Pdf;
        }
        private static string GetSetsRepsHtml(int sets, int reps, double weight)
        {
            string displayWeight = weight == 0 ? "&nbsp; " : weight.ToString();
            string displaySets = sets == 0 ? "&nbsp;" : sets.ToString();
            string displayReps = reps == 0 ? "&nbsp;" : reps.ToString();
            return $@"<div class='h'>{displaySets}</div><div class='h l'>{displayReps}</div><div class='h'>{displayWeight}</div><div class='z'></div>";
        }
        private static string GetExerciseColumnHeader(string exerciseName)
        {
            return $"<div class='j '>{exerciseName}</div>";
        }
        private static string CSS(ExecutionContext context)
        {
            var ret = @"<style type=""text/css"">";

            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "styles.css")))
            {
                ret += reader.ReadToEnd();
            }
            ret += "</style>";
            return ret;
        }
        public static string GenerateAthletePDFHTML(int programId, Guid createdUserToken, Athlete targetAthlete,  DAL.Repositories.ProgramRepo programRepo)
        {
            var allHtml = new List<HtmlAndItemDayPosition>();
            var finalHtml = new StringBuilder();
            var firstName = string.IsNullOrEmpty(targetAthlete.FirstName) ? string.Empty : targetAthlete.FirstName;
            var lastName = string.IsNullOrEmpty(targetAthlete.LastName) ? string.Empty : targetAthlete.LastName;
            var assignedProgram = new DAL.DTOs.AthleteAssignedPrograms.AssignedProgram();
            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                assignedProgram = programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, createdUserToken, targetAthlete.Id);
            }
            else
            {
                assignedProgram = programRepo.GetAssignedProgram(targetAthlete);
            }
            var totalWeeks = assignedProgram.WeekCount;

            assignedProgram.Days.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                if (d.AssignedNotes.Any())
                {
                    //var newNoteHtml = $"<div style='width:100%;padding:5px 0px 5px 0px;border-bottom:1px solid black;'>{n.Note}</div>";

                    d.AssignedNotes.Where(w => w.ProgramDayId == d.Id).GroupBy(
                g => g.Position,
                g => g,
                (key, notes) => new { key = key, notes = notes }).ToList().ForEach(n =>
                {
                    if (n.notes.Any())
                    {
                        var noteHtml = $"<div style='width:100%;padding:5px 0px 5px 0px;border-bottom:2px solid black;border-top:1px solid black;'>{n.notes.First().NoteText}</div>";
                        allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = noteHtml, ItemDayPosition = n.key });
                    }
                });

                }
                if (d.AssignedExercises.Any())
                {
                    d.AssignedExercises.ForEach(ex =>
                    {
                        var newExerciseHtml = new StringBuilder();
                        newExerciseHtml.Append(" <div class='c'>");

                        if (ex.AssignedSetsReps.Any())
                        {
                            newExerciseHtml.Append(GetExerciseColumnHeader(ex.AssignedSetsReps.FirstOrDefault().ExerciseName));
                            ex.AssignedSetsReps.GroupBy(w => w.SetWeekId, w => w, (k, groupSetsReps) => new
                            {
                                WeekId = k,
                                SetsAndRepsForThatWeek = groupSetsReps
                            }).OrderBy(o => o.WeekId).ToList().ForEach(z =>
                            {
                                newExerciseHtml.Append($@"<div class='g'>");
                                z.SetsAndRepsForThatWeek.OrderBy(y => y.PositionInSet).ToList().ForEach(fuck =>
                                {
                                    double computedWeight = fuck.AssignedWorkoutWeight > 0 ? fuck.AssignedWorkoutWeight : 0;
                                    if (computedWeight == 0 && fuck.PercentMaxCalcSubPercent.HasValue && fuck.PercentMaxCalcSubPercent > 0)
                                    {
                                        computedWeight = fuck.PercentMaxCalcSubPercent.Value;
                                    }
                                    else if (computedWeight == 0 && fuck.PercentMaxCalc.HasValue && fuck.PercentMaxCalc > 0)
                                    {
                                        computedWeight = fuck.PercentMaxCalc.Value;
                                    }

                                    newExerciseHtml.Append(GetSetsRepsHtml(fuck.AssignedWorkoutSets, fuck.AssignedWorkoutReps, (int)computedWeight));

                                });
                                newExerciseHtml.Append("</div>");
                            });
                        }
                        newExerciseHtml.Append("</div>");
                        allHtml.Add(new HtmlAndItemDayPosition() { Html = newExerciseHtml.ToString(), ItemDayPosition = ex.Position, DayPosition = d.Position });
                    });
                }
                if (d.AssignedMetrics.Any())
                {
                    d.AssignedMetrics.GroupBy(m => m.ProgramDayItemMetricId).ToList().ForEach(m =>
                    {
                        var newMetricHtml = new StringBuilder();
                        newMetricHtml.Append(" <div class='c'>");
                        newMetricHtml.Append(GetExerciseColumnHeader(m.FirstOrDefault().MetricName));
                        var dispalyWeeks = m.Select(wi => wi.DisplayWeekId);

                        for (var i = 1; i < assignedProgram.WeekCount; i++)
                        {
                            newMetricHtml.Append(" <div class='g'>");
                            if (dispalyWeeks.Contains(i))
                            {
                                newMetricHtml.Append($@"<div class='r'>&nbsp;</div>");
                            }
                            else
                            {
                                newMetricHtml.Append($@"<div class='f'>&nbsp;</div>");
                            }
                            newMetricHtml.Append(" </div> ");
                        }

                        newMetricHtml.Append(" </div>");
                        allHtml.Add(new HtmlAndItemDayPosition() { DayPosition = d.Position, Html = newMetricHtml.ToString(), ItemDayPosition = m.FirstOrDefault().Position });
                    });
                }
                d.AssignedSuperSets.ForEach(ss =>
                {
                    var newSuperSetHtml = new StringBuilder();
                    var orderedSet = ss.Exercises.OrderBy(sse_o => sse_o.PositionInSuperSet).ToArray();
                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append("  <div> ");
                    }
                    ss.Notes.ForEach(n => { newSuperSetHtml.Append($@"<div style='margin-left:10px;text-align:left'>{n.Note} </div>"); });
                    if (ss.Notes.Any())
                    {
                        newSuperSetHtml.Append("  </div> ");
                    }
                    for (int i = 0; i < orderedSet.Length; i++)
                    {
                        var sse = orderedSet[i];
                        if (i != 0)
                        {
                            newSuperSetHtml.Append($@"<div class='d'>{ss.SuperSetDisplayTitle}</div>");
                        }

                        if (i == orderedSet.Length - 1)
                        {
                            newSuperSetHtml.Append(" <div class='c'>");
                        }
                        else
                        {
                            newSuperSetHtml.Append(" <div class='c k'>");
                        }

                        if (sse.SetsAndReps.Any())
                        {
                            newSuperSetHtml.Append(GetExerciseColumnHeader(sse.SetsAndReps.FirstOrDefault().ExerciseName));
                            var orderedWeeks = sse.SetsAndReps.Select(s => s.SuperSetWeekId).Distinct().OrderBy(o => o);
                            orderedWeeks.ToList().ForEach(o =>
                            {
                                newSuperSetHtml.Append($@"<div class='g'>");

                                sse.SetsAndReps.Where(w => w.SuperSetWeekId == o).OrderBy(oss => oss.PositionInSet).ToList().ForEach(x =>
                                {
                                    var assignedWorkoutWeight = x.AssignedWorkoutWeight.HasValue ? x.AssignedWorkoutWeight.Value : 0.0;
                                    var assignedWorkoutSets = x.AssignedWorkoutSets.HasValue ? x.AssignedWorkoutSets.Value : 0;
                                    var assignedWorkoutReps = x.AssignedWorkoutReps.HasValue ? x.AssignedWorkoutReps.Value : 0;

                                    double computedWeight = assignedWorkoutWeight > 0.0 ? assignedWorkoutWeight : 0.0;
                                    if (computedWeight == 0 && x.PercentMaxCalcSubPercent.HasValue)
                                    {
                                        computedWeight = x.PercentMaxCalcSubPercent.Value;
                                    }
                                    else if (computedWeight == 0 && x.PercentMaxCalc.HasValue)
                                    {
                                        computedWeight = x.PercentMaxCalc.Value;
                                    }

                                    newSuperSetHtml.Append(GetSetsRepsHtml(assignedWorkoutSets, assignedWorkoutReps, (int)computedWeight));
                                });
                                newSuperSetHtml.Append("</div>");
                            });
                        }
                        newSuperSetHtml.Append("</div>");
                    };
                    allHtml.Add(new HtmlAndItemDayPosition() { Html = newSuperSetHtml.ToString(), ItemDayPosition = ss.PositionInProgramDay, DayPosition = d.Position });
                });
            });
            finalHtml.Append($"<html><head></head><body><div class='m'>{firstName.ToUpper()}&nbsp;&nbsp;&nbsp;{lastName.ToUpper()}</div><div class='pn'>{assignedProgram.Name.ToUpper()}</div><div class='a'>");
            assignedProgram.Days.OrderBy(p => p.Position).ToList().ForEach(d =>
            {
                finalHtml.Append($@"<div class='e'><div class='i'>Day {d.Position}</div>");
                finalHtml.Append("<div class='b' style='height:30px;'>");
                finalHtml.Append($@"<div class='f'>&nbsp;</div>");
                for (var i = 1; i <= assignedProgram.WeekCount; i++)
                {
                    finalHtml.Append($@"<div class='f'>WK {i}<br/>{GenerateSRWHeader()} </div>");
                }
                finalHtml.Append("</div>");
                //finalHtml.Append("<div class='g'");
                //finalHtml.Append("</div>");
                allHtml.Where(x => x.DayPosition == d.Position).OrderBy(o => o.ItemDayPosition).ToList().ForEach(s =>
        {
            finalHtml.Append(s.Html.ToString());
        });
                finalHtml.Append("</div>");
            });
            finalHtml.Append("</div></body></html>");
            finalHtml.Append(OldCSS(assignedProgram.WeekCount, assignedProgram.Days.Count));
            return finalHtml.ToString();
            //return apiResponse.Pdf;
        }
        public static string renderPdfHeader(string athleteFirstName, string athleteSecondName, string programName, string logoURL)
        {
            var ret = $@"<div class='pdf-header'>
            <div class='pdf-logo'>
                {logoURL}
            </div>
            <div class='pdf-athlete-name'>
                {athleteFirstName}&nbsp;{athleteSecondName}
            </div>
                        <div class='pdf-athlete-details'>
                <div class='pdf-athlete-smallname'>
                {athleteFirstName}&nbsp;{athleteSecondName}
                </div>
                <div class='pdf-athlete-program'>
                    {programName}
                </div>
            </div>
        </div>";

            return ret;


        }

        public static string RenderOneMasterSetRepRow(ProgramDaySuperSet_Sets setRepToRender)
        {
            var row = new StringBuilder();

            var setData = setRepToRender.Sets.HasValue ? setRepToRender.Sets.Value : 0;
            var setValue = setData != 0 ? setData.ToString() : "&nbsp;";
            row.Append($"<div class='week-value col-6'>{setValue}</div>");

            var SetRepdata = setRepToRender.Reps.HasValue ? setRepToRender.Reps : 0;
            var SetRepvalue = SetRepdata != 0 ? SetRepdata.ToString() : "&nbsp;";
            row.Append($"<div class='week-value col-6'>{SetRepvalue}</div>");

            var Weightvalue = string.Empty;
            if (!setRepToRender.Weight.HasValue || setRepToRender.Weight.Value == 0)
            {
                Weightvalue = "&nbsp;";
            }
            else
            {
                Weightvalue = setRepToRender.Weight.ToString();
            }


            row.Append($"<div class='week-value col-6'>{Weightvalue}</div>");

            var minutes = setRepToRender.Minutes.HasValue ? setRepToRender.Minutes.Value : 0;
            var seconds = setRepToRender.Seconds.HasValue ? setRepToRender.Seconds : 0;

            var Timevalue = string.Empty;
            if (minutes != 0)
            {
                Timevalue += $"{minutes}m";
            }
            if (seconds != 0)
            {
                Timevalue += $"{seconds}s";
            }
            row.Append($"<div class='week-value col-6'>{(Timevalue.Length == 0 ? "&nbsp;" : Timevalue)}</div>");

            var Distancevalue = !String.IsNullOrEmpty(setRepToRender.Distance) ? setRepToRender.Distance : "&nbsp;";
            row.Append($"<div class='week-value col-6'>{Distancevalue}</div>");

            var repsAchieved = "<hr>";
            if (setRepToRender.RepsAchieved.HasValue)
            {
                repsAchieved = setRepToRender.RepsAchieved.Value ? "&nbsp;" : "<hr>";
            }
            row.Append($"<div class='week-value col-6'>{repsAchieved}</div>");

            return row.ToString();
        }

        public static string RenderOneSetRepRow(AssignedSuperSetSetRep setRepToRender, bool isEmptySet)
        {
            var row = new StringBuilder();

            var setData = setRepToRender.AssignedWorkoutSets.HasValue ? setRepToRender.AssignedWorkoutSets.Value : 0;
            var setValue = setData != 0 ? setData.ToString() : "&nbsp;";
            row.Append($"<div class='week-value col-6'>{setValue}</div>");

            var SetRepdata = setRepToRender.AssignedWorkoutReps.HasValue ? setRepToRender.AssignedWorkoutReps : 0;
            var SetRepvalue = SetRepdata != 0 ? SetRepdata.ToString() : "&nbsp;";
            row.Append($"<div class=week-value col-6'>{SetRepvalue}</div>");

            var weightNumber = 0.0;
            if (setRepToRender.PercentMaxCalcSubPercent.HasValue)
            {
                weightNumber = setRepToRender.PercentMaxCalcSubPercent.Value;
            }
            else if (setRepToRender.PercentMaxCalc.HasValue)
            {
                weightNumber = setRepToRender.PercentMaxCalc.Value;
            }
            else if (setRepToRender.AssignedWorkoutWeight.HasValue)
            {
                weightNumber = setRepToRender.AssignedWorkoutWeight.Value;
            }
            var Weightvalue = weightNumber == 0 ? "&nbsp;" : weightNumber.ToString();
            row.Append($"<div class='week-value col-6'>{Weightvalue}</div>");

            var minutes = setRepToRender.AssignedWorkoutMinutes.HasValue ? setRepToRender.AssignedWorkoutMinutes.Value : 0;
            var seconds = setRepToRender.AssignedWorkoutSeconds.HasValue ? setRepToRender.AssignedWorkoutSeconds : 0;

            var Timevalue = string.Empty;
            if (minutes != 0)
            {
                Timevalue += $"{minutes}m";
            }
            if (seconds != 0)
            {
                Timevalue += $"{seconds}s";
            }
            row.Append($"<div class='week-value col-6'>{(Timevalue.Length == 0 ? "&nbsp;" : Timevalue)}</div>");

            var Distancevalue = !String.IsNullOrEmpty(setRepToRender.AssignedWorkoutDistance) ? setRepToRender.AssignedWorkoutDistance : "&nbsp;";
            row.Append($"<div class='week-value col-6'>{Distancevalue}</div>");

            if (isEmptySet)
            {
                row.Append($"<div class='week-value col-6'>&nbsp;</div>");
            }
            else
            {
                row.Append($"<div class='week-value col-6'>{(setRepToRender.RepsAchieved ? "&nbsp;" : "<hr>")}</div>");
            }


            return row.ToString();
        }
        private static string GenerateSRWHeader()
        {
            return $@"<div class='h srw '>S</div><div class='h l srw'>R</div><div class='h srw'>W</div>";
        }
        private static string OldCSS(int weekCount, int dayCount)
        {
            var fontSize = 0;
            switch (weekCount)
            {
                case 4:
                    fontSize = 12;
                    break;
                case 3:
                    fontSize = 13;
                    break;
                case 2:
                    fontSize = 14;
                    break;
                case 1:
                    fontSize = 14;
                    break;
                default:
                    fontSize = 12;
                    break;
            }
            return @"<style type='text/css'>.srw {border-bottom:0px solid black !important} .pn{ width:100%; text-align:center;height:3%; font-size:14px} .m{width:100%;text-align:center; height:5%; font-size:30px;}.r {width:80%; border-bottom:1px solid black} .f {text-align:left} .d{text-align:center;} .z{clear:both} body{font-size:" + fontSize + "px;}.a .b, .c, .d, body{width: 100%;}.c{border-right: 1px solid black;padding:10px 0 10px 0;}.e{width: " + ((100 / dayCount) - 1) + "%; float: left;border:1px solid black; height:92%;margin-right:5px; border-radius:5px}.i{ text-align: center;font-size:18px; border-bottom:1px solid black}.f, .j, .g{text-align:center; width: " + (100 / (weekCount + 1)) + "%; min-width: " + (100 / (weekCount + 1)) + "%;}.j{ float: left;}.b{border-bottom: 1px solid black; height: 15px;}.c{clear: both; border-bottom: 2px solid black; overflow: auto;text-align:center;}.f, .g{float: left;}.h, .setsRepsHeader{width: 30%; float: left; text-align: center;}.h{border-bottom: 1px solid black;}.k{border-bottom: 0px solid black;}.k:last-child{border-bottom: 1px solid black;}.l{margin: 0% 2% 0% 2%;}</style>";
        }
        public static List<String> SendMultiPageMasterHTMLToAPI2Pdf(Program targetProgram, Api2Pdf api2pdfClient, Dictionary<string, string> options, int dayCount, ExecutionContext context)
        {
            var tasks = new List<Task>();
            var mergedPdfs = new List<String>();

            var currentMastgerProgram = targetProgram;
            //this bullshit forces that only two days will be printed on 1 page. so a 4 day program will have to be printed on two pages
            //a day program will be printed on two pages.Since api2pdf doesnt take a day option. We need to simulate two different html pages
            //if we want to be sure that we have a completely new page to work with
            for (int i = 0; i < Math.Ceiling(((double)currentMastgerProgram.Days.Count()) / dayCount); i++)
            {
                var targetPage = GenerateMasterNewPDFHTML(currentMastgerProgram.Days.Skip(i * dayCount).Take(dayCount).ToList(), currentMastgerProgram.WeekCount, currentMastgerProgram.Name, context);
                tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(api2pdfClient.WkHtmlToPdf.FromHtml(targetPage, options: options).Pdf)));
            }
            Task.WaitAll(tasks.ToArray());
            return mergedPdfs;

        }
        public static List<String> SendMultiPageAthleteHTMLToAPI2Pdf(AssignedProgram targetProgram, Api2Pdf api2pdfClient, Dictionary<string, string> options, Athlete targetAthlete, int dayCount, ExecutionContext context)
        {
            var tasks = new List<Task>();
            var mergedPdfs = new List<String>();

            var currentAthleteProgram = targetProgram;
            //this bullshit forces that only two days will be printed on 1 page. so a 4 day program will have to be printed on two pages
            //a day program will be printed on two pages.Since api2pdf doesnt take a day option. We need to simulate two different html pages
            //if we want to be sure that we have a completely new page to work with
            for (int i = 0; i < Math.Ceiling(((double)currentAthleteProgram.Days.Count()) / dayCount); i++)
            {
                var targetPage = GenerateAthleteNewPDFHTML(currentAthleteProgram.Days.Skip(i * dayCount).Take(dayCount).ToList(), currentAthleteProgram.WeekCount, currentAthleteProgram.Name, context, targetAthlete.FirstName ?? string.Empty, targetAthlete.LastName ?? string.Empty);
                tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(api2pdfClient.WkHtmlToPdf.FromHtml(targetPage, options: options).Pdf)));
            }
            Task.WaitAll(tasks.ToArray());
            return mergedPdfs;

        }
    }

    public class HtmlAndItemDayPosition
    {
        public string Html { get; set; }
        public int ItemDayPosition { get; set; }
        public int DayPosition { get; set; }
    }
    public class PrintPDFOptions
    {
        public int ProgramId { get; set; }
        public Guid CreatedUserToken { get; set; }
        public bool PrintMasterPdf { get; set; }
        public List<int> PrintOnlyTheseAthletes { get; set; }
        public bool PrintSelectedAthletes { get; set; }

        public bool UseNewPdf { get; set; }

    }
    public class PrintPDFOptionsAthleteOnly
    {
        public int ProgramId { get; set; }
        public int AthleteId { get; set; }
        public Guid CreatedUserToken { get; set; }
    }
}


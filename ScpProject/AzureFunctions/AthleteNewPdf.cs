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
using Models.Athlete;

namespace AzureFunctions
{
    public class AthleteNewPdf
    {

        protected const string TokenJoin = " INNER JOIN userTokens AS ut ON ut.UserId = ";
        public static SqlConnection SqlConn => new SqlConnection(Config.SqlConn);
        public static SendGridClient Client => new SendGridClient(Config.EmailAPIKey);
        public static EmailAddress FromEmail => new EmailAddress("noreply@strengthcoachpro.com", "Strength Coach Pro");
        private const int DaysToPrint = 2;
        public static Dictionary<string, string> Geta2pOptions(string Key, string Value)
        {
            return new Dictionary<string, string>() {
            { "pageSize", "Letter" },
            { "landscape", "true" },
                {Key,Value }
        };
        }

        //public static Dictionary<string, string> Geta2pOptions(string Key, string Value)
        //{
        //    return new Dictionary<string, string>() {
        //        { "marginTop", "1" }            ,
        //    { "pageSize", "Letter" },
        //    { "orientation", "landscape" },
        //    { "marginLeft", "1" }, { "marginRight", "1" },
        //    { "marginBottom", "1" },
        //        {Key,Value }
        //};
        //}

        [FunctionName("AthleteNewPdf")]
        public static async void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            PrintPDFOptions data = JsonConvert.DeserializeObject<PrintPDFOptions>(requestBody);
            var logRepo = new DAL.Repositories.LogRepo(Config.SqlConn);
            var userRepo = new DAL.Repositories.UserRepo(Config.SqlConn);
            var targetCoach = userRepo.Get(data.CreatedUserToken);
            if (Config.logEverything)
            {
                logRepo.LogShit(new Models.LogMessage() { Message = "Enter AthleteNewPdf", UserId = targetCoach.Id, LoggedDate = DateTime.Now, StackTrace = new System.Diagnostics.StackTrace().ToString() });
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
                    if (currentAthleteProgram.Days.Count <= 1)
                    {

                        athleteHtml = GenerateAthleteNewPDFHTML(currentAthleteProgram.Days, currentAthleteProgram.WeekCount, currentAthleteProgram.Name, context, targetAthlete.FirstName ?? string.Empty, targetAthlete.LastName ?? string.Empty);
                        mergedPdfs.Add(a2pClient.HeadlessChrome.FromHtml(athleteHtml, options: options).Pdf);
                        var apiResponse = a2pClient.Merge(mergedPdfs);
                        await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetAthlete.FirstName, context);
                    }
                    else
                    {
                        var athletePFS = SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, DaysToPrint, context);
                        athletePFS.AddRange(mergedPdfs);
                        var apiResponse = a2pClient.Merge(athletePFS);
                        await SendEmailAsync(apiResponse.Pdf, targetProgram.Name, targetCoach.Email, targetAthlete.FirstName, context);
                    }
                }
                else if (allAssignedAtheletes.Any() && allAssignedAtheletes.Count > 1)
                {

                    var tasks = new List<Task>();
                    allAssignedAtheletes.ForEach(a =>
                    {
                        var targetAthlete = athleteRepo.GetAthlete(a);
                        var currentAthleteProgram = new DAL.DTOs.AthleteAssignedPrograms.AssignedProgram();
                        if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
                        {
                            currentAthleteProgram = programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, data.CreatedUserToken, targetAthlete.Id);
                        }
                        else
                        {
                            currentAthleteProgram = programRepo.GetAssignedProgram(targetAthlete);
                        }
                        if (currentAthleteProgram.Days.Count <= 1)
                        {
                            var ahtml = GenerateAthleteNewPDFHTML(currentAthleteProgram.Days, currentAthleteProgram.WeekCount, currentAthleteProgram.Name, context, targetAthlete.FirstName ?? string.Empty, targetAthlete.LastName ?? string.Empty);
                            tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(a2pClient.HeadlessChrome.FromHtml(ahtml, options: options).Pdf)));
                        }
                        else
                        {

                            var meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, DaysToPrint, context));
                            if (string.IsNullOrEmpty(meregedAthletePDF.Pdf))//retry a failed attempt
                            {
                                meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, DaysToPrint, context));
                            }
                            if (string.IsNullOrEmpty(meregedAthletePDF.Pdf))//retry a failed attempt twice
                            {
                                meregedAthletePDF = a2pClient.Merge(SendMultiPageAthleteHTMLToAPI2Pdf(currentAthleteProgram, a2pClient, options, targetAthlete, DaysToPrint, context));
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
            catch (Exception ex)
            {
                if (Config.logEverything)
                {
                    logRepo.LogShit(new Models.LogMessage() { Message = "Fucking error " +  ex.ToString(), UserId = targetCoach.Id, LoggedDate = DateTime.Now, StackTrace = new System.Diagnostics.StackTrace().ToString() });
                }
                var u = ex;
                throw;
            }
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
                tasks.Add(Task.Factory.StartNew(() => mergedPdfs.Add(api2pdfClient.HeadlessChrome.FromHtml(targetPage, options: options).Pdf)));
            }
            Task.WaitAll(tasks.ToArray());
            return mergedPdfs;

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
        public static string GenerateNotes(AssignedNote note, List<int> displayWeeks)
        {
            var displayWeekString = new StringBuilder();
            displayWeekString.Append($"Week {displayWeeks.First()}");
            displayWeeks.Skip(1).ToList().ForEach(x => displayWeekString.Append($",{x}"));
            displayWeekString.Append(":");
            var ret = new StringBuilder();
            ret.Append("<div class='exercise-details-container'>");
            ret.Append("<div class='exercise-notes'>");
            ret.Append($"<h5>{note.Name}</h5><p>{displayWeekString} {note.NoteText}</p>");
            ret.Append("</div>");
            ret.Append("</div>");
            return ret.ToString();
        }
        public static string GenerateMetric(string MetricName, int weekCount, List<int> displayWeeks)
        {
            var ret = new StringBuilder();
            ret.Append("<div class='exercise-details-container'>");
            ret.Append("<div class='exercise-metrics'>");
            ret.Append("<div class='metrics-row'>");
            ret.Append("<div class='metrics-exercise-name'>");
            ret.Append("<h4>Metrics</h4>");
            ret.Append($"<strong>{MetricName}</strong>");
            ret.Append("</div>");//end metrics-exercise-name
            ret.Append("<div class='metrics-week-values'>");

            for (int i = 1; i <= weekCount; i++)
            {
                if (displayWeeks.Contains(i))
                {
                    ret.Append($"<div class='metrics-value col-{weekCount}'><p>&nbsp;&nbsp;</p></div>");
                }
                else
                {
                    ret.Append($"<div class='metrics-dummy-space col-{weekCount}'></div>");
                }
            }
            ret.Append("</div>");//end metris-week-values
            ret.Append("</div>");//end metrics-row
            ret.Append("</div>");//end exercise-metrics
            ret.Append("</div>");//end exercise-metrics

            return ret.ToString();
        }
        public static string GenerateAthleteNewPDFHTML(List<AssignedProgramDays> daysToPrint, int WeekCount, string programName, ExecutionContext context, string firstName = "", string lastName = "")
        {
            var ret = new StringBuilder();
            var numberOfworkoutsPerPage = 2.0;
            int pageCount = (int)Math.Ceiling(((double)daysToPrint.Count) / numberOfworkoutsPerPage);
            ret.Append($@"<html><head>
    <meta charset='UTF-8' />
    <meta http-equiv='Content-Type' content='text/html' />
    <meta http-equiv='X-UA-Compatible' content='IE=edge,chrome=1' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=0' />
    <meta name='apple-mobile-web-app-capable' content='yes' />
    <meta name='handheldfriendly' content='true' />
    <meta name='MobileOptimized' content='width' />
    <meta name='format-detection' content='telephone=no' />
    <meta http-equiv='Cache-control' content='no-cache' />
    <meta http-equiv='X-FRAME-OPTIONS' content='DENY' />
    <link href='https://fonts.googleapis.com/css2?family=Montserrat:wght@300;400;600;700&display=swap' rel='stylesheet' />
</head><body>");

            ret.Append("<div class='pdf-container'>");

            ret.Append(GeneratePDFHeader(firstName, lastName, programName, string.Empty));
            ret.Append("<div class='pdf-body'>");
            for (int i = 0; i < pageCount; i++)
            {
                var pagesToPrint = daysToPrint.Skip(i).Take((int)numberOfworkoutsPerPage).ToList();


                if (pagesToPrint.Count == 1)
                {
                    ret.Append(" <div class='pdf-day-single'>");
                    ret.Append(PrintAssignedProgramDay(pagesToPrint.First(), WeekCount));
                    ret.Append("</div>");
                }
                else
                {
                    ret.Append("<div class='pdf-day-left'>");
                    ret.Append(PrintAssignedProgramDay(pagesToPrint.First(), WeekCount));
                    ret.Append("</div>");
                    ret.Append("<div class='pdf-day-right'>");
                    ret.Append(PrintAssignedProgramDay(pagesToPrint[1], WeekCount));
                    ret.Append("</div>");
                }
                ret.Append("</div>");//ends pdf-body
                ret.Append("</div>");//ends pdf-container
                ret.Append($"</body>{Environment.NewLine} {CSS(context)} {Environment.NewLine } </html>");
            }
            return ret.ToString();
        }
        public static string PrintAssignedProgramDay(AssignedProgramDays targetDay, int weekCount)
        {
            var ret = new StringBuilder();
            var programItem = new List<Tuple<string, int>>();
            if (targetDay.AssignedMetrics.Any())
            {
                targetDay.AssignedMetrics.GroupBy(m => m.ProgramDayItemMetricId).ToList().ForEach(m =>
                {
                    var displayWeeks = m.Select(wi => wi.DisplayWeekId).ToList();
                    programItem.Add(new Tuple<string, int>(GenerateMetric(m.FirstOrDefault().MetricName, weekCount, displayWeeks), m.FirstOrDefault().Position));
                });
            }
            if (targetDay.AssignedNotes.Any())
            {
                targetDay.AssignedNotes.Where(w => w.ProgramDayId == targetDay.Id)
                    .GroupBy(g => g.Position, g => g,
                                (key, notes) => new { key = key, notes = notes }).ToList().ForEach(n =>
                                {
                                    var displayWeeks = n.notes.Select(x => x.DisplayWeekId).OrderBy(x => x).ToList();
                                    if (n.notes.Any())
                                    {
                                        programItem.Add(new Tuple<string, int>(GenerateNotes(n.notes.First(), displayWeeks), n.key));
                                    }
                                });
            }
            if (targetDay.AssignedSuperSets.Any())
            {
                targetDay.AssignedSuperSets.OrderBy(x => x.PositionInProgramDay).ToList().ForEach(x =>
                {
                    var minPosition = x.Exercises.Min(z => z.PositionInSuperSet);
                    var exerciseTuple = new List<Tuple<string, int>>();
                    var exerciseHTML = new StringBuilder();
                    exerciseHTML.Append("<div class='trainingBlock'>");
                    if (x.Notes.Any())
                    {
                        exerciseHTML.Append(CreateSuperSetNote(x.Notes, x.SuperSetDisplayTitle));
                    }
                    x.Exercises.OrderBy(y => y.PositionInSuperSet).ToList().ForEach(y =>
                    {
                        exerciseHTML.Append(CreateSuperSetHeaderBlock(y, weekCount, x.SuperSetDisplayTitle, x.Notes.Count == 0));


                        exerciseHTML.Append(CreateSuperSetBodyBlock(y, weekCount));

                    });
                    exerciseHTML.Append("</div>");
                    programItem.Add(new Tuple<string, int>(exerciseHTML.ToString(), x.PositionInProgramDay));
                    exerciseHTML.Clear();
                });
            }
            ret.Append("<div class='pdf-exercises'>");
            ret.Append(CreateDayHeader(targetDay.Position, weekCount));
            programItem.OrderBy(x => x.Item2).ToList().ForEach(x => ret.Append(x.Item1));
            ret.Append("</div>");
            return ret.ToString();
        }
        public static string CreateSuperSetNote(List<AssignedSuperSetNote> notes, string superSetTitleDisplay)
        {
            var ret = new StringBuilder();

            ret.Append("<div class='exercise-details-container'>");
            ret.Append($"<div class='exercise-names-no-top-border' style='font-size:10px;font-weight:700px'>{superSetTitleDisplay} <div style='font-size:8px;text-align:right'> Notes</div></div>");
            ret.Append($"<div class='week-values-division col-2 superSetNotes'>");
            ret.Append("<ul>");
            notes.OrderBy(x => x.Position).ToList().ForEach(x =>
            {
                ret.Append($"<li>Weeks {string.Join(",", x.DisplayWeeks)} : {x.Note}</li>");
            });
            ret.Append("<ul>");
            ret.Append("</div>");
            ret.Append("</div>");
            return ret.ToString(); ;
        }
        public static string CreateDayHeader(int dayNumber, int weekCount)
        {
            var ret = new StringBuilder();
            ret.Append("<div class='table-header-details'>");
            ret.Append("<div class='days-info-row'>");
            ret.Append($"<div class='day-number'>Day {dayNumber + 1}</div>");
            ret.Append("<div class='day-week-container'>");
            for (int i = 1; i <= weekCount; i++)
            {
                ret.Append($"<div class='week-number col-{weekCount}'>Wk {i}</div>");
            }
            ret.Append("</div>");
            ret.Append("</div>");
            ret.Append("</div>");
            return ret.ToString();
        }
        public static string CreateSuperSetHeaderBlock(AssignedSuperSetExercise assignedSuperSetExercise, int weekCount, string superSetDisplayTitle, bool toggleDisplayText)
        {
            var ret = new StringBuilder();
            ret.Append("<div class='exercise-info-row'>");
            if (toggleDisplayText)
            {
                ret.Append($"<div class='exercise-title borderRight'>{superSetDisplayTitle}</div>");
            }
            else
            {
                ret.Append($"<div class='pairedWith'>Paired With</div>");
            }
            ret.Append("<div class='weekly-parameter-container'>");

            for (int i = 1; i <= weekCount; i++)
            {
                var totalShowBoxesByWeek = AllSetsRepsForTheWeek(assignedSuperSetExercise.SetsAndReps.Where(y => y.WeekPosition == i).ToList());
                ret.Append($"<div class='weekly-parameter col-{weekCount}'>");
                ret.Append(CreateExerciseHeader(totalShowBoxesByWeek));
                ret.Append("</div>");
            }

            ret.Append("</div>");//close weekly-paramter-container
            ret.Append("</div>");//close exercise-info-row

            return ret.ToString();
        }
        public static string CreateSuperSetBodyBlock(AssignedSuperSetExercise assignedSuperSetExercise, int weekCount)
        {
            var setsRepsSplitByWeek = assignedSuperSetExercise.SetsAndReps.GroupBy(x => x.PositionInSet);
            var ret = new StringBuilder();
            if (!setsRepsSplitByWeek.Any())
            {
                ret.Append("<div class='exercise-details-container'>");
                ret.Append("<div class='exercise-names'>");
                ret.Append($"<strong>{assignedSuperSetExercise.ExerciseName}</strong>");
                ret.Append("<i style='font-size:6px'>This Exercise Does not have any Set/Reps assigned to it</i>");
                ret.Append("</div>");//end exercise-names
                ret.Append("</div>");//end exercise-details.

                return ret.ToString();//somehow they got an empty set/rep scheme into the program
            }


            var totalWeeks = assignedSuperSetExercise.SetsAndReps.Select(x => x.WeekPosition).Distinct();//each set/rep is associated with a week. the weekPosition is which week its associted with.

            var totalShowBoxesByWeek = new List<ShowAdvancedOptionsBox>();

            foreach (var week in totalWeeks)
            {
                var showBoxesByWeek = AllSetsRepsForTheWeek(assignedSuperSetExercise.SetsAndReps.Where(y => y.WeekPosition == week).ToList());
                showBoxesByWeek.Week = week;
                totalShowBoxesByWeek.Add(showBoxesByWeek);
            }



            setsRepsSplitByWeek.OrderBy(x => x.Key).ToList();
            var firstSuperSet = true;
            var totalColumnCount = GetHeaderCount(setsRepsSplitByWeek.FirstOrDefault().FirstOrDefault());
            //var l = setsRepsSplitByWeek.Select(x => x.ToList());
            //var showBoxes = ShowAdvancedOptionsBox(setsRepsSplitByWeek.Select(x => x).ToList());
            foreach (IGrouping<int, AssignedSuperSetSetRep> setGroup in setsRepsSplitByWeek)
            {
                ret.Append("<div class='exercise-details-container'>");

                if (firstSuperSet)
                {
                    ret.Append("<div class='exercise-names'>");
                    ret.Append($"<strong>{assignedSuperSetExercise.ExerciseName}</strong>");
                    ret.Append($"<p>{assignedSuperSetExercise.Rest}</p>");
                }
                else
                {
                    ret.Append("<div class='exercise-names-no-top-border'>");
                    ret.Append($"<strong>&nbsp;</strong>");
                }
                ret.Append("</div>");//end exercise-names && exercise-names-no-top-borde
                ret.Append("<div class='week-values-container'>");
                firstSuperSet = false;

                foreach (var s in setGroup.OrderBy(i => i.WeekPosition))
                {
                    ret.Append($"<div class='week-values-division col-{weekCount}'>");
                    ret.Append(CreateExerciseSetRepBody(s, totalShowBoxesByWeek.FirstOrDefault(x => x.Week == s.WeekPosition)));
                    ret.Append("</div>");//end week-values-division col-
                }
                if (setGroup.Count() < weekCount)
                {
                    var totalBlankSetsReps = weekCount - setGroup.Count();
                    for (int i = 0; i < totalBlankSetsReps; i++)
                    {
                        ret.Append($"<div class='week-values-division col-{weekCount}'>");
                        ret.Append(AddEmptyExerciseSetRepBody(totalColumnCount));
                        ret.Append("</div>");//end week-values-division col-
                    }
                }


                ret.Append("</div>");//end week-values-container
                ret.Append("</div>");//end exercise-details-container
            }

            return ret.ToString();
        }
        public static string AddEmptyExerciseSetRepBody(int headerCount)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < headerCount; i++)
            {
                ret.Append($"<div class='week-value col-[replaceWithColumnCount] emptySet'>&nbsp;</div>");
            }
            return ret.ToString().Replace("[replaceWithColumnCount]", headerCount.ToString());
        }
        public static string CreateExerciseSetRepBody(AssignedSuperSetSetRep targetSuperSetRep, ShowAdvancedOptionsBox advancedOptionsToShow)
        {
            var ret = new StringBuilder();
            var totalColumns = 0;
            var emptySpace = "&nbsp;";

            if (advancedOptionsToShow.ShowSet)
            {
                ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{targetSuperSetRep.AssignedWorkoutSets?.ToString() ?? emptySpace }</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowReps)
            {
                ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{targetSuperSetRep.AssignedWorkoutReps?.ToString() ?? emptySpace}</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowRx)
            {
                if (targetSuperSetRep.PercentMaxCalcSubPercent != null && targetSuperSetRep.PercentMaxCalcSubPercent.HasValue)
                {
                    ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{targetSuperSetRep.PercentMaxCalcSubPercent.Value}</div>");
                }
                else if (targetSuperSetRep.AssignedWorkoutWeight != null && targetSuperSetRep.AssignedWorkoutWeight.HasValue)
                {
                    ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{targetSuperSetRep.AssignedWorkoutWeight.Value}</div>");
                }
                else
                {
                    ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>&nbsp;</div>");
                }
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowTime)
            {
                var time = string.Empty;
                time = $"<div class='week-value col-[replaceWithColumnCount]'>&nbsp;</div>";
                if (targetSuperSetRep.AssignedWorkoutMinutes != null && targetSuperSetRep.AssignedWorkoutMinutes.HasValue)
                {
                    time = $"{targetSuperSetRep.AssignedWorkoutMinutes.Value}m";
                }
                if (targetSuperSetRep.AssignedWorkoutSeconds != null && targetSuperSetRep.AssignedWorkoutMinutes.HasValue)
                {
                    time = time + $"{targetSuperSetRep.AssignedWorkoutSeconds.Value}s";
                }
                ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{time}</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowDistance || !String.IsNullOrEmpty(targetSuperSetRep.AssignedWorkoutDistance))
            {
                ret.Append($"<div class='week-value col-[replaceWithColumnCount]'>{targetSuperSetRep.AssignedWorkoutDistance}</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowDistance || targetSuperSetRep.RepsAchieved)
            {
                ret.Append(" <div class='week-value col-[replaceWithColumnCount]'>&nbsp;</div>");
                totalColumns++;
            }
            return ret.ToString().Replace("[replaceWithColumnCount]", totalColumns.ToString());
        }
        public static int GetHeaderCount(AssignedSuperSetSetRep targetSuperSetRep)
        {
            var totalColumns = 0;

            if (targetSuperSetRep.AssignedWorkoutSets.HasValue)
            {
                totalColumns++;
            }
            if (targetSuperSetRep.AssignedWorkoutReps.HasValue)
            {
                totalColumns++;
            }
            if (targetSuperSetRep.ShowWeight || targetSuperSetRep.AssignedWorkoutWeight.HasValue || targetSuperSetRep.PercentMaxCalcSubPercent.HasValue || targetSuperSetRep.PercentMaxCalc.HasValue)
            {
                totalColumns++;
            }
            if (targetSuperSetRep.AssignedWorkoutMinutes.HasValue || targetSuperSetRep.AssignedWorkoutSeconds.HasValue)
            {
                totalColumns++;
            }
            if (!String.IsNullOrEmpty(targetSuperSetRep.AssignedWorkoutDistance))
            {
                totalColumns++;
            }
            if (targetSuperSetRep.RepsAchieved)
            {
                totalColumns++;
            }
            return totalColumns;
        }
        public static ShowAdvancedOptionsBox AllSetsRepsForTheWeek(List<AssignedSuperSetSetRep> setsRepsForWeek)
        {
            var ret = new ShowAdvancedOptionsBox();

            foreach (var item in setsRepsForWeek)
            {
                if (item.AssignedWorkoutSets.HasValue)
                {
                    ret.ShowSet = true;
                }
                if (item.AssignedWorkoutReps.HasValue)
                {
                    ret.ShowReps = true;
                }
                if (item.ShowWeight || item.AssignedWorkoutWeight.HasValue || item.PercentMaxCalcSubPercent.HasValue || item.PercentMaxCalc.HasValue)
                {
                    ret.ShowRx = true;
                }
                if (item.AssignedWorkoutMinutes.HasValue || item.AssignedWorkoutSeconds.HasValue)
                {
                    ret.ShowTime = true;
                }
                if (!String.IsNullOrEmpty(item.AssignedWorkoutDistance))
                {
                    ret.ShowDistance = true;
                }
                if (item.RepsAchieved)
                {
                    ret.ShowRepsAchieved = true;
                }
            }

            return ret;
        }
        public static string CreateExerciseHeader(ShowAdvancedOptionsBox advancedOptionsToShow)
        {
            var ret = new StringBuilder();
            var totalColumns = 0;

            if (advancedOptionsToShow.ShowSet)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>S</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowReps)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>R</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowRx)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>L</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowTime)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>T</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowDistance)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>D</div>");
                totalColumns++;
            }
            if (advancedOptionsToShow.ShowRepsAchieved)
            {
                ret.Append(" <div class='exercise-week-details col-[replaceWithColumnCount]'>RA</div>");
                totalColumns++;
            }
            if (totalColumns == 0)
            {
                return " <div class='exercise-week-details col-1'>&nbsp;</div>";
            }
            else
            {
                return ret.ToString().Replace("[replaceWithColumnCount]", totalColumns.ToString());
            }
        }
        public static string GeneratePDFHeader(string athleteFirstName, string athleteSecondName, string programName, string logoURL)
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
        private static string CSS(ExecutionContext context)
        {
            var ret = @"<style type=""text/css"">";

            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "newPDFSstyles.txt")))
            {
                ret += reader.ReadToEnd();
            }
            ret += "</style>";
            return ret;
        }

    }
    public class ShowAdvancedOptionsBox
    {
        public bool ShowSet { get; set; }
        public bool ShowReps { get; set; }
        public bool ShowRx { get; set; }
        public bool ShowTime { get; set; }
        public bool ShowDistance { get; set; }
        public bool ShowRepsAchieved { get; set; }
        public int Week { get; set; }
    }
}

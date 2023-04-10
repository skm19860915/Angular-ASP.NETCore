using AzureFunctions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Dapper;


namespace OneTimeImportEmail
{
    public class Program
    {
        public static SqlConnection SqlConn => new SqlConnection(Config.SqlConn);
        public bool DebugMode => true;
        public static SendGridClient Client => new SendGridClient(Config.EmailAPIKey);
        public static EmailAddress FromEmail => new EmailAddress("steve@strengthcoachpro.com", "Administrator");
        static void Main(string[] args)
        {

            //var users = GetAllNewUsers();
            //users.ForEach(x =>
            //{
            //    var html = GenerateHtml(x);
            //    SendHtmlAsync(html, new EmailAddress(x.EmailAddr), x.FirstName, x.LastName);
            //});

            var users = GetUsers();
            //  throw new ApplicationException("dont fucking do this without george");
             InsertUsers(users);
            // this doesnt work it sends 0 instead of the userId on the query string
            foreach (var u in users)
            {
                var h = GenerateHtml(u);
                SendHtmlAsync(h, new EmailAddress(u.EmailAddr), u.FirstName, $"http://portal.strengthcoachpro.com/OneTimeRegister/{u.EmailValidationToken}/{u.Id}");
            }
        }
        static List<User> GetAllNewUsers()
        {
            var ret = new List<User>();
            using (SqlConnection conn = new SqlConnection(Config.SqlConn))
            {
                conn.Open();
                var text = @"SELECT id,Email,FirstName,EmailValidationToken FROM users WHERE users.LockedOut = 1";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new User() { Id = reader.GetInt32(0), EmailAddr = reader.GetString(1), FirstName = reader.GetString(2), EmailValidationToken = reader.GetString(3) });
                    }
                }
            }
            return ret;
        }
        static List<User> GetUsers()
        {
            string body = string.Empty;
            var ret = new List<User>();
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine("TestCoach.txt")))
            {
                var targetLine = reader.ReadLine();
                while (targetLine != null)
                {
                    var splitStuff = targetLine.Split(',');
                    ret.Add(new User() { FirstName = splitStuff[0], LastName = splitStuff[1], EmailAddr = splitStuff[2], EmailValidationToken = Guid.NewGuid().ToString(), OrgName = splitStuff[2] });
                    targetLine = reader.ReadLine();
                }
            }
            return ret;
        }
        static void InsertUsers(List<User> targetUsers)
        {
            using (SqlConnection conn = new SqlConnection(Config.SqlConn))
            {
                conn.Open();
                var text = @"
DECLARE @workoutsMappingTable table(oldId int, newId int, name varchar(max))
DECLARE @uomMappingTable table(oldId int, newId int, unitType varchar(max))
DECLARE @metricMappingTable table(oldId int, newId int, name varchar(max))
DECLARE @exerciseMappingTable table(oldId int, newId int, name varchar(max))
DECLARE @weeksMappingTable TABLE (oldId int, newId int, parentWorkoutId int, position int)
DECLARE @exerciseTagsMappings TABLE(oldId int, newId int, Name varchar(max))
DECLARE @workoutTagsMappings TABLE(oldId int, newId int, Name varchar(max))
DECLARE @metricTagsMappings TABLE(oldId int, newId int, Name varchar(max))
DECLARE @orgId  INT; 
DECLARE @userId INT;

INSERT INTO organizations (name,createduserid) VALUES (@orgName, 0);  

SELECT  @orgId =  SCOPE_IDENTITY();

INSERT INTO users (UserName,Password,isCoach,Email,ImageContainerName,FirstName,LastName,EmailValidationToken,IsEmailValidated,IsHeadCoach,OrganizationId,failedEntryAttempts,lockedout, isdeleted  )
VALUES(@UserName,@Password,1,@Email,@ImageContainerName,@FirstName,@LastName,@EmailValidationTOken,0,1,@orgId,0,0,0);

SELECT @userid = SCOPE_IDENTITY();

UPDATE Organizations
SET CreatedUserId = @userId 
WHERE [name] = @orgName

INSERT INTO UserToOrganizationRoles
select @userId,1,@orgId, @userid

INSERT INTO unitofMeasurements
OUTPUT inserted.id, inserted.unitType  INTO @uomMappingTable (newId,unitType)
SELECT unitType,@userId,@orgId FROM unitofMeasurements 
WHERE organizationid = 1 



INSERT INTO MetricTags
OUTPUT inserted.id, inserted.name INTO @MetricTagsMappings(newId, name)
SELECT name,@userId,null,0 from MetricTags
where createduserId = 13 and isdeleted = 0

UPDATE e
SET oldId =id
FROM MetricTags AS et
inner join @MetricTagsMappings AS e on et.name = e.name
where createduserId = 13 and isdeleted = 0


UPDATE uomMapping 
SET uomMapping.oldId = uom.id
FROM @uomMappingTable  AS uomMapping
INNER JOIN unitofMeasurements AS uom ON uom.unitType = uommapping.unitType
WHERE uom.organizationId = 1 

INSERT INTO METRICS
OUTPUT inserted.id, inserted.[name] into @metricMappingTable([newId], [name])
SELECT m.[name], @userid, uom.newId, m.IsDeleted,m.canModify,@orgId
FROM metrics AS m 
LEFT JOIN @uomMappingTable AS uom ON m.unitOfMeasurementId = uom.oldId
WHERE m.organizationId = 1 AND m.IsDeleted = 0

update mmt
SET oldId = m.id
FROM @metricMappingTable as mmt
INNER JOIN METRICS as m ON M.[Name] = MMT.[name]
WHERE M.OrganizationId = 1 AND M.IsDeleted = 0

INSERT INTO TagsToMetrics
SELECT mt.newId,mmt.newId
FROM TagsToMetrics AS ttm
INNER JOIN @metricMappingTable AS mmt on mmt.oldId = ttm.MetricId
INNER JOIN @metricTagsMappings AS mt ON mt.oldId = ttm.TagId

INSERT INTO exercises (name,notes,CreatedUserId,IsDeleted,[Percent],PercentMetricCalculationId,CanModify,OrganizationId)
OUTPUT inserted.id, inserted.name INTO @exerciseMappingTable(newId, name)
SELECT E.[name],notes,@userId,isDeleted,[percent],mmm.[NewId],1,@orgId 
FROM exercises AS E
LEFT JOIN @metricMappingTable AS mmm ON percentMetricCalculationId = mmm.oldId
WHERE organizationId = 1 AND e.IsDeleted = 0

UPDATE e
SET oldId = id
FROM Exercises AS ex 
INNER JOIN @exerciseMappingTable AS e on e.name = ex.name
WHERE organizationId = 1 AND ex.IsDeleted = 0

INSERT INTO exerciseTags
OUTPUT inserted.id, inserted.name INTO @exerciseTagsMappings(newId, name)
SELECT name,@userId,null,0 FROM exerciseTags
where createduserId = 13 and isdeleted = 0

UPDATE e
SET oldId =id
FROM exerciseTags AS et
inner join @exerciseTagsMappings AS e on et.name = e.name
where createduserId = 13 and isdeleted = 0

INSERT INTO TagsToExercises
SELECT mt.newId,mmt.newId
FROM TagsToExercises AS tte
INNER JOIN @exerciseMappingTable AS mmt on mmt.oldId = tte.ExerciseId
INNER JOIN @exerciseTagsMappings AS mt ON mt.oldId = tte.TagId

INSERT INTO workouts
OUTPUT inserted.id, inserted.name INTO @workoutsMappingTable(newId, name)
SELECT name, notes,createdDate,@userId, isDeleted,canmodify,@orgId
FROM workouts where OrganizationId = 1 and IsDeleted = 0

UPDATE wmTable
SET wmTable.oldId = w.id
FROM @workoutsMappingTable AS wmTable
INNER JOIN workouts AS w ON w.name = wmTable.name WHERE organizationId = 1 AND ISDELETED = 0

INSERT INTO WorkoutTags
OUTPUT inserted.id, inserted.name INTO @WorkoutTagsMappings(newId, name)
SELECT name,@userId,null,0 from workoutTags
where createduserId = 13 and isdeleted = 0

UPDATE e
SET oldId = id
FROM WorkoutTags AS et
inner join @WorkoutTagsMappings AS e on et.name = e.name
where createduserId = 13 and isdeleted = 0

INSERT INTO TagsToWorkouts
SELECT mmt.newId,mt.newId
FROM TagsToWorkouts AS tte
INNER JOIN @workoutsMappingTable AS mmt on mmt.oldId = tte.WorkoutId
INNER JOIN @workoutTagsMappings AS mt ON mt.oldId = tte.TagId

INSERT INTO weeks
OUTPUT inserted.id, inserted.position, inserted.ParentWorkoutId INTO @weeksMappingTable(newId, position, parentWorkoutId)
SELECT position , wm.newId
FROM weeks AS w
INNER JOIN workouts AS wo on wo.id = w.ParentWorkoutId
INNER JOIN @workoutsMappingTable AS wm ON wm.oldId = w.ParentWorkoutId
where wo.OrganizationId = 1 and wo.IsDeleted = 0

UPDATE W
SET oldId = we.id
FROM weeks AS we
inner join @workoutsMappingTable AS wmt ON WMT.oldId = WE.ParentWorkoutId
INNER JOIN @weeksMappingTable AS w ON w.parentWorkoutId = wmt.newId and w.position = we.Position

INSERT INTO [sets] 
SELECT s.[position],s.[sets],s.reps,s.[percent],s.[weight], wmt.newId
FROM [sets] AS S
INNER JOIN @weeksmappingTable AS wmt on s.ParentWeekId = wmt.oldId



------------------------------DELETE


------------------------------DELETE

 ";
                using (var sqlConn = new SqlConnection(Config.SqlConn))
                {
                    foreach (var u in targetUsers)
                    {
                        SqlConn.Execute(text,
                            new
                            {
                                UserName = u.FirstName + "userName",
                                Password = u.FirstName + "password",
                                Email = u.EmailAddr,
                                ImageContainerName = Guid.NewGuid().ToString(),
                                FirstName = u.FirstName,
                                LastName = u.LastName,
                                EmailValidationTOken = u.EmailValidationToken,
                                OrgName = u.FirstName + "OrgName" + Guid.NewGuid().ToString()
                            });
                    }
                }
            }
        }
        static string GenerateHtml(User targetUsers)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine("NewCoachRegistrationEmail.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{NewCoachFirstName}", targetUsers.FirstName).Replace("{registrationURL}", $"http://portal.strengthcoachpro.com/OneTimeRegister/{targetUsers.EmailValidationToken}/{targetUsers.Id}");
            //body = body.Replace("{NewCoachFirstName}", targetUsers.FirstName).Replace("{registrationURL}", $"http://localhost:4200/OneTimeRegister/{targetUsers.EmailValidationToken}/{targetUsers.Id}");
            return body;
        }
        static async Task SendHtmlAsync(string html, EmailAddress toURL, string firstName, string registrationLink)
        {

            var plainTextContent = $@"Hello {firstName} , your account at Strength Coach Pro has been created!

Click the link below to finish your registration.

Complete Registration {registrationLink}

We look forward to having you in the Strength Coach Pro Family.

Regards,

Steve O";
            var msg = MailHelper.CreateSingleEmail(FromEmail, toURL, "Welcome To Strength Coach Pro!", plainTextContent, html);
            var response = Client.SendEmailAsync(msg).Result;
        }
    }
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddr { get; set; }
        public string EmailValidationToken { get; set; }
        public string OrgName { get; set; }
    }
}

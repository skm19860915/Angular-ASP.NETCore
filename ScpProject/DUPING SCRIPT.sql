DECLARE @OriginalProgramID INT = 1
DECLARE @NewProgramID INT 
DECLARE @NewProgramName NVARCHAR(100) = 'test program to copy - copy parker'

DECLARE @NewProgramDays Table (position int, insertedId int, oldId int)
DECLARE @NewProgramDayItems TABLE (oldId int, [newId] int,newProgramDayId int,oldProgramDayId int, position int,itemenum int)
DECLARE @NewProgramDayItemExercises TABLE (oldId INT, [newId] INT, exerciseId INT, workoutId INT, oldProgramDayItemId INT, newProgramDayItemId INT)
DECLARE @NewProgramDayExercisesWeeks TABLE (oldId INT, [newID] INT , position INT, oldProgramDayItemExerciseId INT, newProgramDayItemExerciseId INT)
DECLARE @NewProgramDayItemSuperSets TABLE (oldId INT, [newID] INT, oldProgramDayItemId INT, newProgramDayItemId INT)
DECLARE @NewProgramDayItemSuperSetExercise TABLE(oldId INT, [newID] INT, oldProgramDayItemSuperSetId INT, newProgramDayItemSuperSetId INT, position INT, exerciseId INT)
DECLARE @NewProgramItemSuperSetWeeks TABLE (oldId INT, [newId] INT, position INT, oldSuperSetExerciseID INT, newSuperSetExerciseID INT)

--dupe programs
INSERT INTO PROGRAMS ([name],CreatedUserId,WeekCount,IsDeleted,CanModify)
SELECT @NewProgramName,CreatedUserId, WeekCOunt,0, 1
FROM Programs where Id = @OriginalProgramID
--save new program id
SELECT @NewProgramID = SCOPE_IDENTITY()
-- dupe program days
INSERT INTO programDays  (position,programId)
OUTPUT  INSERTED.[Position],INSERTED.[Id] INTO  @newProgramDays(position,InsertedId)
SELECT position, @newProgramId FROM programDays where programid = @OriginalProgramID
-- update the new programdays to match the old ProgramId to NewProgramId
UPDATE npd
SET oldId = pd.Id 
FROM @NewProgramDays as npd
INNER JOIN ProgramDays AS pd ON pd.Position = npd.position
WHERE pd.ProgramId = @originalProgramId
--dupe program dayitems
INSERT INTO ProgramDayItems (ProgramDayId, Position,ItemEnum)
OUTPUT INSERTED.Id ,INSERTED.Position, INSERTED.ProgramDayId, INSERTED.ItemEnum INTO @NewProgramDayItems([newId],position,newProgramDayId, itemenum)
SELECT npd.InsertedId, pdi.position,pdi.itemenum
FROM @NewProgramDays AS npd 
INNER JOIN ProgramDayItems AS pdi ON pdi.ProgramDayId = npd.oldId
--update dupped programdayItems tables with old data to have match it up
UPDATE npdI
SET NPDI.oldId  = pdi.Id , npdi.oldProgramDayId = pdi.ProgramDayId 
FROM @NewProgramDayItems as npdi
INNER JOIN ProgramDayItems AS pdI ON pdi.Position = npdi.position AND pdi.ItemEnum = npdi.ItemEnum
INNER JOIN @NewProgramDays AS npd ON PDI.ProgramDayId = NPD.oldId AND npdi.newProgramDayId = npd.insertedId
--dupe metrics
INSERT INTO programDayItemMetrics (metricId,ProgramDayItemId)
SELECT pdim.MetricId , npdi.newId  
FROM programDayItemMetrics AS pdim
INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = pdim.ProgramDayItemId AND NPDI.itemenum = 6
--dupExerciseItems
INSERT INTO ProgramDayItemExercises (exerciseId,workoutId,programDayItemId)
OUTPUT INSERTED.Id , inserted.ExerciseId, inserted.WorkoutId,inserted.ProgramDayItemId INTO @NewProgramDayItemExercises ([newId],exerciseId,workoutId, newProgramDayItemId)
SELECT pdie.ExerciseId,pdie.WorkoutId,npdi.[newId]
FROM ProgramDayItemExercises AS pdie
INNER JOIN @NewProgramDayItems AS npdi ON pdie.ProgramDayItemId = npdi.oldId AND npdi.itemenum = 1
--UPDATE dupped programDayItemExercises
UPDATE npdie
SET npdie.oldId = pdie.Id, npdie.oldProgramDayItemId = pdie.ProgramDayItemId
FROM @NewProgramDayItemExercises AS npdie
INNER JOIN @NewProgramDayItems AS npdi ON npdi.[newId] = npdie.newProgramDayItemId
INNER JOIN ProgramDayItemExercises AS pdie ON pdie.ProgramDayItemId = npdi.oldId 
-- dupeProgramWeeks
INSERT INTO programWeeks (position, ProgramDayItemExerciseId)
OUTPUT inserted.Id, inserted.Position,inserted.ProgramDayItemExerciseId INTO @NewProgramDayExercisesWeeks([newID],position,newProgramDayItemExerciseId)
SELECT pw.position,npdie.[newId]
FROM ProgramWeeks AS pw
INNER JOIN @NewProgramDayItemExercises AS npdie ON pw.ProgramDayItemExerciseId = npdie.oldId
--update dupped weeks
UPDATE npdiew
SET npdiew.oldId = pw.Id, npdiew.oldProgramDayItemExerciseId = pw.ProgramDayItemExerciseId
FROM @NewProgramDayExercisesWeeks AS npdiew
INNER JOIN @NewProgramDayItemExercises AS npdie ON npdie.[newId]  = npdiew.newProgramDayItemExerciseId 
INNER JOIN @NewProgramDayItems AS npdi ON npdi.oldId = npdie.oldProgramDayItemId
INNER JOIN ProgramWeeks AS pw ON pw.ProgramDayItemExerciseId = npdie.oldId  AND npdiew.position = pw.Position
--dupe sets
INSERT INTO programSets(position,[sets],[reps],[percent],[weight],parentprogramweekID)
SELECT ps.position,ps.[sets],ps.[reps],ps.[percent],ps.[weight], npdew.[newID]
FROM @NewProgramDayExercisesWeeks AS npdew
INNER JOIN ProgramSets AS ps ON ps.ParentProgramWeekId = npdew.oldId
--dupe SUPERSETS
INSERT INTO ProgramDayItemSuperSets (programDayItemId)
OUTPUT inserted.Id, inserted.ProgramDayItemId INTO @NewProgramDayItemSuperSets ([newId],[newProgramDayItemId])
SELECT npdi.[newId] 
FROM @NewProgramDayItems as npdi
INNER JOIN ProgramDayItemSuperSets AS pdiss ON pdiss.ProgramDayItemId = npdi.oldId 
--update duped SUPERSETS
UPDATE @NewProgramDayItemSuperSets
SET  oldId = pdiss.Id, oldProgramDayItemId = pdiss.ProgramDayItemId
FROM  ProgramDayItemSuperSets AS pdiss 
INNER JOIN @NewProgramDayItems AS npdi ON pdiss.ProgramDayItemId = npdi.oldId
INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.newProgramDayItemId = npdi.[newId]
--dupSuperSetExercises
INSERT INTO SuperSetExercises(ProgramDayItemSuperSetId,Position,ExerciseId)
OUTPUT inserted.Id , inserted.ExerciseId, inserted.Position, inserted.ProgramDayItemSuperSetId INTO @NewProgramDayItemSuperSetExercise([newId], exerciseId,position,newProgramDayItemSuperSetId)
SELECT npdiss.[newID], sse.Position,sse.ExerciseId
FROM @NewProgramDayItemSuperSets AS npdiss
INNER JOIN SuperSetExercises AS sse ON npdiss.oldid = sse.ProgramDayItemSuperSetId

UPDATE @NewProgramDayItemSuperSetExercise
SET oldId =sse.id, oldProgramDayItemSuperSetId = sse.ProgramDayItemSuperSetId
FROM  SuperSetExercises AS sse 
INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.oldid = sse.ProgramDayItemSuperSetId	
INNER JOIN @NewProgramDayItemSuperSetExercise AS npdisse ON npdisse.position = sse.Position AND npdisse.exerciseId = sse.ExerciseId AND npdisse.newProgramDayItemSuperSetId = npdiss.[newID]

INSERT INTO ProgramDayItemSuperSetWeeks (position,SuperSetExerciseId)
OUTPUT  inserted.id , inserted.Position, inserted.SuperSetExerciseId  INTO @NewProgramItemSuperSetWeeks([newId],position,newSuperSetExerciseId)
SELECT pdissw.position, npdisse.[newID]
FROM @NewProgramDayItemSuperSetExercise AS npdisse
INNER JOIN ProgramDayItemSuperSetWeeks AS pdissw ON npdisse.oldId = pdissw.SuperSetExerciseId

UPDATE @NewProgramItemSuperSetWeeks
SET oldId =sse.id, oldSuperSetExerciseID  = sse.ProgramDayItemSuperSetId
FROM  ProgramDayItemSuperSetWeeks AS pdissw
INNER JOIN  SuperSetExercises AS sse ON pdissw.SuperSetExerciseId = sse.ProgramDayItemSuperSetId
INNER JOIN @NewProgramDayItemSuperSets AS npdiss ON npdiss.oldid = sse.ProgramDayItemSuperSetId	
INNER JOIN @NewProgramDayItemSuperSetExercise AS npdisse ON npdisse.position = sse.Position AND npdisse.exerciseId = sse.ExerciseId AND npdisse.newProgramDayItemSuperSetId = npdiss.[newID]

INSERT INTO ProgramDayItemSuperSet_Set( position,[sets],[reps],[percent],[weight], ProgramDayItemSuperSetWeekId)
SELECT pdiss_s.position,pdiss_s.[sets],pdiss_s.[reps],pdiss_s.[percent],pdiss_s.[weight], npissw.[newId]
FROM ProgramDayItemSuperSet_Set AS pdiss_s
INNER JOIN @NewProgramItemSuperSetWeeks AS npissw ON pdiss_s.ProgramDayItemSuperSetWeekId = npissw.oldId


SELECT * FROM @NewProgramItemSuperSetWeeks
--SELECT * FROM @NewProgramDayItemSuperSetExercise
--SELECT * FROM @NewProgramDayItemSuperSets 
--select * from ProgramDayItemSuperSets
--select * from @NewProgramDayItemSuperSets
--select * from @NewProgramDayItems 
--SELECT * FROM ProgramSets 
--SELECT * FROM @NewProgramDayItemExercises
--select * from @NewProgramDayExercisesWeeks
--SELECT * FROM @NewProgramDayItems 

--SELECT * FROM @NewProgramDays
--SELECT * FROM ProgramDayItems 
--SELECT * FROM ProgramDayItemMetrics 
--select * from ProgramDayItemTypes 
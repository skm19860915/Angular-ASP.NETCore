--roll back

DECLARE @NewProgramName NVARCHAR(100) = 'test program to copy - copy parker'
DECLARE @NewProgramID INT 
SELECT @NewProgramID = ID FROM Programs where [Name] = @NewProgramName

DELETE pdissw
FROM ProgramDayItemSuperSetWeeks AS pdissw
INNER JOIN SuperSetExercises AS sse ON sse.id = SuperSetExerciseId
INNER JOIN ProgramDayItemSuperSets AS pdiss ON sse.ProgramDayItemSuperSetId = pdiss.Id
INNER JOIN ProgramDayItems AS pdI on PDISS.ProgramDayItemId = PDI.Id
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)

DELETE sse
FROM SuperSetExercises AS sse
INNER JOIN ProgramDayItemSuperSets AS pdiss ON sse.ProgramDayItemSuperSetId = pdiss.Id
INNER JOIN ProgramDayItems AS pdI on PDISS.ProgramDayItemId = PDI.Id
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)


DELETE PDISS
FROM ProgramDayItemSuperSets AS pdiss
INNER JOIN ProgramDayItems AS pdI on PDISS.ProgramDayItemId = PDI.Id
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)

DELETE ps
FROM ProgramSets AS ps
INNER JOIN ProgramWeeks AS pw ON pw.id = ps.ParentProgramWeekId
INNER JOIN ProgramDayItemExercises  AS pdie ON pdie.id = pw.ProgramDayItemExerciseId 
INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdie.ProgramDayItemId
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)

DELETE pw
FROM ProgramWeeks AS pw
INNER JOIN ProgramDayItemExercises  AS pdie ON pdie.id = pw.ProgramDayItemExerciseId 
INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdie.ProgramDayItemId
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)


DELETE PDIe
FROM ProgramDayItemExercises  AS pdie
INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdie.ProgramDayItemId
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)

DELETE PDIM
FROM ProgramDayItemMetrics  AS pdim
INNER JOIN ProgramDayItems AS pdi ON pdi.Id = pdim.ProgramDayItemId
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)


DELETE PDI
FROM ProgramDayItems AS pdI
INNER JOIN ProgramDays  as pd on PD.Id = PDI.ProgramDayId
WHERE pd.ProgramId NOT IN (1,2)

DELETE FROM ProgramDays WHERE ProgramId = @NewProgramID
DELETE FROM Programs WHERE [name] = @NewProgramName

--SELECT @NewProgramID 
--SELECT * FROM Programs
--SELECT * FROM ProgramDays
--SELECT * FROM ProgramDayItems 
--SELECT * FROM ProgramDayItemMetrics 
--SELECT * FROM ProgramDayItemExercises 
--SELECT * FROM ProgramWeeks
--SELECT * FROM ProgramSets 
--select * from ProgramDayItemSuperSets 
--SELECT * from SuperSetExercises 
--SELECT * FROM ProgramDayItemSuperSetWeeks 
select * from ProgramDayItemSuperSet_Set

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { Program, ProgramDTO, ProgramDayDTO, ProgramDayItemExerciseDTO, ProgramWeekDTO, ProgramSetDTO, ProgramMetricDTO, ProgramSurveyDTO, ProgramDayItemNoteDTO, ProgramSuperSetNoteDTO, ProgramDayItemVideoDTO } from '../Models/Program/Program';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Day } from '../Models/Program/Day';
import { ProgramDayItemEnum } from '../Models/Program/ProgramDayItemEnum';
import { ProgramDayItemExercise } from '../Models/Program/ProgramDayItemExercise';
import { Metric } from '../Models/Metric/Metric';
import { Survey } from '../Models/Survey';
import { Exercise } from '../Models/Exercise';
import { WorkoutDetails } from '../Models/SetsAndReps/WorkoutDetails';
import { Workout } from '../Models/SetsAndReps/Workout';
import { AssignedProgram, AssignedSurveys, AssignedQuestion, AssignedDays, AssignedMetric, AssignedExercise, AssignedSetRep, AssignedSuperSet, AssignedSuperSetExercise, AssignedSuperSet_SetRep, AssignedNote, AssignedSuperSetNote, VideoHoster, AssignedVideo } from '../Models/Program/AssignedProgram';
import { HistoricProgram } from '../Models/Program/HistoricProgram';
import { map } from 'rxjs/operators';
import { Week } from '../Models/Week';
import { ProgramDayItemSuperSet, SuperSet_Exercise, SuperSet_Note } from '../Models/Program/ProgramDayItemSuperSet';
import { SuperSet_Week } from '../Models/SuperSet/SuperSet_Week';
import { SuperSet_Set } from '../Models/SuperSet/SuperSet_Set';
import { ValidationError, ValidationErrorContainer } from '../Models/Error/ProgramBuilderErrors';
import { Movie } from '../Models/MultiMedia/Movie';


@Injectable({
  providedIn: 'root'
})
export class ProgramBuilderService {

  private _headers;
  private savedProgram = null;

  constructor(private http: HttpClient, private cookieService: CookieService) {
    this._headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true'
    });
  }

  MarkDayAsCompleted(programDayId: number, weekNumber: number, athleteId : number) {
    return this.http.post(environment.endpointURL + `/api/Program/MarkDayAsCompleted/${programDayId}/${weekNumber}/${athleteId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  private _weekCount: number = 1;
  GetCurrentWeekCount(): number {
    return this._weekCount;
  }


  GetSavedProgram() {
    this.savedProgram = JSON.parse(localStorage.getItem('draftProgram'));
    return this.savedProgram;
  }
  SetSavedProgram(program) {
    if (program.CanModify) {
      this.savedProgram = program;
      localStorage.setItem('draftProgram', JSON.stringify(this.savedProgram));
    }
  }
  ClearSavedProgram() {
    this.savedProgram = null;
    localStorage.removeItem('draftProgram')
  }

  GetSavedWeekProgram() {
    this.savedProgram = JSON.parse(localStorage.getItem('draftWeekProgram'));
    return this.savedProgram;
  }
  SetSavedweekProgram(program) {
    if (program.CanModify) {
      this.savedProgram = program;
      localStorage.setItem('draftWeekProgram', JSON.stringify(this.savedProgram));
    }
  }
  ClearSavedWeekProgram() {
    this.savedProgram = null;
    localStorage.removeItem('draftWeekProgram')
  }


  GetSavedSnapShotProgram(athleteId : number) {
    this.savedProgram = JSON.parse(localStorage.getItem(`athleteSnapshot${athleteId}`));
    return this.savedProgram;
  }
  SetSavedSnapShotProgram(program,athleteId : number) {
    if (program.CanModify) {
      this.savedProgram = program;
      localStorage.setItem(`athleteSnapshot${athleteId}`, JSON.stringify(this.savedProgram));
    }
  }
  ClearSavedSnapShotProgram(athleteId : number) {
    this.savedProgram = null;
    localStorage.removeItem(`athleteSnapshot${athleteId}`)
  }
  UpdateSnapshotForUser(targetProgram: Program, athleteId:number) {
    var convertedProg = this.ConvertToProgramDTO(targetProgram);
    return this.http.post(environment.endpointURL + "/api/Program/UpdateSnapShotProgram/"+athleteId
      , JSON.stringify(convertedProg), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }


  SetCurrentWeekCount(weekCount: number) {
    this._weekCount = weekCount;
  }
  PrintPdfProgram(programId: number, printMasterPdf: boolean, printSelectedAthletes: boolean, athleteIdsToPrint: number[], printUsingAdvancedOptions: boolean) {
    return this.http.post(environment.endpointURL + `/api/Program/PrintPDFProgram`,
      JSON.stringify({ PrintUsingAdvancedOptions: printUsingAdvancedOptions, ProgramId: programId, PrintMasterPdf: printMasterPdf, AthleteIdsToPrint: athleteIdsToPrint, PrintSelectedAthletes: printSelectedAthletes }), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  GetAllPastPrograms(athleteId: number): Observable<HistoricProgram[]> {
    return this.http.get<HistoricProgram[]>(environment.endpointURL + "/api/Program/GetAllPastPrograms/" + "/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  CreateProgram(targetProgram: Program) {

    var convertedProg = this.ConvertToProgramDTO(targetProgram);
    return this.http.post(environment.endpointURL + "/api/Program/CreateNewProgram"
      , JSON.stringify(convertedProg), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }
  GetAllPrograms(): Observable<Program[]> {
    return this.http.get<Program[]>(environment.endpointURL + "/api/Program/GetAllPrograms/", {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }

  public GetProgram(id: number) {

    return this.http.get(environment.endpointURL + "/api/Program/GetProgram/" + id, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return this.convertBackedendProgramToFrontEndProgram(x); }));
  }

  public GetSnapShotProgram(athleteId: number) {

    return this.http.get(`${environment.endpointURL}/api/Athletes/GetSnapShotForModifying/${athleteId}`, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return this.convertBackedendProgramToFrontEndProgram(x); }));
  }

  public GetAssignedProgram(assignedProgramId: number) {
    var urlToUse = assignedProgramId == 0 ? "" : "/" + assignedProgramId;
    return this.http.get(environment.endpointURL + "/api/Athletes/GetAssignedProgram" + urlToUse, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return (this.ConvertAssignedProgramToDisplayMode(x)); }));
  }
  public GetAthleteProgramByWeightRoom(athleteId: number) {
    return this.http.get(environment.endpointURL + "/api/Athletes/GetAssignedProgramWeightRoomAccount/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return (this.ConvertAssignedProgramToDisplayMode(x)); }));
  }
  public GetAnAthletesAssignedProgramByProgramId(assignedProgramId: number,isSnapShot : number, athleteId: number) {
    return this.http.get(environment.endpointURL + `/api/Athletes/GetAnAthletesAssignedProgramByProgramId/${assignedProgramId}/${isSnapShot}/${athleteId}`, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return (this.ConvertAssignedProgramToDisplayMode(x)); }));
  }

  public GetAnAthletesAssignedProgram(athleteId: number) {
    return this.http.get(environment.endpointURL + "/api/Athletes/GetAnAthletesAssignedProgram/" + athleteId, {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    }).pipe(map(x => { return (this.ConvertAssignedProgramToDisplayMode(x)); }));
  }

  DuplicateProgram(programId: number): any {
    return this.http.post(environment.endpointURL + `/api/Program/DuplicateProgram/${programId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  HardDeleteProgram(programId: number): any {
    return this.http.get(environment.endpointURL + `/api/Program/HardDelete/${programId}`
      , {
        withCredentials: true,
        observe: 'body',
        headers: this._headers
      });
  }
  ArchiveProgram(programId: number): any {
    return this.http.post(environment.endpointURL + `/api/Program/ArchiveProgram/${programId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  UnArchiveProgram(programId: number): any {
    return this.http.post(environment.endpointURL + `/api/Program/UnArchiveProgram/${programId}`
      , '', {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    });
  }
  UpdateProgram(targetProgram: Program) {
    var convertedProg = this.ConvertToProgramDTO(targetProgram);
    return this.http.post(environment.endpointURL + "/api/Program/UpdateProgram"
      , JSON.stringify(convertedProg), {
      withCredentials: true,
      observe: 'body',
      headers: this._headers
    })
  }

  private ConvertAssignedProgramToDisplayMode(assignedProgram: any): AssignedProgram {
    var ret = new AssignedProgram();
    ret.Id = assignedProgram.Id;
    ret.Name = assignedProgram.Name

    ret.WeekCount = assignedProgram.WeekCount;
    ret.AthleteId = assignedProgram.AthleteId;
    ret.CompletedDays = assignedProgram.CompletedDays;
    ret.Days = [];
    ret.IsSnapShot = assignedProgram.IsSnapShot;

    if (assignedProgram.Days == undefined || assignedProgram.Days == null) return;
    for (var x = 0; x < assignedProgram.Days.length; x++) {
      var newDay = new AssignedDays();
      newDay.Id = assignedProgram.Days[x].Id;
      newDay.Position = assignedProgram.Days[x].Position;
      newDay.IsActive = x == 0;

      newDay.AssignedProgramDayItem = [];
      for (var e = 0; e < assignedProgram.Days[x].AssignedNotes.length; e++) {
        var newNote = new AssignedNote();
        var targetNote = assignedProgram.Days[x].AssignedNotes[e];
        newNote.AssignedProgramId = targetNote.AssignedProgramId;
        newNote.ProgramDayItemNoteId = targetNote.ProgramDayItemNoteId;
        newNote.ProgramDayId = targetNote.ProgramDayId;
        newNote.Position = targetNote.Position;
        newNote.Name = targetNote.Name;
        newNote.NoteText = targetNote.NoteText;
        newNote.DisplayWeekId = targetNote.DisplayWeekId;

        newDay.AssignedProgramDayItem.push({
          Id: newNote.ProgramDayItemNoteId,
          ItemType: ProgramDayItemEnum.note,
          Position: newNote.Position,
          ProgramItem: { SelectedNote: newNote }
        })
      }
      for (var e = 0; e < assignedProgram.Days[x].AssignedSuperSets.length; e++) {
        var SuperSet = new AssignedSuperSet();
        // WELL NO FUCKING SHIT. AS OF THIS WRITING 9/24 all the the code to assign properties and create a new assignedSuperset is useless because
        // we are assigning the JSON object from the web api striaght to our assignedSuperset. Look at targetSuperSet. It never has its properties changed
        // but is being mapped the the return
        let targetSuperSet = assignedProgram.Days[x].AssignedSuperSets[e];
        SuperSet.SuperSetId = targetSuperSet.Id;
        SuperSet.ProgramDayItemPosition = targetSuperSet.Position;
        if (targetSuperSet.Notes.Length >= 1) {
          for (let i = 0; i < targetSuperSet.Notes.Length; i++) {
            let newSuperSetNote = new AssignedSuperSetNote()
            newSuperSetNote.Id = targetSuperSet.Note[i].Id;
            newSuperSetNote.Note = targetSuperSet.Note[i].Note;
            newSuperSetNote.Position = targetSuperSet.Note[i].Position;
            newSuperSetNote.ProgramDayItemSuperSetId = targetSuperSet.Note[i].ProgramDayItemSuperSetI;
            //newSuperSetNote.DisplayWeeks = targetSuperSet.Note[i].DisplayWeeks;
          }
        }

        if (targetSuperSet.Exercises.length >= 1) {
          for (var a = 0; a < assignedProgram.Days[x].AssignedSuperSets[e].Exercises.length; a++) {
            let selectedExercise = new AssignedSuperSetExercise();
            let targetExercise = assignedProgram.Days[x].AssignedSuperSets[e].Exercises[a];
            selectedExercise.Name = targetExercise.ExerciseName;
            selectedExercise.ProgramDayId = targetExercise.ProgramDayId;
            selectedExercise.VideoURL = targetExercise.VideoURL;
            selectedExercise.VideoProvider = VideoHoster.none;
            selectedExercise.Rest = selectedExercise.Rest;
            // if (targetExercise.VideoURL.length > 0) {yup look at comment on line 195, if that is gone 9/24 is when it was writtne
            //   selectedExercise.VideoProvider = targetExercise.VideoURL.indexOf("vimeo") > 0 ? VideoHoster.vimeo : VideoHoster.youtube;
            // }

            selectedExercise.SetsAndReps = [];
            for (var s = 0; s < targetExercise.SetsAndReps.length; s++) {
              let assignedSetsAndReps = new AssignedSuperSet_SetRep();
              let targeSuperSetReps = targetExercise.SetsAndReps[s];
              assignedSetsAndReps.AssignedProgramId = targeSuperSetReps.AssignedProgramId;
              assignedSetsAndReps.AssignedWorkoutPercent = targeSuperSetReps.AssignedWorkoutPercent;
              assignedSetsAndReps.PositionInSet = targeSuperSetReps.PositionInSet;
              assignedSetsAndReps.AssignedWorkoutReps = targeSuperSetReps.AssignedWorkoutReps;
              assignedSetsAndReps.AssignedWorkoutSets = targeSuperSetReps.AssignedWorkoutSets;
              assignedSetsAndReps.AssignedWorkoutWeight = targeSuperSetReps.AssignedWorkoutWeight;
              assignedSetsAndReps.CompletedSetPercent = targeSuperSetReps.CompletedSetPercent;
              assignedSetsAndReps.CompletedSetSets = targeSuperSetReps.CompletedSetSets;
              assignedSetsAndReps.CompletedSetWeight = targeSuperSetReps.CompletedSetWeight;
              assignedSetsAndReps.SuperSetWeekId = targeSuperSetReps.SetWeekId;
              assignedSetsAndReps.OriginalSuperSetSetId = targeSuperSetReps.OriginalSetId;
              assignedSetsAndReps.AthleteId = targeSuperSetReps.AthleteId;
              assignedSetsAndReps.PercentMaxCalc = targeSuperSetReps.PercentMaxCalc;
              assignedSetsAndReps.PercentMaxCalcSubPercent = targeSuperSetReps.PercentMaxCalcSubPercent;
              assignedSetsAndReps.WeekPosition = targetSuperSet.WeekPosition;
              assignedSetsAndReps.OriginalSuperSetSetId = targetSuperSet.OriginalSuperSet_SetId;
              selectedExercise.SetsAndReps.push(assignedSetsAndReps);
            }
          }
          newDay.AssignedProgramDayItem.push(
            {
              Id: e,
              ItemType: ProgramDayItemEnum.superset,
              Position: targetSuperSet.PositionInProgramDay,
              ProgramItem: targetSuperSet
            });
        }
      }
      for (var e = 0; e < assignedProgram.Days[x].AssignedExercises.length; e++) {
        if (assignedProgram.Days[x].AssignedExercises[e].AssignedSetsReps.length > 0) {
          var selectedExercise = new AssignedExercise();
          var targetExercise = assignedProgram.Days[x].AssignedExercises[e].AssignedSetsReps[0];
          selectedExercise.Name = targetExercise.ExerciseName;
          selectedExercise.ProgramDayId = targetExercise.ProgramDayId;
          selectedExercise.ProgramDayItemPosition = targetExercise.ProgramDayItemPosition;
          selectedExercise.AssignedSetsAndReps = [];
          for (var a = 0; a < assignedProgram.Days[x].AssignedExercises[e].AssignedSetsReps.length; a++) {
            var assignedSetsAndReps = new AssignedSetRep();
            var targetSetRep = assignedProgram.Days[x].AssignedExercises[e].AssignedSetsReps[a];
            assignedSetsAndReps.AssignedProgramId = targetSetRep.AssignedProgramId;
            assignedSetsAndReps.AssignedWorkoutPercent = targetSetRep.AssignedWorkoutPercent;
            assignedSetsAndReps.PositionInSet = targetSetRep.PositionInSet;
            assignedSetsAndReps.AssignedReps = targetSetRep.AssignedWorkoutReps;
            assignedSetsAndReps.AssignedSets = targetSetRep.AssignedWorkoutSets;
            assignedSetsAndReps.AssignedWeight = targetSetRep.AssignedWorkoutWeight;
            assignedSetsAndReps.CompletedSetPercent = targetSetRep.CompletedSetPercent;
            assignedSetsAndReps.CompletedSetSets = targetSetRep.CompletedSetSets;
            assignedSetsAndReps.CompletedSetWeight = targetSetRep.CompletedSetWeight;
            assignedSetsAndReps.SetWeekId = targetSetRep.SetWeekId;
            assignedSetsAndReps.OriginalSetId = targetSetRep.OriginalSetId;
            assignedSetsAndReps.AthleteId = targetSetRep.AthleteId;
            assignedSetsAndReps.PercentMaxCalc = targetSetRep.PercentMaxCalc;
            assignedSetsAndReps.PercentMaxCalcSubPercent = targetSetRep.PercentMaxCalcSubPercent;
            selectedExercise.AssignedSetsAndReps.push(assignedSetsAndReps);
          }
          newDay.AssignedProgramDayItem.push(
            {
              Id: assignedProgram.Days[x].AssignedExercises[e].ProgramDayItemExerciseId,
              ItemType: ProgramDayItemEnum.workout,
              Position: selectedExercise.ProgramDayItemPosition,
              ProgramItem: { SelectedExercise: selectedExercise }
            });
        }
      }
      for (var m = 0; m < assignedProgram.Days[x].AssignedMetrics.length; m++) {

        var selectedMetric = new AssignedMetric();
        var targetMetric = assignedProgram.Days[x].AssignedMetrics[m];
        selectedMetric.AssignedProgramId = targetMetric.AssignedProgramId;
        selectedMetric.CompletedWeight = targetMetric.CompletedWeight;
        selectedMetric.MetricId = targetMetric.MetricId;
        selectedMetric.MetricName = targetMetric.MetricName;
        selectedMetric.Position = targetMetric.Position;
        selectedMetric.ProgramDayId = targetMetric.ProgramDayId;
        selectedMetric.DisplayWeekId = targetMetric.DisplayWeekId;
        selectedMetric.ProgramDayItemMetricId = targetMetric.ProgramDayItemMetricId;

        newDay.AssignedProgramDayItem.push(
          {
            Id: targetMetric.Id,
            ItemType: ProgramDayItemEnum.metric,
            Position: targetMetric.Position,
            ProgramItem: { SelectedMetric: selectedMetric }
          });
      }
      for (var m = 0; m < assignedProgram.Days[x].AssignedVideos.length; m++) {

        var selectedVideo = new AssignedVideo();
        var targetVideo = assignedProgram.Days[x].AssignedVideos[m];
        selectedVideo.AssignedProgramId = targetVideo.AssignedProgramId;
        selectedVideo.MovieId = targetVideo.MovieId;
        selectedVideo.MovieName = targetVideo.Name;
        selectedVideo.Position = targetVideo.Position;
        selectedVideo.ProgramDayId = targetVideo.ProgramDayId;
        selectedVideo.DisplayWeekId = targetVideo.DisplayWeekId;
        selectedVideo.ProgramDayItemMovieId = targetVideo.ProgramDayItemMovieId;
        selectedVideo.MovieURL = targetVideo.URL;

        newDay.AssignedProgramDayItem.push(
          {
            Id: targetVideo.ProgramDayItemMovieId,
            ItemType: ProgramDayItemEnum.video,
            Position: selectedVideo.Position,
            ProgramItem: { SelectedVideo: selectedVideo }
          });
      }
      for (var s = 0; s < assignedProgram.Days[x].AssignedSurveys.length; s++) {
        var newSurvey = new AssignedSurveys();
        var target_Survey = assignedProgram.Days[x].AssignedSurveys[s];
        newSurvey.SurveyId = target_Survey.SurveyId;
        newSurvey.SurveyName = target_Survey.SurveyName;
        newSurvey.DisplayWeeks = target_Survey.DisplayWeeks;
        newSurvey.ProgramDayId = target_Survey.ProgramId;

        var questions: AssignedQuestion[] = [];
        for (var q = 0; q < target_Survey.Questions.length; q++) {
          questions.push({
            QuestionId: target_Survey.Questions[q].QuestionId,
            QuestionDisplayText: target_Survey.Questions[q].QuestionDisplayText,
            QuestionTypeId: target_Survey.Questions[q].QuestionTypeId,
            ProgramId: target_Survey.Questions[q].ProgramId,
            Answer: target_Survey.Questions[q].Answer,
            AssignedProgramId: target_Survey.Questions[q].AssignedProgramId,
            DisplayWeekId: target_Survey.Questions[q].DisplayWeekId
          });
        }
        newSurvey.Questions = [];
        newSurvey.Questions = questions;
        newDay.AssignedProgramDayItem.push(
          {
            Id: target_Survey.ProgramDayItemSurveyId,
            ItemType: ProgramDayItemEnum.survey,
            Position: target_Survey.Position,
            ProgramItem: { SelectedSurvey: newSurvey },
          });

      }
      ret.Days.push(newDay)
    }
    return ret;

  }
  private convertBackedendProgramToFrontEndProgram(BackendProgramObject: any): Program {
    var ret = new Program();
    ret.Id = BackendProgramObject.Id;
    ret.Name = BackendProgramObject.Name

    ret.WeekCount = BackendProgramObject.WeekCount;
    ret.CanModify = BackendProgramObject.CanModify;
    ret.Days = [];

    if (BackendProgramObject.Days === null) return;

    for (var x = 0; x < BackendProgramObject.Days.length; x++) {
      var newDay = new Day();
      newDay.Id = BackendProgramObject.Days[x].Id;
      newDay.Position = BackendProgramObject.Days[x].Position;
      newDay.IsActive = x == 0;

      newDay.Items = [];
      for (var s = 0; s < BackendProgramObject.Days[x].SuperSets.length; s++) {
        var daySS = new ProgramDayItemSuperSet();
        for (var sn = 0; sn < BackendProgramObject.Days[x].SuperSets[s].Notes.length; sn++) {
          var newNote = new SuperSet_Note();
          newNote.Position = BackendProgramObject.Days[x].SuperSets[s].Notes[sn].Position;
          newNote.Note = BackendProgramObject.Days[x].SuperSets[s].Notes[sn].Note;
          newNote.Id = BackendProgramObject.Days[x].SuperSets[s].Notes[sn].Id;

          for (var dw = 0; dw < BackendProgramObject.Days[x].SuperSets[s].Notes[sn].DisplayWeeks.length; dw++) {
            newNote.WeekIds.push(BackendProgramObject.Days[x].SuperSets[s].Notes[sn].DisplayWeeks[dw]);
          }
          daySS.Notes.push(newNote);
        }
        for (var se = 0; se < BackendProgramObject.Days[x].SuperSets[s].Exercises.length; se++) {
          var newExercise = new SuperSet_Exercise();
          var frank = BackendProgramObject.Days[x].SuperSets[s].Exercises[se];
          newExercise.ExerciseId = frank.ExerciseId;
          newExercise.Name = frank.Name;
          newExercise.Position = frank.Position;
          newExercise.SelectedWorkout = new WorkoutDetails();
          newExercise.SelectedWorkout.ShowRestBox = frank.Rest !== null && frank.Rest !== undefined;
          newExercise.SelectedWorkout.Rest = frank.Rest;
          for (var sew = 0; sew < BackendProgramObject.Days[x].SuperSets[s].Exercises[se].Weeks.length; sew++) {
            var newSSWeek = new SuperSet_Week();
            newSSWeek.Position = BackendProgramObject.Days[x].SuperSets[s].Exercises[se].Weeks[sew].Position;
            newSSWeek.Id = BackendProgramObject.Days[x].SuperSets[s].Exercises[se].Weeks[sew].Id;

            for (var sews = 0; sews < BackendProgramObject.Days[x].SuperSets[s].Exercises[se].Weeks[sew].SetsAndReps.length; sews++) {
              var newSSweekSet = new SuperSet_Set();
              var bob = BackendProgramObject.Days[x].SuperSets[s].Exercises[se].Weeks[sew].SetsAndReps[sews]
              newSSweekSet.Id = bob.Id;
              newSSweekSet.Percent = bob.Percent;
              newSSweekSet.Position = bob.Position;
              newSSweekSet.Reps = bob.Reps;
              newSSweekSet.Sets = bob.Sets;
              newSSweekSet.Weight = bob.Weight;
              newSSweekSet.Seconds = bob.Seconds;
              newSSweekSet.Minutes = bob.Minutes;
              newSSweekSet.Other = bob.Other;
              newSSweekSet.Distance = bob.Distance;
              newSSweekSet.RepsAchieved = bob.RepsAchieved;
              if (bob.Percent !== null && bob.Percent !== undefined) {
                newExercise.SelectedWorkout.ShowPercentageBox = true;
              }
              if (bob.Reps !== null && bob.Reps !== undefined) {
                newExercise.SelectedWorkout.ShowRepsBox = true;
              }
              if (bob.Sets !== null && bob.Sets !== undefined) {
                newExercise.SelectedWorkout.ShowSetsBox = true;
              }
              if (bob.Weight !== null && bob.Weight !== undefined) {
                newExercise.SelectedWorkout.ShowWeight = true;
              }
              if (bob.Seconds !== null && bob.Seconds !== undefined) {
                newExercise.SelectedWorkout.ShowTimeBox = true;
              }
              if (bob.Minutes !== null && bob.Minutes !== undefined) {
                newExercise.SelectedWorkout.ShowTimeBox = true;
              }
              if (bob.Percent !== null && bob.Percent !== undefined) {
                newExercise.SelectedWorkout.ShowPercentageBox = true;
              }
              if (bob.Other !== null && bob.Other !== undefined) {
                newExercise.SelectedWorkout.ShowOtherBox = true;
              }
              if (bob.Distance !== null && bob.Distance !== undefined) {
                newExercise.SelectedWorkout.ShowDistanceBox = true;
              }
              if (bob.RepsAchieved !== null && bob.RepsAchieved !== undefined && bob.RepsAchieved) {
                newExercise.SelectedWorkout.ShowRepsAchievedBox = true;
              }
              newSSWeek.SetsAndReps.push(newSSweekSet);
            }
            newExercise.Weeks.push(newSSWeek);
          }
          daySS.Exercises.push(newExercise);
        }
        newDay.Items.push(
          {
            Id: BackendProgramObject.Days[x].SuperSets[s].Id,
            ItemType: ProgramDayItemEnum.superset,
            Position: BackendProgramObject.Days[x].SuperSets[s].Position,
            ShowCreationMenu: false,
            ShowDetails: true,
            ProgramItem: daySS,
            //@ts-ignore
            SuperSetDisplayTitle: BackendProgramObject.Days[x].SuperSets[s].SuperSetDisplayTitle
          });
      }
      for (var e = 0; e < BackendProgramObject.Days[x].Exercises.length; e++) {

        var dayIEx = new ProgramDayItemExercise();
        dayIEx.SelectedExercise = new Exercise();
        dayIEx.SelectedWorkout = new Workout();
        dayIEx.SelectedExercise.Id = BackendProgramObject.Days[x].Exercises[e].Exercise.Id;
        dayIEx.SelectedExercise.Name = BackendProgramObject.Days[x].Exercises[e].Exercise.Name;
        dayIEx.SelectedWorkout.Id = BackendProgramObject.Days[x].Exercises[e].Workout.Id;
        dayIEx.SelectedWorkout.Name = BackendProgramObject.Days[x].Exercises[e].Workout.Name

        dayIEx.Workout = new WorkoutDetails();

        dayIEx.Workout.Id = BackendProgramObject.Days[x].Exercises[e].Workout.Id;
        dayIEx.Workout.Name = BackendProgramObject.Days[x].Exercises[e].Workout.Name;

        dayIEx.Workout.TotalWorkout = [];

        dayIEx.Weeks = [];
        for (var w = 0; w < BackendProgramObject.Days[x].Exercises[e].Weeks.length; w++) {
          var newWeek = new Week();
          newWeek.Id = BackendProgramObject.Days[x].Exercises[e].Weeks[w].Id;
          newWeek.Position = BackendProgramObject.Days[x].Exercises[e].Weeks[w].Position;
          newWeek.SetsAndReps = [];

          for (var s = 0; s < BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps.length; s++) {
            newWeek.SetsAndReps.push({
              Id: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Id,
              Position: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Position,
              Sets: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Sets,
              Reps: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Weight,
              Percent: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Percent,
              Weight: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Weight,
              ParentWeekId: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].ParentProgramWeekId,
              Minutes: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Minutes,
              Seconds: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Seconds,
              Distance: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Distance,
              RepsAchieved: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].RepsAchieved,
              Other: BackendProgramObject.Days[x].Exercises[e].Weeks[w].SetsAndReps[s].Other
            })

          }
          dayIEx.Weeks.push(newWeek);
        }
        newDay.Items.push(
          {
            Id: BackendProgramObject.Days[x].Exercises[e].Id,
            ItemType: ProgramDayItemEnum.workout,
            Position: BackendProgramObject.Days[x].Exercises[e].Position,
            ShowCreationMenu: false,
            ShowDetails: true,
            ProgramItem: dayIEx
          });
      }
      for (var s = 0; s < BackendProgramObject.Days[x].Surveys.length; s++) {
        var newSurvey = new Survey();
        newSurvey.Id = BackendProgramObject.Days[x].Surveys[s].SurveyId;
        newSurvey.Name = BackendProgramObject.Days[x].Surveys[s].SurveyName;
        var dWeeks: number[] = [];
        for (var mW = 0; mW < BackendProgramObject.Days[x].Surveys[s].DisplayWeeks.length; mW++) {
          dWeeks.push(BackendProgramObject.Days[x].Surveys[s].DisplayWeeks[mW]);
        }
        var questions = [];
        for (var q = 0; q < BackendProgramObject.Days[x].Surveys[s].Questions.length; q++) {
          questions.push({
            QuestionId: BackendProgramObject.Days[x].Surveys[s].Questions[q].Id,
            Question: BackendProgramObject.Days[x].Surveys[s].Questions[q].QuestionDisplayText,
            QuestionType: BackendProgramObject.Days[x].Surveys[s].Questions[q].QuestionTypeId,
            SurveyId: newSurvey.Id
          });
        }
        newSurvey.Questions = [];
        newSurvey.Questions = questions;
        newSurvey.WeekIds = dWeeks;
        newDay.Items.push(
          {
            Id: BackendProgramObject.Days[x].Surveys[s].Id,
            ItemType: ProgramDayItemEnum.survey,
            Position: BackendProgramObject.Days[x].Surveys[s].Position,
            ShowCreationMenu: false,
            ShowDetails: true,
            ProgramItem: { SelectedSurvey: newSurvey },
          });
      }
      for (var n = 0; n < BackendProgramObject.Days[x].Notes.length; n++) {
        var dWeeks: number[] = [];
        for (var mW = 0; mW < BackendProgramObject.Days[x].Notes[n].DisplayWeeks.length; mW++) {
          dWeeks.push(BackendProgramObject.Days[x].Notes[n].DisplayWeeks[mW]);
        }
        newDay.Items.push(
          {
            Id: BackendProgramObject.Days[x].Notes[n].Id,
            ItemType: ProgramDayItemEnum.note,
            Position: BackendProgramObject.Days[x].Notes[n].Position,
            ShowCreationMenu: false,
            ShowDetails: true,
            ProgramItem:
            {
              SelectedNote:
              {
                Name: BackendProgramObject.Days[x].Notes[n].Title,
                Note: BackendProgramObject.Days[x].Notes[n].Note,
                WeekIds: dWeeks
              }
            }
          });
      }
      for (var m = 0; m < BackendProgramObject.Days[x].Metrics.length; m++) {
        var dWeeks: number[] = [];
        for (var mW = 0; mW < BackendProgramObject.Days[x].Metrics[m].DisplayWeeks.length; mW++) {
          dWeeks.push(BackendProgramObject.Days[x].Metrics[m].DisplayWeeks[mW]);
        }
        newDay.Items.push(
          {
            Id: BackendProgramObject.Days[x].Metrics[m].Id,
            ItemType: ProgramDayItemEnum.metric,
            Position: BackendProgramObject.Days[x].Metrics[m].Position,
            ShowCreationMenu: false,
            ShowDetails: true,
            ProgramItem:
            {
              SelectedMetric:
              {
                Id: BackendProgramObject.Days[x].Metrics[m].Metric.Id,
                Name: BackendProgramObject.Days[x].Metrics[m].Metric.Name,
                UnitOfMeasurementId: BackendProgramObject.Days[x].Metrics[m].Metric.UnitOfMeasurementId,
                WeekIds: dWeeks
              }
            }
          });
      }
      for (var v = 0; v < BackendProgramObject.Days[x].Movies.length; v++) {
        var dWeeks: number[] = [];
        for (var mW = 0; mW < BackendProgramObject.Days[x].Movies[v].DisplayWeeks.length; mW++) {
          dWeeks.push(BackendProgramObject.Days[x].Movies[v].DisplayWeeks[mW]);
        }
        newDay.Items.push({
          Id: BackendProgramObject.Days[x].Movies[v].Id,
          ItemType: ProgramDayItemEnum.video,
          Position: BackendProgramObject.Days[x].Movies[v].Position,
          ShowCreationMenu: false,
          ShowDetails: true,
          ProgramItem: {
            SelectedVideo: {
              Id: BackendProgramObject.Days[x].Movies[v].Video.Id,
              Name: BackendProgramObject.Days[x].Movies[v].Video.Name,
              URL: BackendProgramObject.Days[x].Movies[v].Video.URL,
              WeekIds: dWeeks
            }
          }
        })
      }
      ret.Days.push(newDay);
    }


    return ret;
  }

  private ConvertToProgramDTO(t: Program): ProgramDTO {
    debugger;
    var ret = new ProgramDTO();
    ret.Id = t.Id;
    ret.Name = t.Name;
    ret.Tags = t.Tags;
    ret.WeekCount = t.WeekCount;
    var errorList = new ValidationErrorContainer('');
    ret.Days = [];
    t.Days.forEach(day => {
      var newDay = new ProgramDayDTO();
      newDay.Id = day.Id;
      newDay.Position = day.Position;
      newDay.Exercises = [];
      newDay.Metrics = [];
      newDay.Surveys = [];
      newDay.Notes = [];
      newDay.SuperSets = [];
      newDay.Videos = [];
      day.Items.forEach(item => {

        switch (item.ItemType) {
          case ProgramDayItemEnum.superset:

            for (var i = 0; i < item.ProgramItem.Exercises.length; i++) {

              for (var w = 0 ; w < item.ProgramItem.Exercises[i].Weeks.length; w++)
              {
                var target = item.ProgramItem.Exercises[i].Weeks[w];
                var setsAndRepPositionsToRemove = [];
                for (var t = 0 ; t<  target.SetsAndReps.length; t++ )
                {
                  var setsAndRepsToLookAt = target.SetsAndReps[t];
                  if ( setsAndRepsToLookAt.Distance == undefined && setsAndRepsToLookAt.Minutes == undefined && setsAndRepsToLookAt.Other == undefined && setsAndRepsToLookAt.Percent == undefined && setsAndRepsToLookAt.Reps == undefined && setsAndRepsToLookAt.Seconds == undefined && setsAndRepsToLookAt.Sets == undefined)
                  {
                    setsAndRepPositionsToRemove.push(t)
                  }
                }
                //I am reversing the array because if we remove array elements from last to first we wont have to do anything crazy
                var reveresedArray = setsAndRepPositionsToRemove.reverse();
                for (var l = 0 ; l< reveresedArray.length; l++)
                {
                  target.SetsAndReps.splice(reveresedArray[l],1)
                }
              }
              if (item.ProgramItem.Exercises[i].ExerciseId == undefined) {
                var valError = new ValidationError("A Super Set Has a Workout That Doesnt Have An Exercise Selected");
                valError.DayNumber = day.Position + 1;;
                valError.ExerciseType = 'Super Set Exercise'
                valError.ValidationErrorMessage = "A Super Set Has a Workout That Doesnt Have An Exercise Selected"
                errorList.AllValidationErrors.push(valError)
              }
              if (item.ProgramItem.Exercises[i].SelectedWorkout.Id === undefined &&
                (item.ProgramItem.Exercises[i].Weeks.length === 1 && //these will check if the user included an non pre-built setsandreps
                  item.ProgramItem.Exercises[i].Weeks[0].SetsAndReps[0].Percent === 0 &&
                  item.ProgramItem.Exercises[i].Weeks[0].SetsAndReps[0].Sets === 0 &&
                  item.ProgramItem.Exercises[i].Weeks[0].SetsAndReps[0].Reps === 0 &&
                  item.ProgramItem.Exercises[i].Weeks[0].SetsAndReps[0].Weight === 0
                )
              ) {
                //todo: this validation doesnt actually display
                var valError = new ValidationError("A Super Set Has an Exercise That Doesnt Have A Workout Selected");
                valError.DayNumber = day.Position;
                valError.ExerciseType = 'Super Set Exercise'
                valError.ValidationErrorMessage = "A Super Set Has an Exercise That Doesnt Have A Workout Selected"
                errorList.AllValidationErrors.push(valError)
              }
            }
            let newNotes = []
            for (var i = 0; i < item.ProgramItem.Notes.length; i++) {
              if (item.ProgramItem.Notes[i].WeekIds.length == 0) {
                var valError = new ValidationError("A Super Set Note Doesn't Have Any Weeks Marked As Display");
                valError.DayNumber = day.Position;
                valError.ExerciseType = 'Super Set Note'
                valError.ValidationErrorMessage = "A Super Set Note Doesn't Have Any Weeks Marked As Display"
                errorList.AllValidationErrors.push(valError)
              }
              let newNote = new ProgramSuperSetNoteDTO();
              newNote.Note = item.ProgramItem.Notes[i].Note;
              newNote.Position = item.ProgramItem.Notes[i].Position;
              newNote.DisplayWeeks = item.ProgramItem.Notes[i].WeekIds;
              newNotes.push(newNote);
            }
            let exercises = []
            for (var x = 0; x < item.ProgramItem.Exercises.length; x++) {
              exercises.push(item.ProgramItem.Exercises[x]);
              exercises[x].Rest = item.ProgramItem.Exercises[x].SelectedWorkout.Rest;
              exercises[x].ShowWeight = item.ProgramItem.Exercises[x].SelectedWorkout.ShowWeight;
            }
            //@ts-ignore


            newDay.SuperSets.push({ Position: item.Position, Exercises: exercises, Notes: newNotes, SuperSetDisplayTitle: item.SuperSetDisplayTitle });
            break;
          case ProgramDayItemEnum.note:
            var note = (item.ProgramItem.SelectedNote)
            var newNote = new ProgramDayItemNoteDTO();
            newNote.Name = note.Name;
            newNote.Note = note.Note;
            newNote.Position = item.Position;
            newDay.Notes.push(newNote);
            if (note.WeekIds.length == 0) {
              var valError = new ValidationError("A Note Doesnt Have A Display Week Selected.");
              valError.DayNumber = day.Position + 1;
              valError.ExerciseType = 'Note'
              valError.ValidationErrorMessage = "A Note Doesnt Have A Display Week Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            for (var i = 0; i < note.WeekIds.length; i++) {
              newNote.DisplayWeeks.push(note.WeekIds[i]);
            }
            break;
          case ProgramDayItemEnum.metric:
            var metric = (<Metric>item.ProgramItem.SelectedMetric)
            var newMet = new ProgramMetricDTO();
            if (metric.Id == undefined) {
              var valError = new ValidationError("A Metric Wasn't Selected.");
              valError.DayNumber = day.Position + 1;
              valError.ExerciseType = 'Metric'
              valError.ValidationErrorMessage = "A Metric Wasn't Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            newMet.MetricId = metric.Id;
            newMet.Position = item.Position;
            if (metric.WeekIds.length == 0) {
              var valError = new ValidationError("A Metric  Doesnt Have A Display Week Selected.");
              valError.DayNumber = day.Position;
              valError.ExerciseType = 'Metric'
              valError.ValidationErrorMessage = "A Metric  Doesnt Have A Display Week Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            for (var i = 0; i < metric.WeekIds.length; i++) {
              newMet.DisplayWeeks.push(metric.WeekIds[i]);
            }
            newDay.Metrics.push(newMet);
            break;
          case ProgramDayItemEnum.survey:
            var survey = (<Survey>item.ProgramItem.SelectedSurvey);
            var newSurvey = new ProgramSurveyDTO();
            newSurvey.Position = item.Position
            newSurvey.SurveyId = survey.Id;
            newDay.Surveys.push(newSurvey);
            if (survey.Id == undefined) {
              var valError = new ValidationError("A Survey Wasn't Selected.");
              valError.DayNumber = day.Position + 1;;
              valError.ExerciseType = 'Survey'
              valError.ValidationErrorMessage = "A Survey Wasn't Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            if (survey.WeekIds.length == 0) {
              var valError = new ValidationError("A Survey  Doesnt Have A Display Week Selected.");
              valError.DayNumber = day.Position + 1;;
              valError.ExerciseType = 'Survey'
              valError.ValidationErrorMessage = "A Survey  Doesnt Have A Display Week Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            for (var i = 0; i < survey.WeekIds.length; i++) {
              newSurvey.DisplayWeeks.push(survey.WeekIds[i]);
            }
            break;
          case ProgramDayItemEnum.workout:
            var ex = (<ProgramDayItemExercise>item.ProgramItem);
            var newEx = new ProgramDayItemExerciseDTO();
            if (ex.SelectedExercise.Id == undefined) {
              var valError = new ValidationError("A Workout Was Selected Which Doesnt Have an Exercise Selected");
              valError.DayNumber = day.Position + 1;;
              valError.ExerciseType = 'Workout'
              valError.ValidationErrorMessage = "A Workout Was Selected Which Doesnt Have an Exercise Selected"
              errorList.AllValidationErrors.push(valError)
              return;
            }
            newEx.ExerciseId = ex.SelectedExercise.Id;
            newEx.WorkoutId = ex.SelectedWorkout.Id;
            newEx.Position = item.Position;
            newEx.Weeks = [];

            ex.Weeks.forEach(week => {
              var newWeek = new ProgramWeekDTO();
              newWeek.SetsAndReps = [];
              newWeek.Id = week.Id;
              newWeek.Position = week.Position;
              newWeek.SetsAndReps = [];

              week.SetsAndReps.forEach(sr => {
                var setDTO = new ProgramSetDTO();
                setDTO.Id = sr.Id;
                setDTO.Percent = sr.Percent;
                setDTO.Position = sr.Position;
                setDTO.Reps = sr.Reps;
                setDTO.Sets = sr.Sets;
                setDTO.Weight = sr.Weight;
                newWeek.SetsAndReps.push(setDTO)
              });
              newEx.Weeks.push(newWeek);
            })
            newDay.Exercises.push(newEx)

            break;
          case ProgramDayItemEnum.video:
            var video = (<Movie>item.ProgramItem.SelectedVideo)
            var newVideo = new ProgramDayItemVideoDTO();
            if (video.Id == undefined) {
              var valError = new ValidationError("A Video Wasn't Selected.");
              valError.DayNumber = day.Position + 1;;
              valError.ExerciseType = 'Video'
              valError.ValidationErrorMessage = "A Video Wasn't Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            if (video.WeekIds.length == 0) {
              var valError = new ValidationError("A Video  Doesnt Have A Display Week Selected.");
              valError.DayNumber = day.Position + 1;;
              valError.ExerciseType = 'Metric'
              valError.ValidationErrorMessage = "A Video  Doesnt Have A Display Week Selected."
              errorList.AllValidationErrors.push(valError)
              return;
            }
            newVideo.MovieId = video.Id;
            newVideo.Position = item.Position;
            for (var i = 0; i < video.WeekIds.length; i++) {
              newVideo.DisplayWeeks.push(video.WeekIds[i]);
            }
            newDay.Videos.push(newVideo);
            break;
          default:
            break;
        }
      });
      ret.Days.push(newDay)
    });
    if (errorList.AllValidationErrors.length > 0) {
      throw errorList;
    }
    return ret;

  }
}

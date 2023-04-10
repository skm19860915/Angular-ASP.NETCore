import { Component, OnInit, Input, SimpleChange } from '@angular/core';
import { Program } from '../../Models/Program/Program'
import { ProgramBuilderService } from '../../Services/program-builder.service';
import { AthleteService } from '../../Services/athlete.service';
import { SurveyService } from '../../Services/survey.service';
import { CompletedSet, CompletedSuperSet } from '../../Models/Athlete/CompletedSet';
import { AssignedProgram, AssignedMetric, AssignedSuperSet, AssignedProgramDayItem, AssignedSuperSetExercise, AssignedSuperSet_SetRep, AssignedDays } from '../../Models/Program/AssignedProgram';
import { NgxSmartModalService } from 'ngx-smart-modal';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../Services/user.service';
import { ProgramDayItemEnum } from '../../Models/Program/ProgramDayItemEnum';
import { AthletePastWorkout } from '../../Models/AthletePastWorkout/AthletePastWorkout';
import { CompletedDay } from '../../Models/Program/CompletedDay';

@Component({
  selector: 'app-athlete-workout',
  templateUrl: './athlete-workout.component.html',
  styleUrls: ['./athlete-workout.component.less']
})
export class AthleteWorkoutComponent implements OnInit {
  public targetVideoToDisplay: string;
  public AlertMessages: AlertMessage[] = [];
  public Program: AssignedProgram = this.GenerateEmptyProgram();
  public athleteService: AthleteService;
  public surveyService: SurveyService;
  public PBuilderService: ProgramBuilderService;
  private _PastAssignedWorkoutId: number;
  public WeekCount: number;
  public DayCount: number;
  public CurrentDay: number = 0;
  public CoachIsViewing: boolean = false;
  public videoProvider: number = 0;
  public videoDigits: string = '';
  public Loading: boolean = true;
  public DisplayPencil : boolean = false;
  @Input() DisplayPrintButton = true;
  @Input() AthleteId: number = 0;//if this is set its a coach viewing the page
  @Input() ViewSurveysOnly: boolean = false;
  @Input() set PastAssignedWorkoutModel(value: AthletePastWorkout) {
    if (value !== undefined || value !== null) {
      this._PastAssignedWorkoutId = value.AssignedProgramID;
      this.PBuilderService.GetAnAthletesAssignedProgramByProgramId(value.AssignedProgramID, value.IsSnapShot, this.AthleteId).subscribe((x: AssignedProgram) => {
        this.Program = x;
        this.DayCount = this.Program.Days.length;
        this.WeekCount = this.Program.WeekCount;
        this.Loading = false;
        this.ToggleVisibleBoxes(this.Program);
      });
    }
  }
  @Input() IsWeightRoomView: boolean = false;

  @Input() AutoLoad: string = "true";
  constructor(private userService: UserService, private route: ActivatedRoute, public ngxSmartModalService: NgxSmartModalService,
    private sService: SurveyService, private pBuilderService: ProgramBuilderService, aService: AthleteService
    , private router: Router) {
    this.athleteService = aService;
    this.surveyService = sService;
    this.PBuilderService = pBuilderService;

  }

  SetActiveDayAndWeek(days : AssignedDays[], CompletedDays : CompletedDay[], weekCount : number)
  {
    //something is setting the days as active so just to be safe I am clearing out all days and
    days.forEach(x => x.IsActive = false);
    if (CompletedDays.length === 0)
    {
      days[0].IsActive = true;
      this.Program.CurrentWorkoutWeekId =1;
    }
    else
    {
      var lastCompletedWeek =  Math.max.apply(Math,CompletedDays.map(x => x.WeekNumber))
      var arrayOfOnlylastCompletedDaysWithbyWeek = CompletedDays.filter(x => x.WeekNumber === lastCompletedWeek)
      var lastCompletedDay = Math.max.apply(Math,arrayOfOnlylastCompletedDaysWithbyWeek.map(x => x.ProgramDayId));

      var lastPossibleDayOfProgram = days[days.length-1].Id;
      var lastPossibleWeek = weekCount;

      //check to see if it is the last day and last week. If so there is no where to go. Maybe say a toast goodjob your done ??
      if ( lastPossibleDayOfProgram === lastCompletedDay && lastCompletedWeek === lastPossibleWeek)
      {
        days[days.length-1].IsActive = true;
        this.Program.CurrentWorkoutWeekId = weekCount;
        this.CurrentDay = days.length -1;
      }
      //this is the last day but there are still more weeks so set it to first day and next week
      else if ( lastPossibleDayOfProgram == lastCompletedDay)
      {
        days[0].IsActive = true;
        this.Program.CurrentWorkoutWeekId = lastCompletedWeek+1;
        this.CurrentDay = 0;
      }
      //it isnt the last day, and there are more weeks so just mark it as the next day with same week
      else
      {
        var foundIndex = days.findIndex(x => x.Id === lastCompletedDay)
        days[ foundIndex+1].IsActive = true;
        this.Program.CurrentWorkoutWeekId = lastCompletedWeek;
        this.CurrentDay = foundIndex+1;
      }
    }
  }
  GetProgram() {
    this.route.params.subscribe(params => {
      if (params['athleteId'] != undefined)//coach navigated to this page, if id is null then athlete navigated
      {
        this.AthleteId = params['athleteId']
      }
      if (this.userService.IsWeightRoomAccount())
      {
        this.PBuilderService.GetAthleteProgramByWeightRoom(this.AthleteId).subscribe((x: AssignedProgram) => {
          this.Program = x;
          this.DisplayPencil  = this.Program.IsSnapShot && this.DisplayPrintButton;
          if (x === undefined) {
            this.Loading = false;
            return;
          }

          this.SetActiveDayAndWeek(this.Program.Days, this.Program.CompletedDays, this.Program.WeekCount);
          this.AthleteId = this.Program.AthleteId
          this.DayCount = this.Program.Days.length;
          this.WeekCount = this.Program.WeekCount; this.Program.WeekCount
          this.Loading = false;
          this.ToggleVisibleBoxes(this.Program);

        });
      }
      else if (!this.userService.IsCoach()) {
        this.PBuilderService.GetAssignedProgram(0).subscribe((x: AssignedProgram) => {
          this.Program = x;
          if (x === undefined) {
            this.Loading = false;
            return;
          }
          this.SetActiveDayAndWeek(this.Program.Days, this.Program.CompletedDays, this.Program.WeekCount);

          this.AthleteId = this.Program.AthleteId
          this.DayCount = this.Program.Days.length;
          this.WeekCount = this.Program.WeekCount;
          this.Loading = false;
          this.ToggleVisibleBoxes(this.Program);

        });
      }

      else {
        this.CoachIsViewing = this.userService.IsCoach() && !window.location.href.toLowerCase().includes("weightroom");//todo make this an input param
        this.PBuilderService.GetAnAthletesAssignedProgram(this.AthleteId).subscribe((x: AssignedProgram) => {

          this.Program = x;
          if (x === undefined) {
            this.Loading = false;
            return;
          }
          this.SetActiveDayAndWeek(this.Program.Days, this.Program.CompletedDays, this.Program.WeekCount);

          this.DayCount = this.Program.Days.length;
          this.WeekCount = this.Program.WeekCount;
          this.Loading = false;
          this.ToggleVisibleBoxes(this.Program);
        });
      }
    });
  }
  ngOnInit() {

    if (this.AutoLoad != "true") { return; }
    this.GetProgram();
  }
  ToggleVisibleBoxes(p: AssignedProgram) {

    p.Days.forEach(x => {
      x.AssignedProgramDayItem.filter(z => z.ItemType === ProgramDayItemEnum.superset).forEach((i: AssignedProgramDayItem) => {
        let element: AssignedSuperSet = i.ProgramItem;

        element.Exercises.forEach((e: AssignedSuperSetExercise) => {

          e.SetsAndReps.forEach((a: AssignedSuperSet_SetRep) => {
            if (a.AssignedWorkoutReps !== null || a.CompletedRepsAchieved !== null) {
              e.ShowReps = true;
            }
            if (a.AssignedWorkoutSets !== null || a.CompletedSetSets) {
              e.ShowSets = true;
            }
            if (a.AssignedRest !== null) {
              e.ShowRest = true;
            }
            if (a.AssignedWorkoutDistance !== null) {
              e.ShowDistance = true;
            }
            if (a.AssignedWorkoutMinutes !== null || a.AssignedWorkoutSeconds !== null) {
              e.ShowTime = true;
            }
            if (a.RepsAchieved === true) {
              e.ShowRepsAchieved = true;
            }

          });
        });

      });
    });

  }


  HasDayBeenMarkedCompleted() {

    let dayFound: boolean = false;
    this.Program.CompletedDays.forEach(x => {
      if (x.ProgramDayId === this.Program.Days[this.CurrentDay].Id && x.WeekNumber === this.Program.CurrentWorkoutWeekId) {
        dayFound = true;
      }
    });
    return dayFound;

  }

  UpdateMetric(targetMetric: AssignedMetric) {
    //so right now the program that is returned from ngOnInit is the actuall assigned program. SO this.Program.Id is the assigned program Id
    this.athleteService.AddCompletedMetric(targetMetric.CompletedWeight, targetMetric.MetricId, targetMetric.ProgramDayItemMetricId, targetMetric.DisplayWeekId, this.AthleteId, targetMetric.AssignedProgramId).subscribe(success => {
    });

  }
  CompleteSet(s) {

    var completedSet = new CompletedSet();
    completedSet.Position = s.PositionInSet;
    completedSet.Sets = s.AssignedSets;
    completedSet.Reps = s.AssignedReps;
    completedSet.Percent = s.AssignedWorkoutPercent;
    completedSet.Weight = s.CompletedSetWeight;
    completedSet.AssignedProgramId = s.AssignedProgramId;
    completedSet.OriginalSetId = s.OriginalSetId
    completedSet.AthleteId = this.AthleteId;
    this.athleteService.CompleteSet(completedSet).subscribe(success => { });
  }
  CompleteSuperSet(s) {
    var completedSet = new CompletedSuperSet();
    completedSet.Position = s.PositionInSet;
    completedSet.Sets = s.AssignedSets;
    completedSet.Reps = s.AssignedReps;
    completedSet.Percent = s.AssignedWorkoutPercent;
    completedSet.Weight = s.CompletedSetWeight;
    completedSet.AssignedProgramId = s.AssignedProgramId;
    completedSet.OriginalSuperSet_SetId = s.OriginalSuperSet_SetId
    completedSet.AthleteId = this.AthleteId;
    completedSet.RepsAchieved = s.CompletedRepsAchieved;
    this.athleteService.CompleteSuperSet(completedSet).subscribe(success => { }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Update UNSUCCESFULL", errorMessage, true)
    });
  }

  ShowWorkoutByWeek(currentWeekIdDisplay: number, programItem, programItemWeek) {

    if (currentWeekIdDisplay >= programItem.Weeks.length && programItemWeek.Position == (programItem.Weeks.length - 1)) {
      return true;
    }
    else if (currentWeekIdDisplay < 0 && programItemWeek.Position == 0) {
      return true;
    }
    else {
      return currentWeekIdDisplay == programItemWeek.Position
    }
  }
  ShowItemByWeek(weekIds, currentWeekIdDisplay: number) {
    var found = false;
    if (weekIds == currentWeekIdDisplay) {
      found = true;
    }
    for (var i = 0; i < weekIds.length; i++) {
      if (weekIds[i] == currentWeekIdDisplay || weekIds == currentWeekIdDisplay) {
        found = true;
      }
    }
    return found;
  }

  IncreaseWeekView() {
    if (this.Program.WeekCount == this.Program.CurrentWorkoutWeekId) {
      this.Program.CurrentWorkoutWeekId = 1;
    }
    else {
      this.Program.CurrentWorkoutWeekId++;
    }
  }
  DecreaseWeekView() {
    if (this.Program.CurrentWorkoutWeekId == 1) {
      this.Program.CurrentWorkoutWeekId = this.Program.WeekCount;
    }
    else {
      this.Program.CurrentWorkoutWeekId--;
    }
  }
  IncreaseDay() {

    if (this.CurrentDay + 1 === this.DayCount) {
      this.CurrentDay = 0;
    }
    else {
      this.CurrentDay++;
    }
    for (var i = 0; i < this.Program.Days.length; i++) {
      this.Program.Days[i].IsActive = (this.Program.Days[i].Position === this.CurrentDay);
    }
  }
  DecreaseDay() {

    if (this.CurrentDay === 0) {
      this.CurrentDay = this.DayCount - 1;
    }
    else {
      this.CurrentDay--;
    }
    for (var i = 0; i < this.Program.Days.length; i++) {
      this.Program.Days[i].IsActive = (this.Program.Days[i].Position === this.CurrentDay);
    }
  }
  MarkDayAsCompleted() {
    let programDayId = this.Program.Days[this.CurrentDay].Id;
    let weekCount = this.Program.CurrentWorkoutWeekId;
    this.pBuilderService.MarkDayAsCompleted(programDayId, weekCount, this.AthleteId).subscribe(x => { });
    this.Loading = true;
    this.GetProgram();
  }
  TabSwitch(day) {
    this.Program.Days.forEach(x => x.IsActive = false);
    day.IsActive = true;

  }
  AnswerOpenEndedQuestion(response, question, programItem, weekId) {
    //so right now the program that is returned from ngOnInit is the actuall assigned program. SO this.Program.Id is the assigned program Id
    this.surveyService.AnswerOpenEndedQuestion(response, question.QuestionId, programItem.Id, weekId, this.AthleteId).subscribe(x => { });
  }
  AnswerYesNoQuestion(value, question, programItem, weekId) {
    if (value == undefined) { return; }
    question.Answer = value;

    //so right now the program that is returned from ngOnInit is the actuall assigned program. SO this.Program.Id is the assigned program Id
    this.surveyService.AnswerYesNoQuestion(question.Answer, question.QuestionId, programItem.Id, weekId, this.AthleteId).subscribe(x => { });
  }
  AnswerScaleQuestion(scaleValue: number, question, programItem, weekId) {
    question.Answer = scaleValue;
    //so right now the program that is returned from ngOnInit is the actuall assigned program. SO this.Program.Id is the assigned program Id
    this.surveyService.AnswerScaleQuestion(scaleValue, question.QuestionId, programItem.Id, weekId, this.AthleteId).subscribe(x => { });

  }

  DisplayTargetVideo(videoURL: string) {
    this.targetVideoToDisplay = videoURL;
    this.ngxSmartModalService.setModalData({ url: videoURL }, 'exerciseVideoModal');
    this.ngxSmartModalService.open('exerciseVideoModal')
  }

  GenerateEmptyProgram(): AssignedProgram {
    var ret = new AssignedProgram();
    ret.Days = [{
      Id: 1,
      Position: 1,
      AssignedProgramDayItem: [],
      IsActive: false,
    }];
    return ret;
  }

  ShowVideo(url: string, videoProvider: number) {
    this.videoProvider = videoProvider;
    this.videoDigits = url.substring(url.lastIndexOf('/') + 1)
    this.ngxSmartModalService.open('videoModel');
  }

  PrintWorkout(programId: number) {
    this.athleteService.PrintWorkout(programId, this.AthleteId).subscribe(success => {
      this.DisplayMessage("PDF Is Being Generated", "Your Program Is Being Generated And Will Be Emailed To You", false)
    },

      error => {
        this.DisplayMessage("PDF GENERATING UNSUCCESSFULL", "There was an Error Generating your program, please contact Customer Support " + error.error, true)
      }
    )
  }
  ModifySnapShot() {
    this.router.navigate([`AthleteProfile/${this.AthleteId}/EditProgram/${this.AthleteId}`]);
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}

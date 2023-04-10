import { Component, OnInit, OnDestroy, ChangeDetectorRef, Input, ViewChild, EventEmitter } from '@angular/core';
import { Day } from '../../Models/Program/Day';
import { Program } from '../../Models/Program/Program'
import { ProgramBuilderService } from '../../Services/program-builder.service';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { ProgramDayItemEnum } from '../../Models/Program/ProgramDayItemEnum';
import { ProgramDayItemExercise } from "../../Models/Program/ProgramDayItemExercise";
import { Observable, Subscription, Subject } from 'rxjs';
import { ProgramDayItem } from '../../Models/Program/ProgramDayItem';
import { Exercise } from '../../Models/Exercise';
import { Set } from '../../Models/SetsAndReps/Set';
import { Movie } from '../../Models/Movie';
import { Week } from '../../Models/Week';
import { ITaggable } from '../../Interfaces/ITaggable';
import { WorkoutService } from '../../Services/workout.service';
import { Workout } from '../../Models/SetsAndReps/Workout';
import { ExerciseService } from '../../Services/exercise.service';
import { MetricsService } from '../../Services/metrics.service';
import { Metric } from '../../Models/Metric/Metric';
import { SurveyService } from '../../Services/survey.service';
import { Survey } from '../../Models/Survey';
import { ActivatedRoute, Router } from '@angular/router';
import { ProgramDayItemSuperSet, SuperSet_Exercise, SuperSet_Note } from '../../Models/Program/ProgramDayItemSuperSet';
import { SuperSet_Week } from '../../Models/SuperSet/SuperSet_Week';
import { SuperSet_Set } from '../../Models/SuperSet/SuperSet_Set';
import { ArraySortPipe, HideDeletedSortPipe, TagFilterPipe } from '../../Pipes';
import { ValidationErrorContainer } from '../../Models/Error/ProgramBuilderErrors';
import { fadeInAnimation } from '../../animation/fadeIn';
import { ScpTagInputComponent } from '../shared/scp-tag-input/scp-tag-input.component';
import { AlertMessage } from '../../Models/AlertMessage'
import { WorkoutDetails } from '../../Models/SetsAndReps/WorkoutDetails';
import { MetricComponent } from '../metric/metric.component';
import { Question } from '../../Models/Question';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { MultiMediaService } from '../../Services/multi-media.service';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-program-builder-week-view',
  templateUrl: './program-builder-week-view.component.html',
  styleUrls: ['./program-builder-week-view.component.less'],
  animations: [fadeInAnimation]
})

export class ProgramBuilderWeekViewComponent implements OnInit, OnDestroy {
  @ViewChild(ScpTagInputComponent) scpTagInputChild: ScpTagInputComponent;
  private scpTagInput: ScpTagInputComponent;
  public ShowError: boolean = false;
  public Errors: string[] = [];
  public Program: Program;
  public AllTags: TagModel[] = [];
  public ProgramService: ProgramBuilderService;
  public TagItems: TagModel[] = [];
  public newProgramTagItems: TagModel[] = [];
  public AllPrograms: Observable<Program[]>
  public AllExercises: Exercise[];
  public AllWorkouts: Workout[];
  public AllMetrics: Metric[];
  public AllSurveys: Survey[];
  public searchString: string;
  public WeekIds: number[] = [1];
  public subs: Subscription = new Subscription();
  public ShowMetricCreationWindow: boolean = false;
  public ShowCreateExerciseWindow: boolean = false;
  public ShowSetsRepsWindow: boolean = false;
  public ShowSurveyWindow: boolean = false;
  public ClearProgramConfirmation: boolean = false;
  public SelectedExercise: Exercise = new Exercise();
  public newExerciseTagItems: TagModel[] = [];
  public AlertMessages: AlertMessage[] = [];
  public ExerciseTags: TagModel[] = [];
  public SelectedWorkout: WorkoutDetails = new WorkoutDetails();
  public newWorkoutTagItems: TagModel[] = [];
  public SelectedMetric: Metric = new Metric();
  public metricController: MetricComponent;
  public AllMetricTags: TagModel[];
  public AllVideoTags: TagModel[];
  public newMovieTagItems: TagModel[];
  public AllQuestions: Question[];
  public WorkoutTags: TagModel[];
  public NewlyCreatedSurvey: Survey = new Survey();
  public SurveyTags: TagModel[];
  public done: Day[] = [new Day()];
  public List_Ids: string[] = ['DragAndDrop-1'];
  @Input() Model: boolean = false;
  public SavingProgram: boolean = false;
  public LoadingProgram: boolean = false;
  public AllMovies: Movie[];
  public ShowVideoCreationWindow: boolean = false;
  public ViewOnlyMode: boolean = false;
  public SelectedVideo: Movie = new Movie();
  public exerciseSubject: Subject<TagModel[]> = new Subject<TagModel[]>();
  public workoutSubject: Subject<TagModel[]> = new Subject<TagModel[]>();
  public surveySubject: Subject<TagModel[]> = new Subject<TagModel[]>();
  public videoSubject: Subject<TagModel[]> = new Subject<TagModel[]>();
  public metricSubject: Subject<TagModel[]> = new Subject<TagModel[]>();
  public DeleteDayConfirmation: boolean = false;
  public DeleteExerciseConfirmation: boolean = false;
  public ExerciseToDelete: Exercise = undefined
  public newMetricTagItems: TagModel[] = [];
  public AlertTagComponentReset: EventEmitter<boolean> = new EventEmitter();
  public WYSIWYGConfig: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    minHeight: '5rem',
    maxHeight: 'auto',
    width: 'auto',
    minWidth: '0',
    placeholder: 'Enter text here...',
    defaultParagraphSeparator: '',
    defaultFontName: '',
    defaultFontSize: '',
    fonts: [
      { class: 'arial', name: 'Arial' },
      { class: 'times-new-roman', name: 'Times New Roman' },
      { class: 'calibri', name: 'Calibri' },
      { class: 'comic-sans-ms', name: 'Comic Sans MS' }
    ],
    sanitize: true,
    toolbarPosition: 'top'
  };

  public d: Day;

  public MenuItems =
    [
      { Text: "Training Block", ItemType: ProgramDayItemEnum.superset, imageSrc: 'assets/lhs-icons/setsIcon.png', imageActiveSrc: 'assets/lhs-icons/setsIcon-active.png', isSelected: false },
      { Text: "Metric Block", ItemType: ProgramDayItemEnum.metric, imageSrc: 'assets/lhs-icons/stats.png', imageActiveSrc: 'assets/lhs-icons/stats-active.png', isSelected: false },
      { Text: "Survey Block", ItemType: ProgramDayItemEnum.survey, imageSrc: 'assets/lhs-icons/surveyIcon.png', imageActiveSrc: 'assets/lhs-icons/surveyIcon-active.png', isSelected: false },
      { Text: "Note Block", ItemType: ProgramDayItemEnum.note, imageSrc: 'assets/lhs-icons/NoteBlockIco.png', imageActiveSrc: 'assets/lhs-icons/NoteBlockIco-active.png', isSelected: false },
      { Text: "Multimedia Block", ItemType: ProgramDayItemEnum.video, imageSrc: '', imageActiveSrc: '', isSelected: false }];

  public multimediaActiveImgSrc: string = "../../../assets/lhs-icons/multimedia-active.png";
  public multimediaImgSrc: string = "../../../assets/lhs-icons/multimedia.png";
  public noteActiveImgSrc: string = "../../../assets/lhs-icons/document-active.png";
  public noteImgSrc: string = "../../../assets/lhs-icons/document.png";
  public surveyActiveImgSrc: string = "../../../assets/lhs-icons/surveyIcon-active.png";
  public surveyImgSrc: string = "../../../assets/lhs-icons/surveyIcon.png";
  public metricActiveImgSrc: string = "../../../assets/lhs-icons/stats-active.png";
  public metricImgSrc: string = "../../../assets/lhs-icons/stats.png";
  public trainingActiveImgSrc: string = "../../../assets/lhs-icons/setsIcon-active.png";
  public trainingImgSrc: string = "../../../assets/lhs-icons/setsIcon.png";

  constructor(ProgramBuilderService: ProgramBuilderService, public tagFilterPipe: TagFilterPipe, public tagService: TagService
    , private WorkoutService: WorkoutService, private exerciseService: ExerciseService, public metricService: MetricsService
    , public SurveyService: SurveyService, public workoutService: WorkoutService
    , private route: ActivatedRoute, private cd: ChangeDetectorRef, private sortPipe: ArraySortPipe, public router: Router, public hidePipe: HideDeletedSortPipe
    , public multimediaService: MultiMediaService,) {
    this.ProgramService = ProgramBuilderService;

    this.GetAllExercises();
    this.GetAllMovies();
    this.metricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, false); });
    this.WorkoutService.GetWorkouts().subscribe(x => { this.AllWorkouts = this.hidePipe.transform(x, false); });
    this.SurveyService.GetAllSurveys().subscribe(x => { this.AllSurveys = this.hidePipe.transform(x, false); });
    this.SurveyService.GetAllQuestions().subscribe(x => this.AllQuestions = x);
  }

  ShowModule(d) {
    d.isShown = !d.isShown;
  }

  SetActiveCard(day: Day) {
    this.Program.Days.forEach(x => x.IsActive = false);
    day.IsActive = true;
  }

  GenerateEmptyProgram(): Program {
    this.ProgramService.SetCurrentWeekCount(1);
    var ret = new Program();
    ret.WeekCount = this.ProgramService.GetCurrentWeekCount();

    let day1 = this.GenerateDay();
    // let day2 = this.GenerateDay();
    // let day3 = this.GenerateDay();
    // let day4 = this.GenerateDay();
    ret.Days = [day1];
    this.SetWeek(1);
    return ret;
  }

  GenerateDay() {
    const day = new Day();
    day.IsActive = true;
    day.Id = Math.round(Math.random() * 1000);

    const trainingItem = new ProgramDayItem();
    trainingItem.ItemType = ProgramDayItemEnum.superset;
    trainingItem.ProgramItem = new ProgramDayItemSuperSet();
    //@ts-ignore
    trainingItem.SuperSetDisplayTitle = "Training Block"
    const newExercise = new SuperSet_Exercise();
    newExercise.Name = "No Exercise Selected";
    newExercise.SelectedWorkout.Name = "No Workout Selected"
    newExercise.Id = Math.floor(Math.random() * 100000) + 1;
    newExercise.Position = 1;
    for (let i = 1; i <= this.ProgramService.GetCurrentWeekCount(); i++) {
      let newWeek = new SuperSet_Week();
      newWeek.SetsAndReps.push(new SuperSet_Set());
      newWeek.Id = i;
      newWeek.Position = i;
      newExercise.Weeks.push(newWeek);
    }

    trainingItem.ProgramItem.SelectedWorkout = new WorkoutDetails();
    trainingItem.ProgramItem.SelectedExercise = new Exercise();
    trainingItem.ProgramItem.Exercises.push(newExercise);
    day.Items.push(trainingItem);

    const metricItem = new ProgramDayItem();
    metricItem.ItemType = ProgramDayItemEnum.metric;
    metricItem.ProgramItem.SelectedMetric = new Metric();
    metricItem.ProgramItem.SelectedMetric.WeekIds = [];
    day.Items.push(metricItem);

    const surveyItem = new ProgramDayItem();
    surveyItem.ItemType = ProgramDayItemEnum.survey;
    surveyItem.ProgramItem.SelectedSurvey = new Survey();
    surveyItem.ProgramItem.SelectedSurvey.WeekIds = [];
    day.Items.push(surveyItem);

    const noteItem = new ProgramDayItem();
    noteItem.ItemType = ProgramDayItemEnum.note;
    noteItem.ProgramItem.SelectedNote = {};
    noteItem.ProgramItem.SelectedNote.Name = "";
    noteItem.ProgramItem.SelectedNote.Note = "";
    noteItem.ProgramItem.SelectedNote.WeekIds = [];
    day.Items.push(noteItem);

    const multimediaItem = new ProgramDayItem();
    multimediaItem.ItemType = ProgramDayItemEnum.video;
    multimediaItem.ProgramItem.SelectedVideo = new Movie();
    multimediaItem.ProgramItem.SelectedVideo.WeekIds = [];
    day.Items.push(multimediaItem);

    return day;
  }

  ngAfterViewInit() {
    this.cd.detectChanges();
  }

  SaveDraftProgram() {
    if (this.Program.CanModify) {
      this.ProgramService.SetSavedweekProgram(this.Program);
    }
  }

  GetDropDownIds() {
    let count = this.done.length;
    let ret = `'DragAndDrop-${this.Program.Days[0].Id}'`;
    for (let i = 1; i < this.Program.Days.length; i++) {
      ret = ret + ` ,'DragAndDrop-${this.Program.Days[i].Id}'`;
    }
  }

  ngOnDestroy() {
    this.Program = this.GenerateEmptyProgram();
  }

  ngOnInit() {
    this.Program = this.GenerateEmptyProgram();
    this.done = this.Program.Days;

    this.tagService.GetAllTags(TagType.Metric).subscribe(d => {
      this.AllMetricTags = [];
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllMetricTags.push(newTM)
      }
    });
    this.tagService.GetAllTags(TagType.Exercise).subscribe(d => {
      this.ExerciseTags = [];
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.ExerciseTags.push(newTM)
      }
    });
    this.tagService.GetAllTags(TagType.Movie).subscribe(d => {
      this.AllVideoTags = [];
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllVideoTags.push(newTM)
      }
    });
    this.tagService.GetAllTags(TagType.Workout).subscribe(d => {
      this.WorkoutTags = [];
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.WorkoutTags.push(newTM)
      }
    });
    this.tagService.GetAllTags(TagType.Survey).subscribe(d => {
      this.SurveyTags = [];
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.SurveyTags.push(newTM)
      }
    });

    // this.route.params.subscribe(params => {
    //   if (params['id'] !== undefined) {
    //     this.LoadingProgram = true;
    //     this.ProgramService.GetProgram(params['id']).subscribe(x => {
    //       this.Program = x;
    //       this.SetWeek(this.Program.WeekCount)
    //       this.LoadingProgram = false;
    //       this.Program.Days.forEach(x => {
    //         x.Items.sort((a, b) => a.Position - b.Position);
    //       });
    //       this.done = this.Program.Days;
    //     });
    //     this.ProgramService.ClearSavedWeekProgram();
    //   }
    //   else {
    //     this.Program = this.ProgramService.GetSavedWeekProgram();
    //     if (this.Program === null) {
    //       this.Program = this.GenerateEmptyProgram();
    //     }
    //     else {
    //       this.SetWeek(this.Program.WeekCount)
    //     }
    //     this.Program.Days.forEach(x => {
    //       x.Items.sort((a, b) => a.Position - b.Position);
    //     });
    //     this.done = this.Program.Days;
    //   }

    //   this.GetAllExercises();
    //   this.tagService.GetAllTags(TagType.Program).subscribe(d => {
    //     for (var i = 0; i < d.length; i++) {
    //       var newTM = new TagModel();
    //       newTM.display = d[i].Name;
    //       newTM.value = d[i].Id;
    //       this.AllTags.push(newTM)
    //     }
    //   });
    // });
  }

  MoveSuperSetExerciseDown(exercise: SuperSet_Exercise, superset: ProgramDayItemSuperSet) {
    if (exercise.Position == superset.Exercises.length) { return; }
    superset.Exercises[exercise.Position + 1].Position = exercise.Position;
    exercise.Position++;
    superset.Exercises = this.sortPipe.transform(superset.Exercises, 'Position');
  }

  MoveSuperSetExerciseUp(exercise: SuperSet_Exercise, superset: ProgramDayItemSuperSet) {
    if (exercise.Position == 1) { return; }
    superset.Exercises[exercise.Position - 1].Position = exercise.Position;
    exercise.Position--;
    superset.Exercises = this.sortPipe.transform(superset.Exercises, 'Position');
  }

  RemoveSuperSetExercise(exercise: SuperSet_Exercise, superset: ProgramDayItemSuperSet) {
    let foundAt = -1;
    for (let index = 0; index < superset.Exercises.length; index++) {
      if (superset.Exercises[index].Id == exercise.Id) {
        foundAt = index;
      }
    }

    for (let index = foundAt + 1; index < superset.Exercises.length; index++) {
      superset.Exercises[index].Position--;
    }
    superset.Exercises.splice(foundAt, 1);
  }

  UpdateSurvey(event) {
    let survey = event.event;
    let programDayItem = event.module

    if (event.event === undefined) {
      programDayItem.ProgramItem.SelectedSurvey = { WeekIds: [] };
      return;
    }
    var oldWeekIds = programDayItem.ProgramItem.SelectedSurvey.WeekIds;
    programDayItem.ProgramItem.SelectedSurvey = survey;
    this.SurveyService.GetAllSurveyQuestions(survey.Id).subscribe(x => programDayItem.ProgramItem.SelectedSurvey.Questions = x);
    programDayItem.ProgramItem.SelectedSurvey.WeekIds = oldWeekIds;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  UpdateMetric(event) {
    let metric = event.event;
    let programDayItem = event.module
    if (event.event === undefined) {
      programDayItem.ProgramItem.SelectedMetric = { WeekIds: [] };
      return;
    }
    var oldWeekIds = programDayItem.ProgramItem.SelectedMetric.WeekIds;
    programDayItem.ProgramItem.SelectedMetric = metric;
    programDayItem.ProgramItem.SelectedMetric.WeekIds = oldWeekIds;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  GetAllExercises(): void {
    this.exerciseService.GetAllExercises().subscribe(x => { this.AllExercises = this.hidePipe.transform(x, false); });
  }

  AddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Program;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
    this.newProgramTagItems.push(s);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  RemoveTag(s: TagModel) {
    var index = this.TagItems.findIndex(x => { return x.display == s.display });
    this.newProgramTagItems.splice(index, 1);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  SetSelectedTag(tag: TagModel) {
    this.TagItems = [];
    this.TagItems.push(tag);
  }

  ToggleResetProgramConfirm() {
    this.ClearProgramConfirmation = !this.ClearProgramConfirmation;
  }

  ResetProgram() {
    this.Program = this.GenerateEmptyProgram();
    this.Errors = [];
    this.done = this.Program.Days;
    this.AlertTagComponentReset.emit(true);
    this.ProgramService.ClearSavedWeekProgram()
    this.ClearProgramConfirmation = false;
  }

  CloseInitialMenu() {
  }

  SetSelectedProgram(selectedProgram: Program) {
    this.Program = selectedProgram;
  }

  IsDisplayWeekInArray(weekIds: number[], id: number) {
    return weekIds.indexOf(id) > -1;
  }

  SetWeek(weekCount: number) {
    this.ProgramService.SetCurrentWeekCount(weekCount);
    this.WeekIds = [];
    for (var i = 1; i <= weekCount; i++) {
      this.WeekIds.push(i)
    }
  }

  AddWeek(days: Day[]) {
    this.ProgramService.SetCurrentWeekCount(this.ProgramService.GetCurrentWeekCount() + 1);
    this.Program.WeekCount = this.ProgramService.GetCurrentWeekCount();
    this.WeekIds.push(this.ProgramService.GetCurrentWeekCount())
    days.forEach(x => {
      x.Items.forEach(y => {
        if (y.ItemType == ProgramDayItemEnum.workout) {
          var newWeek = new Week();
          newWeek.SetsAndReps.push(new Set());
          newWeek.Id = this.ProgramService.GetCurrentWeekCount();
          newWeek.Position = this.ProgramService.GetCurrentWeekCount();
          <ProgramDayItemExercise>y.ProgramItem.Weeks.push(newWeek);
        }
        else if (y.ItemType == ProgramDayItemEnum.superset) {
          <ProgramDayItemSuperSet>y.ProgramItem.Exercises.forEach(function (x: SuperSet_Exercise) {
            var s = new SuperSet_Week()
            s.Id = this.Program.WeekCount;
            s.Position = this.Program.WeekCount;
            s.ParentWorkoutId = 0;
            s.SetsAndReps = [];
            s.SetsAndReps.push(new SuperSet_Set());
            x.Weeks.push(s);
          }, this);
        }
      });
    });
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  RemoveWeekIdFromWeekIds(targetId: number, weekIds: number[]) {
    for (var i = 0; i < weekIds.length; i++) {
      if (weekIds[i] == targetId) {
        weekIds.splice(i, 1);
      }
    }
  }

  RemoveWeek(days: Day[]) {
    if (this.ProgramService.GetCurrentWeekCount() <= 1) return;
    this.ProgramService.SetCurrentWeekCount(this.ProgramService.GetCurrentWeekCount() - 1);
    this.Program.WeekCount = this.ProgramService.GetCurrentWeekCount();
    this.WeekIds.pop();
    days.forEach(x => {
      x.Items.forEach(y => {
        if (y.ItemType == ProgramDayItemEnum.workout) {
          <ProgramDayItemExercise>y.ProgramItem.Weeks.pop();
        }

        else if (y.ItemType == ProgramDayItemEnum.metric) {
          this.RemoveWeekIdFromWeekIds(this.ProgramService.GetCurrentWeekCount(), y.ProgramItem.SelectedMetric.WeekIds)
        }

        else if (y.ItemType == ProgramDayItemEnum.survey) {
          this.RemoveWeekIdFromWeekIds(this.ProgramService.GetCurrentWeekCount(), y.ProgramItem.SelectedSurvey.WeekIds)
        }

        else if (y.ItemType == ProgramDayItemEnum.note) {
          this.RemoveWeekIdFromWeekIds(this.ProgramService.GetCurrentWeekCount(), y.ProgramItem.SelectedNote.WeekIds)
        }
        else if (y.ItemType == ProgramDayItemEnum.superset) {
          <ProgramDayItemSuperSet>y.ProgramItem.Exercises.forEach(function (x: SuperSet_Exercise) {
            x.Weeks.pop();
          });
        }
      });
    });
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  AddDay(days: Day[]) {
    days.forEach(x => x.IsActive = false);
    days.push(this.GenerateDay());
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  RemoveDay(targetDay: Day, days: Day[]) {
    var index = days.findIndex(x => x.Id == targetDay.Id);
    days.splice(index, 1);

    for (var i = 0; i < days.length; i++) {
      days[i].Position = i + 1;
    }
    days[0].IsActive = true;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  RemoveLastDay(days: Day[]) {
    if (days.length == 1) { return; }
    days.pop();
    for (var i = 0; i < days.length; i++) {
      days[i].IsActive = i == 0;
      days[i].Position = i;
    }
  }

  SaveProgram(targetProgram: Program, associatedTags: TagModel[]) {
    //console.log(targetProgram)
    this.SavingProgram = true;
    this.Errors = [];
    if (targetProgram.Name == '' || targetProgram.Name == undefined) {
      this.Errors.push("The Program Needs To Have A Name To Save");
      this.DisplayMessage("SAVE UNSUCCESSFULL", "The Program Needs To Have A Name To Save", true);
    }
    var foundAnItemToSave = false;
    targetProgram.Days.forEach(function (y) {
      if (y.Items.length > 0) {
        foundAnItemToSave = true;
        return;
      }
    });

    if (!foundAnItemToSave) {
      this.Errors.push("The Program Doesn't Have Anything To Save");
      this.DisplayMessage("SAVE UNSUCCESSFULL", "The Program Doesn't Have Anything To Save", true);
    }

    if (this.Errors.length > 0) {
      this.SavingProgram = false;
      return;
    }

    this.ShowError = false;
    var newProg: Program = new Program();
    newProg.Id = targetProgram.Id;
    newProg.Name = targetProgram.Name;
    newProg.Days = targetProgram.Days;
    newProg.WeekCount = this.ProgramService.GetCurrentWeekCount();
    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Program;

          newProg.Tags.push(newTag);
        }
      });
    });

    if (newProg.Id > 0) {
      try {
        this.ProgramService.UpdateProgram(newProg).subscribe(
          success => {
            this.GenerateEmptyProgram();
            this.newProgramTagItems = []
            this.SavingProgram = false;
            this.router.navigate(["/Program"]);
          },
          error => {
            var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
            this.DisplayMessage("SAVE UNSUCCESSFULL", errorMessage, true)
            this.SavingProgram = false;
          });
      }
      catch (error) {
        if (error.AllValidationErrors != undefined) {
          this.ShowError = true;
          this.SavingProgram = false;
          <ValidationErrorContainer><unknown>error.AllValidationErrors.forEach(x => {
            this.Errors.push(`On Day ${x.DayNumber + 1} ${x.ValidationErrorMessage}`);
          });
        }
      }
    }
    else {
      //console.log(newProg)
      try {
        this.ProgramService.CreateProgram(newProg).subscribe(
          success => {
            this.newProgramTagItems = []
            this.SavingProgram = false;
            this.ProgramService.ClearSavedWeekProgram();

            this.ResetProgram();
            this.router.navigate(["/Program"]);
          },
          error => {
            var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
            this.DisplayMessage("SAVE UNSUCCESSFULL", errorMessage, true)
            this.SavingProgram = false;
          });
      }
      catch (error) {
        if (error.AllValidationErrors != undefined) {
          this.ShowError = true;
          this.SavingProgram = false;
          this.DisplayMessage("SAVE UNSUCCESSFULL", error.AllValidationErrors, true);
          <ValidationErrorContainer><unknown>error.AllValidationErrors.forEach(x => {
            this.Errors.push(`On Day ${x.DayNumber} ${x.ValidationErrorMessage}`);
          });
        }
        else {
          console.log(error)
          this.SavingProgram = false;
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("SAVE UNSUCCESSFULL", errorMessage, true)
        }
      }
    }
  }

  IsEmptyMenu(d: Day) {
    return d.Items.length == 0;
  }

  RemoveItem(programDays: Day[], targetItem: ProgramDayItem) {
    let found = programDays.find(x => x.IsActive) || new Day();
    let itemToRemove: number = -1;
    itemToRemove = found.Items.findIndex(x => x.Id === targetItem.Id);
    found?.Items.splice(itemToRemove, 1)
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  ShowDayDetails(dayItem: ProgramDayItem) {
    dayItem.ShowDetails = !dayItem.ShowDetails;
  }

  SelectSearch(term: string, item: ITaggable) {
    var terms = term.split(' ');
    term = term.toLocaleLowerCase();
    var foundCount = 0;
    var termsCount = terms.length;
    for (var x = 0; x < terms.length; x++) {
      if (terms[x].length == 0) { foundCount++; continue; }
      for (var i = 0; i < item.Tags.length; i++) {
        if (item.Tags[i].Name.toLocaleLowerCase().indexOf(terms[x]) > -1) {
          foundCount++;
        }
      }
    }
    return item.Name.toLocaleLowerCase().indexOf(term) > -1 || foundCount == termsCount;
  }

  UpdateSuperSetExercise(event) {
    if (event.event === undefined) {
      event.module.SelectedExercise = undefined;
      return;
    }
    event.module.ExerciseId = event.event.Id
    event.module.SelectedExercise = event.event;
    event.module.SelectedExercise.ExerciseId = event.event.Id;
    event.module.SelectedExercise.Name = event.event.Name;
    this.ProgramService.SetSavedweekProgram(this.Program);

    // let exercise = event.event;
    // let programDayItem = event.module
    // if (event.event === undefined) {
    //   programDayItem.ProgramItem.SelectedExercise = undefined;
    //   return;
    // }
    // programDayItem.ProgramItem.SelectedExercise = exercise;
    // this.ProgramService.SetSavedweekProgram(this.Program);
  }

  UpdateSuperSet_SetsReps(event) {
    let e = event.module;
    let setRep = event.event;
    if (event.event === undefined) {
      event.module.SelectedExercise = undefined;
      for (var i = 0; i < this.ProgramService.GetCurrentWeekCount(); i++) {
        e.Weeks[i].SetsAndReps = [];
        e.Weeks[i].SetsAndReps.push(new SuperSet_Set());
      }
      return;
    }
    this.WorkoutService.GetWorkoutOutDetails(setRep.Id).subscribe(x => {
      e.SelectedWorkout = x;
      e.SelectedWorkout.ShowRestBox = x.Rest !== undefined && x.Rest !== null;

      for (var i = 0; i < this.ProgramService.GetCurrentWeekCount(); i++) {
        e.Weeks[i].SetsAndReps = [];
        e.Weeks[i].Position = i + 1;

        if (x.TotalWorkout[i] == undefined) {
          e.Weeks[i].SetsAndReps.push(new SuperSet_Set());
        }
        else {
          for (var t = 0; t < x.TotalWorkout[i].SetsAndReps.length; t++) {

            var newS = new SuperSet_Set();
            var targetStuff = x.TotalWorkout[i].SetsAndReps[t];
            newS.Id = targetStuff.Id;
            newS.ParentSuperSetWeekId = targetStuff.ParentWeekId;
            newS.Percent = targetStuff.Percent;
            newS.Position = t + 1;
            newS.Reps = targetStuff.Reps;
            newS.Sets = targetStuff.Sets;
            newS.Weight = targetStuff.Weight;
            newS.Minutes = targetStuff.Minutes;
            newS.Other = targetStuff.Other;
            newS.Seconds = targetStuff.Seconds;
            newS.Distance = targetStuff.Distance;
            newS.RepsAchieved = targetStuff.RepsAchieved;

            if (newS.Reps !== undefined && newS.Reps !== null) {
              e.SelectedWorkout.ShowRepsBox = true;
            }
            if (newS.Percent !== undefined && newS.Percent !== null) {
              e.SelectedWorkout.ShowPercentageBox = true;
            }
            if (newS.Sets !== undefined && newS.Sets !== null) {
              e.SelectedWorkout.ShowSetsBox = true;
            }

            e.SelectedWorkout.ShowWeight = x.ShowWeight;

            if (newS.Minutes !== undefined && newS.Minutes !== null) {
              e.SelectedWorkout.ShowTimeBox = true;
            }
            if (newS.Seconds !== undefined && newS.Seconds !== null) {
              e.SelectedWorkout.ShowTimeBox = true;
            }
            if (newS.Other !== undefined && newS.Other !== null) {
              e.SelectedWorkout.ShowOtherBox = true;
            }
            if (newS.Distance !== undefined && newS.Distance !== null) {
              e.SelectedWorkout.ShowDistanceBox = true;
            }
            e.SelectedWorkout.ShowRepsAchievedBox = false;
            if (newS.RepsAchieved !== undefined && newS.RepsAchieved !== null) {
              e.SelectedWorkout.ShowRepsAchievedBox = newS.RepsAchieved;
            }
            e.Weeks[i].SetsAndReps.push(newS);
          }
          e.Weeks[i].SetsAndReps.push(new SuperSet_Set());//let them add sets and reps on the bottomn
        }
      }
      this.ProgramService.SetSavedweekProgram(this.Program);
    });
  }

  ToggleWeekIds(id: number, WeekIds: number[]) {
    var found = false;
    for (var i = 0; i < WeekIds.length; i++) {
      if (WeekIds[i] == id) {
        WeekIds.splice(i, 1);
        found = true;
      }
    }
    if (!found) {
      WeekIds.push(id)
    }
    this.ProgramService.SetSavedweekProgram(this.Program)
  }

  RemoveWeekFromSetAndRep(target: Week[]) {
    if (target.length > 0) target.pop();
  }

  CheckAddAnotherSet(targetWeek: Week, workout: WorkoutDetails) {
    var removingASet = false;
    for (let i = 0; i < targetWeek.SetsAndReps.length - 1; i++) {
      let targetSet = targetWeek.SetsAndReps[i];
      if (
        // @ts-ignore
        (!workout.ShowPercentageBox || (targetSet.Percent === undefined || targetSet.Percent === null || targetSet.Percent === '')) &&
        // @ts-ignore
        (!workout.ShowSetsBox || (targetSet.Sets === undefined || targetSet.Sets === null || targetSet.Sets === '')) &&
        // @ts-ignore
        (!workout.ShowRepsBox || (targetSet.Reps === undefined || targetSet.Reps === null || targetSet.Reps === '')) &&
        // @ts-ignore
        (!workout.ShowTimeBox || ((targetSet.Minutes === undefined || targetSet.Minutes === null || targetSet.Minutes === '') && (targetSet.Seconds === undefined || targetSet.Seconds === null || targetSet.Seconds === ''))) &&
        // @ts-ignore
        (!workout.ShowDistanceBox || (targetSet.Distance === undefined || targetSet.Distance === null || targetSet.Distance === ''))
      ) {
        targetWeek.SetsAndReps.splice(i, 1);
        removingASet = true;
      }
    }

    if (!removingASet) {
      let lastSet = targetWeek.SetsAndReps[targetWeek.SetsAndReps.length - 1]
      if (!(workout.ShowSetsBox && (lastSet.Sets === undefined || lastSet.Sets === null)) &&
        !(workout.ShowRepsBox && (lastSet.Reps === undefined || lastSet.Reps === null)) &&
        !(workout.ShowTimeBox && ((lastSet.Minutes === undefined || lastSet.Minutes === null) || (lastSet.Seconds === undefined || lastSet.Seconds === null))) &&
        !(workout.ShowDistanceBox && (lastSet.Distance === undefined || lastSet.Distance === null))) {
        this.AddSetToWeek(targetWeek);
      }
    }
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  AddNewExerciseToSuperSet(targetSuperSetProgramDayItem) {
    var newExercise = new SuperSet_Exercise();
    newExercise.Name = "No Exercise Selected";
    newExercise.SelectedWorkout.Name = "No Workout Selected"
    newExercise.Position = targetSuperSetProgramDayItem.ProgramItem.Exercises.length + 1;
    for (var i = 1; i <= this.Program.WeekCount; i++) {
      let newWeek = new SuperSet_Week();
      newWeek.SetsAndReps.push(new SuperSet_Set());
      newWeek.Id = i;
      newWeek.Position = i;
      newExercise.Weeks.push(newWeek);
    }
    newExercise.Id = Math.round(Math.random() * 1000);
    targetSuperSetProgramDayItem.ProgramItem.Exercises.push(newExercise);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  AddNewNoteToSuperSet(targetSuperSetProgramDayItem) {
    var newNote = new SuperSet_Note();
    newNote.Id = Math.round(Math.random() * 1000)
    newNote.Position = targetSuperSetProgramDayItem.ProgramItem.Notes.length + 1;
    targetSuperSetProgramDayItem.ProgramItem.Notes.push(newNote);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  RemoveNote(targetNoteId: number, superset: ProgramDayItemSuperSet) {
    let foundAt = -1;
    for (let index = 0; index < superset.Notes.length; index++) {
      if (superset.Notes[index].Id == targetNoteId) {
        foundAt = index;
      }
    }

    for (let index = foundAt + 1; index < superset.Notes.length; index++) {
      superset.Notes[index].Position--;
    }
    superset.Notes.splice(foundAt, 1);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  UpdateSelected(selectedItem) {
    this.MenuItems.forEach(x => { x.isSelected = false });
    selectedItem.isSelected = true;
  }

  TabSwitch(day) {
    this.Program.Days.forEach(x => x.IsActive = false);
    day.IsActive = true;
  }

  ShowConfirmExit() {
  }

  CancelExit() {
  }

  Exit() {
    this.router.navigate(['/Program'])
  }

  MoveSelectedDayLeft() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    if (this.Program.Days[targetIndex].Position === 0 || this.Program.Days.length === 1) {
      return;
    }
    var targetPosition = this.Program.Days[targetIndex].Position;
    let targetToDecrement = this.Program.Days.findIndex(x => x.Position === targetPosition - 1);

    this.Program.Days[targetIndex].Position = this.Program.Days[targetIndex].Position - 1;
    this.Program.Days[targetToDecrement].Position = this.Program.Days[targetToDecrement].Position + 1;
    let test = this.Program.Days.splice(0, this.Program.Days.length);
    this.Program.Days = test;
    this.done = test;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  MoveSelectedDayRight() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    if (this.Program.Days[targetIndex].Position === this.Program.Days.length - 1 || this.Program.Days.length === 1) {
      return;
    }

    var targetPosition = this.Program.Days[targetIndex].Position;
    let targetToIncrement = this.Program.Days.findIndex(x => x.Position === targetPosition + 1);

    this.Program.Days[targetIndex].Position = this.Program.Days[targetIndex].Position + 1;

    this.Program.Days[targetToIncrement].Position = this.Program.Days[targetToIncrement].Position - 1;
    let test = this.Program.Days.splice(0, this.Program.Days.length);
    this.Program.Days = test;
    this.done = test;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  ToggleDeleteDayConfirmation() {
    this.DeleteDayConfirmation = !this.DeleteDayConfirmation;
  }

  DeleteDay() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    let targetPosition = this.Program.Days[targetIndex].Position;

    this.Program.Days.forEach(x => {
      if (x.Position > targetPosition) {
        x.Position = x.Position - 1;
      }
    });
    this.Program.Days.splice(targetIndex, 1)
    if (this.Program.Days.length > 0) {
      this.Program.Days[0].IsActive = true;
    }
    this.ToggleDeleteDayConfirmation();
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  CopyDay(day: Day) {
    this.SetActiveCard(day);
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    let copiedDay = JSON.parse(JSON.stringify(this.Program.Days[targetIndex]));
    (copiedDay as Day).IsActive = false;
    (copiedDay as Day).Position = this.Program.Days.length;
    (copiedDay as Day).Id = Math.floor(Math.random() * 100000) + 1;
    this.Program.Days.push(copiedDay);
    this.List_Ids.push('DragAndDrop-' + (copiedDay as Day).Id);
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  ToggleMetricCreationWindow() {
    this.ShowMetricCreationWindow = !this.ShowMetricCreationWindow;
    this.SelectedMetric = new Metric();
    this.newMetricTagItems = [];
  }

  ToggleShowCreateExerciseWindow() {
    this.ShowCreateExerciseWindow = !this.ShowCreateExerciseWindow;
    this.newExerciseTagItems = [];
  }

  ToggleShowSetsRepsWindow() {
    this.ShowSetsRepsWindow = !this.ShowSetsRepsWindow;
    this.SelectedWorkout.TotalWorkout = [];
    this.AddWeekToSetsAndReps(this.SelectedWorkout.TotalWorkout);
  }

  AddWeekToSetsAndReps(target: Week[]) {
    var newWeek = new Week();
    newWeek.Position = this.SelectedWorkout.TotalWorkout.length + 1;
    newWeek.SetsAndReps = [];
    this.AddSetToWeek(newWeek);
    target.push(newWeek);
  }

  AddSetToWeek(targetWeek: Week) {
    var set = new Set();
    set.Position = targetWeek.SetsAndReps.length;
    targetWeek.SetsAndReps.push(set);
  }

  ToggleShowCreateSurveyWindow() {
    this.ShowSurveyWindow = !this.ShowSurveyWindow;
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  SaveExercise(exer: Exercise, associatedTags: TagModel[]) {
    if (exer.Name == '' || exer.Name == undefined) {
      this.DisplayMessage('Exercise Cannot Be Created', 'This Exercises doesnt have a name', true)
      return;
    }

    var newExer: Exercise = new Exercise();
    newExer.IsDeleted = false;
    newExer.Name = exer.Name;
    newExer.Percent = exer.Percent
    newExer.PercentMetricCalculationId = exer.PercentMetricCalculationId;
    newExer.VideoURL = exer.VideoURL

    newExer.Notes = exer.Notes;
    newExer.Id = 0;
    newExer.CreatedUserId = 0;
    associatedTags.forEach((value, index) => {
      this.ExerciseTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newExer.Tags.push(newTag);
        }
      });
    });

    this.exerciseService.CreateExercise(newExer)
      .subscribe(
        success => {
          this.SelectedExercise = new Exercise();
          this.newExerciseTagItems = [];
          this.scpTagInputChild.ClearTags();
          this.GetAllExercises();
          this.ToggleShowCreateExerciseWindow()
          this.DisplayMessage('Exercise Created Successfully', 'Exercise Updated Successfully', false)
        },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage('Exercise Created Unsuccessfully', errorMessage, true)
        });
  }

  AddNewEditExerciseTags(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Exercise;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTM = new TagModel();
        newTM.display = s.display;
        newTM.value = data;
        this.AllTags.push(newTM)
        this.ExerciseTags.push({ display: s.display, value: data })
        this.exerciseSubject.next(this.ExerciseTags);
      });
    }
    this.newExerciseTagItems.push(s);
  }

  SaveForTrainingBlock(data: any) {
    this.SaveWorkout(data.targetWorkout, data.associatedTags);
  }

  SaveWorkout(targetWorkout: WorkoutDetails, associatedTags: TagModel[]) {
    if (targetWorkout.Name == '' || targetWorkout.Name == undefined) {
      this.DisplayMessage("Save UNSUCCESSFULL", "The Set and Rep Needs To Have A Name To Save", true)
      return;
    }
    targetWorkout.Tags = [];
    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Workout;
          targetWorkout.Tags.push(newTag);
        }
      });
    });

    this.workoutService.CreateWorkout(targetWorkout).subscribe(
      success => {
        this.DisplayMessage("Save SUCCESSFULL", "Workout Saved", false)
        this.newWorkoutTagItems = [];//overrideing the viewCreateNewWorkout because we want a clean tag list
        this.WorkoutService.GetWorkouts().subscribe(x => { this.AllWorkouts = this.hidePipe.transform(x, false); });
        this.ToggleShowSetsRepsWindow();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Save UNSUCCESSFULL", errorMessage, true)
      });
  }

  SaveForMetricBlock(data: any) {
    this.SaveMetric(data.targetMetric, data.associatedTags);
  }

  SaveMetric(targetMetric: Metric, associatedTags: TagModel[]) {
    if (targetMetric.Name == undefined || targetMetric.Name == '') {
      this.DisplayMessage('Metric Saved Unsuccessfully', 'The Metric Name cannot be empty', true)
      return;
    }

    var newMetric = new Metric();
    newMetric.Id = targetMetric.Id;
    newMetric.Name = targetMetric.Name;
    newMetric.UnitOfMeasurementId = targetMetric.UnitOfMeasurementId
    associatedTags.forEach((value, index) => {
      this.AllMetricTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Metric;
          newMetric.Tags.push(newTag);
        }
      });
    });

    this.metricService.CreateMetric(newMetric).subscribe(success => {
      this.SelectedMetric = new Metric();
      this.DisplayMessage('Metric Saved Successfully', 'Metric Saved Successfully', false)
      this.metricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, false); });
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage('Metric Saved Unsuccessfully', errorMessage, true)
    });
  }

  SaveForSurveyBlock(data: Survey) {
    this.SaveSurvey(data);
  }

  SaveSurvey(newSurvey: Survey) {
    this.SurveyService.CreateSurvey(newSurvey).subscribe(success => {
      this.ToggleShowCreateSurveyWindow();
      this.DisplayMessage('Survey Saved Successfully', 'Survey Saved Successfully', false)
      this.SurveyService.GetAllSurveys().subscribe(x => { this.AllSurveys = this.hidePipe.transform(x, false); });
      this.NewlyCreatedSurvey = new Survey();
      this.SurveyTags = [];
    }
      , error => {
        this.ToggleShowCreateSurveyWindow();
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Survey Save UnSuccessfully', errorMessage, true)
      }
    )
  }
  GetAllMovies() {
    this.multimediaService.GetAllMovies().subscribe(x => {
      this.AllMovies = x;
    });
  }
  UpdateVideo(event) {
    let movie = event.event;
    let programDayItem = event.module
    if (event.event === undefined) {
      programDayItem.ProgramItem.SelectedVideo = { WeekIds: [] };
      return;
    }
    var oldWeekIds = programDayItem.ProgramItem.SelectedVideo.WeekIds;
    programDayItem.ProgramItem.SelectedVideo = movie;
    programDayItem.ProgramItem.SelectedVideo.WeekIds = oldWeekIds;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }

  HideCreateMultimediaForm(id: number) {
    ($('.video-' + id) as any).collapse("hide");
  }

  HideCreateSurveyForm(id: number) {
    ($('.survey-' + id) as any).collapse("hide");
  }

  HideCreateMetricForm(id: number) {
    ($('.metric-' + id) as any).collapse("hide");
  }

  HideCreateWorkoutForm(id: number) {
    ($('.workout-' + id) as any).collapse("hide");
  }

  HideCreateExerciseForm(id: number) {
    ($('.exercise-' + id) as any).collapse("hide");
  }

  ToggleVideoCreationWindow() {
    this.ShowVideoCreationWindow = !this.ShowVideoCreationWindow;
  }

  SaveForMultimediaBlock(data: any) {
    console.log(data)
    this.SaveMovie(data.targetMovie, data.tags);
  }

  SaveMovie(targetMovie: Movie, tags: TagModel[]) {
    if (targetMovie.URL === '' || targetMovie.URL === undefined) {
      return;
    }
    if (targetMovie.Name === '' || targetMovie.Name === undefined) {
      return;
    }

    if (tags !== undefined && tags !== null) {
      tags.forEach((value, index) => {
        this.AllVideoTags.forEach((sourceTag: TagModel) => {
          if (sourceTag.display == value.display) {
            var newTag = new Tag();
            newTag.Id = sourceTag.value;
            newTag.Name = sourceTag.display;
            newTag.Type = TagType.Movie;
            targetMovie.Tags.push(newTag);
          }
        });
      });
    }
    this.multimediaService.CreateMovie(targetMovie).subscribe(
      success => {
        this.newMovieTagItems = [];
        this.scpTagInputChild.ClearTags();
        this.DisplayMessage('Video Saved Successfully', 'Video Saved Successfully', false);
        this.GetAllMovies();
        this.ToggleVideoCreationWindow();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Movie Saved Unsuccessfully', error, true)
      });
  }

  ToggleDeleteExerciseConfirmation(exericse: Exercise, programItem: ProgramDayItemExercise) {
    this.DeleteExerciseConfirmation = !this.DeleteExerciseConfirmation
    this.ExerciseToDelete = exericse;
  }

  DeleteExercise() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    this.Program.Days[targetIndex].Items.forEach(x => {
      if (x.ProgramItem.Exercises !== undefined) {
        targetSpot = -1
        var targetSpot = (x.ProgramItem.Exercises as Exercise[]).findIndex(targetToDelete => targetToDelete.Id === this.ExerciseToDelete.Id);
        if (targetSpot !== -1) {
          (x.ProgramItem.Exercises as Exercise[]).splice(targetSpot, 1)
        }
      }
    });
    this.DeleteExerciseConfirmation = !this.DeleteExerciseConfirmation;
    this.ExerciseToDelete = null;
    this.ProgramService.SetSavedweekProgram(this.Program);
  }
}

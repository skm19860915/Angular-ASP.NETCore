import { Component, OnInit, OnDestroy, ChangeDetectorRef, Input, ViewChild, ViewEncapsulation, Output, EventEmitter } from '@angular/core';
import { Day } from '../../Models/Program/Day';
import { Program } from '../../Models/Program/Program'
import { ProgramBuilderService } from '../../Services/program-builder.service';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { ProgramDayItemEnum } from '../../Models/Program/ProgramDayItemEnum';
import { ProgramDayItemExercise } from "../../Models/Program/ProgramDayItemExercise";
import { Observable, Subscription, interval, Subject } from 'rxjs';
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
import { take } from 'rxjs/operators';
import { WorkoutDetails } from '../../Models/SetsAndReps/WorkoutDetails';
import { UnitOfMeasurement } from '../../Models/UnitOfMeasurement';
import { MetricComponent } from '../metric/metric.component';
import { Question } from '../../Models/Question';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { MultiMediaService } from '../../Services/multi-media.service';
import { CdkDrag, CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-snap-shot-program-editor',
  templateUrl: './snap-shot-program-editor.component.html',
  styleUrls: ['./snap-shot-program-editor.component.less'],
  animations: [fadeInAnimation]
})
export class SnapShotProgramEditorComponent implements OnInit, OnDestroy {
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
  public CreateMeasurement: boolean = false;
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
  public AllUnitsOfMeasurement: UnitOfMeasurement[] = [];
  public NewMetricTags: TagModel[] = [];
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
  public ExerciseToDelete: Exercise = undefined;
  public AthleteId: number;
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
  };//hacking the fact that we cannot use the toolBarHiddon method. look for .editor ::ng-deep insertVideo. Its a css class that I am using to maks the button


  public MenuItems =
    [//{ Text: "Workout Block", ItemType: ProgramDayItemEnum.workout, imageSrc: 'assets/ProgramBuilderMenu/heartIcon.png' },
      { Text: "Training Block", ItemType: ProgramDayItemEnum.superset, imageSrc: 'assets/lhs-icons/setsIcon.png', imageActiveSrc: 'assets/lhs-icons/setsIcon-active.png', isSelected: false },
      { Text: "Metric Block", ItemType: ProgramDayItemEnum.metric, imageSrc: 'assets/lhs-icons/stats.png', imageActiveSrc: 'assets/lhs-icons/stats-active.png', isSelected: false },
      { Text: "Survey Block", ItemType: ProgramDayItemEnum.survey, imageSrc: 'assets/lhs-icons/surveyIcon.png', imageActiveSrc: 'assets/lhs-icons/surveyIcon-active.png', isSelected: false },
      { Text: "Note Block", ItemType: ProgramDayItemEnum.note, imageSrc: 'assets/lhs-icons/NoteBlockIco.png', imageActiveSrc: 'assets/lhs-icons/NoteBlockIco-active.png', isSelected: false },
      { Text: "Multimedia Block", ItemType: ProgramDayItemEnum.video, imageSrc: '', imageActiveSrc: '', isSelected: false }];

  constructor(ProgramBuilderService: ProgramBuilderService, public tagFilterPipe: TagFilterPipe, public tagService: TagService
    , private WorkoutService: WorkoutService, private exerciseService: ExerciseService, public metricService: MetricsService
    , public SurveyService: SurveyService, public workoutService: WorkoutService
    , private route: ActivatedRoute, private cd: ChangeDetectorRef, private sortPipe: ArraySortPipe, public router: Router, public hidePipe: HideDeletedSortPipe
    , public multimediaService: MultiMediaService,) {
    this.ProgramService = ProgramBuilderService;

    this.GetAllMovies();
    this.GetAllMeasurements();
    this.metricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, false); });
    this.WorkoutService.GetWorkouts().subscribe(x => { this.AllWorkouts = this.hidePipe.transform(x, false); });
    this.SurveyService.GetAllSurveys().subscribe(x => { this.AllSurveys = this.hidePipe.transform(x, false); });
    this.SurveyService.GetAllQuestions().subscribe(x => this.AllQuestions = x);
  }

  drop(event: CdkDragDrop<ProgramDayItem[]>) {
    if (!this.Program.CanModify) return; //they cannot drag and drop programs that are assigned
    //they are re-ordering
    if (event.container === event.previousContainer) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    }

    else {
      var movedItem: any = this.MenuItems[event.previousIndex];
      var newItem = new ProgramDayItem();
      newItem.Id = Math.floor(Math.random() * 100000) + 1; // need to wire up a random number generator
      //because we can remove/add items and mess with the length. And I dont want
      //to go through the hassel of updating the ids on all movement
      newItem.ShowCreationMenu = true;
      newItem.ItemType = movedItem.ItemType
      newItem.ShowDetails = true;
      var day = this.Program.Days.filter(x => x.IsActive)[0];
      //event.container.addItem(newItem);
      day.Items.push(newItem);

      if (event.container.data === undefined) {
        event.container.data = [];
      }
      transferArrayItem(day.Items, event.container.data, day.Items.length - 1, event.currentIndex);
      newItem.ProgramItem = {};
      switch (movedItem.ItemType) {
        case ProgramDayItemEnum.workout:
          var newEx = new ProgramDayItemExercise();
          newEx.Weeks = [];
          for (var i = 1; i <= this.ProgramService.GetCurrentWeekCount(); i++) {
            let newWeek = new Week();
            newWeek.SetsAndReps.push(new Set());
            newWeek.Id = i;
            newWeek.Position = i;
            newEx.Weeks.push(newWeek);
          }
          newItem.ProgramItem = newEx;
          newItem.ProgramItem.Workout = {};
          newItem.ProgramItem.Workout.TotalWorkout = {};
          newItem.ProgramItem.SelectedExercise = {};
          newItem.ProgramItem.SelectedExercise.Name = "No Exercise Selected";
          newItem.ProgramItem.SelectedWorkout = {};
          newItem.ProgramItem.SelectedWorkout.Name = "No Workout Selected";

          break;
        case ProgramDayItemEnum.superset:
          var newSS = new ProgramDayItemSuperSet();
          newItem.ProgramItem = newSS;
          //@ts-ignore
          newItem.SuperSetDisplayTitle = "Training Block"
          var newExercise = new SuperSet_Exercise();
          newExercise.Name = "No Exercise Selected";
          newExercise.SelectedWorkout.Name = "No Workout Selected"
          newExercise.Id = Math.floor(Math.random() * 100000) + 1; // need to wire up a random number generator, this way i can delete them and add them without worriying about a sequential id
          newExercise.Position = 1;
          for (var i = 1; i <= this.ProgramService.GetCurrentWeekCount(); i++) {
            let newWeek = new SuperSet_Week();
            newWeek.SetsAndReps.push(new SuperSet_Set());
            newWeek.Id = i;
            newWeek.Position = i;
            newExercise.Weeks.push(newWeek);
          }
          newSS.Exercises.push(newExercise);
          break;
        case ProgramDayItemEnum.metric:
          newItem.ProgramItem.SelectedMetric = {}
          newItem.ProgramItem.SelectedMetric.Name = "No Metric Selected"
          newItem.ProgramItem.SelectedMetric.WeekIds = [];
          break;
        case ProgramDayItemEnum.survey:
          newItem.ProgramItem.SelectedSurvey = {};
          newItem.ProgramItem.SelectedSurvey.Name = "No Survey Selected"
          newItem.ProgramItem.SelectedSurvey.Questions = [];
          newItem.ProgramItem.SelectedSurvey.WeekIds = [];
          break;
        case ProgramDayItemEnum.video:
          newItem.ProgramItem.SelectedVideo = {};
          newItem.ProgramItem.SelectedVideo.Name = "No Video Selected"
          newItem.ProgramItem.SelectedVideo.WeekIds = [];
          break;
        case ProgramDayItemEnum.note:
          newItem.ProgramItem.SelectedNote = {};
          newItem.ProgramItem.SelectedNote.Name = "";
          newItem.ProgramItem.SelectedNote.Note = "";
          newItem.ProgramItem.SelectedNote.WeekIds = [];
          break;
        default:
          break;
      }
    }

    for (let i = 0; i < event.container.data.length; i++) {
      event.container.data[i].Position = i;
    }
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }

  ngAfterViewInit() {
    this.cd.detectChanges();//hack to stop the ExpressionChangedAfterItHasBeenCHeckedError thatis occuring within the ngxsmartmodel scripts
  }
  SaveDraftProgram() {
    // we only want to cache programs that can be modified. If we cache a program that cannot be modified
    //it resulted in a program that was cached and the users were unable to uncache it
    if (this.Program.CanModify) {
      this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    ////////////////////////////
    //I am instantiaing the tags in the subscribe. because the Two-drop-down-search wasnt getting the tags passed to it in some instances. For some reason this works
    // I think its because it is a complex object and tracking complex object changes needs some further magic that i dont yet possess
    ////////////////////////////
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

    this.route.params.subscribe(params => {

      if (params['athleteId'] !== undefined) {
        this.LoadingProgram = true;
        this.AthleteId = params['athleteId']
        this.ProgramService.GetSnapShotProgram(params['athleteId']).subscribe(x => {
          this.Program = x;
          this.SetWeek(this.Program.WeekCount)
          this.LoadingProgram = false;
          for (let i = 0; i < this.Program.Days.length; i++) {
            this.List_Ids.push('DragAndDrop-' + this.Program.Days[i].Id);
          }
          this.Program.Days.forEach(x => {
            x.Items.sort((a, b) => a.Position - b.Position);
          });
          this.done = this.Program.Days;
        });
        this.ProgramService.ClearSavedSnapShotProgram(this.AthleteId)
      }
      else {
        this.Program = this.ProgramService.GetSavedSnapShotProgram(this.AthleteId);
        if (this.Program === null) {
          this.Program = this.GenerateEmptyProgram();
        }
        else {
          this.SetWeek(this.Program.WeekCount)
        }
        for (let i = 0; i < this.Program.Days.length; i++) {
          this.List_Ids.push('DragAndDrop-' + this.Program.Days[i].Id);
        }
        this.Program.Days.forEach(x => {
          x.Items.sort((a, b) => a.Position - b.Position);
        });
        this.done = this.Program.Days;
      }

      this.GetAllExercises();
      this.tagService.GetAllTags(TagType.Program).subscribe(d => {
        for (var i = 0; i < d.length; i++) {
          var newTM = new TagModel();
          newTM.display = d[i].Name;
          newTM.value = d[i].Id;
          this.AllTags.push(newTM)
        }
      });
    });
  }

  //make it later on the superset workout
  MoveSuperSetExerciseDown(exercise: SuperSet_Exercise, superset: ProgramDayItemSuperSet) {
    if (exercise.Position == superset.Exercises.length) { return; }
    superset.Exercises[exercise.Position + 1].Position = exercise.Position;
    exercise.Position++;
    superset.Exercises = this.sortPipe.transform(superset.Exercises, 'Position');
  }
  //make it earlier on supersetworkout
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

  UpdateSurvey(event) {//survey: Survey, programDayItem: any) {
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }


  GetAllExercises(): void {
    //this.AllExercises =
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
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  };
  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.newProgramTagItems.splice(index, 1);
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.ClearSavedSnapShotProgram(this.AthleteId)
    this.ClearProgramConfirmation = false;

  }

  GenerateEmptyProgram(): Program {

    this.ProgramService.SetCurrentWeekCount(1);
    var ret = new Program();
    ret.WeekCount = this.ProgramService.GetCurrentWeekCount();
    ret.Days = [{
      Id: 1,
      IsActive: true,

      Items: [],
      Position: 1
    }];
    this.SetWeek(1);
    return ret;
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }

  AddDay(days: Day[]) {
    let tempId = Math.floor(Math.random() * Math.floor(10000))
    days.forEach(x => x.IsActive = false);
    days.push({ Id: tempId, IsActive: false, Items: [], Position: 0 });//the id is off but its getting remapped below so it doesnt matter
    for (var i = 0; i < days.length; i++) {
      days[i].Position = i;
    }
    days[0].IsActive = true;
    this.List_Ids.push('DragAndDrop-' + tempId);
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);

  }

  RemoveDay(targetDay: Day, days: Day[]) {
    var index = days.findIndex(x => x.Id == targetDay.Id);
    days.splice(index, 1);

    for (var i = 0; i < days.length; i++) {
      days[i].Position = i + 1; //position tracks position now, not id this will track the actual Id
    }
    days[0].IsActive = true;
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }

  RemoveLastDay(days: Day[]) {
    if (days.length == 1) { return; }
    days.pop();
    for (var i = 0; i < days.length; i++) {
      days[i].IsActive = i == 0;
      days[i].Position = i; //position tracks position now, not id this will track the actual Id
    }
  }


  SaveProgram(targetProgram: Program, associatedTags: TagModel[]) {
    this.SavingProgram = true;
    this.Errors = [];
    if (!targetProgram.CanModify) {
      this.Errors.push("Cannot Modify this program, it is in use.");
      this.SavingProgram = false;
      return;
    }
    if (targetProgram.Name == '' || targetProgram.Name == undefined) {
      this.Errors.push("The Program Needs To Have A Name To Save");
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
    newProg.Id = targetProgram.Id;
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
    try {
      this.ProgramService.UpdateSnapshotForUser(newProg,this.AthleteId).subscribe(
        success => {
          this.DisplayMessage("SAVE SUCCESSFULL", "Program Saved", false)
          this.GenerateEmptyProgram();
          this.newProgramTagItems = []
          this.SavingProgram = false;
          this.router.navigate([`AthleteProfile/${this.AthleteId}/Workout/${this.AthleteId}`]);
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
            this.Errors.push(`On Day ${x.DayNumber} ${x.ValidationErrorMessage}`);

          });
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  UpdateSuperSet_SetsReps(event) {//setRep: Set, e: SuperSet_Exercise) {
    let e = event.module;
    let setRep = event.event;
    if (event.event === undefined) {
      event.module.SelectedExercise = undefined;
      //clear out the current set/rep scheme
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
        e.Weeks[i].SetsAndReps = [];//clear out this weeks stuff so we can change it
        e.Weeks[i].Position = i + 1;

        if (x.TotalWorkout[i] == undefined)//The user has made a program with more weeks then the setsReps scheme they are trying to import
        {
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

            //doing it this way because between the sets they can say no rest and 0 weight with 30 sets
            //random shit like that. So if they ever put a value in then we do something;
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
      this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId)
  }
  AddWeekSetAndRep(targetWeek: Week[]) {
    var newWeek = new Week();
    newWeek.Position = this.SelectedWorkout.TotalWorkout.length + 1;
    newWeek.SetsAndReps = [];
    this.AddSetToWeek(newWeek);
    targetWeek.push(newWeek);
  }
  RemoveWeekFromSetAndRep(target: Week[]) {
    if (target.length > 0) target.pop();
  }
  AddSetToWeek(targetWeek: Week) {
    var set = new Set();
    set.Position = targetWeek.SetsAndReps.length;
    targetWeek.SetsAndReps.push(set);
  }
  CheckAddAnotherSet(targetWeek: Week, workout: WorkoutDetails) {
    //yup copy pasta, untill we do good programming and reuse models
    var removingASet = false;
    //we never want to remove the last element
    for (let i = 0; i < targetWeek.SetsAndReps.length - 1; i++) {
      let targetSet = targetWeek.SetsAndReps[i];
      //george english that shit. First of all why the ts-ignore? when a user input something and then deletes it the model will reflect an empty strring
      //so in order for something to be truely empty it most either be undefined/null/empty string
      //English time mother fuckers
      //!workout.ShowPercentageBox. This is true when the box is not clicked. Thus letting us bypass checking that set because we can
      //safely assume nothing is in there and that box can be deleted
      //!workout.ShowPercentageBox. this is false when the box is clicked meaning we need to check whats in there. Since
      //it returns false the rest of the or statement is evaluated.
      // Then we check if the box is truely empty, if it is the whole thing returns true and we go to the next line
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
        //we are at the end of the SetsAndReps array, and the last item has values. then we need to add a new row
        targetWeek.SetsAndReps.splice(i, 1);
        removingASet = true;
      }

    }

    if (!removingASet) {
      let lastSet = targetWeek.SetsAndReps[targetWeek.SetsAndReps.length - 1]
      //english that shit george!
      //!(workout.ShowPercentageBox && (lastSet.Percent === undefined || lastSet.Percent === null)) &&
      // heres how it works
      // workout.ShowPercntageBox, if that is false we automatically leave the if statement. Turn that to positive. Because there is no check need
      // and we can add another set, because they arent using Percentage
      //if workout.ShowPercentageBox is true, then they could have data in the percent back so next we check the percent box
      // if it is null OR undefined, then the  statement turns to true and the Bang operator turns it to false and they cannot add another week
      //if the percent box has a value that OR statement turns to false, making the entire statement false. Then the Bang operator turns
      //it all to true and we can add another week. This is the same for all boxes.
      if (
        !(workout.ShowSetsBox && (lastSet.Sets === undefined || lastSet.Sets === null)) &&
        !(workout.ShowRepsBox && (lastSet.Reps === undefined || lastSet.Reps === null)) &&
        !(workout.ShowTimeBox && ((lastSet.Minutes === undefined || lastSet.Minutes === null) || (lastSet.Seconds === undefined || lastSet.Seconds === null))) &&
        !(workout.ShowDistanceBox && (lastSet.Distance === undefined || lastSet.Distance === null))
      ) {
        //we are at the end of the SetsAndReps array, and the last item has values. then we need to add a new row
        this.AddSetToWeek(targetWeek);
      }
    }

    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  AddNewNoteToSuperSet(targetSuperSetProgramDayItem) {
    var newNote = new SuperSet_Note();
    newNote.Id = Math.round(Math.random() * 1000)
    newNote.Position = targetSuperSetProgramDayItem.ProgramItem.Notes.length + 1;
    targetSuperSetProgramDayItem.ProgramItem.Notes.push(newNote);
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
      //if its the first element we dont want them moving
      //if its the only element we dont want them moving
      return;
    }
    var targetPosition = this.Program.Days[targetIndex].Position;
    let targetToDecrement = this.Program.Days.findIndex(x => x.Position === targetPosition - 1);

    this.Program.Days[targetIndex].Position = this.Program.Days[targetIndex].Position - 1;

    this.Program.Days[targetToDecrement].Position = this.Program.Days[targetToDecrement].Position + 1;
    //beware. I am running pure pipes NO IMPURE PIPES. If you dont know what impure pipes are ask or google.
    //Angular change detection will not detect changes on arrays (push/pop/changing variables inside a custom object)
    //an Impure pipe would but that can fuck the app up with memory management. So i am faking out the
    // array being changed. that way the ui is forced to update
    let test = this.Program.Days.splice(0, this.Program.Days.length);
    this.Program.Days = test;
    this.done = test;
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  MoveSelectedDayRight() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    if (this.Program.Days[targetIndex].Position === this.Program.Days.length - 1 || this.Program.Days.length === 1) {
      //if its the last element we dont want them moving
      //if its the only element we dont want them moving
      return;
    }

    var targetPosition = this.Program.Days[targetIndex].Position;
    let targetToIncrement = this.Program.Days.findIndex(x => x.Position === targetPosition + 1);

    this.Program.Days[targetIndex].Position = this.Program.Days[targetIndex].Position + 1;

    this.Program.Days[targetToIncrement].Position = this.Program.Days[targetToIncrement].Position - 1;
    let test = this.Program.Days.splice(0, this.Program.Days.length);
    this.Program.Days = test;
    this.done = test;
    //beware. I am running pure pipes NO IMPURE PIPES. If you dont know what impure pipes are ask or google.
    //Angular change detection will not detect changes on arrays (push/pop/changing variables inside a custom object)
    //an Impure pipe would but that can fuck the app up with memory management. So i am faking out the
    // array being changed. that way the ui is forced to update
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  CopyDay() {
    let targetIndex = this.Program.Days.findIndex(x => x.IsActive == true);
    let copiedDay = JSON.parse(JSON.stringify(this.Program.Days[targetIndex]));
    (copiedDay as Day).IsActive = false;
    (copiedDay as Day).Position = this.Program.Days.length;
    (copiedDay as Day).Id = Math.floor(Math.random() * 100000) + 1; // need to wire up a random number generator
    //because we can remove/add items and mess with the length. And I dont want
    //to go through the hassel of updating the ids on all movement
    this.Program.Days.push(copiedDay);
    this.List_Ids.push('DragAndDrop-' + (copiedDay as Day).Id);
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  ToggleMetricCreationWindow() {
    this.ShowMetricCreationWindow = !this.ShowMetricCreationWindow;
  }
  ToggleMeasurementCreationWindow() {
    this.CreateMeasurement = !this.CreateMeasurement;
  }
  ToggleShowCreateExerciseWindow() {
    this.ShowCreateExerciseWindow = !this.ShowCreateExerciseWindow;
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
  }

  SaveForTrainingBlock(data:any){
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
  GetAllMeasurements() {
    this.metricService.GetAllMeasurements().subscribe(x => {
      this.AllUnitsOfMeasurement = x;
      this.AllUnitsOfMeasurement.forEach(y => y.Name = y.UnitType);//lazy hack to get the searchable working);
    });
  }
  SaveMeasurement(measurementName: string) {

    this.metricService.CreateUnitOfMeasurement(measurementName).subscribe(
      success => {
        this.GetAllMeasurements();
        this.DisplayMessage('Measurement Saved Successfully', 'Measurement Saved Successfully', false)
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Measurement Saved Unsuccessfully', errorMessage, true)
      });
  }

  SaveForMetricBlock(data:any){
    this.SaveMetric(data.targetMet);
  }

  SaveMetric(targetMet: Metric) {
    if (targetMet.Name == undefined || targetMet.Name == '') {
      this.DisplayMessage('Measurement Saved Unsuccessfully', 'The Metric Name cannot be empty', true)
      return;
    }

    var newMet = new Metric();
    newMet.Id = targetMet.Id;
    newMet.Name = targetMet.Name;
    newMet.UnitOfMeasurementId = targetMet.UnitOfMeasurementId
    this.NewMetricTags.forEach((value, index) => {
      this.AllMetricTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newMet.Tags.push(newTag);
        }
      });
    });

    this.metricService.CreateMetric(newMet).subscribe(success => {
      this.SelectedMetric = new Metric();
      this.DisplayMessage('Metric Saved Successfully', 'Metric Saved Successfully', false)
      this.metricService.GetAllMetrics().subscribe(x => { this.AllMetrics = this.hidePipe.transform(x, false); });
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage('Metric Saved Unsuccessfully', errorMessage, true)
    });
  }

  SaveForSurveyBlock(data:Survey){
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
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
  ToggleVideoCreationWindow() {
    this.ShowVideoCreationWindow = !this.ShowVideoCreationWindow;
  }

  SaveForMultimediaBlock(data:any){
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
            newTag.Type = TagType.Exercise;
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
  CloseModal() {
    if (!this.CreateMeasurement) {
      this.ToggleMetricCreationWindow();
    } else {
      this.CreateMeasurement = !this.CreateMeasurement;
    }
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
        var targetSpot = (x.ProgramItem.Exercises as Exercise[]).findIndex(targetToDelete => (targetToDelete as any).Position === (this.ExerciseToDelete as any).Position);
        if (targetSpot !== -1) {
          (x.ProgramItem.Exercises as Exercise[]).splice(targetSpot, 1)
          let positionThatIsBeingDeleted = x.ProgramItem.Exercises[targetSpot].Position;
          x.ProgramItem.Exercises.forEach(element => {
              if (element.Position > positionThatIsBeingDeleted)
              {
                element.Position--;
              }
          });

        }
      }
    });
    this.DeleteExerciseConfirmation = !this.DeleteExerciseConfirmation;
    this.ExerciseToDelete = null;
    this.ProgramService.SetSavedSnapShotProgram(this.Program,this.AthleteId);
  }
}


import { Component, OnInit, OnDestroy, Input, EventEmitter, Output, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Tag, TagType } from '../../Models/Tag';
import { Survey } from '../../Models/Survey';
import { SurveyService } from '../../Services/survey.service';
import { Observable, of, Subscription, interval } from 'rxjs';
import { Question, QuestionType } from '../../Models/Question';
import { take } from 'rxjs/operators';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { TagFilterPipe } from 'src/app/Pipes';
import { BackendAssistantCoach } from '../../Models/AssistantCoach';
import { OrganizationService } from '../../Services/organization.service';
import { ScaleThreshold } from '../../Models/Survey/ScaleThreshold';
import { YesNoThreshold } from '../../Models/Survey/YesNoThreshold';
import { ThrowStmt } from '@angular/compiler';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';

@Component({
  selector: 'app-survey',
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.less'],
  animations: [fadeInAnimation]
})
export class SurveyComponent implements OnInit, OnDestroy {
  public ShowArchive: boolean = false;
  public searchString: string;
  public AllTags: TagModel[] = [];
  public TagItems: TagModel[] = [];
  public CreateNewSurvey: boolean = false;
  @Input() View: string = "Surveys";
  @Input() Model: boolean = false;
  @Output() ModelClose: EventEmitter<any> = new EventEmitter();
  public SurveyView: string = "ShowAddQuestionMenu";
  public SelectedSurvey: Survey = new Survey();
  public QuestionHelp: string = 'scale';
  public NewQuestionName: string = '';
  public AllSurveys: Observable<Survey[]>;
  public AllQuestions: Question[];
  public subs: Subscription = new Subscription();
  public SelectedQuestion: Question = new Question();
  public AlertMessages: AlertMessage[] = [];
  public SurveyTags: TagModel[] = [];
  public UnmodifiedSurveys: Observable<Survey[]>;
  public NewSurveyTagItems: TagModel[] = [];
  public ShowCreateQuestionModal: boolean = false;
  public ShowHardDeleteWindow: boolean = false;
  public hardDeleteSurveyId: number = 0;
  public EditNotifications: boolean = false;
  public CoachIds: number[];
  public AllCoaches: BackendAssistantCoach[] = [];
  public questionPaginationStart: number = 0;
  public questionPaginationEnd : number = 0;
  public surveyPaginationStart: number = 0;
  public surveyPaginationEnd : number = 0;

  public Comparers: {}[] = [
    { Id: 1, Name: "Equals" },
    { Id: 2, Name: "Greater Than" },
    { Id: 3, Name: "Less Than" },
    { Id: 4, Name: "Equal To Or Greater Than" },
    { Id: 5, Name: " Equal To Or Less Than" }];
  public scaleThresholds: ScaleThreshold[] = [];
  public YesNoThresholds: YesNoThreshold[] = [];

  constructor( private cd: ChangeDetectorRef,public orgService: OrganizationService, public tagFilterPipe: TagFilterPipe, public tagService: TagService, public surveyService: SurveyService) {

    this.SelectedSurvey = new Survey();
  }

  ngOnDestroy() {
  }
  ngOnInit() {
    this.scaleThresholds.push(new ScaleThreshold());
    this.YesNoThresholds.push(new YesNoThreshold());

    this.orgService.GetAllCoaches().subscribe(x => {
      this.AllCoaches = x;
      this.AllCoaches.map(x => x.FullName = x.FirstName + " " + x.LastName);
    });
    this.surveyService.GetAllQuestions().subscribe(x => this.AllQuestions = x);
    this.GetAllSurveys();
    this.tagService.GetAllTags(TagType.Survey).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });
  }
  ngAfterViewInit() {
  }

  UpdateQuestionPaginiationDisplay(s:DisplayPaginatedItems) {
    this.questionPaginationStart = s.Start;
   this.questionPaginationEnd = s.End;
   this.cd.detectChanges();
 }
 UpdateSurveyPaginiationDisplay(s:DisplayPaginatedItems) {
  this.surveyPaginationStart = s.Start;
 this.surveyPaginationEnd = s.End;
 this.cd.detectChanges();
}
  ClearAllThresholds() {
    this.YesNoThresholds = [];
    this.scaleThresholds = [];
    this.scaleThresholds.push(new ScaleThreshold());
    this.YesNoThresholds.push(new YesNoThreshold());
  }
  ToggleThresholdValue(t: YesNoThreshold) {
    t.ThresholdValue = !t.ThresholdValue;
  }
  AddYesNoThreshold() {
    var newThreshold = new YesNoThreshold();
    newThreshold.Id = Math.random();
    this.YesNoThresholds.push(newThreshold)
  }
  AddScaleThreshold() {
    var newThreshold = new ScaleThreshold();
    newThreshold.Id = Math.random();
    this.scaleThresholds.push(newThreshold);
  }
  RemoveYesThreshold(targetThreshold: YesNoThreshold) {
    for (var i = 0; i <= this.YesNoThresholds.length - 1; i++) {
      if (this.YesNoThresholds[i].Id == targetThreshold.Id) {
        this.YesNoThresholds.splice(i, 1);
        return;
      }
    }
  }
  RemoveScaleThreshold(targetThreshold: ScaleThreshold) {
    for (var i = 0; i <= this.scaleThresholds.length - 1; i++) {
      if (this.scaleThresholds[i].Id == targetThreshold.Id) {
        this.scaleThresholds.splice(i, 1);
        return;
      }
    }
  }
  AddCoachYesNoThreshold(coachId: number, yesNoThresholds: YesNoThreshold) {
    for (var i = 0; i <= yesNoThresholds.Coaches.length - 1; i++) {
      if (yesNoThresholds.Coaches[i].Id == coachId) {
        return;
      }
    }
    for (var i = 0; i <= this.AllCoaches.length - 1; i++) {
      if (this.AllCoaches[i].Id == coachId) {
        yesNoThresholds.Coaches.push(this.AllCoaches[i])
      }
    }
  }
  AddCoachScaleThreshold(coachId: number, scaleThresholds: ScaleThreshold) {
    for (var i = 0; i <= scaleThresholds.Coaches.length - 1; i++) {
      if (scaleThresholds.Coaches[i].Id == coachId) {
        return;
      }
    }
    for (var i = 0; i <= this.AllCoaches.length - 1; i++) {
      if (this.AllCoaches[i].Id == coachId) {
        scaleThresholds.Coaches.push(this.AllCoaches[i])
      }
    }
  }

  RemoveCoachFromYesNoThreshold(targetCoach: BackendAssistantCoach, yesNoThresholds: YesNoThreshold) {
    for (var i = 0; i <= yesNoThresholds.Coaches.length - 1; i++) {
      if (yesNoThresholds.Coaches[i].Id == targetCoach.Id) {
        yesNoThresholds.Coaches.splice(i, 1);
        return;
      }
    }
  }
  RemoveCoachFromScaleThreshold(targetCoach: BackendAssistantCoach, scaleThresholds: ScaleThreshold) {
    for (var i = 0; i <= scaleThresholds.Coaches.length - 1; i++) {
      if (scaleThresholds.Coaches[i].Id == targetCoach.Id) {
        scaleThresholds.Coaches.splice(i, 1);
        return;
      }
    }
  }

  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }
  CloseModel() {
    this.ModelClose.emit(true);
  }
  ModifySelectedSurvey(surv: Survey) {
    this.SelectedSurvey = surv;
    this.NewSurveyTagItems = [];
    this.SelectedSurvey.Tags.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.NewSurveyTagItems.push({ display: value.Name, value: value.Id });
    });
    this.View = 'ModifySurvey';
    this.UpdateDisplayedSurveyQuestions(surv.Id);
    window.scrollTo(0, 0);
  }

  UpdateDisplayedSurveyQuestions(id: number) {
    this.surveyService.GetAllSurveyQuestions(id).subscribe(data => {
      this.SelectedSurvey.Questions = data;
    });
  }

  RemoveQuestionFromSurvey(SurveysToQuestionsId: number, surveyId: number) {
    if (!this.SelectedSurvey.CanModify) return;
    this.surveyService.RemoveQuestionToSurvey(SurveysToQuestionsId).subscribe(success => {
      this.UpdateDisplayedSurveyQuestions(surveyId);
      this.SelectedQuestion = new Question();
    });
  }
  ModifySelectedQuestion(q: Question) {
    this.ShowCreateQuestionModal = true;
    this.surveyService.GetQuestionDetails(q.QuestionId).subscribe(x => {
      this.SelectedQuestion = x;
      this.scaleThresholds = x.ScaleThresholds;
      if (this.scaleThresholds !== null) {
        this.scaleThresholds.forEach(x => {
          x.Coaches = [];
          x.CoachIds.forEach(i => {
            this.AllCoaches.forEach(z => {
              if (z.Id === i) {
                x.Coaches.push(z);
              }
            });
          })
        });
      }
      else {
        this.scaleThresholds = [];
      }
      this.YesNoThresholds = x.YesNoThresholds;
      if (this.YesNoThresholds !== null) {
        this.YesNoThresholds.forEach(x => {
          x.Coaches = [];
          x.CoachIds.forEach(i => {
            this.AllCoaches.forEach(z => {
              if (z.Id === i) {
                x.Coaches.push(z);
              }
            });
          })
        });
      }
      else {
        this.YesNoThresholds = [];
      }
    });
  }

  SetSelectedQuestion(q: Question) {
    this.SelectedQuestion = q;
  }

  UpdateSelectedQuestion(id: number) {
    if (this.SelectedQuestion.CanModify === true)
      this.SelectedQuestion.QuestionType = id;
  }

  ModifyQuestions() {
    this.View = 'ModifyQuestions'
  }
  AddQuestionToCurrentSurvey(questionId: number, surveyId: number) {
    if (!this.SelectedSurvey.CanModify) return;
    if (this.SelectedSurvey.Questions == undefined) { this.SelectedSurvey.Questions = []; }
    var found = false;
    this.SelectedSurvey.Questions.forEach(function (x) {
      if (x.QuestionId == questionId) {
        found = true;
        return;
      }
    });
    if (!found) {
      this.surveyService.AddQuestionToSurvey(surveyId, questionId).subscribe(success => { this.UpdateDisplayedSurveyQuestions(surveyId); });
    }
  }
  ViewCurrentSelectedSurveyDetails() {
    this.View = 'ModifySurvey';
  };
  ModifySurveyQuestions() {
    this.View = 'ModifySurveyQuestions';
  }
  NewQuestion() {
    this.ShowCreateQuestionModal = true;
    this.SelectedQuestion = new Question();
    this.SelectedQuestion.CanModify = true;
  }


  ViewAllSurveys() {
    this.GetAllSurveys();
    this.View = "Surveys";
  }
  SetSelectedTag(tag: TagModel) {
    this.TagItems = [];
    this.TagItems.push(tag);
    this.ViewAllSurveys();
  }

  SaveSurvey(id: number, Name: string, description: string) {
    if (Name == '' || Name == undefined) {
      this.DisplayMessage('The Survey Needs To Have A Name To Save', "Save Unsuccessfull", true)
      return;
    }

    if (!this.SelectedSurvey.CanModify) return;
    var newSurvey = new Survey();
    newSurvey.Name = Name;
    newSurvey.Description = description;
    newSurvey.Id = id;

    this.NewSurveyTagItems.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newSurvey.Tags.push(newTag);
        }
      });
    });
    if (id == undefined || id == 0) {
      this.surveyService.CreateSurvey(newSurvey).subscribe(success => {
        this.SelectedSurvey.Id = success;
        this.View = 'ModifySurveyQuestions'
        this.UpdateDisplayedSurveyQuestions(success);
        this.DisplayMessage('Survey Saved', "Save Successfull", false)
      }, error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage('Survey Not Saved', errorMessage, true)

      });
    }
    else {
      this.surveyService.UpdateSurvey(newSurvey).subscribe(
        success => {
          this.DisplayMessage('Survey Updated', "Update Successfull", false)
        }
        , error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Survey Not Updated ", errorMessage, true)
        });
    }
  }
  UpdateQuestion(surveyId, questionName, questionType, questionId) {
    if (questionName == '' || questionName == undefined) {
      this.DisplayMessage("The Question Needs To Have A Name To Save", "Save UNSUCCESSFULL", true)
      return;
    }
    if (questionType == '' || questionType == undefined) {
      this.DisplayMessage("The Question Needs To Have A Name To Save", "Save UNSUCCESSFULL", true)
      return;
    }
    var newQ = new Question();
    newQ.Question = questionName;
    newQ.QuestionType = questionType;
    newQ.SurveyId = surveyId;
    newQ.QuestionId = questionId;

    if (this.scaleThresholds !== null && this.scaleThresholds.length > 0 && newQ.QuestionType === QuestionType.Scale) {
      this.scaleThresholds.forEach(x => {
        x.CoachIds = [];//clear out the old Ids before storing what the user wants
        x.Coaches.forEach(y => {
          x.CoachIds.push(y.Id);
        });
      });
      newQ.ScaleThresholds = this.scaleThresholds
    }

    if (this.YesNoThresholds !== null && this.YesNoThresholds.length > 0 && newQ.QuestionType === QuestionType.Boolean) {
      this.YesNoThresholds.forEach(x => {
        x.CoachIds = [];//clear out the old Ids before storing what the user wants
        x.Coaches.forEach(y => {
          x.CoachIds.push(y.Id);
        });
      });
      newQ.YesNoThresholds = this.YesNoThresholds
    }

    this.surveyService.UpdateQuestion(newQ).subscribe(
      success => {
        this.DisplayMessage("Question Saved", "SAVE SUCCESSFULL", false)
        this.SelectedQuestion = new Question();
        this.ClearAllThresholds();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Question Save Unsuccessfull", errorMessage, true)
      });
    this.ToggleCreateQuestionModal()

  }
  CreateQuestion(surveyId, questionName, questionType) {
    if (questionName == '' || questionName == undefined) {
      this.DisplayMessage("The Question Needs To Have A Name To Save", "Save UNSUCCESSFULL", true)
      return;
    }
    if (questionType == '' || questionType == undefined) {
      this.DisplayMessage("The Question Needs To Have A Name To Save", "Save UNSUCCESSFULL", true)
      return;
    }
    var newQ = new Question();
    newQ.Question = questionName;
    newQ.QuestionType = questionType;

    if (surveyId > 0 && surveyId !== undefined) {
      newQ.SurveyId = surveyId
    }

    if (this.scaleThresholds.length > 0 && newQ.QuestionType === QuestionType.Scale) {
      this.scaleThresholds.forEach(x => {
        x.Coaches.forEach(y => {
          x.CoachIds.push(y.Id);
        });
      });
      newQ.ScaleThresholds = this.scaleThresholds
    }

    if (this.YesNoThresholds.length > 0 && newQ.QuestionType === QuestionType.Boolean) {
      this.YesNoThresholds.forEach(x => {
        x.Coaches.forEach(y => {
          x.CoachIds.push(y.Id);
        });
      });
      newQ.YesNoThresholds = this.YesNoThresholds
    }

    this.surveyService.CreateQuestion(newQ).subscribe(
      success => {
        this.DisplayMessage("Question Saved", "SAVE SUCCESSFULL", false)
        this.UpdateDisplayedSurveyQuestions(surveyId);
        this.SelectedQuestion = new Question();
        this.ClearAllThresholds();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Question Save Unsuccessfull", errorMessage, true)
      });
    this.ToggleCreateQuestionModal()
  }


  CreateSurvey() {
    this.View = "CreateSurvey"
    this.SelectedSurvey = new Survey();
    this.NewSurveyTagItems = [];
    this.TagItems.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.NewSurveyTagItems.push({ display: value.display, value: value.value });
    });
  }
  ReturnToTagSearch() {
    this.View = "AllTags";
  }
  AddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Survey;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.AllTags.push(newTag);
      });

    }
    this.TagItems.push(s);
    this.UnmodifiedSurveys.subscribe(x => {
      this.AllSurveys = (of(this.tagFilterPipe.transform(x, this.TagItems)));
    });
  };
  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => x.display == s.display);
    this.TagItems.splice(index, 1);
    this.UnmodifiedSurveys.subscribe(x => {
      this.AllSurveys = (of(this.tagFilterPipe.transform(x, this.TagItems)));
    });
  }

  GetAllSurveys(): void {
    this.UnmodifiedSurveys = this.surveyService.GetAllSurveys();
    this.AllSurveys = this.UnmodifiedSurveys;
  }
  DuplicateSurvey(surveyId: number): void {
    this.surveyService.DuplicateSurvey(surveyId).subscribe(
      success => {
        this.DisplayMessage("Survey Duplicated", "DUPLICATION SUCCESSFULL", false)
        this.GetAllSurveys();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("DUPLICATION UNSUCCESSFULL", errorMessage, true)
      });
  }
  UnArchiveSurvey(surveyId: number) {
    this.surveyService.UnArchiveSurvey(surveyId).subscribe(
      success => {
        this.DisplayMessage("Survey UnArchived", "UNARCHIVE SUCCESSFULL", false)

        this.GetAllSurveys();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("UNARCHIVE UNSUCCESSFULL", errorMessage, true)
      });
  }
  ArchiveSurvey(surveyId: number) {
    this.surveyService.ArchiveSurvey(surveyId).subscribe(
      success => {
        this.DisplayMessage("Survey Archived", "ARCHIVE SUCCESSFULL", false)
        this.GetAllSurveys();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("ARCHIVE UNSUCCESSFULL", errorMessage, true)
      });
  }
  ConfirmArchive(exerciseId: number) {
  }
  CancelArchive() {
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
  AddNewSurveyTag(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Survey;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTM = new TagModel();
        newTM.display = s.display;
        newTM.value = data;
        this.AllTags.push(newTM)

      });
    }
    this.NewSurveyTagItems.push(s);
  }

  RemoveNewSurveyTag(s: TagModel) {
    var index = this.NewSurveyTagItems.findIndex(x => { return x.display == s.display });
    this.NewSurveyTagItems.splice(index, 1);
  }
  public ToggleHardDeleteModal(exerciseId) {
    this.hardDeleteSurveyId = exerciseId;
    this.ShowHardDeleteWindow = !this.ShowHardDeleteWindow;
  }
  public HardDelete() {
    this.surveyService.HardDeleteSurvey(this.hardDeleteSurveyId).subscribe(success => {
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Exercises DELETED", "Exercises Successfully Deleted", false);
      this.GetAllSurveys();
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Exercise NOT DELETED", errorMessage, true);
    });
  }
  public ToggleCreateQuestionModal() {
    this.ShowCreateQuestionModal = !this.ShowCreateQuestionModal;
    this.ClearAllThresholds();
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { Question } from 'src/app/Models/Question';
import { Survey } from 'src/app/Models/Survey';
import { Tag, TagType } from 'src/app/Models/Tag';
import { TagModel } from 'src/app/Models/TagModel';
import { TagService } from 'src/app/Services/tag-service.service';

@Component({
  selector: 'app-survey-form',
  templateUrl: './survey-form.component.html',
  styleUrls: ['./survey-form.component.less'],
  animations: [fadeInAnimation]
})
export class SurveyFormComponent implements OnInit {
  @Input() NewlyCreatedSurvey: Survey = new Survey();
  @Input() ShowSurveyWindow: string = "";
  @Input() AllQuestions: Question[] = [];
  @Input() SurveyTags: TagModel[] = [];
  @Output() CancelCallBack = new EventEmitter<boolean>();
  @Output() SaveCallBack = new EventEmitter<Survey>();
  @Input() SetModalStyle: boolean = true;

  public AllTags: TagModel[] = [];

  constructor(public tagService: TagService) { }

  ngOnInit(): void {
  }

  Save(newSurvey: Survey) {
    this.SaveCallBack.emit(newSurvey);
  }

  Cancel() {
    this.CancelCallBack.emit(true)
  }

  AddNewSurveyTags(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.SurveyTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Survey;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.SurveyTags.push(newTag);
        this.AllTags.push(newTag)
        if (this.NewlyCreatedSurvey.Tags.findIndex(x => x.Name === s.display) === -1) {
          this.NewlyCreatedSurvey.Tags.push({ Id: data, Name: s.display, Type: TagType.Survey });
        }
      });
    }
  }

  RemoveNewSurveyTags(t: TagModel) {
    var index = this.SurveyTags.findIndex(x => x.display == t.display);
    this.SurveyTags.splice(index, 1);
    var found = this.NewlyCreatedSurvey.Tags.findIndex(x => x.Name === t.display);
    if (found > 0) {
      this.NewlyCreatedSurvey.Tags.splice(found, 1);
    }
  }

  AddQuestionToCurrentSurvey(targetQuestion: Question, newSurvey: Survey) {
    if (this.NewlyCreatedSurvey.Questions == undefined) { this.NewlyCreatedSurvey.Questions = []; }
    newSurvey.Questions.push(targetQuestion)
  }

  RemoveQuestionFromSurvey(targetQuestion: Question, newSurvey: Survey) {
    let i = newSurvey.Questions.findIndex((x: Question) => x.QuestionId == targetQuestion.QuestionId);
    newSurvey.Questions.splice(i, 1);
  }
}

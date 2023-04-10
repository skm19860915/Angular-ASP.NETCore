import { Component, OnInit, Input, Output, EventEmitter, forwardRef, SimpleChanges, OnChanges, ChangeDetectorRef } from '@angular/core';
import { TagModel } from 'src/app/Models/TagModel';
import { Tag } from '../../../Models/Tag';


@Component({
  selector: 'app-scp-tag-input',
  templateUrl: './scp-tag-input.component.html',
  styleUrls: ['./scp-tag-input.component.less']
})
export class ScpTagInputComponent implements OnInit, OnChanges {


  public tagInput: string;
  public enteredTags: string[] = [];
  @Output() TagAdded: EventEmitter<any> = new EventEmitter(true);
  @Output() TagRemoved: EventEmitter<any> = new EventEmitter(true);
  @Input() ResetTags: EventEmitter<boolean>;
  @Input() autocompleteItems: TagModel[];
  @Input() ExistingTags: any[];
  public randomNumberForName: number;//This is incase we have mutliple components on 1 page the names wont colide
  public PreExistingTagsForIntellisense: string;

  constructor(private cd: ChangeDetectorRef) {
    this.randomNumberForName = Math.floor(Math.random() * 100000);
  }

  ngOnInit() {
    if (this.ResetTags) {
      this.ResetTags.subscribe(() => {
        this.enteredTags = [];
        this.ExistingTags = [];
        this.cd.detectChanges();
      });
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    this.ClearTags();
    if (changes.ExistingTags) {
      if (this.ExistingTags != undefined && this.ExistingTags.length > 0) {
        this.ExistingTags.forEach(x => this.enteredTags.push(x.display));
      }
    }
  }

  ClearTags() {
    this.enteredTags = [];
    this.cd.detectChanges();
  }
  submit(newTag) {
    if (this.enteredTags.indexOf(newTag) == -1) {
      this.enteredTags.push(newTag)
      this.TagAdded.emit({ display: newTag })
    }
    this.tagInput = "";
  }
  removeTag(targetTag) {
    this.enteredTags.splice(this.enteredTags.indexOf(targetTag), 1);
    this.TagRemoved.emit({ display: targetTag });
  }
}

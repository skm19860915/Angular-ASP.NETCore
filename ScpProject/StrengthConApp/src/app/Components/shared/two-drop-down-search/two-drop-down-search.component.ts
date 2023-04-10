import { Component, OnInit, Input, EventEmitter, Output, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ITaggable } from '../../../Interfaces/ITaggable';
import { TagModel } from '../../../Models/TagModel';

@Component({
  selector: 'two-drop-down-search',
  templateUrl: './two-drop-down-search.component.html',
  styleUrls: ['./two-drop-down-search.component.less']
})
export class TwoDropDownSearchComponent implements OnInit, OnDestroy {

  @Input() set AllTags(value: TagModel[]) {
    if (value !== undefined && value.length > 0) {
      this._AllTags = value;
      this.DataLoadCheck();
    }
  };
  @Input() set AllTaggedItems(value: ITaggable[]) {
    if (value !== undefined) {
      this._AllTaggedItems = value;
      this.ResetAllTaggedItems();
      this.DataLoadCheck();
    }

  }
  @Input() CurrentProgramBuilderModule: any;

  @Input() set SelectedItem(value: number) {
    if (value > 0) {
      this._SelectedItem = value;
      this.DataLoadCheck();
    }
  }
  @Output() SelectedItemChange: EventEmitter<any> = new EventEmitter();
  public AllDataLoaded: boolean = false;
  public dataLoadTracker: number = 0;
  public _AllTags: TagModel[] = [new TagModel()];
  public _AllTaggedItems: ITaggable[]
  public _SelectedItem: number;
  public TagsToSearchWith: number[] = [];
  public AllTaggedItemsClone: ITaggable[] = [];
  public SelectedItemId: number;
  public SelectedTags: number[];

  private eventsSubscription: Subscription;

  @Input() UpdateTagEvent: Observable<TagModel[]>;

  @Input() TagVisible: boolean = true;

  constructor() {

  }

  DataLoadCheck() {
    this.dataLoadTracker++;;
    if (this.dataLoadTracker === 3) {
      this.SetSelectedItem(this._SelectedItem);
    }
  }

  ngOnInit() {
    this.ResetAllTaggedItems();
    if (this.UpdateTagEvent !== undefined)
      this.eventsSubscription = this.UpdateTagEvent.subscribe(x => {
        this._AllTags = []; //reseting the reference of this._allTags to something differnt to force update
        x.forEach(y => this._AllTags.push(y))
      });
  }
  ngOnDestroy() {
    this.eventsSubscription.unsubscribe();
  }
  SetSelectedItem(id: number) {
    this._SelectedItem = id;
    this.SetSelectedTags(id);

  }

  SetSelectedTags(id: number) {
    if (this._AllTaggedItems === undefined || this._AllTaggedItems.length === 0 || id === undefined) return;
    let targetTaggedItem = this._AllTaggedItems.filter(x => x.Id == id);
    this.SelectedTags = [];
    if (targetTaggedItem.length === 0) return;
    targetTaggedItem[0].Tags.forEach(x => {
      this.SelectedTags.push(x.Id);
    })
  }

  ResetAllTaggedItems() {
    if (this._AllTaggedItems === undefined) return;
    this.AllTaggedItemsClone = JSON.parse(JSON.stringify(this._AllTaggedItems));
  }

  SelectedItemUpdate(event) {
    this.SelectedItemChange.emit({ event: event, module: this.CurrentProgramBuilderModule });
    this.SetSelectedTags(event.Id);
  }
  RemoveTag(event) {

    this.TagsToSearchWith = this.TagsToSearchWith.filter(x => x !== event.value.value)
    this.Filter();
  }
  AddTag(event: TagModel) {
    this.TagsToSearchWith.push(event.value);
    this.Filter();
  }
  Filter() {
    this.AllTaggedItemsClone = [];
    this._AllTaggedItems.forEach((x: ITaggable) => {

      let push = true;
      for (let i = 0; i < this.TagsToSearchWith.length; i++) {
        let found = x.Tags.findIndex(z => z.Id === this.TagsToSearchWith[i])
        if (found === -1) {
          push = false;
        }
      }

      if (push) {
        this.AllTaggedItemsClone.push(x)
      }
    });
  }
}

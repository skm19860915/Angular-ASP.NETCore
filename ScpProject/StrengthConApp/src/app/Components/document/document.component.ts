import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { interval, of } from 'rxjs';
import { take } from 'rxjs/operators';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { Tag, TagType } from 'src/app/Models/Tag';
import { TagModel } from 'src/app/Models/TagModel';
import { Document } from '../../Models/Document';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';
import { TagFilterPipe } from 'src/app/Pipes';
import { Agreement } from 'src/app/Models/Agreement';

@Component({
  selector: 'app-document',
  templateUrl: './document.component.html',
  styleUrls: ['./document.component.less'],
  animations: [fadeInAnimation]
})
export class DocumentComponent implements OnInit {
  public SelectedDocument: Document = new Document();
  public NewDocumentTagItems: TagModel[] = [];
  public TagItems: TagModel[] = [];
  public AllTags: TagModel[] = [];
  @Input() View: string = 'Document';
  public paginationStart: number = 0;
  public paginationEnd: number = 0;
  public AlertMessages: AlertMessage[] = [];
  public AllDocuments: Document[];
  public UnModifiedDocuments: Document[];
  public AllAgreements: Agreement[];
  public LatestAgreemantId: number = 0;

  constructor(private cd: ChangeDetectorRef, public tagFilterPipe: TagFilterPipe) { }

  ngOnInit(): void {
    this.UnModifiedDocuments = this.GetAllDocuments();
    this.AllDocuments = this.UnModifiedDocuments;
    this.GetAllAgreements();
  }

  UpdatePaginiationDisplay(s: DisplayPaginatedItems) {
    this.paginationStart = s.Start;
    this.paginationEnd = s.End;
    this.cd.detectChanges();
  }

  GetAllAgreements() {
    this.AllAgreements = [
      { "Id": 1, "Description": "<h2>Lorem ipsum dolor sit amet1.</h2>", "IsDeleted": false },
      { "Id": 2, "Description": "Lorem ipsum dolor sit amet2", "IsDeleted": false },
      { "Id": 3, "Description": "Lorem ipsum dolor sit amet3", "IsDeleted": false },
      { "Id": 4, "Description": "Lorem ipsum dolor sit amet4", "IsDeleted": false },
    ];
    this.LatestAgreemantId = this.AllAgreements.length;
  }

  GetAllDocuments(): Document[] {

    this.AllDocuments = [
      { "Id": 1, "Name": "Document1", "Description": "<h1>This is document1.</h1>", "IsDeleted": false,
      "Tags":[{"Id": 1, "Name": "t1", "Type": 8}, {"Id": 2, "Name": "t2", "Type": 8}, {"Id": 3, "Name": "t3", "Type": 8}],
      "Agreements":[{"Id": 1, "Description": "<h4>Lorem ipsum dolor sit amet1.</h4>", "IsDeleted": false},
                    {"Id": 2, "Description": "Lorem ipsum dolor sit amet2.", "IsDeleted": false}]},
      { "Id": 2, "Name": "Document2", "Description": "<h1>This is document2.</h1>", "IsDeleted": false,
      "Tags":[{"Id": 4, "Name": "t4", "Type": 8}, {"Id": 5, "Name": "t5", "Type": 8}],
      "Agreements":[{"Id": 3, "Description": "Lorem ipsum dolor sit amet3.", "IsDeleted": false},
                    {"Id": 4, "Description": "Lorem ipsum dolor sit amet4.", "IsDeleted": false}]},
      { "Id": 3, "Name": "Document3", "Description": "<h5>This is document3.</h5>", "IsDeleted": false,
      "Tags":[{"Id": 10, "Name": "t8", "Type": 8}, {"Id": 12, "Name": "t9", "Type": 8}, {"Id": 13, "Name": "t6", "Type": 8}],
      "Agreements":[{"Id": 10, "Description": "Lorem ipsum dolor sit amet10.", "IsDeleted": false}]},
    ];

    return this.AllDocuments;
  }

  ViewCreateDocument() {
    this.View = "CreateDocument"
    this.SelectedDocument = new Document();
    this.NewDocumentTagItems = [];
    this.TagItems.forEach((value, index) => {
      this.NewDocumentTagItems.push({ display: value.display, value: value.value });
    });
  }

  Cancel() {
    this.View = 'Document';
  }

  Save(data:any){
    this.SaveDocument(data.targetDocument, data.associatedTags);
  }

  SaveDocument(targetDocument:Document, associatedTags: TagModel[]){
    if (targetDocument.Name == '' || targetDocument.Name == undefined) {
      this.DisplayMessage("Save UNSUCCESSFULL", "The Document Needs To Have A Name To Save", true)
      return;
    }
    targetDocument.Id = this.AllDocuments.length + 1;
    targetDocument.Tags = [];
    // associatedTags.forEach((value, index) => {
    //   this.AllTags.forEach((sourceTag: TagModel) => {
    //     if (sourceTag.display == value.display) {
    //       var newTag = new Tag();
    //       newTag.Id = sourceTag.value;
    //       newTag.Name = sourceTag.display;
    //       newTag.Type = TagType.Document;
    //       targetDocument.Tags.push(newTag);
    //     }
    //   });
    // });

    associatedTags.forEach((value, index) => {
      var newTag = new Tag();
      newTag.Id = Math.random();;
      newTag.Name = value.display;
      newTag.Type = TagType.Document;
      targetDocument.Tags.push(newTag);
    });

    this.AllDocuments.push(targetDocument);

    this.View = 'Document';
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    var newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
    interval(3000).pipe(take(1)).subscribe(x => this.AlertMessages.splice(0, 1));
  }

  AddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Document;
      // this.tagService.CreateTag(tagToAdd).subscribe((data) => {

      //   newTag.value = data;
      //   this.AllTags.push(newTag);
      // });
    }
    this.TagItems.push(newTag);
    this.AllDocuments = this.tagFilterPipe.transform(this.UnModifiedDocuments, this.TagItems);
  };

  RemoveTag(s: TagModel) {
    var index = this.TagItems.findIndex(x => { return x.display == s.display });
    this.TagItems.splice(index, 1);
    this.AllDocuments = this.tagFilterPipe.transform(this.UnModifiedDocuments, this.TagItems);
  }

  DownloadDocument(id: number){

  }

  ArchiveDocument(id: number){

  }

  UnArchiveDocument(id: number){

  }

  ModifySelectedDocument(selectedDocument: Document) {
    window.scroll(0, 0);
    this.SelectedDocument = selectedDocument;

    this.View = "CreateDocument";
    this.NewDocumentTagItems = [];
    this.SelectedDocument.Tags.forEach((value, index) => {
      this.NewDocumentTagItems.push({ display: value.Name, value: value.Id });
    });
  }
}

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { Agreement } from 'src/app/Models/Agreement';
import { Document } from 'src/app/Models/Document';
import { Tag, TagType } from 'src/app/Models/Tag';
import { TagModel } from 'src/app/Models/TagModel';
import { TagService } from 'src/app/Services/tag-service.service';
import { DocumentService } from '../../../Services/Document.service';

@Component({
  selector: 'app-document-form',
  templateUrl: './document-form.component.html',
  styleUrls: ['./document-form.component.less'],
  animations: [fadeInAnimation]
})
export class DocumentFormComponent implements OnInit {
  @Input() View: string = "";
  @Input() SelectedDocument: Document = new Document();
  @Input() NewDocumentTagItems: TagModel[] = [];
  @Input() LatestAgreemantId: number = 0;
  @Output() SaveCallBack = new EventEmitter<any>();
  @Output() CancelCallBack = new EventEmitter<boolean>();

  public AllTags: TagModel[] = [];
  public DeleteAgreementConfirmation: boolean = false;
  public CreateAgreementConfirmation: boolean = false;
  public AgreementDescription: string = '';
  public DeleteAgreementId: number = 0;
  public UpdatedLatestAgreementId: number = 0;
  public DocumentService : DocumentService;

  public WYSIWYGConfig: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    minHeight: '15rem',
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

  constructor(public tagService: TagService, documentService: DocumentService) {
    this.DocumentService = documentService;
  }

  ngOnInit(): void {
    this.UpdatedLatestAgreementId = this.LatestAgreemantId;
  }

  ToggleDeleteAgreementConfirm(id: number) {
    this.DeleteAgreementId = id;
    this.DeleteAgreementConfirmation = !this.DeleteAgreementConfirmation;
  }

  DeleteAgreement(target: Agreement[]){
    target.forEach((value, index)=>{
      if( value.Id == this.DeleteAgreementId ){
        target.splice(index, 1);
      }
    });

    this.DeleteAgreementConfirmation = !this.DeleteAgreementConfirmation;
  }

  ToggleCancelAgreementConfirm() {
    this.AgreementDescription = '';
    this.CreateAgreementConfirmation = !this.CreateAgreementConfirmation;
  }

  ViewCreateAgreement() {
    this.CreateAgreementConfirmation = !this.CreateAgreementConfirmation;
  }

  AddAgreement(target: Agreement[]){
    var newItem = new Agreement();
    this.UpdatedLatestAgreementId = this.UpdatedLatestAgreementId + 1;
    newItem.Id = this.UpdatedLatestAgreementId;
    newItem.Description = this.AgreementDescription;
    newItem.IsDeleted = false;
    target.push(newItem);

    this.AgreementDescription = '';
    this.CreateAgreementConfirmation = !this.CreateAgreementConfirmation;
  }

  RemoveDocumentTags(s: TagModel) {
    var index = this.NewDocumentTagItems.findIndex(x => { return x.display == s.display });
    this.NewDocumentTagItems.splice(index, 1);
  }

  AddDocumentTags(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Document;
      // this.tagService.CreateTag(tagToAdd).subscribe((data) => {
      //   var newTM = new TagModel();
      //   newTM.display = s.display;
      //   newTM.value = data;
      //   this.AllTags.push(newTM)
      // });

      var newTM = new TagModel();
      newTM.display = s.display;
      newTM.value = Math.random();
      this.AllTags.push(newTM)
    }
    this.NewDocumentTagItems.push(s);
  }

  Save(targetDocument: Document, associatedTags: TagModel[]) {
    var data = { targetDocument, associatedTags };
    this.DocumentService.CreateDocument(targetDocument).subscribe(x => {console.log(x)});
    this.SaveCallBack.emit(data);
  }

  Cancel() {
    this.CancelCallBack.emit(true)
  }
}

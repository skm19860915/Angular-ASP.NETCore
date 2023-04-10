import { Component, OnInit, Input, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
import { RosterService } from '../../Services/roster.service'
import { Tag, TagType } from '../../Models/Tag';
import { Exercise } from '../../Models/Exercise';
import { Observable, of, interval } from 'rxjs';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Program } from '../../Models/Program/Program';
import { ProgramBuilderService } from '../../Services/program-builder.service';
import { Athlete } from '../../Models/Athlete';
import { AssignProgram } from '../../Models/AssignProgram';
import { ExcludeTagFilterPipe, TagFilterPipe, SearchTaggableFilterPipe } from '../../Pipes';
import { fadeInAnimation } from '../../animation/fadeIn';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { take } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AssignedAthleteCheck } from 'src/app/Models/Athlete/AssignedAthleteCheck';
import { DisplayPaginatedItems } from '../shared/paginator/paginator.component';

@Component({
  selector: 'app-program',
  templateUrl: './program.component.html',
  styleUrls: ['./program.component.less'],
  animations: [fadeInAnimation]
})
export class ProgramComponent implements OnInit {
  public ShowArchive: boolean = false;
  @Input() Model: boolean = false;
  public View: string = "Programs";
  public AllPrograms: Program[] = [];
  public UnModifiedPrograms: Program[] = [];
  public UnmodifiedAthetes: Athlete[] = [];
  public ProgramTagItems: TagModel[] = [];
  public AllProgramTags: TagModel[] = [];
  public SubView: string = "AllPrograms";
  public SelectedProgram: Program;
  public AllAthletes: Athlete[];
  public AllAthleteTags: TagModel[] = [];
  public AthleteIncludeFilterTags: TagModel[] = [];
  public AthleteExcludeFilterTags: TagModel[] = [];
  public AthleteTagItems: TagModel[] = [];
  public AthleteExcludedTagItems: TagModel[] = [];
  public PrintMasterPdf: boolean = true;
  public OnlyPrintChecked: boolean = true;
  private allChecked: boolean = false; //holds the the fact that we clicked on check all. Doing this because a second click on check all should uncheck all\
  public programMenu: boolean = false;
  public ShowProgramPrintMenu: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public ShowWhiteBoardView: boolean = false;
  public darkTheme: boolean = true;
  public Day: number = 1;
  public Week: number = 1;
  public programSearchString: string;
  public ShowAthletesWithAssignedProgramsModal: boolean = false;
  public printUsingAdvancedOptions: boolean = false;
  public TagItems: TagModel[] = [];
  public AthletesThatHaveAssignedPrograms: AssignedAthleteCheck[] = [];
  public processing: boolean = false;
  public ShowHardDeleteWindow: boolean = false;
  public hardDeleteProgramId: number = 0;
  public ProcessDelete: boolean = false;
  public paginationStart: number = 0;
  public paginationEnd: number = 0;
  public allAthleteTags: TagModel[] = [];
  public allProgramTags: TagModel[] = [];
  private NameSearchFilter: string;

  constructor(private cd: ChangeDetectorRef, public router: Router, public RosterService: RosterService, public tagService: TagService, public programBuilderService: ProgramBuilderService,
    public ExcludedTagFilterPipe: ExcludeTagFilterPipe, public TagFilterPipe: TagFilterPipe, public SearchableTagFilterPipe: SearchTaggableFilterPipe) {
    this.GetAllAthletes();

    this.tagService.GetAllTags(TagType.Athlete).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.allAthleteTags.push(newTM)
      }
    });

    this.tagService.GetAllTags(TagType.Program).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.allProgramTags.push(newTM)
      }
    });

  }
  ToggleWhiteBoardView() {
    this.ShowWhiteBoardView = !this.ShowWhiteBoardView;
    this.Day = 1;
    this.Week = 1;
  }
  UpdatePaginiationDisplay(s: DisplayPaginatedItems) {
    this.paginationStart = s.Start;
    this.paginationEnd = s.End;
    this.cd.detectChanges();
  }
  ToggleDarkTheme() {
    this.darkTheme = !this.darkTheme;
  }
  RedirectToWhiteBoardView() {
    if (this.darkTheme) {
      window.open(`./WhiteBoardView/${this.SelectedProgram.Id}/1/${this.Week}/${this.Day - 1}`)
      // this.router.navigate([`/WhiteBoardView/${this.SelectedProgram.Id}/1/${this.Week}/${this.Day}`]);
    }
    else {
      this.router.navigate([`/WhiteBoardView/${this.SelectedProgram.Id}/0/${this.Week}/${this.Day - 1}`]);
    }
  }
  ToggleShowProgramPrintMenu() {
    this.ShowProgramPrintMenu = !this.ShowProgramPrintMenu;
  }

  ProgramList() {
    this.AllAthletes = this.UnmodifiedAthetes;
    this.AthleteIncludeFilterTags = [];
    this.AthleteExcludeFilterTags = [];
    this.programMenu = false;
  }
  ManipulateProgram(program) {
    window.scroll(0, 0);
    this.programMenu = true;
    this.SelectedProgram = program;
  }
  ViewAllPrograms() {
    this.SubView = 'AllPrograms'
  }
  ViewAllAthleteTagsTags() {
    this.SubView = 'AllAthleteTags'
  }
  ViewAllAthlete() {
    this.SubView = 'AllAthlete'
  }

  CancelAssignProgram() {
    this.View = "Programs"
    this.SelectedProgram = new Program();
  }

  ngOnInit() {
    this.GetAllPrograms();
    this.tagService.GetAllTags(TagType.Program).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllProgramTags.push(newTM)
      }
    });
  }
  ToggleArchive() {
    this.ShowArchive = !this.ShowArchive;
  }

  GetAllAthletes() {
    this.RosterService.GetAllAthletes().subscribe(x => {
      x.forEach(y => y.Name = y.FirstName + ' ' + y.LastName) //need to rig up the seach
      this.AllAthletes = x;
      this.UnmodifiedAthetes = x;
    });
  }

  GetAllPrograms(): void {
    this.programBuilderService.GetAllPrograms().subscribe(x => { this.AllPrograms = x; this.UnModifiedPrograms = x; });
  }

  SetSelectedTag(tag: TagModel) {
    this.ProgramTagItems = [];
    this.ProgramTagItems.push(tag);
    this.ViewAllPrograms();
  }

  CheckAthletesForAssignedPrograms(programId: number) {
    let checkedAthletes: number[] = [];
    this.AllAthletes.forEach(x => {
      if (x.Checked) { checkedAthletes.push(x.Id); }
    });
    this.RosterService.CheckAtheletesForAssignedPrograms(checkedAthletes).subscribe(x => {
      this.ShowAthletesWithAssignedProgramsModal = true;
      this.AthletesThatHaveAssignedPrograms = x;
      debugger;
      if (x.length === 0) {
        this.AssignProgramToAllFilteredAthletes(programId);
      }
    });
  }
  ToggleShowAthletesWithAssignedProgramsModal() {
    this.ShowAthletesWithAssignedProgramsModal = !this.ShowAthletesWithAssignedProgramsModal;
  }
  ToggleOnlyCheckedAdvancedOptionsPrint() {
    this.printUsingAdvancedOptions = !this.printUsingAdvancedOptions;
  }
  AssignProgramToAllFilteredAthletes(programId: number) {
  //  this.processing = true;
    if (this.ShowAthletesWithAssignedProgramsModal) {
      this.ToggleShowAthletesWithAssignedProgramsModal()

    }
    var assDTO = new AssignProgram();
    let checkedAthletes: number[] = [];
    this.AllAthletes.forEach(x => {
      if (x.Checked) { checkedAthletes.push(x.Id); }
    });
    assDTO.AthleteIds = checkedAthletes;
    assDTO.ProgramId = programId;
    this.DisplayMessage("The Program Is Being Assigned (this may take a few minutes) you can continue to use the app.", "Program Assigned Updated", false)
    this.RosterService.AssignProgramToAthletes(assDTO).subscribe(
      success => {
   //     this.DisplayMessage("Program Assigned SUCCESSFULL", "Program Assigned Updated", false)
   //     this.processing = false;
      },
      error => {
  //      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
    //    this.DisplayMessage("Program Assigned UNSUCCESSFULL", errorMessage, true)
      //  this.processing = false;
      });
  }


  AssignProgramToAthlete(athleteId: number, programId: number) {
    var assDTO = new AssignProgram();
    assDTO.AthleteIds.push(athleteId);
    assDTO.ProgramId = programId;
    this.RosterService.AssignProgramToAthletes(assDTO).subscribe(success => {
      this.DisplayMessage("Program Assigned SUCCESSFULL", "Program Assigned", false)
    },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Program Assigned UNSUCCESSFULL", errorMessage, true)
      });
  }

  SetSelectedAthleteTag(tag: TagModel) {
    this.AthleteTagItems = [];
    this.AthleteTagItems.push(tag);
    this.ViewAllAthlete();
  }

  AssignSelectedAthlete() { }

  AddProgramTag(s: TagModel) {

    if (this.AllProgramTags.find(d => { return d.display == s.display }) == null) {
      // use onAdding instead
      //or you can pop the last item, because the last item is what wasnt found
      //maybe send an alert
      this.ProgramTagItems.pop();
    }
  };

  AddAthleteTag(s: TagModel) {

    if (this.AllAthleteTags.find(d => { return d.display == s.display }) == null) {
      // use onAdding instead
      //or you can pop the last item, because the last item is what wasnt found
      //maybe send an alert
      this.ProgramTagItems.pop();
    }
  };

  AssignSelectedProgram(targetP: Program) {
    this.RosterService.GetAllAthletes().subscribe(x => {
      x.forEach(y => y.Name = y.FirstName + ' ' + y.LastName) //need to rig up the seach
      this.AllAthletes = x;
    });
    this.View = "Athletes"
    this.SelectedProgram = targetP;

    this.tagService.GetAllTags(TagType.Athlete).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllAthleteTags.push(newTM)
      }
    });

  }
  RemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AllProgramTags.splice(index, 1);
    this.TagItems.splice(index, 1);

    this.AllPrograms = this.TagFilterPipe.transform(this.UnModifiedPrograms, this.TagItems)
  }
  AddTag(s: TagModel) {

    if (this.AllProgramTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Exercise;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.display = s.display;
        newTag.value = data;
        this.AllProgramTags.push(newTag);

      });
    }
    this.TagItems.push(s);
    this.AllPrograms = this.TagFilterPipe.transform(this.UnModifiedPrograms, this.TagItems)
  };
  SearchThroughAthletes() {
    let includeTagsFilter = this.TagFilterPipe.transform(this.UnmodifiedAthetes, this.AthleteIncludeFilterTags);
    let includeAndExcludeTagsFilter = this.ExcludedTagFilterPipe.transform(includeTagsFilter, this.AthleteExcludeFilterTags);

    if (this.NameSearchFilter !== null && this.NameSearchFilter !== undefined && this.NameSearchFilter !== '') {
      this.AllAthletes = includeAndExcludeTagsFilter.filter(x => {
        var ret = false;
        if (x.FirstName !== undefined &&  x.FirstName !== null) {
          ret = ret || x.FirstName.toLowerCase().includes(this.NameSearchFilter)
        }
        if (x.LastName !== undefined && x.LastName !== null) {
        ret = ret || x.LastName.toLowerCase().includes(this.NameSearchFilter)
        }
        return ret;
      });
    }

    else {
      this.AllAthletes = includeAndExcludeTagsFilter;
    }
  }
  NameChange(event) {
    this.NameSearchFilter = event;
    this.SearchThroughAthletes();
  }
  AthleteIncludeRemoveTag(s: TagModel) {
    var index = this.AthleteIncludeFilterTags.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AthleteIncludeFilterTags.splice(index, 1);
    this.SearchThroughAthletes();
  }
  AthleteInculdeAddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AthleteIncludeFilterTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
      });
    }
    this.AthleteIncludeFilterTags.push(s);
    this.SearchThroughAthletes();
  };
  AthleteExcludeRemoveTag(s: TagModel) {
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.AthleteExcludeFilterTags.findIndex(x => { return x.display == s.display });// this.TagItems.findIndex(x => x.display == s.display);
    this.AthleteExcludeFilterTags.splice(index, 1);
    this.SearchThroughAthletes();
  }

  AthleteExcludeAddTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AthleteExcludeFilterTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.value = data;
      });
    }
    this.AthleteExcludeFilterTags.push(s);
    this.SearchThroughAthletes();
  };
  DuplicateProgram(program): void {
    program.isBusy = true;
    this.programBuilderService.DuplicateProgram(program.Id).subscribe(
      success => {
        program.isBusy = true;
        this.DisplayMessage("DUPLICATION SUCCESSFULL", "Program Duplicated", false)
        this.GetAllPrograms();
      },
      error => {
        program.isBusy = true;
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("DUPLICATION UNSUCCESSFULL", errorMessage, true)
      });
  }
  UnArchiveProgram(programId: number) {
    this.programBuilderService.UnArchiveProgram(programId).subscribe(
      success => {
        this.DisplayMessage("Program UnArchived", "UNARCHIVE SUCCESSFULL", false)
        this.GetAllPrograms();
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("ARCHIVE UNSUCCESSFULL", errorMessage, true)
      });
  }
  ArchiveProgram(programId: number) {
    this.programBuilderService.ArchiveProgram(programId).subscribe(
      success => {
        this.GetAllPrograms();
        this.DisplayMessage("Program Archived", "Archive Successfull", false)
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("ARCHIVE UNSUCCESSFULL", errorMessage, false)
      });
  }
  ConfirmArchive(programId: number) {
  }
  CancelArchive() {
  }
  toggleOnlyCheckedPrint() {
    this.OnlyPrintChecked = !this.OnlyPrintChecked;
  }
  togglePrintMaster() {
    this.PrintMasterPdf = !this.PrintMasterPdf;
  }

  PrintPdf(programId: number) {
    let checkedAthletes: number[] = [];
    this.AllAthletes.forEach(x => {
      if (x.Checked) { checkedAthletes.push(x.Id); }
    });
    this.ToggleShowProgramPrintMenu();
    this.programBuilderService.PrintPdfProgram(programId, this.PrintMasterPdf, this.OnlyPrintChecked, checkedAthletes, this.SelectedProgram.HasAdvancedOptions || this.printUsingAdvancedOptions).subscribe(success => {
      this.DisplayMessage("PDF Is Being Generated", "Your Program Is Being Generated And Will Be Emailed To You", false)
    },
      error => {
        this.DisplayMessage("PDF GENERATING UNSUCCESSFULL", "There was an Error Generating your program, please contact Customer Support " + error.error, true)
      });

  }


  public CheckAllAthletes() {
    this.allChecked = !this.allChecked;
    this.AllAthletes.forEach(x => x.Checked = this.allChecked);

  };

  public ToggleAthleteIdInList(athleteId: number) {
    this.AllAthletes.forEach(x => {
      if (x.Id === athleteId) {
        x.Checked = !x.Checked
      }
    });
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  public ToggleHardDeleteModal(exerciseId) {

    this.hardDeleteProgramId = exerciseId;
    this.ShowHardDeleteWindow = !this.ShowHardDeleteWindow;
  }
  public HardDelete() {
    this.ProcessDelete = true;
    this.programBuilderService.HardDeleteProgram(this.hardDeleteProgramId).subscribe(success => {
      this.ProcessDelete = false;
      this.ToggleHardDeleteModal(0);
      this.DisplayMessage("Program DELETED", "Program Successfully Deleted", false);
      this.GetAllPrograms();
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.ToggleHardDeleteModal(0);
      this.ProcessDelete = false;
      this.DisplayMessage("Program NOT DELETED", errorMessage, true);
    });
  }

  NavigateToBuilder() {
    this.router.navigateByUrl('/ProgramBuilder');
  }
}


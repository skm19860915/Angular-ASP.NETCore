import { Component, OnInit } from '@angular/core';
import { RosterService } from '../../../Services/roster.service';
import { Athlete } from '../../../Models/Athlete';
import { TagService } from '../../../Services/tag-service.service';
import { TagModel } from '../../../Models/TagModel';
import { Tag, TagType } from '../../../Models/Tag';
import { SearchThroughTagsPipe, ExcludeTagFilterPipe, TagFilterPipe } from '../../../Pipes';
import { UserService } from '../../../Services/user.service';
import type { DisplayPaginatedItems } from '../../shared/paginator/paginator.component';
import { fadeInAnimation } from '../../../animation/fadeIn';

@Component({
  selector: 'app-weight-room',
  templateUrl: './weight-room.component.html',
  styleUrls: ['./weight-room.component.less'],
  animations: [fadeInAnimation]
})
export class WeightRoomComponent implements OnInit {
  public AllAthletes: Athlete[] = [];
  public UnModifiedAthletes: Athlete[] = [];
  public CheckedAthletes: Athlete[] = [];
  private allChecked: boolean = false; //holds the the fact that we clicked on check all. Doing this because a second click on check all should uncheck all\
  public AllTags: TagModel[] = [];
  public unModifiedTags: TagModel[] = [];
  public selectedTags: TagModel[] = [];
  public AthleteSearchString: string;
  public tagSearchString: string;
  public view: string = "weightroom";
  public SelectedAthletes: Athlete[] = [];
  public NameSearchFilter: string;
  public paginationStart: number = 0;
  public paginationEnd: number = 0;
  public showMenu: boolean = false;


  constructor(public ExcludedTagFilterPipe: ExcludeTagFilterPipe, public TagFilterPipe: TagFilterPipe, public userService: UserService, public searchThroughTagsPipe: SearchThroughTagsPipe, public excludeTagFilter: ExcludeTagFilterPipe, public tagFilterPipe: TagFilterPipe, public tagService: TagService, public rosterService: RosterService) { }

  ngOnInit() {
    this.userService.LogInWeightRoomUser();
    this.GetAllAthletes();
    this.tagService.GetAllTags(TagType.Athlete).subscribe(d => {
      //i am dumb and i should just use a tag
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
        this.unModifiedTags.push(newTM);
      }
    });
  }

  public StartWorkout() {
    this.view = "workout";
    if (!this.SelectedAthletes.length) {
      this.showMenu = true;
    }
  }
  public StartWeightRoom() {
    this.view = "weightroom";
    this.showMenu = false;
  }

  public ToggleSelectedAthletes(athlete: Athlete) {
    if (this.SelectedAthletes.includes(athlete)) {
      this.SelectedAthletes.splice(this.SelectedAthletes.indexOf(athlete), 1);
    }
    else {
      this.SelectedAthletes.push(athlete);
    }

    if (this.SelectedAthletes.length > 4) {
      this.SelectedAthletes.shift();
    }
  }

  public ToggleAthleteIdInList(athleteId: number, athlete: Athlete) {

    athlete.Checked = !athlete.Checked;

    if (athlete.Checked) {
      this.CheckedAthletes.push(athlete)
    }
    else {
      let i = this.CheckedAthletes.findIndex(x => x.Id === athleteId);
      this.CheckedAthletes.splice(i, 1)
    }
    console.log(this.CheckedAthletes);
  }
  public ToggleAllCheckedAthletes() {
    this.allChecked = !this.allChecked;
    this.CheckedAthletes = [];//wipe out all the checked athletes, then toggle them. The toggle will see they are not in there and add
    if (this.allChecked) {
      this.AllAthletes.forEach(a => {
        a.Checked = true;
        this.CheckedAthletes.push(a);
      });
    }
    else {
      this.AllAthletes.forEach(a => {
        a.Checked = false
      });
    }
  };

  public ToggleSelectedTag(tag: TagModel) {
    let foundIndex = this.selectedTags.findIndex(x => x.display === tag.display);
    if (foundIndex > -1) {

      this.selectedTags.splice(foundIndex, 1);
    }
    else {
      this.selectedTags.push(this.unModifiedTags[this.unModifiedTags.findIndex(x => x.display === tag.display)])
    }
    this.FilterAthletes();
  }

  public FilterAthletes() {

    this.AllAthletes = this.UnModifiedAthletes;
    this.AllAthletes.forEach((x: Athlete) => x.Checked = false);
    this.AllAthletes = this.tagFilterPipe.transform(this.UnModifiedAthletes, this.selectedTags);

    if (this.NameSearchFilter !== null && this.NameSearchFilter !== undefined && this.NameSearchFilter !== '') {
      this.AllAthletes = this.AllAthletes.filter(x => {
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

    this.AllAthletes.forEach(x => x.Checked = this.allChecked)
    this.CheckedAthletes = [];
    this.AllAthletes.forEach(x => {
      if (x.Checked) {
        this.CheckedAthletes.push(x);
      }
    });
  }

  public FilterTag(searchString: string) {
    this.selectedTags = [];
    if (searchString == '' || searchString == undefined) {
      this.AllTags = [];
      for (var i = 0; i < this.unModifiedTags.length; i++) {
        if (this.AllTags.findIndex(x => x.display == this.unModifiedTags[i].display) == -1) {
          this.AllTags.push(this.unModifiedTags[i]);
        }
      }
    }
    else {

      let tempTags = this.searchThroughTagsPipe.transform(this.unModifiedTags, searchString);
      this.AllTags = [];
      for (var i = 0; i < tempTags.length; i++) {
        if (this.AllTags.findIndex(x => x.display == tempTags[i].display) == -1) {
          this.AllTags.push(tempTags[i]);
        }
      }
    }
  }
  public IsTagSelected(value) {
    return this.selectedTags.find(x => x.value === value);
  }

  public UpdatePaginationDisplay(paginationEvent: DisplayPaginatedItems) {
    this.paginationStart = paginationEvent.Start;
    this.paginationEnd = paginationEvent.End;
  }

  GetAllAthletes() {
    this.rosterService.GetAllAthletes().subscribe(x => {
      x.forEach(y => y.Name = y.FirstName + ' ' + y.LastName) //need to rig up the seach
      this.AllAthletes = x;
      this.UnModifiedAthletes = x;
    });
  }

  NameChange(event) {
    this.NameSearchFilter = event;
    this.FilterAthletes();
  }

  ToggleMenu() {
    this.showMenu = !this.showMenu;
  }

}

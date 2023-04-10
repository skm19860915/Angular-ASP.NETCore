import { Component, OnInit, Input } from '@angular/core';
import { Tag, TagType } from '../../Models/Tag';
import { interval } from 'rxjs';
import { TagModel } from '../../Models/TagModel';
import { TagService } from '../../Services/tag-service.service';
import { Athlete } from '../../Models/Athlete';
import { RosterService } from '../../Services/roster.service';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { MetricsService } from '../../Services/metrics.service';
import { Metric } from '../../Models/Metric/Metric';
import { HideDeletedSortPipe, TagFilterPipe } from '../../Pipes';
import { ITaggable } from '../../Interfaces/ITaggable';
import { IDatePickerConfig } from 'ng2-date-picker';
import { AlertMessage } from '../../Models/AlertMessage';
import { take } from 'rxjs/operators';
import { fadeInAnimation } from 'src/app/animation/fadeIn';


@Component({
  selector: 'app-roster',
  templateUrl: './roster.component.html',
  styleUrls: ['./roster.component.less'],
  animations: [fadeInAnimation]
})
export class RosterComponent implements OnInit {
  @Input() Model: boolean = false;
  public AllTags: TagModel[] = [];
  public TagItems: TagModel[] = [];
  public newRosterTagItems: TagModel[] = [];
  public AllAthletes: Athlete[] = [];
  public UnModifiedAthletes: Athlete[] = [];
  public CreateNewExercise: boolean = false;
  public View: string = 'Athletes';
  public ExerciseFilterTags: TagModel[] = [];
  public SelectedAthlete: Athlete = new Athlete();
  public searchString: string;
  public uploadURL: string = environment.endpointURL + '/api/MultiMedia/CreateProfilePicture';
  public AllMetrics: Metric[] = []; // all possible metrics
  public DisplayMetrics: Metric[] = [];
  public TargetMetricId: number;
  public AddedMetrics: AthleteMetric[] = [];
  public AlertMessages: AlertMessage[] = [];
  public datePickerConfig: IDatePickerConfig = { format: "MM/DD/YYYY", }

  public uploader: FileUploader = new FileUploader({
    url: this.uploadURL,
    disableMultipart: false,
    headers:
      [
        {
          name: 'Access-Control-Allow-Origin',
          value: '*'
        }, {
          name: 'Access-Control-Allow-Credentials',
          value: 'true'
        }]
  });//todo: Bad hack to get this to work. 
  //we will change the endpoint to be the url plus the athlete ID, this way we can associate an athlete with a profile picture

  constructor(public tagFilterPipe: TagFilterPipe,public hidePipe: HideDeletedSortPipe, public metricService: MetricsService, public rosterService: RosterService, public tagService: TagService,) {

  }

  ngOnInit() {
    this.metricService.GetAllMetrics().subscribe(x => { this.DisplayMetrics = this.hidePipe.transform(x,false); this.AllMetrics = this.hidePipe.transform(x,false); });
    this.GetAllAthletes();
    this.tagService.GetAllTags(TagType.Athlete).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });
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
  AddMetricToCompletedList(metricId: number) {
    var newMetricList: Metric[] = [];
    for (let index = 0; index < this.DisplayMetrics.length; index++) {
      const element = this.DisplayMetrics[index];
      if (element.Id != metricId) {
        newMetricList.push(element);
      }
      else {
        this.AddedMetrics.push({ MetricId: element.Id, MetricName: element.Name, Value: 0, CompletedDate: new Date() });
      }
    }
    this.DisplayMetrics = newMetricList;
  }
  RemoveMetricFromCompletedList(metricId: number) {

    var newMetricList: Metric[] = []
    this.DisplayMetrics.forEach(x => newMetricList.push(x));
    for (let index = 0; index < this.AllMetrics.length; index++) {
      const element = this.AllMetrics[index];
      if (element.Id == metricId) {
        newMetricList.push(element);
      }
    }
    this.DisplayMetrics = newMetricList;
    this.TargetMetricId = undefined;
    for (let index = 0; index < this.AddedMetrics.length; index++) {
      const element = this.AddedMetrics[index];
      if (element.MetricId == metricId) {
        this.AddedMetrics.splice(index, 1);
      }
    }
  }
  GetAllAthletes() {
    this.rosterService.GetAllAthletes().subscribe(x => {
      x.forEach(y => y.Name = y.FirstName + ' ' + y.LastName) ;
      this.AllAthletes = x;
      this.UnModifiedAthletes = x
    });
  }



  CreateAthlete(targetAthlete: Athlete, associatedTags: TagModel[], uploader: FileUploader) {
    var newAthlete = new Athlete();
    if (targetAthlete.Email == '' || targetAthlete.Email == undefined) {
      this.DisplayMessage("Save UNSUCCESSFULL","Athletes Need To Have An Email",true)
      return;
    }
    if (targetAthlete.Id > 0) { }
    newAthlete.Email = targetAthlete.Email;
    newAthlete.FirstName = targetAthlete.FirstName;
    newAthlete.LastName = targetAthlete.LastName;
    newAthlete.Weight = targetAthlete.Weight;
    newAthlete.HeightPrimary = targetAthlete.HeightPrimary;
    newAthlete.HeightSecondary = targetAthlete.HeightSecondary;
    newAthlete.Birthday = targetAthlete.Birthday;

    associatedTags.forEach((value, index) => {
      this.AllTags.forEach((sourceTag: TagModel) => {
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newAthlete.Tags.push(newTag);
        }
      });
    });

    this.rosterService.CreateAthlete(newAthlete, this.AddedMetrics).subscribe(
      success => {
        this.DisplayMessage("Save SUCCESSFULL","Athlete Saved",false)
        
        // if (uploader != undefined && uploader[0] != undefined) {
          uploader[0].url = this.uploadURL + "/" + success;
          uploader[0].upload();
        // }
        targetAthlete = new Athlete();
        this.SelectedAthlete = new Athlete();
        this.newRosterTagItems = [];
        this.uploader.queue.pop();
        this.GetAllAthletes();
        this.metricService.GetAllMetrics().subscribe(x => { this.DisplayMetrics = this.hidePipe.transform(x,false); this.AllMetrics = this.hidePipe.transform(x,false); });
        this.AddedMetrics = [];
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Save UNSUCCESSFULL", errorMessage, true)
      });
  }



  AddTag(s: TagModel) {
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        var newTag = new TagModel()
        newTag.display = s.display;
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
    this.TagItems.push(s)
    this.AllAthletes = this.tagFilterPipe.transform(this.UnModifiedAthletes, this.TagItems)
  };
  RemoveTag(s: TagModel) {
    
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
    var index = this.TagItems.findIndex(x => x.display == s.display);
    this.TagItems.splice(index, 1);
    this.AllAthletes = this.tagFilterPipe.transform(this.UnModifiedAthletes, this.TagItems);
  }

  ngAfterViewInit() {

  }

  ViewCreateAthleteMenu() {
    this.View = 'CreateAthlete';
    this.newRosterTagItems = [];
    this.TagItems.forEach((value, index) => {//the stupid onAdd doesnt add an object it just adds a fucking string not an object
      this.newRosterTagItems.push({ display: value.display, value: value.value });
    });
  }
  ClearUploaderQueue(uploader)
  {
    uploader.queue.pop();
  }
  ForceUploaderQueueToBeJustOne(uploader) {
    if (uploader.queue.length > 1) {
      uploader.queue[0] = uploader.queue[1];
      uploader.queue.pop();
    }

  }

  ViewAllAthletes() {
    this.View = 'Athletes';
  }

  SetSelectedTag(tag: TagModel) {
    this.TagItems = [];
    this.TagItems.push(tag);
    //this.ViewAllExercises();
  }

  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
}

export class AthleteMetric {
  public MetricId: number;
  public Value: number;
  public MetricName: string;
  public CompletedDate: Date;
}



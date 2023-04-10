import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Athlete } from '../../Models/Athlete';
import { AlertMessage } from '../../Models/AlertMessage';
import { interval } from 'rxjs';
import { RosterService } from '../../Services/roster.service';
import { Tag, TagType } from '../../Models/Tag';
import { MetricsService } from '../../Services/metrics.service';
import { TagModel } from '../../Models/TagModel';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { take } from 'rxjs/operators';
import { Metric } from '../../Models/Metric/Metric';
import { HideDeletedSortPipe } from '../../Pipes';
import { OrganizationService } from '../../Services/organization.service'
import { TagService } from '../../Services/tag-service.service';
import { AthleteService } from '../../Services/athlete.service';
import { SubscriptionInfo } from '../../Models/Organization/SubscriptionInfo';
import { AthleteCount } from '../../Models/Organization/AthleteCount';
import { fadeInAnimation } from '../../animation/fadeIn';
@Component({
  selector: 'app-create-athlete',
  templateUrl: './create-athlete.component.html',
  styleUrls: ['./create-athlete.component.less'],
  animations: [fadeInAnimation]
})
export class CreateAthleteComponent implements OnInit {

  public TargetMetricId: number;
  public AlertMessages: AlertMessage[] = [];
  public AllTags: TagModel[] = [];
  public AthleteTags: TagModel[] = [];
  public SelectedAthlete: Athlete = new Athlete();
  public AddedMetrics: AthleteMetric[] = [];
  public newRosterTagItems: TagModel[] = [];
  public uploadURL: string = environment.endpointURL + '/api/MultiMedia/CreateProfilePicture';
  public DisplayMetrics: Metric[] = []
  public AllMetrics: Metric[] = [];
  public ShowAuthorizationStatement: boolean = false;
  public subPlan: SubscriptionInfo = new SubscriptionInfo();
  public athleteTotal: AthleteCount = new AthleteCount();
  public Processing: boolean = false;
  public upgradeConfirmation: string = '';

  public IsAuthorized: boolean = false;
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

  constructor(public athleteService: AthleteService, public tagService: TagService, public orgService: OrganizationService, public rosterService: RosterService, public metricService: MetricsService, public hidePipe: HideDeletedSortPipe) { }

  ngOnInit() {
    this.metricService.GetAllMetrics().subscribe(x => {
      if (x.length == 0) return;
      this.DisplayMetrics = this.hidePipe.transform(x, false); this.AllMetrics = this.hidePipe.transform(x, false);
    });
    this.metricService.GetAllMetrics().subscribe(x => {
      if (x.length == 0) return;
      this.DisplayMetrics = this.hidePipe.transform(x, false); this.AllMetrics = this.hidePipe.transform(x, false);
    });
    this.metricService.GetAllMetrics().subscribe(x => {
      if (x.length == 0) return;
      this.DisplayMetrics = this.hidePipe.transform(x, false); this.AllMetrics = this.hidePipe.transform(x, false);
    });
    this.GetTotalAthleteCount();
  }
  GetTotalAthleteCount() {
    this.orgService.GetCurrentAthleteCountAndMax().subscribe(x => {
      this.ShowAuthorizationStatement = x.TotalAthletes >= x.MaxAthletes;

      this.athleteTotal = x;
      if (this.ShowAuthorizationStatement) {
        this.orgService.GetOrganizationSubscription().subscribe(x => { this.subPlan = x })
      }
    });
  }
  CancelAccountUpgrade() {
    this.ShowAuthorizationStatement = false;
    this.Processing = false
  }
  UpgradeAccount() {
    this.Processing = false;
    this.ShowAuthorizationStatement = false;
    this.orgService.UpgradeSubscription().subscribe(success => {
      this.IsAuthorized = true;
      this.DisplayMessage("Upgrade Successful", "Upgrade successful", false);
    },
      error => {
        this.IsAuthorized = false;
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Upgrade Unsuccesfull", errorMessage, true)
      });
  }
  ForceUploaderQueueToBeJustOne(uploader) {
    if (uploader.queue.length > 1) {
      uploader.queue[0] = uploader.queue[1];
      uploader.queue.pop();
    }

  }
  ClearUploaderQueue(uploader) {
    uploader.queue.pop();
  }
  public RemoveAthleteTag(s: TagModel) {
    var index = this.AthleteTags.findIndex(x => { return x.display == s.display });
    this.AthleteTags.splice(index, 1);
  }
  public AddNewAthleteTag(s: TagModel) {
    var newTag = new TagModel()
    newTag.display = s.display;
    if (this.AllTags.find(d => { return d.display == s.display }) == null) {
      var tagToAdd = new Tag();
      tagToAdd.Name = s.display;
      tagToAdd.Type = TagType.Athlete;
      this.tagService.CreateTag(tagToAdd).subscribe((data) => {
        newTag.value = data;
        this.AllTags.push(newTag);
      });
    }
    this.AthleteTags.push(s);
    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
  }



  CreateAthlete(targetAthlete: Athlete, associatedTags: TagModel[], addedMetrics, uploader: FileUploader) {
    this.Processing = true;

    if (targetAthlete.FirstName === '' || targetAthlete.FirstName === undefined || targetAthlete.LastName === '' || targetAthlete.LastName === undefined)
    {
      this.DisplayMessage("Athletes must have a First and Last Name to save", "Save UnSuccessfull", true);
      this.Processing = false;
      return;
    }
    //everyone can have unlimited athletes.
    // if (this.athleteTotal != undefined) {
    //   if ((this.athleteTotal.TotalAthletes >= this.athleteTotal.MaxAthletes) && !this.IsAuthorized) {
    //     this.ShowAuthorizationStatement = true;
    //     return;
    //   }
    // }

    var newAthlete = new Athlete();

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
    let newMetricList = [];

    ///this nonsense below is because the new datepicker has completed date as an object with properties startDate and endDate. i used to just send the entier metric list 
    ///to the api but that expected completedDate to be just a date. thus the object wouldnt deserilize becuase completedDate turned into an object
    if (addedMetrics.length > 0) {
      for (var i = 0; i < addedMetrics.length; i++) {
        newMetricList.push({
          MetricId: addedMetrics[i].MetricId,
          MetricName: addedMetrics[i].MetricName,
          Value: addedMetrics[i].Value,
          CompletedDate: addedMetrics[i].CompletedDate.startDate
        })
      }
    }
    this.rosterService.CreateAthlete(newAthlete, newMetricList).subscribe(
      success => {
        this.GetTotalAthleteCount();
        this.DisplayMessage("Athlete Saved", "Save Successfull", false)
        if (uploader.queue.length > 0) {
          uploader.queue[0].url = this.uploadURL + "/" + success;
          uploader.queue[0].upload();
        }
        targetAthlete = new Athlete();
        this.SelectedAthlete = new Athlete();
        this.newRosterTagItems = [];
        this.uploader.queue.pop();
        this.metricService.GetAllMetrics().subscribe(x => { this.DisplayMetrics = this.hidePipe.transform(x, false); this.AllMetrics = this.hidePipe.transform(x, false); });
        this.AddedMetrics = [];
        this.Processing = false;
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Save UNSUCCESSFULL", errorMessage, true)
        this.Processing = false;
      });
  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }

  SelectSearch() { }



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
}
export class AthleteMetric {
  public MetricId: number;
  public Value: number;
  public MetricName: string;
  public CompletedDate: Date;
}

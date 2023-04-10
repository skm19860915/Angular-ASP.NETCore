import { Component, OnInit } from '@angular/core';
import { Athlete } from 'src/app/Models/Athlete';
import { ActivatedRoute, Router } from '@angular/router';
import { RosterService } from 'src/app/Services/roster.service';
import { AlertMessage } from 'src/app/Models/AlertMessage';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';
import { MetricsService } from 'src/app/Services/metrics.service';
import { Metric } from 'src/app/Models/Metric/Metric';
import { AthleteMetric } from '../roster/roster.component';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { TagModel } from 'src/app/Models/TagModel';
import { UserService } from '../../Services/user.service';
import { AthleteService } from 'src/app/Services/athlete.service';
import { TagType, Tag } from 'src/app/Models/Tag';
import { HideDeletedSortPipe } from 'src/app/Pipes';
import { fadeInAnimation } from '../../animation/fadeIn';
import { TagService } from 'src/app/Services/tag-service.service';


@Component({
  selector: 'app-athlete-bio',
  templateUrl: './athlete-bio.component.html',
  styleUrls: ['./athlete-bio.component.less'],
  animations: [fadeInAnimation]
})
export class AthleteBioComponent implements OnInit {

  public CurrentAthlete: Athlete = new Athlete();
  public ShowAthleteDeleteWindow: boolean = false;
  public SendingAnotherConfirmationEmail: boolean = false;
  public AlertMessages: AlertMessage[] = [];
  public DisplayMetrics: Metric[] = [];
  public AllMetrics: Metric[] = []; // all possible metrics
  public TargetMetricId: number;
  public AddedMetrics: AthleteMetric[] = [];
  public Updating: boolean = false;
  public ValidEmail: boolean = true;
  public EmailInUse: boolean = false;
  public AllTags: TagModel[] = [];
  public athleteDeleteConfirmation: string = '';
  public SelectSearch: string = '';
  public IsCoach: boolean = false;
  public datePickerConfig ={ format:"DD-MM-YYYY"}
  public uploadURL: string = environment.endpointURL + '/api/MultiMedia/CreateProfilePicture';
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

  constructor(public tagService: TagService, public router: Router, public athleteService: AthleteService, public userService: UserService, private route: ActivatedRoute, private rosterService: RosterService, public metricService: MetricsService,
    public hidePipe: HideDeletedSortPipe,) {



    }

  ngOnInit() {

    this.tagService.GetAllTags(TagType.Athlete).subscribe(d => {
      for (var i = 0; i < d.length; i++) {
        var newTM = new TagModel();
        newTM.display = d[i].Name;
        newTM.value = d[i].Id;
        this.AllTags.push(newTM)
      }
    });

    this.IsCoach = this.userService.IsCoach();
    this.metricService.GetAllMetrics().subscribe(x => {
      if (x.length == 0) return;
      this.DisplayMetrics = this.hidePipe.transform(x, false);
      this.AllMetrics = this.DisplayMetrics;
    });

    this.route.params.subscribe(params => {

      if (params['athleteId'] != undefined) {//coach navigate to athletes page
        this.rosterService.GetAthlete(params['athleteId']).subscribe(success => {

          this.CurrentAthlete = success;
          this.CurrentAthlete.DisplayTags = [];
          this.CurrentAthlete.Birthday = new Date(success.Birthday + "z");
          success.Tags.forEach(element => {
            this.CurrentAthlete.DisplayTags.push({ display: element.Name, value: element.Id });
          });
        })
      }
      else {
        this.rosterService.GetLoggedInAthlete().subscribe(x => {

          this.CurrentAthlete = x
          x.Tags.forEach(element => {
            this.CurrentAthlete.DisplayTags.push({ display: element.Name, value: element.Id });
          });
        })
        //athlete logged in, to view their stuff
      }
    });
  }
  ToggleAthleteDeleteWindow() {
    this.ShowAthleteDeleteWindow = !this.ShowAthleteDeleteWindow;
  }

  public ResendAthleteRegistration(athleteId: number) {
    this.SendingAnotherConfirmationEmail = true;
    this.rosterService.ResendAthleteRegistartion(athleteId).subscribe(success => {
      this.DisplayMessage("Registartion Resent SUCCESSFULL", "A New Registration Email Was Sent To Your Athlete", false)
      this.SendingAnotherConfirmationEmail = false;
    }, error => {
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.DisplayMessage("Registartion Resent UNSUCCESSFULL", errorMessage, true)
      this.SendingAnotherConfirmationEmail = false;
    })
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
      if (element.Id === metricId) {
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
  UpdateAthlete(targetAthlete: Athlete, associatedTags: TagModel[], addedMetrics, uploader: FileUploader) {
    var newAthlete = new Athlete();
    this.Updating = true;

    newAthlete.Id = targetAthlete.Id;
    newAthlete.FirstName = targetAthlete.FirstName;
    newAthlete.LastName = targetAthlete.LastName;
    newAthlete.Weight = targetAthlete.Weight;
    newAthlete.HeightPrimary = targetAthlete.HeightPrimary;
    newAthlete.HeightSecondary = targetAthlete.HeightSecondary;
    newAthlete.Birthday = targetAthlete.Birthday;
    newAthlete.Email = targetAthlete.Email;


    for (let index = 0; index < associatedTags.length; index++) {
      const value = associatedTags[index];
      for (let index = 0; index < this.AllTags.length; index++) {
        const sourceTag = this.AllTags[index];
        if (sourceTag.display == value.display) {
          var newTag = new Tag();
          newTag.Id = sourceTag.value;
          newTag.Name = sourceTag.display;
          newTag.Type = TagType.Exercise;
          newAthlete.Tags.push(newTag);
        }
      }
    }

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

    this.athleteService.UpdateAthlete(newAthlete, newMetricList).subscribe(success => {
      if (uploader != undefined && uploader.queue[0] != undefined) {
        uploader.queue[0].url = this.uploadURL + "/" + targetAthlete.Id;
        uploader.queue[0].upload();
      }

      this.uploader.queue.pop();
      this.AddedMetrics = [];
      this.Updating = false;
      this.DisplayMessage("SAVE  SUCCESSFULL", "Update Successfull", false)

    }, error => {
      this.AddedMetrics = [];
      var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
      this.Updating = false;
      this.DisplayMessage("Upgrade UNSUCCESSFULL", errorMessage, true);
    });
  }
  ValidateFirstEmail(email: string) {
    this.ValidEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email)
    this.userService.EmailInUse(email).subscribe((success: boolean) => {
      this.EmailInUse = success;
    });
  }
  ClearUploaderQueue(uploader) {
    uploader.queue.pop();
  }
  ForceUploaderQueueToBeJustOne(uploader) {
    if (uploader.queue.length > 1) {
      uploader.queue[0] = uploader.queue[1];
      uploader.queue.pop();
    }

  }




  DeleteAthlete() {
    this.athleteService.DeleteAthlete(this.CurrentAthlete.Id).subscribe(
      success => {
        this.DisplayMessage("Delete SUCCESSFULL", "Athlete Deleted Successfull", false)
        this.router.navigate(['/Roster']);
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Delete UNSUCCESSFULL", errorMessage, false)
      });
  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
  public RemoveAthleteTag(s: TagModel) {
    var index = this.CurrentAthlete.DisplayTags.findIndex(x => { return x.display == s.display });
    this.CurrentAthlete.DisplayTags.splice(index, 1);
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
    this.CurrentAthlete.DisplayTags.push(s)

    //this sucks, but we can no longer use the PIPE in the html. Until we figure out how to get the NGMODEL from the Parent to the child control scpTagInput
  }
}

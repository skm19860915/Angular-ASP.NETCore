import { Component, OnInit, Input } from '@angular/core';
import { AssistantCoach, Role, BackendAssistantCoach } from '../../Models/AssistantCoach';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../../environments/environment';
import { RosterService } from '../../Services/roster.service';
import { OrganizationService } from '../../Services/organization.service';
import { AlertMessage } from '../../Models/AlertMessage';
import { fadeInAnimation } from 'src/app/animation/fadeIn';
import { interval } from 'rxjs';
import { take } from 'rxjs/operators';


@Component({
  selector: 'app-coaches-roster',
  templateUrl: './coaches-roster.component.html',
  styleUrls: ['./coaches-roster.component.less'],
  animations: [fadeInAnimation]
})

export class CoachesRosterComponent implements OnInit {
  @Input() Model: boolean = false;
  public deleteConfirmation: string = "";
  public SelectedCoach: BackendAssistantCoach;
  public FirstName: string = "";
  public LastName: string = "";
  public Email: string = "";
  public View: string = "CoachList"
  public Coaches: BackendAssistantCoach[] = [];
  public ShowCoachDeleteWindow: boolean = false;
  public AllRoles: Role[] = []
  public AlertMessages: AlertMessage[] = [];
  public uploadURL: string = environment.endpointURL + '/api/MultiMedia/CreateProfilePicture';
  public ShowCoachCreationWindow: boolean = false;
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

  constructor(public rosterService: RosterService, public orgSerivce: OrganizationService) {
  }

  ngOnInit() {
    this.orgSerivce.GetAllRoles().subscribe(success => { this.AllRoles = success; });
    this.orgSerivce.GetAllNonHeadCoaches().subscribe(success => {
      this.Coaches = success;
      for (var i = 0; i < success.length; i++) {
        if (!success[i].IsDeleted)
          this.SelectedCoach = success[i];
      }
    });
  }

  IsRoleAssigned(roleId: number, coach: BackendAssistantCoach) {
    for (let i = 0; i < coach.Roles.length; i++) {
      const element = coach.Roles[i];
      if (element.Id == roleId) {
        return true;
      }
    }
    return false;
  }
  ToggleRole(targetRole: Role, coach: BackendAssistantCoach) {
    var itemIndex = -1;
    for (let i = 0; i < coach.Roles.length; i++) {
      const element = coach.Roles[i];
      if (element.Id == targetRole.Id) {
        itemIndex = i;
        break;
      }
    }
    if (itemIndex > -1) {
      coach.Roles.splice(itemIndex, 1)
      this.orgSerivce.UnAssignRole(targetRole.Id, coach.Id).subscribe(success => {
        this.DisplayMessage("Role Assigned SUCCESSFULL", "Role UnAssigned Saved", false)
      },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Role UnAssigned UNSUCCESSFULL", errorMessage, true)
        });
    }
    else {
      coach.Roles.push({ Id: targetRole.Id, Name: targetRole.Name });
      this.orgSerivce.AssignRole(targetRole.Id, coach.Id).subscribe(success => {
        this.DisplayMessage("Role Assigned SUCCESSFULL", "Role Assigned Saved", false)
      },
        error => {
          var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
          this.DisplayMessage("Role Assigned UNSUCCESSFULL", errorMessage, true)
        });
    }
  }
  CancelCreateAssistant() {
    this.FirstName = "";
    this.LastName = "";
    this.Email = "";
    this.View = "CoachList"
  }
  ForceUploaderQueueToBeJustOne(uploader) {
    if (uploader.queue.length > 1) {
      uploader.queue[0] = uploader.queue[1];
      uploader.queue.pop();
    }
  }
  ResendCoachEmail(targetCoachId: number) {
    this.rosterService.ResendCoachRegistrationEmail(targetCoachId).subscribe(
      success => { this.DisplayMessage("Registration Email Sent", "Email Send SUCCESSFULL", false); },
      error => { this.DisplayMessage("Registration Email Sent", "Email Send UNSUCCESSFULL", true); }
    )


  }
  CreateAssistantCoach(firstName: string, lastName: string, email: string, uploader: FileUploader) {
    var newCoach = new AssistantCoach();
    newCoach.FirstName = firstName;
    newCoach.LastName = lastName;
    newCoach.Email = email;
    this.rosterService.CreateAssistantCoach(newCoach).subscribe(
      success => {
        if (uploader != undefined && uploader[0] != undefined) {
          uploader[0].url = this.uploadURL + "/" + success;
          uploader[0].upload();
        }

        this.uploader.queue.pop();
        this.FirstName = "";
        this.LastName = "";
        this.Email = "";
        this.DisplayMessage("Assistant Coach Saved", "Save SUCCESSFULL", false);

        this.orgSerivce.GetAllNonHeadCoaches().subscribe(success => {
          this.Coaches = success;
          for (var i = 0; i < success.length; i++) {
            if (!success[i].IsDeleted)
              this.SelectedCoach = success[i];
          }
        });
      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Save UNSUCCESSFULL", errorMessage, true)
      });
    this.ToggleCreateCoachesWindow();
  }
  ViewCreateACoachMenu() {
    this.View = "CreateCoach"
  }
  ViewCoaches() {

    this.View = "Coaches"
  }
  SetSelectedCoach(coach: BackendAssistantCoach) {
    this.SelectedCoach = coach;
  }

  ArchiveCoach() {
    this.ShowCoachDeleteWindow = false;
    this.deleteConfirmation = "";
    this.orgSerivce.ArchiveCoach(this.SelectedCoach.Id).subscribe(
      success => {
        this.orgSerivce.GetAllNonHeadCoaches().subscribe(success => {
          this.Coaches = success;
          this.DisplayMessage("Coach Delete Successfull", "Coach Delete Successful", false);
          for (var i = 0; i < success.length; i++) {
            if (!success[i].IsDeleted)
              this.SelectedCoach = success[i];
          }
          if (success.length == 0) {
            this.SelectedCoach = undefined;
          }
        });

      },
      error => {
        var errorMessage = error.error == undefined || typeof error.error == typeof {} ? error.error.ExceptionMessage : error.error;
        this.DisplayMessage("Coach Delete Failed", error, true);
      });
  }
  ConfirmArchive() {
    this.ShowCoachDeleteWindow = true;
  }
  CancelArchive() {
    this.ShowCoachDeleteWindow = false;
  }
  DisplayMessage(title: string, message: string, isError: boolean) {
    const newMessage = new AlertMessage();
    newMessage.Title = title;
    newMessage.Message = message;
    newMessage.IsError = isError;
    this.AlertMessages.push(newMessage)
  }
  ToggleCreateCoachesWindow() {
    this.ShowCoachCreationWindow = !this.ShowCoachCreationWindow;
    this.FirstName = '';
    this.LastName = '';
    this.Email = '';
  }

}



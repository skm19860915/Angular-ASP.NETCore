import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Services/user.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-email-verification',
  templateUrl: './user-email-verification.component.html',
  styleUrls: ['./user-email-verification.component.less']
})
export class UserEmailVerificationComponent implements OnInit {
public ErrorMessage : string = "";
  constructor(public userService: UserService, private route: ActivatedRoute, public router: Router) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userService.RegisterCoach(params['emailToken']).subscribe(success => { this.router.navigate(['/Home']) }, failure => { this.ErrorMessage =` We are sorry, your registration has an error in it. Will you alert your customer service representitive with the following information ${params['emailToken']}` });
    });
  }

}

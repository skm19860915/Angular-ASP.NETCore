import { Component, OnInit } from '@angular/core';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  constructor(private userService: UserService) { }

  ngOnInit() {

    if (window.location.href.indexOf("?session_id=") > 0) {
      var url = window.location.href.split("?session_id=");
      var sessionId = url[1];
      this.userService.SaveStripeGuid(sessionId, url[0]);
    }
    else if (window.location.href.indexOf("?cancleout=")) {
      var index = window.location.href.indexOf("?")
      var searchParams = new URLSearchParams(window.location.href.substring(index));
      var orgId = searchParams.getAll("orgId")[0]
      this.userService.DeleteOrgId(parseInt(orgId));
    }
  }
}

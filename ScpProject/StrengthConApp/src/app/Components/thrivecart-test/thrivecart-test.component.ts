import { Component, OnInit } from '@angular/core';
import { UserService } from './../../Services/user.service';

@Component({
  selector: 'app-thrivecart-test',
  templateUrl: './thrivecart-test.component.html',
  styleUrls: ['./thrivecart-test.component.less']
})
export class ThrivecartTestComponent implements OnInit {

  constructor(public userservice: UserService) { }

  ngOnInit() {
    this.userservice.genericLog("success" + document.URL).subscribe(x => { });
  }

}

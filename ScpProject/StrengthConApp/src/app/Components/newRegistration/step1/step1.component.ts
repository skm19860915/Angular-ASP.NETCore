import { Component, OnInit, Input, Output , EventEmitter} from '@angular/core';
import { OrganizationService } from '../../../Services/organization.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-step1',
  templateUrl: './step1.component.html',
  styleUrls: ['./step1.component.less']
})
export class Step1Component implements OnInit {
public organizationName : string; 
  private orgService: OrganizationService
  constructor(private orgServ: OrganizationService, private router: Router) {
    this.orgService = orgServ;

  }

  @Input() wizardState: number;
  @Output() wizardStateChange : EventEmitter<number> = new EventEmitter(true);
  @Output() OrganizationNameChange : EventEmitter<string> = new EventEmitter(true);
  ngOnInit() {
  }
  CreateOrganization(name: string) {
   this.wizardStateChange.emit( ++this.wizardState);
   this.OrganizationNameChange.emit(name);
    // this.orgService.RegisterOrganization(name).subscribe(success => {
    //     this.OrganizationId = <number>success;
    // },
    //   error => {
    //   });
   }

   Cancel(){
    this.wizardStateChange.emit(1);
    this.router.navigate(['/Login']);
  }
}

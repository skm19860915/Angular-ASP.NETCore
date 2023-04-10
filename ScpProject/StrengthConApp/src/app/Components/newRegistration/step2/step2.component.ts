import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-step2',
  templateUrl: './step2.component.html',
  styleUrls: ['./step2.component.less']
})
export class Step2Component implements OnInit {


  constructor(public router: Router) { }
  @Input() wizardState: number;
  @Output() wizardStateChange: EventEmitter<number> = new EventEmitter(true);
  @Output() SelectedPlanChange: EventEmitter<number> = new EventEmitter(true);

  SelectedPlanId = 4;
  ngOnInit() {
    this.SelectedPlanChange.emit(this.SelectedPlanId);
  }

  planSelected() {
    this.wizardStateChange.emit(++this.wizardState);
    //   this.orgService.RegisterOrganization(name).subscribe(success => {
    //     //  this.OrganizationId = <number>success;
    //   },
    //     error => {
    //     });
  }
  SelectPlan(planId: number) {
    this.SelectedPlanId = planId;
    this.SelectedPlanChange.emit(planId);
  }
  Cancel() {
    this.wizardStateChange.emit(1);
    this.router.navigate(['/Login']);
  }
}

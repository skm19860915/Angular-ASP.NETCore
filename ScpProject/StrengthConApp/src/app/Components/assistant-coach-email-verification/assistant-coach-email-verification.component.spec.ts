import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssistantCoachEmailVerificationComponent } from './assistant-coach-email-verification.component';

describe('AssistantCoachEmailVerificationComponent', () => {
  let component: AssistantCoachEmailVerificationComponent;
  let fixture: ComponentFixture<AssistantCoachEmailVerificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssistantCoachEmailVerificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssistantCoachEmailVerificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteSurveyComponent } from './athlete-survey.component';

describe('AthleteSurveyComponent', () => {
  let component: AthleteSurveyComponent;
  let fixture: ComponentFixture<AthleteSurveyComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteSurveyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteSurveyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteEmailVerificationComponent } from './athlete-email-verification.component';

describe('AthleteEmailVerificationComponent', () => {
  let component: AthleteEmailVerificationComponent;
  let fixture: ComponentFixture<AthleteEmailVerificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteEmailVerificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteEmailVerificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

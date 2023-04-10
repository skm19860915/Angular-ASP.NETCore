import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteWorkoutComponent } from './athlete-workout.component';

describe('AthleteWorkoutComponent', () => {
  let component: AthleteWorkoutComponent;
  let fixture: ComponentFixture<AthleteWorkoutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteWorkoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteWorkoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

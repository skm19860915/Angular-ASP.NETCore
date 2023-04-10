import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthletepastWorkoutComponent } from './athletepast-workout.component';

describe('AthletepastWorkoutComponent', () => {
  let component: AthletepastWorkoutComponent;
  let fixture: ComponentFixture<AthletepastWorkoutComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthletepastWorkoutComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthletepastWorkoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

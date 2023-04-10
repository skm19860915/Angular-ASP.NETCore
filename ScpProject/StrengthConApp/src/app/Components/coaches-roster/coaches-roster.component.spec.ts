import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CoachesRosterComponent } from './coaches-roster.component';

describe('CoachesRosterComponent', () => {
  let component: CoachesRosterComponent;
  let fixture: ComponentFixture<CoachesRosterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CoachesRosterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CoachesRosterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

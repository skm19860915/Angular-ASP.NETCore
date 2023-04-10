import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateAthleteComponent } from './create-athlete.component';

describe('CreateAthleteComponent', () => {
  let component: CreateAthleteComponent;
  let fixture: ComponentFixture<CreateAthleteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateAthleteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateAthleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

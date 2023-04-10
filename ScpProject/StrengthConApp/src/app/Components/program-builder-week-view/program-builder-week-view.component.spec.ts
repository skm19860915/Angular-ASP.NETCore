import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgramBuilderWeekViewComponent } from './program-builder-week-view.component';

describe('ProgramBuilderWeekViewComponent', () => {
  let component: ProgramBuilderWeekViewComponent;
  let fixture: ComponentFixture<ProgramBuilderWeekViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProgramBuilderWeekViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgramBuilderWeekViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

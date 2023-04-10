import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SetsAndRepsTableComponent } from './sets-and-reps-table.component';

describe('SetsAndRepsTableComponent', () => {
  let component: SetsAndRepsTableComponent;
  let fixture: ComponentFixture<SetsAndRepsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SetsAndRepsTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SetsAndRepsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

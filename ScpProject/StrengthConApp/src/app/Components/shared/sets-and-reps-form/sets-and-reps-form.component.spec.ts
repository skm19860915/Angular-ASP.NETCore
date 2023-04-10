import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetsAndRepsFormComponent } from './sets-and-reps-form.component';

describe('SetsAndRepsFormComponent', () => {
  let component: SetsAndRepsFormComponent;
  let fixture: ComponentFixture<SetsAndRepsFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetsAndRepsFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetsAndRepsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

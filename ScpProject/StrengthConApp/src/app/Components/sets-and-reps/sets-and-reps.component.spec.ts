import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetsAndRepsComponent } from './sets-and-reps.component';

describe('SetsAndRepsComponent', () => {
  let component: SetsAndRepsComponent;
  let fixture: ComponentFixture<SetsAndRepsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetsAndRepsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetsAndRepsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

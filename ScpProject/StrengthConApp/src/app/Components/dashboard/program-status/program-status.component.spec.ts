import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgramStatusComponent } from './program-status.component';

describe('ProgramStatusComponent', () => {
  let component: ProgramStatusComponent;
  let fixture: ComponentFixture<ProgramStatusComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProgramStatusComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgramStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

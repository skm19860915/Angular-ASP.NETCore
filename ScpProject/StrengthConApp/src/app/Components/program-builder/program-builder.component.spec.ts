import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgramBuilderComponent } from './program-builder.component';

describe('ProgramBuilderComponent', () => {
  let component: ProgramBuilderComponent;
  let fixture: ComponentFixture<ProgramBuilderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProgramBuilderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgramBuilderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

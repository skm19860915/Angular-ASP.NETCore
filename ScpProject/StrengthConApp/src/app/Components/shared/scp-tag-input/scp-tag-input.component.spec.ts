import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScpTagInputComponent } from './scp-tag-input.component';

describe('ScpTagInputComponent', () => {
  let component: ScpTagInputComponent;
  let fixture: ComponentFixture<ScpTagInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScpTagInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScpTagInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

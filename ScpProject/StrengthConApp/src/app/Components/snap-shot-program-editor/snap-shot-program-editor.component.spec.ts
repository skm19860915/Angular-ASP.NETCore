import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnapShotProgramEditorComponent } from './snap-shot-program-editor.component';

describe('SnapShotProgramEditorComponent', () => {
  let component: SnapShotProgramEditorComponent;
  let fixture: ComponentFixture<SnapShotProgramEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SnapShotProgramEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SnapShotProgramEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

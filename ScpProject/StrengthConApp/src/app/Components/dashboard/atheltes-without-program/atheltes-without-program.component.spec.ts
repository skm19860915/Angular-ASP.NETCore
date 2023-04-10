import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AtheltesWithoutProgramComponent } from './atheltes-without-program.component';

describe('AtheltesWithoutProgramComponent', () => {
  let component: AtheltesWithoutProgramComponent;
  let fixture: ComponentFixture<AtheltesWithoutProgramComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AtheltesWithoutProgramComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AtheltesWithoutProgramComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

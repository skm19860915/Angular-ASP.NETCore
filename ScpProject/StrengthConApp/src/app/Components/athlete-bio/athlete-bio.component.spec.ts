import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteBioComponent } from './athlete-bio.component';

describe('AthleteBioComponent', () => {
  let component: AthleteBioComponent;
  let fixture: ComponentFixture<AthleteBioComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteBioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteBioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

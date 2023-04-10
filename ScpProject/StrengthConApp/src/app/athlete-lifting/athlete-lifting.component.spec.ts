import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteLiftingComponent } from './athlete-lifting.component';

describe('AthleteLiftingComponent', () => {
  let component: AthleteLiftingComponent;
  let fixture: ComponentFixture<AthleteLiftingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteLiftingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteLiftingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

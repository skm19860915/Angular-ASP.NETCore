import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AthleteMetricComponent } from './athlete-metric.component';

describe('AthleteMetricComponent', () => {
  let component: AthleteMetricComponent;
  let fixture: ComponentFixture<AthleteMetricComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AthleteMetricComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AthleteMetricComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

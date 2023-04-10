import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetricFormMeasurementComponent } from './metric-form-measurement.component';

describe('MetricFormMeasurementComponent', () => {
  let component: MetricFormMeasurementComponent;
  let fixture: ComponentFixture<MetricFormMeasurementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MetricFormMeasurementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MetricFormMeasurementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

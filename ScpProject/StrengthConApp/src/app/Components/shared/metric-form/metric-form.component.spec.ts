import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MetricFormComponent } from './metric-form.component';

describe('MetricFormComponent', () => {
  let component: MetricFormComponent;
  let fixture: ComponentFixture<MetricFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MetricFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MetricFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

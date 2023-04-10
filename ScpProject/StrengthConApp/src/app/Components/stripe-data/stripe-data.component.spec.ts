import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StripeDataComponent } from './stripe-data.component';

describe('StripeDataComponent', () => {
  let component: StripeDataComponent;
  let fixture: ComponentFixture<StripeDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StripeDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StripeDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

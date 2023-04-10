import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomTrifoldViewComponent } from './weight-room-trifold-view.component';

describe('WeightRoomTrifoldViewComponent', () => {
  let component: WeightRoomTrifoldViewComponent;
  let fixture: ComponentFixture<WeightRoomTrifoldViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightRoomTrifoldViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomTrifoldViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

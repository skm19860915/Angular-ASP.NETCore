import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomSingleViewComponent } from './weight-room-single-view.component';

describe('WeightRoomSingleViewComponent', () => {
  let component: WeightRoomSingleViewComponent;
  let fixture: ComponentFixture<WeightRoomSingleViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightRoomSingleViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomSingleViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

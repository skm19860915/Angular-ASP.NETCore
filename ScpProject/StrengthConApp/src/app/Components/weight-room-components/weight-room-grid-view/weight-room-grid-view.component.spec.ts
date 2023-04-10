import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomGridViewComponent } from './weight-room-grid-view.component';

describe('WeightRoomGridViewComponent', () => {
  let component: WeightRoomGridViewComponent;
  let fixture: ComponentFixture<WeightRoomGridViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightRoomGridViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomGridViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

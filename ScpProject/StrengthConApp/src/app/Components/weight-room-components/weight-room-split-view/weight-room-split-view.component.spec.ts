import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomSplitViewComponent } from './weight-room-split-view.component';

describe('WeightRoomSplitViewComponent', () => {
  let component: WeightRoomSplitViewComponent;
  let fixture: ComponentFixture<WeightRoomSplitViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightRoomSplitViewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomSplitViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

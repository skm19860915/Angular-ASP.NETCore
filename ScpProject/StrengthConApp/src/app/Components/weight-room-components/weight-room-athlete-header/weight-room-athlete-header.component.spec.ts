import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomAthleteHeaderComponent } from './weight-room-athlete-header.component';

describe('WeightRoomAthleteHeaderComponent', () => {
  let component: WeightRoomAthleteHeaderComponent;
  let fixture: ComponentFixture<WeightRoomAthleteHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WeightRoomAthleteHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomAthleteHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

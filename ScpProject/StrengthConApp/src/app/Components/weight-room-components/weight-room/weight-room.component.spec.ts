import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WeightRoomComponent } from './weight-room.component';

describe('WeightRoomComponent', () => {
  let component: WeightRoomComponent;
  let fixture: ComponentFixture<WeightRoomComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WeightRoomComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WeightRoomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

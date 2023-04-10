import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UnreadNotificationsComponent } from './unread-notifications.component';

describe('UnreadNotificationsComponent', () => {
  let component: UnreadNotificationsComponent;
  let fixture: ComponentFixture<UnreadNotificationsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UnreadNotificationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnreadNotificationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

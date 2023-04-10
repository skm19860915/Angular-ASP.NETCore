import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserEmailVerificationComponent } from './user-email-verification.component';

describe('UserEmailVerificationComponent', () => {
  let component: UserEmailVerificationComponent;
  let fixture: ComponentFixture<UserEmailVerificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserEmailVerificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserEmailVerificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

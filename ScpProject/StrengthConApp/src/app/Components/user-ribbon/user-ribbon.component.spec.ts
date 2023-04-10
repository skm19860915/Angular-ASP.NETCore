import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserRibbonComponent } from './user-ribbon.component';

describe('UserRibbonComponent', () => {
  let component: UserRibbonComponent;
  let fixture: ComponentFixture<UserRibbonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserRibbonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserRibbonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

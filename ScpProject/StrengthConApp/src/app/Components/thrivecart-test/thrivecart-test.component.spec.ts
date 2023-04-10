import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ThrivecartTestComponent } from './thrivecart-test.component';

describe('ThrivecartTestComponent', () => {
  let component: ThrivecartTestComponent;
  let fixture: ComponentFixture<ThrivecartTestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ThrivecartTestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ThrivecartTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

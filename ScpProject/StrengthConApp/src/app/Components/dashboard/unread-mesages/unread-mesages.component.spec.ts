import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UnreadMesagesComponent } from './unread-mesages.component';

describe('UnreadMesagesComponent', () => {
  let component: UnreadMesagesComponent;
  let fixture: ComponentFixture<UnreadMesagesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UnreadMesagesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnreadMesagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

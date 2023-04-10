import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TwoDropDownSearchComponent } from './two-drop-down-search.component';

describe('TwoDropDownSearchComponent', () => {
  let component: TwoDropDownSearchComponent;
  let fixture: ComponentFixture<TwoDropDownSearchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TwoDropDownSearchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TwoDropDownSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

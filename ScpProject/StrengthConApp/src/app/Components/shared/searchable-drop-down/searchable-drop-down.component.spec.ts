import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchableDropDownComponent } from './searchable-drop-down.component';

describe('SearchableDropDownComponent', () => {
  let component: SearchableDropDownComponent;
  let fixture: ComponentFixture<SearchableDropDownComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SearchableDropDownComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchableDropDownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

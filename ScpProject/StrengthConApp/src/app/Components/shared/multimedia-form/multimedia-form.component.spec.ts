import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MultimediaFormComponent } from './multimedia-form.component';

describe('MultimediaFormComponent', () => {
  let component: MultimediaFormComponent;
  let fixture: ComponentFixture<MultimediaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MultimediaFormComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MultimediaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

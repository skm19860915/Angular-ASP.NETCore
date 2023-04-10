import { TestBed } from '@angular/core/testing';

import { AthleteService } from './athlete.service';

describe('AthleteService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AthleteService = TestBed.get(AthleteService);
    expect(service).toBeTruthy();
  });
});

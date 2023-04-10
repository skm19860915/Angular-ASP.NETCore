import { TestBed } from '@angular/core/testing';

import { MultiMediaService } from './multi-media.service';

describe('MultiMediaService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MultiMediaService = TestBed.get(MultiMediaService);
    expect(service).toBeTruthy();
  });
});

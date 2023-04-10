import { TestBed } from '@angular/core/testing';

import { ProgramBuilderService } from './program-builder.service';

describe('ProgramBuilderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ProgramBuilderService = TestBed.get(ProgramBuilderService);
    expect(service).toBeTruthy();
  });
});

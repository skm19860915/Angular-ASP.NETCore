import {ProgramDayItem } from './ProgramDayItem';

export class Day{
    public Id:number;
    public IsActive: boolean;
    public Items : ProgramDayItem[] =[];
    public Position : number;
  }
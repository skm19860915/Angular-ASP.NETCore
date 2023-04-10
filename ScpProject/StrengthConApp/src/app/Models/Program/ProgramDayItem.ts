import {ProgramDayItemEnum} from "./ProgramDayItemEnum";
import { WorkoutDetails } from "../SetsAndReps/WorkoutDetails";

export class ProgramDayItem
{
  public Id: number;
  public ItemType: ProgramDayItemEnum;
  public Position : number;
  public ShowCreationMenu : boolean;
  public ShowDetails: boolean;
  public ProgramItem: any = {};

}
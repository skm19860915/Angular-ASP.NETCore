import { BackendAssistantCoach } from "../AssistantCoach";

export class YesNoThreshold {
    public CoachId: number = 0;
    public ThresholdValue: boolean = true;
    public Id: number = 0;
    public Coaches: BackendAssistantCoach[] = [];
    public CoachIds: number[] = [];
    public QuestionId: number;
}
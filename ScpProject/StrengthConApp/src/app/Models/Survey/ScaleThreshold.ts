import { BackendAssistantCoach } from "../AssistantCoach";

export class ScaleThreshold {
    public CoachId: number = 0;
    public Comparer: number = 0;
    public ThresholdValue: number = 0;
    public Id: number = 0;
    public Coaches: BackendAssistantCoach[] = [];
    public CoachIds: number[] = [];
    public QuestionId: number = 0;
}
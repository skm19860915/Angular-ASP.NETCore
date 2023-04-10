import { ScaleThreshold } from '../Models/Survey/ScaleThreshold';
import { YesNoThreshold } from '../Models/Survey/YesNoThreshold'
export class Question {
    public QuestionId: number;
    public Question: string;
    public QuestionType: QuestionType;
    public SurveyId: number;
    public YesNoThresholds: YesNoThreshold[] = [];
    public ScaleThresholds: ScaleThreshold[] = [];
    public CanModify : boolean;
}

export class MappedQuestionToSurvey {
    public SurveyToQuestionId: number;
    public QuestionId: number;
    public Question: string;
    public QuestionType: QuestionType;
    public SurveyId: number;
    public YesNoThresholds: YesNoThreshold[] = [];
    public ScaleThresholds: ScaleThreshold[] = []
    public CanModify : boolean;
}


export enum QuestionType {
    Boolean = 1,
    Scale = 2,
    OpenEnded = 3
}
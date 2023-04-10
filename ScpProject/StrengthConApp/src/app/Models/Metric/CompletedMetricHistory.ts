export class CompletedMetricHistory {
    public MetricId: number;
    public Name: string;
    public CompletedDate: Date;
    public Value: number;
    public IsSelected: boolean = false;
    public DisplayDate: NGXDisplayDate;
}
export class NGXDisplayDate {
    public startDate: Date;
    public endDate: Date;
}
export class GroupedMeasuredMetric {
    public CompletedMetrics: AthleteCompletedMeasurementById[] = [];
}
export class AthleteCompletedMeasurementById {
    public Metrics: MeasuredMetric[] = [];
    public UnitOfMeasurementId: number;
    public UnitOfMeasurementName: string;
}
export class MeasuredMetric {
    public MetricValue: number;
    public AssignedProgramId: number;
    public CompletedDate: Date;
    public MetricId: number;
    public MetricName: string;
    public UnitOfMeasurementId: number;
    public UnitOfMeasurementName: string
    public ProgramName: string;
    public CompletedDateDisplay: string;
}
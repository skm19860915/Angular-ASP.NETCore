export class CompleteRegistration {
    public UserName: string;
    public Password: string;
    public ConfirmPassword: string;
    public EmailValidationToken: string;
    public AthleteId: number;
}
export class CompleteCoachRegistration {
    public UserName: string;
    public Password: string;
    public ConfirmPassword: string;
    public EmailValidationToken: string;
    public UserId: number;
}
export class OneTime {
    public UserName: string;
    public Password: string;
    public ConfirmPassword: string;
    public EmailValidationToken: string;
    public UserId: number;
    public OrgName: string;
}
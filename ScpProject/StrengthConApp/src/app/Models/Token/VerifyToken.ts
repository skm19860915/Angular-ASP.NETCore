export class VerifyToken {
    public IsUser: boolean = false;
    public IsWeightRoomView: boolean = false;
    public IsOrganizationACustomer: boolean = false;
    public IsHeadCoach: boolean = false;
    public UserId: number;
    public StripeGuid: string;
    public HasBadCreditCard : boolean = false;
    public IsCreditCardExpiring : boolean = false;
    public HasSubscriptionEnded: boolean = false;
}
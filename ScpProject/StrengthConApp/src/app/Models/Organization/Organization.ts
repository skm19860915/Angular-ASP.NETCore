export class Organization {
    public Name: string;
    public IsCustomer: boolean;
    public CurrentSubscriptionPlanId: number;
    public PrimaryColorHex: string;
    public SecondaryColorHex: string;
    public PrimaryFontColorHex: string;
    public SecondaryFontColorHex: string;
    public CreditCardExpiring :boolean; 
    public SubscriptionEnded : boolean; 
    public BadCreditCard :boolean
    public ExpiredCard: boolean = false;
    public StripeFailedToProcess: boolean = false;
}
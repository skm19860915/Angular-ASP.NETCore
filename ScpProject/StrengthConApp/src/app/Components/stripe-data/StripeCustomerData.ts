export class StripeCustomerData {
    public FirstName: string;
    public LastName: string;
    public AddressLine1: string;
    public AddressLine2: string;
    public Phone: string;
    public Zip: string;
    public Email: string;
    public City: string;
    public State: string;
    public Country: string;
    public CardNumber: string;
    public ExpiryMonth: number;
    public ExpiryYear: number;
    public CVC: number;
    public Coupon: string;
    public Adddress1:string;
    public Adddress2:string;
    
    constructor(FirstName,
        LastName,
        AddressLine1,
        AddressLine2,
        Phone,
        Zip,
        Email,
        City,
        State,
        Country,
        CardNumber,
        ExpiryMonth,
        CVC, ExpiryYear, Coupon) {
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.AddressLine1 = AddressLine1;
        this.AddressLine2 = AddressLine2;
        this.Phone = Phone;
        this.Zip = Zip;
        this.Email = Email;
        this.City = City;
        this.State = State;
        this.Country = Country;
        this.CardNumber = CardNumber;
        this.ExpiryMonth = ExpiryMonth;
        this.ExpiryYear = ExpiryYear;
        this.CVC = CVC;
        this.Coupon = Coupon;
    }
}

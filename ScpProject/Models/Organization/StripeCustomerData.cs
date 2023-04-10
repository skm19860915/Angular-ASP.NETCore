namespace Models.Organization
{
    public class StripeCustomerData
    {
        public StripeCustomerData(string firstName, string lastName, string addressLine1, string addressLine2, string phone, string zip, string email, string city, string state, string country, string cardNumber, int expiryMonth, int expiryYear, int cVC, string coupon)
        {
            FirstName = firstName;
            LastName = lastName;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            Phone = phone;
            Zip = zip;
            Email = email;
            City = city;
            State = state;
            Country = country;
            CardNumber = cardNumber;
            ExpiryMonth = expiryMonth;
            ExpiryYear = expiryYear;
            CVC = cVC;
            Coupon = coupon;    
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Phone { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public int CVC { get; set; }
        public string Coupon { get; set; }
    }
}

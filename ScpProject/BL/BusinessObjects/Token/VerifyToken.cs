namespace BL.BusinessObjects.Token
{

    public class VerifyToken
    {
        public bool IsUser { get; set; }
        public bool IsWeightRoomView { get; set; }
        //this is set to false on initial creation of Organization. Once the user completes the stripe checkout and puts in their credit card
        //this will be set to true. Once Set to TRUE it will always be true. There will be other flags for their status as a customer
        public bool IsOrganizationACustomer { get; set; }
        public bool IsHeadCoach { get; set; }
        public int UserId { get; set; }
        public string StripeGuid { get; set; }
        public bool HasBadCreditCard { get; set; }
        public bool HasSubscriptionEnded { get; set; }
        public bool IsCreditCardExpiring { get; set; }
    }
}

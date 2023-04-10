using Models.Payment;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Organization
{
    public class Organization
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public int? CurrentSubscriptionPlanId { get; set; }
        public int CreatedUserId { get; set; }
        public string StripeGuid { get; set; }
        //this is set to false on initial creation of Organization. Once the user completes the stripe checkout and puts in their credit card
        //this will be set to true. Once Set to TRUE it will always be true. There will be other flags for their status as a customer
        public bool IsCustomer { get; set; }
        public bool ExpiredCard { get; set; }//are they a customer in good standings (do they owe money, is their card expired)
        public bool StripeFailedToProcess { get; set; }//when stripe says a payment faild
        public double HowMuchTheyOwe { get; set; }
        public bool HasSubscription { get; set; }//do they have a current subscription. did they cancle their subscription?
        public int? ProfilePictureId { get; set; }
        public string PrimaryColorHex { get; set; }
        public string SecondaryColorHex { get; set; }
        public string PrimaryFontColorHex { get; set; }
        public string Phone { get; set; }
        public string SecondaryFontColorHex { get; set; }

        public string CurrentPaymentMethod { get; set; }

        [ForeignKey("CurrentSubscriptionPlanId")]
        public virtual SubscriptionType SubType { get; set; }
        [ForeignKey("ProfilePictureId")]
        public virtual MultiMedia.Picture ProfilePicture { get; set; }
       public bool CreditCardExpiring { get; set; }
        public bool SubscriptionEnded { get; set; }
        public bool BadCreditCard { get; set; }
    }    
}

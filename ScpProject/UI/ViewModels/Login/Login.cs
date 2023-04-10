using System;
using System.Collections.Generic;

namespace Controllers.ViewModels.Login
{
    public class Login
    {
        public Guid UserToken { get; set; }
        public bool IsCoach { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        //this is set to false on initial creation of Organization. Once the user completes the stripe checkout and puts in their credit card
        //this will be set to true. Once Set to TRUE it will always be true. There will be other flags for their status as a customer
        public bool IsCustomer { get; set; }
        public bool IsHeadCoach { get; set; }
        /// <summary>
        /// [1] adminRole
        /// 
        /// </summary>
        public List<Boolean> Roles { get; set; }
    }
}
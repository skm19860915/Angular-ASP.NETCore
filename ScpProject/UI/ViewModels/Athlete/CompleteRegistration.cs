namespace Controllers.ViewModels.Athlete
{
    public class CompleteRegistration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailValidationToken  { get; set; }
        public int AthleteId { get; set; }
    }
}
namespace Controllers.ViewModels.Roster
{
    public class AssistantCoach_CompleteRegistration
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string EmailValidationToken { get; set; }
        public int UserId { get; set; }
    }
}
namespace API_QR.DTOs
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; } // Optional, if you want to allow email updates
        public bool EmailVerified { get; set; } // Optional, if you want to allow email verification status updates
    }


}

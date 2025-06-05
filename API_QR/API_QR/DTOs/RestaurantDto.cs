namespace API_QR.DTOs
{
    public class RestaurantDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string LogoURL { get; set; }
        public string CoverImageURL { get; set; }
        public int OwnerUserID { get; set; }
        public int PlanID { get; set; }
        public string Language { get; set; } = "vi";
        public string Currency { get; set; } = "VND";
        public decimal TaxRate { get; set; } = 10;
        public decimal ServiceChargeRate { get; set; } = 0;
    }
}

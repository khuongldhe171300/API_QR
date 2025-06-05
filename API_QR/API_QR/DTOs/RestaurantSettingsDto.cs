namespace API_QR.DTOs
{
    public class RestaurantSettingsDto
    {
        public string Language { get; set; }
        public string Currency { get; set; }
        public decimal TaxRate { get; set; }
        public decimal ServiceChargeRate { get; set; }
    }
}

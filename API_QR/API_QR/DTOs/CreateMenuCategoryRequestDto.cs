namespace API_QR.DTOs
{
    public class CreateMenuCategoryRequestDto
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}

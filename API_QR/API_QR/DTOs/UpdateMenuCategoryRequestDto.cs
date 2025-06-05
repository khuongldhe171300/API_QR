namespace API_QR.DTOs
{
    public class UpdateMenuCategoryRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }
        public int? DisplayOrder { get; set; }
    }

}

namespace API_QR.DTOs
{
    public class CreateMenuItemRequestDto
    {
        public int? CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ImageURL { get; set; }
        public int? PreparationTime { get; set; }
        public int? Calories { get; set; }
        public string? Ingredients { get; set; }
        public string? AllergenInfo { get; set; }
        public bool IsVegetarian { get; set; } = false;
        public bool IsVegan { get; set; } = false;
        public bool IsGlutenFree { get; set; } = false;
        public bool IsSpicy { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public string? Options { get; set; }
        public string? Addons { get; set; }

        public IFormFile? ImageFile { get; set; }
        public List<string>? AdditionalImages { get; set; }
    }
}

namespace API_QR.DTOs
{
    public class UpdateMenuItemRequestDto
    {
        public int? CategoryID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ImageURL { get; set; }
        public int? PreparationTime { get; set; }
        public int? Calories { get; set; }
        public string? Ingredients { get; set; }
        public string? AllergenInfo { get; set; }
        public bool? IsVegetarian { get; set; }
        public bool? IsVegan { get; set; }
        public bool? IsGlutenFree { get; set; }
        public bool? IsSpicy { get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsAvailable { get; set; }
        public int? DisplayOrder { get; set; }
        public string? Options { get; set; }
        public string? Addons { get; set; }
    }
}

namespace Application.Model.ProductManagement;

public class Subcategory : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    [Required]
    public string? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
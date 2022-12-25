namespace Application.Model.ProductManagement;



public class CatalogueSection : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    
    public string CatalogueId { get; set; }
    public Catalogue? Catalogue { get; set; }
    public int LineNumber { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string Description { get; set; }
}
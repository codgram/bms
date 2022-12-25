namespace Application.Model.ProductManagement;



public class CatalogueItem : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    
    public string CatalogueSectionId { get; set; }
    public CatalogueSection? CatalogueSection { get; set; }
    public string ItemId { get; set; }
    public Item? Item { get; set; }
    public int LineNumber { get; set; }
}
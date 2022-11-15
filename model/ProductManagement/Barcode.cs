namespace Application.Model.ProductManagement;


public class Barcode : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public string BarcodeNo { get; set; }

    [Required]
    public string? ItemId { get; set; }
    public Item? Item { get; set; }
    public string UnitOfMeasure { get; set; }
}
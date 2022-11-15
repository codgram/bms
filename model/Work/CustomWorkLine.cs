namespace Application.Model.Work;


public class CustomWorkLine : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string CustomWorkHeaderId { get; set; }
    public CustomWorkHeader? CustomWorkHeader { get; set; }

    // [DataType(DataType.DateTime)]
    // [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }
    public string BarcodeNo { get; set; }
    public decimal Quantity { get; set; }

    
}
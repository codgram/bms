using System.Text.Json.Serialization;

namespace Application.Model.ProductManagement;


public class SalesPrice : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string StoreId { get; set; }
    public Store? Store { get; set; }

    public string ItemId { get; set; }

    [JsonIgnore]
    public Item? Item { get; set; }

    public Currency Currency { get; set; }

    public string UnitOfMeasure { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public decimal Price { get; set; }

    public LineStatus Status { get; set; }

}
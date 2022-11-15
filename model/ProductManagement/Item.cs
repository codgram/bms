namespace Application.Model.ProductManagement;


public class Item : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string CompanyId { get; set; }
    public Company? Company { get; set; }
    public ItemType Type { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
}
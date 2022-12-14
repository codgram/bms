namespace Application.Model.ProductManagement;



public class Category : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    
    public string DivisionId { get; set; }
    public Division? Division { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
namespace Application.Model.ProductManagement;

public class Group : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
namespace Application.Model.ProductManagement;

public class Subgroup : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string GroupId { get; set; }
    public Group? Group { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
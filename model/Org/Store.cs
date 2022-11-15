namespace Application.Model.Org;


public class Store : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string EntityId { get; set; }
    public Entity? Entity { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
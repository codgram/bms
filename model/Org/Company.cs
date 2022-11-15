namespace Application.Model.Org;


public class Company : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public bool IsDefault { get; set; }
}
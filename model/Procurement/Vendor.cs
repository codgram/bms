namespace Application.Model.Procurement;


public class Vendor : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string CompanyId { get; set; }
    public Company Company { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

}
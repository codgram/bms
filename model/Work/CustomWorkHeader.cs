namespace Application.Model.Work;


public class CustomWorkHeader : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string CompanyId { get; set; }
    public Company? Company { get; set; }
    public string DocumentNo { get; set; }

    public DocumentStatus Status { get; set; }

}
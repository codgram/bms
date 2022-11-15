using Application.Model.Enum;

namespace Application.Model.Work;

public class WorkHeader : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    [Required]
    public string? StoreId { get; set; }
    public Store? Store { get; set; }

    public string DocumentNo { get; set; }
    public string LicensePlateNo { get; set; }

    public WorkHeaderType Type { get; set; }
    




    public DocumentStatus Status { get; set; }

}
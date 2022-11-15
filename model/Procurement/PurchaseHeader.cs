

namespace Application.Model.Procurement;



public class PurchaseHeader : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string CompanyId { get; set; }
    public Company? Company { get; set; }

    [Required]
    public string? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    public string DocumentNo { get; set; }

    public DateTime ExpectedOn { get; set; }
    public DateTime ReceivedOn { get; set; }

    public DocumentStatus Status { get; set; }
    
}
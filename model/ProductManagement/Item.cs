using Application.Model.Procurement;

namespace Application.Model.ProductManagement;


public class Item : Detail
{
    [Key]
    [Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    [Column(Order = 1)]
    public string CompanyId { get; set; }
    public Company? Company { get; set; }

    [Column(Order = 2)]
    public string SubgroupId { get; set; }
    public Subgroup? Subgroup { get; set; }

    [Column(Order = 3)]
    public string? VendorId { get; set; }
    public Vendor? Vendor { get; set; }

    [Column(Order = 4)]
    public ItemType Type { get; set; }

    [Column(Order = 5)]
    public string Code { get; set; }

    [Column(Order = 6)]
    public string Name { get; set; }

    [Column(Order = 7)]
    public string? Brand { get; set; }

    [Column(Order = 8)]
    public string? Description { get; set; }

    [Column(Order = 9)]
    public string? Size { get; set; }

    [Column(Order = 10)]
    public string? BaseUOM { get; set; }

    [Column(Order = 11)]
    public string? PurchaseUOM { get; set; }

    [Column(Order = 12)]
    public string? TransferUOM { get; set; }

    [Column(Order = 13)]
    public string? Tag { get; set; }

    [Column(Order = 14)]
    public bool HasVariant { get; set; }

    [Column(Order = 15)]
    public ItemStatus Status { get; set; }
}
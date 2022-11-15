namespace Application.Model;


public class Detail
{

    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
    public DateTime? DeletedOn { get; set; } = null;

    public string? CreatedById { get; set; }
    [ForeignKey("CreatedById")]
    public ApplicationUser? CreatedBy { get; set; }
    
    public string? ModifiedById { get; set; }
    [ForeignKey("ModifiedById")]
    public ApplicationUser? ModifiedBy { get; set; }

}
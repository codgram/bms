namespace Application.Model.Org;

public class CompanySubscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }

    public string CompanyId { get; set; }
    public Company? Company { get; set; }
}
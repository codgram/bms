namespace Application.Model.Org;


public class UserStore : Detail
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public string StoreId { get; set; }
    public Store? Store { get; set; }
}
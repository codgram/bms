namespace Application.Model.Org;

public class CurrencyExchange : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string CompanyId { get; set; }
    public Company? Company { get; set; }
    public Currency Currency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Rate { get; set; }

    public LineStatus Status { get; set; }
}

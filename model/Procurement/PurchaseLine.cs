

using Application.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Model.Procurement;



public class PurchaseLine : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; }

    public string PurchaseHeaderId { get; set; }
    public PurchaseHeader? PurchaseHeader { get; set; }

    public int LineNo { get; set; }

    [Required]
    public string? ItemId { get; set; }
    public Item? Item { get; set; }

    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Total
    {
        get
        {
            return OrderedQuantity * UnitPrice;
        }
    }





    public async Task<int> GenerateLineNo(ApplicationDbContext context, string purchaseHeaderId)
    {
        var purchaseLines = await context.PurchaseLine.Where(r => r.PurchaseHeaderId == purchaseHeaderId).ToListAsync();
        var maxLine = purchaseLines.Select(r => r.LineNo).Max();

        if(purchaseLines == null)
        {
            return 1;
        }
        else
        {
            return maxLine + 1;
        }
    }
    
}
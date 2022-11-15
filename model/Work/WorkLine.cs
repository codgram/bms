using Application.Model.Enum;
using Application.Model.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Model.Work;

public class WorkLine : Detail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public string WorkHeaderId { get; set; }
    public WorkHeader? WorkHeader { get; set; }

    public int LineNo { get; set; }

    [Required]
    public string? ItemId { get; set; }
    public Item? Item { get; set; }

    public decimal Quantity { get; set; }
    




    public DocumentStatus Status { get; set; }




    public async Task<int> GenerateLineNo(ApplicationDbContext context, string workHeaderId)
    {
        var workLines = await context.WorkLine.Where(r => r.WorkHeaderId == workHeaderId).ToListAsync();
        var maxLine = workLines.Select(r => r.LineNo).Max();

        return maxLine + 1;
    }

}
namespace Application.Model;

public class DataState
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string SortLabel { get; set; }
    public SortDirection SortDirection { get; set; }
}
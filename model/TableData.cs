namespace Application.Model;

public class TableData<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalItems { get; set; }
}
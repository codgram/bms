using Application.Model.Org;

public class StateContainer
{
    private string? savedString;
    public string? documentNo;

    private Company? company;



    public string Property
    {
        get => savedString ?? string.Empty;
        set
        {
            savedString = value;
            NotifyStateChanged();
        }
    }
    public string DocumentNo
    {
        get => documentNo ?? string.Empty;
        set
        {
            savedString = value;
            NotifyStateChanged();
        }
    }

    public Company? Company
    {
        get => company ?? null;
        set
        {
            company = value;
            NotifyStateChanged();
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}
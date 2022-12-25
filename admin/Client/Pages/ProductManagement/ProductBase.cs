using Application.Model.Enum;
using Application.Model.Procurement;
using Application.Model.ProductManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net.Http.Json;

namespace Application.Client.Pages.ProductManagement;

public class ProductBase : ComponentBase
{

    
    [Parameter] public string action { get; set; }
    [Parameter] public string Id { get; set; }
    [Inject] public IJSRuntime _jsRuntime { get; set; }
    [Inject] public ISnackbar _snackBar { get; set; }
    [Inject] public NavigationManager _navigationManager { get; set; }
    [Inject] public StateContainer _stateContainer { get; set; }
    [Inject] public HttpClient _client { get; set; }

    [Inject] IHttpClientFactory ClientFactory { get; set; }

    public MudTable<Item> table;
    public bool dense = false;
    public bool hover = true;
    public bool striped = false;
    public bool bordered = false;
    public bool loading = false;
    public string searchString = null;
    public string searchString1 = "";
    public Item selectedItem1 = null;

    public Item Item = new Item();
    public Item SelectedItem { get; set; }
    public HashSet<Item> selectedItems = new HashSet<Item>();

    public List<Item> Items = new List<Item>();
    public List<Subgroup> Subgroups { get; set; }
    public List<Vendor> Vendors { get; set; }


    public bool FilterFunc1(Item Item) => FilterFunc(Item, searchString1);

    public async Task<MudBlazor.TableData<Item>> ServerReload(TableState state)
    {
        loading = true;
        var data = await _client.GetFromJsonAsync<MudBlazor.TableData<Item>>($"api/items?companyId={_stateContainer.Company.Id}&page={state.Page}&pageSize={state.PageSize}&searchString={searchString}&sort={state.SortLabel}&sortDirection={state.SortDirection}");
        loading = false;
        return data;
    }

    public void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    public async Task<List<Subgroup>> GetSubgroupsAsync()
    {
        return await _client.GetFromJsonAsync<List<Subgroup>>($"api/subgroups?companyId={_stateContainer.Company.Id}");
    }

    public async Task<List<Vendor>> GetVendorsAsync()
    {
        return await _client.GetFromJsonAsync<List<Vendor>>($"api/vendors?companyId={_stateContainer.Company.Id}");
    }

    public async Task<Item> GetItemAsync(string id)
    {
        return await _client.GetFromJsonAsync<Item>($"api/items/{id}");
    }

    public bool FilterFunc(Item Item, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Item.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Item.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Item.Description}".Contains(searchString))
            return true;
        return false;
    }


    public async Task OnValidSubmit() 
    {
        HttpResponseMessage response;
        if(action.ToLower() == "add" && String.IsNullOrEmpty(Id))
        {
            Item.Status = ItemStatus.Active;
            Item.CompanyId = _stateContainer.Company.Id;
            response = await _client.PostAsJsonAsync<Item>("api/items", Item);
        }
        else if(action.ToLower() == "edit" && !String.IsNullOrEmpty(Id))
        {
            response = await _client.PutAsJsonAsync<Item>($"api/items/{Id}", Item);
        }
        else
        {
            _snackBar.Add("Invalid action", Severity.Error);
            return;
        }

        if(response.IsSuccessStatusCode)
        {
            _snackBar.Add($"New Item is created", Severity.Success);
            _navigationManager.NavigateTo($"pfm/items");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            _snackBar.Add($"{error}", Severity.Error);
            
        }
    }

    public void SelectItem(Item item)
    {
        SelectedItem = item;
    }

    public void OnInValidSubmit() 
    {
        _snackBar.Add($"Please make sure to fill all necessary data!", Severity.Error);

    }

    public async void OnDeleteAsync(string id)
    {
        // prompt the user to confirm
        var confirmResult = await _jsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
            var item = Items.FirstOrDefault(j => j.Id == id);

        if(confirmResult) {
            var request = await _client.DeleteAsync($"api/items/{id}");

            if(request.IsSuccessStatusCode) {
                _snackBar.Add($"Item {item.Code} is deleted", Severity.Success);
                Items.Remove(item);
                StateHasChanged();
            }
            else {
                _snackBar.Add($"Item is not deleted", Severity.Error);
            }
        }
        else {
            _snackBar.Add($"Operation was canceled", Severity.Info);
        }
        
    }

    

    public void Export()
    {
        _navigationManager.NavigateTo($"api/items/export/csv?companyId={_stateContainer.Company.Id}&searchString={searchString}", true);
    }
}

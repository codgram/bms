using Application.Model.Procurement;
using Application.Model.ProductManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net.Http.Json;

namespace Application.Client.Pages.Procurement;

public class ProcurementBase : ComponentBase
{
    [Inject] public IJSRuntime _jsRuntime { get; set; }
    [Inject] public ISnackbar _snackBar { get; set; }
    [Inject] public NavigationManager _navigationManager { get; set; }
    [Inject] public StateContainer _stateContainer { get; set; }
    [Inject] public HttpClient _client { get; set; }

    public bool dense = false;
    public bool hover = true;
    public bool striped = false;
    public bool bordered = false;
    public string searchString1 = "";
    public string SubmitButtonText = "Add";
    
    public Vendor Vendor = new Vendor();
    public List<Vendor> Vendors = new List<Vendor>();
    public Vendor SelectedVendor = null;
    public HashSet<Vendor> SelectedVendorSet = new HashSet<Vendor>();
    public bool FilterVendorFunc(Vendor Vendor) => FilterVendor(Vendor, searchString1);


    

    public async Task<List<T>> GetObjectsAsync<T>(string entity)
    {
        return await _client.GetFromJsonAsync<List<T>>($"api/{entity}?companyId={_stateContainer.Company.Id}");
    }
    
    public async Task GetVendorAsync(string id)
    {
        SubmitButtonText = "Save";
        Vendor = await _client.GetFromJsonAsync<Vendor>($"api/vendors/{id}");
    }


    public bool FilterVendor(Vendor Vendor, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Vendor.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Vendor.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Vendor.Description}".Contains(searchString))
            return true;
        return false;
    }




    public void OnInValidSubmit() 
    {
        _snackBar.Add($"Please make sure to fill all necessary data!", Severity.Error);

    }

    public void ResetObject(string entity)
    {
        if (entity.ToLower() == "Vendor")
        {
            Vendor = new Vendor();
            SubmitButtonText = "Add";
        }
    }

    public async void OnDeleteAsync(string id, string entity)
    {
        // prompt the user to confirm
        var confirmResult = await _jsRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this {entity}?");
        
        var vendor = Vendors.FirstOrDefault(j => j.Id == id);

        if(confirmResult) {
            var request = await _client.DeleteAsync($"api/{entity}/{id}");

            if(request.IsSuccessStatusCode) {
                _snackBar.Add($"{entity} is deleted", Severity.Success);
                RemoveEntityFromList(id, entity);
                StateHasChanged();
            }
            else {
                _snackBar.Add($"Vendor is not deleted", Severity.Error);
            }
        }
        else {
            _snackBar.Add($"Operation was canceled", Severity.Info);
        }
        
    }

    public void RemoveEntityFromList(string id, string entity)
    {
        if(entity == "vendor")
        {
            var vendor = Vendors.FirstOrDefault(j => j.Id == id);
            Vendors.Remove(vendor);
        }
    }

    
}

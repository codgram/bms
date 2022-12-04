using Application.Model.Procurement;
using Application.Model.ProductManagement;
using Application.Model.Enum;
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
    public ButtonAction ButtonAction { get; set; } = ButtonAction.Add;
    
    public Vendor Vendor = new Vendor();
    public List<Vendor> Vendors = new List<Vendor>();
    public Vendor SelectedVendor = null;
    public HashSet<Vendor> SelectedVendorSet = new HashSet<Vendor>();
    public bool FilterVendorFunc(Vendor Vendor) => FilterVendor(Vendor, searchString1);

    public PurchaseHeader PurchaseHeader = new PurchaseHeader();
    public List<PurchaseHeader> PurchaseHeaders = new List<PurchaseHeader>();
    public PurchaseHeader SelectedPurchaseHeader = null;
    public HashSet<PurchaseHeader> SelectedPurchaseHeaderSet = new HashSet<PurchaseHeader>();
    public bool FilterPurchaseHeaderFunc(PurchaseHeader PurchaseHeader) => FilterPurchaseHeader(PurchaseHeader, searchString1);


    public PurchaseLine PurchaseLine = new PurchaseLine();
    public PurchaseLineDetail PurchaseLineDetail = new PurchaseLineDetail();
    public List<PurchaseLine> PurchaseLines = new List<PurchaseLine>();
    public PurchaseLine SelectedPurchaseLine = null;
    public HashSet<PurchaseLine> SelectedPurchaseLineSet = new HashSet<PurchaseLine>();
    public bool FilterPurchaseLineFunc(PurchaseLine PurchaseLine) => FilterPurchaseLine(PurchaseLine, searchString1);


    

    public async Task<List<T>> GetObjectsAsync<T>(string entity)
    {
        return await _client.GetFromJsonAsync<List<T>>($"api/{entity}?companyId={_stateContainer.Company.Id}");
    }

    public async Task<List<T>> GetObjectsAsync<T>(string entity, string? filter)
    {
        return await _client.GetFromJsonAsync<List<T>>($"api/{entity}?companyId={_stateContainer.Company.Id}&{filter}");
    }

    public async Task GetPurchaseHeaderAsync(string id)
    {
        SubmitButtonText = "Save";
        PurchaseHeader = await _client.GetFromJsonAsync<PurchaseHeader>($"api/purchase/headers/{id}");
    }

    public async Task GetPurchaseLineAsync(string id)
    {
        SubmitButtonText = "Save";
        PurchaseLine = await _client.GetFromJsonAsync<PurchaseLine>($"api/purchase/lines/{id}");
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

    public bool FilterPurchaseHeader(PurchaseHeader PurchaseHeader, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (PurchaseHeader.DocumentNo.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (PurchaseHeader.Vendor.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{PurchaseHeader.Status}".Contains(searchString))
            return true;
        return false;
    }

        public bool FilterPurchaseLine(PurchaseLine PuchaseLine, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (PuchaseLine.LineNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (PuchaseLine.Item.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }




    public void OnInValidSubmit() 
    {
        _snackBar.Add($"Please make sure to fill all necessary data!", Severity.Error);

    }

    public void ResetObject(string entity)
    {
        if (entity == "Vendor")
        {
            Vendor = new Vendor();
            SubmitButtonText = "Add";
        }
        else if (entity == "purchaseHeader")
        {
            PurchaseHeader = new PurchaseHeader();
            SubmitButtonText = "Add";
        }
        else if (entity == "purchaseLine")
        {
            PurchaseLine = new PurchaseLine();
            SubmitButtonText = "Add";
        }
    }

    public async void OnDeleteAsync(string id, string entity)
    {
        // prompt the user to confirm
        var confirmResult = await _jsRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this {entity}?");

        if(confirmResult) {
            var request = await _client.DeleteAsync($"api/{entity}/{id}");

            if(request.IsSuccessStatusCode) {
                _snackBar.Add($"{entity} is deleted", Severity.Success);
                RemoveEntityFromList(id, entity);
                StateHasChanged();
            }
            else if(request.StatusCode == System.Net.HttpStatusCode.NotFound) {
                _snackBar.Add($"{entity} is not found", Severity.Error);
            }
            else if(request.StatusCode == System.Net.HttpStatusCode.BadRequest) {

                // Get error message from response
                var response = await request.Content.ReadAsStringAsync();

                // snackbar
                _snackBar.Add(response, Severity.Error);
            }
            else {
                _snackBar.Add($"Error while deleting {entity}", Severity.Error);
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
        if(entity == "purchase/headers")
        {
            var purchaseHeader = PurchaseHeaders.FirstOrDefault(j => j.Id == id);
            PurchaseHeaders.Remove(purchaseHeader);
        }
    }

    
}

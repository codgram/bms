using Application.Model.ProductManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net.Http.Json;

namespace Application.Client.Pages.ProductManagement;

public class ProductHierarchyBase : ComponentBase
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
    
    public Division Division = new Division();
    public List<Division> Divisions = new List<Division>();
    public Division SelectedDivision = null;
    public HashSet<Division> SelectedDivisionSet = new HashSet<Division>();
    public bool FilterDivisionFunc(Division Division) => FilterDivision(Division, searchString1);


    public Category Category = new Category();
    public List<Category> Categories = new List<Category>();
    public Category SelectedCategory = null;
    public HashSet<Category> SelectedCategorySet = new HashSet<Category>();
    public bool FilterCategoryFunc(Category Category) => FilterCategory(Category, searchString1);

    
    public Group Group = new Group();
    public List<Group> Groups = new List<Group>();
    public Group SelectedGroup = null;
    public HashSet<Group> SelectedGroupSet = new HashSet<Group>();
    public bool FilterGroupFunc(Group Group) => FilterGroup(Group, searchString1);


    public Subgroup Subgroup = new Subgroup();
    public List<Subgroup> Subgroups = new List<Subgroup>();
    public Subgroup SelectedSubgroup = null;
    public HashSet<Subgroup> SelectedSubgroupSet = new HashSet<Subgroup>();
    public bool FilterSubgroupFunc(Subgroup Subgroup) => FilterSubgroup(Subgroup, searchString1);

    

    public async Task<List<T>> GetObjectsAsync<T>(string entity)
    {
        return await _client.GetFromJsonAsync<List<T>>($"api/{entity}?companyId={_stateContainer.Company.Id}");
    }


    public async Task GetDivisionAsync(string id)
    {
        SubmitButtonText = "Save";
        Division = await _client.GetFromJsonAsync<Division>($"api/divisions/{id}");
    }

    public async Task GetCategoryAsync(string id)
    {
        SubmitButtonText = "Save";
        Category = await _client.GetFromJsonAsync<Category>($"api/categories/{id}");
    }

    public async Task GetGroupAsync(string id)
    {
        SubmitButtonText = "Save";
        Group = await _client.GetFromJsonAsync<Group>($"api/groups/{id}");
    }
    public async Task GetSubgroupAsync(string id)
    {
        SubmitButtonText = "Save";
        Subgroup = await _client.GetFromJsonAsync<Subgroup>($"api/subgroups/{id}");
    }

    public bool FilterDivision(Division Division, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Division.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Division.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Division.Description}".Contains(searchString))
            return true;
        return false;
    }

    public bool FilterCategory(Category Category, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Category.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Category.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Category.Description}".Contains(searchString))
            return true;
        return false;
    }

    public bool FilterGroup(Group Group, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Group.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Group.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Group.Description}".Contains(searchString))
            return true;
        return false;
    }
    public bool FilterSubgroup(Subgroup Subgroup, string searchString)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (Subgroup.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (Subgroup.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if ($"{Subgroup.Description}".Contains(searchString))
            return true;
        return false;
    }



    public void OnInValidSubmit() 
    {
        _snackBar.Add($"Please make sure to fill all necessary data!", Severity.Error);

    }

    public void ResetObject(string entity)
    {
        if (entity.ToLower() == "Division")
        {
            Division = new Division();
            SubmitButtonText = "Add";
        }
        else if (entity.ToLower() == "category")
        {
            Category = new Category();
            SubmitButtonText = "Add";
        }
        else if (entity.ToLower() == "group")
        {
            Group = new Group();
            SubmitButtonText = "Add";
        }
        else if (entity.ToLower() == "subgroup")
        {
            Subgroup = new Subgroup();
            SubmitButtonText = "Add";
        }
    }

    public async void OnDeleteAsync(string id, string entity)
    {
        // prompt the user to confirm
        var confirmResult = await _jsRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this {entity}?");
        
        var division = Divisions.FirstOrDefault(j => j.Id == id);

        if(confirmResult) {
            var request = await _client.DeleteAsync($"api/{entity}/{id}");

            if(request.IsSuccessStatusCode) {
                _snackBar.Add($"{entity} is deleted", Severity.Success);
                RemoveEntityFromList(id, entity);
                StateHasChanged();
            }
            else {
                _snackBar.Add($"Division is not deleted", Severity.Error);
            }
        }
        else {
            _snackBar.Add($"Operation was canceled", Severity.Info);
        }
        
    }

    public void RemoveEntityFromList(string id, string entity)
    {
        if(entity == "division")
        {
            var division = Divisions.FirstOrDefault(j => j.Id == id);
            Divisions.Remove(division);
        }
        else if(entity == "category")
        {
            var category = Categories.FirstOrDefault(j => j.Id == id);
            Categories.Remove(category);
        }
        else if(entity == "group")
        {
            var group = Groups.FirstOrDefault(j => j.Id == id);
            Groups.Remove(group);
        }
        else if(entity == "subgroup")
        {
            var subgroup = Subgroups.FirstOrDefault(j => j.Id == id);
            Subgroups.Remove(subgroup);
        }
    }

    
}

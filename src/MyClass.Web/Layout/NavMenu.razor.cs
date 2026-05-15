using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace MyClass.Web.Layout;

public partial class NavMenu
{
    [Parameter]
    public ClassContext? CurrentClass { get; set; }

    private bool IsTeacher => LoginStateService.Current?.IsTeacher == true;

    private bool IsStudent => LoginStateService.Current?.IsTeacher == false;

    protected override void OnInitialized()
    {
        LoginStateService.Changed += OnLoginStateChanged;
    }

    private void OnLoginStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        LoginStateService.Changed -= OnLoginStateChanged;
    }
}

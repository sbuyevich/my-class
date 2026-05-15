using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace MyClass.Web.Layout;

public partial class NavMenu
{
    [Parameter]
    public ClassContext? CurrentClass { get; set; }

    private bool IsTeacher => LoginStateService.CurrentLoginState?.IsTeacher == true;

    private bool IsStudent => LoginStateService.CurrentLoginState?.IsTeacher == false;

    protected override void OnInitialized()
    {
        LoginStateService.LoginStateChanged += OnLoginStateChanged;
    }

    private void OnLoginStateChanged()
    {
        _ = InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        LoginStateService.LoginStateChanged -= OnLoginStateChanged;
    }
}

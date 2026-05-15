using Microsoft.AspNetCore.Components;

namespace MyClass.Web.Components;

public partial class RoleGate
{
    [Parameter]
    public bool RequireTeacher { get; set; }

    [Parameter]
    public string DeniedMessage { get; set; } = "You do not have access to this page.";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private bool _loadedLoginState;

    protected override void OnInitialized()
    {
        LoginStateService.LoginStateChanged += OnLoginStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var state = LoginStateService.CurrentLoginState ?? await SessionStorage.GetLoginStateAsync();
        LoginStateService.SetLoginState(state);
        _loadedLoginState = true;
        StateHasChanged();
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
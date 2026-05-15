namespace MyClass.Web.Layout;

public partial class AppGreeting
{
    private string GreetingText
    {
        get
        {
            var state = LoginStateService.CurrentLoginState;

            if (state is null)
            {
                return string.Empty;
            }

            var displayName = string.IsNullOrWhiteSpace(state.DisplayName)
                ? state.UserName
                : state.DisplayName;

            return $"Hi, {displayName}";
        }
    }

    protected override void OnInitialized()
    {
        LoginStateService.LoginStateChanged += OnLoginStateChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || LoginStateService.CurrentLoginState is not null)
        {
            return;
        }

        var state = await SessionStorage.GetLoginStateAsync();

        if (state is not null)
        {
            LoginStateService.SetLoginState(state);
        }
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
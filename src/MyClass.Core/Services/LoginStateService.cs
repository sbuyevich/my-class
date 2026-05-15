using MyClass.Core.Models;

namespace MyClass.Core.Services;

public sealed class LoginStateService : ILoginStateService
{
    public LoginState? CurrentLoginState { get; private set; }

    public event Action? LoginStateChanged;

    public void SetLoginState(LoginState? state)
    {
        CurrentLoginState = state;
        LoginStateChanged?.Invoke();
    }
}



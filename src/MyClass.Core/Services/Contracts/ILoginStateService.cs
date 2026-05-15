using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface ILoginStateService
{
    LoginState? CurrentLoginState { get; }

    event Action? LoginStateChanged;

    void SetLoginState(LoginState? state);
}



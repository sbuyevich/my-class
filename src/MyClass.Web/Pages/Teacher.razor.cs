using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyClass.Core.Models;

namespace MyClass.Web.Pages;

public partial class Teacher
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    private Result<string>? _studentEntryUrlResult;
    private string? _qrCodeDataUri;
    private LoginState? _loginState;
    private bool _isResettingStudents;

    protected override void OnParametersSet()
    {
        var lanBaseUrl = UrlService.GetLanBaseUrl(Navigation.Uri);

        if (lanBaseUrl is null)
        {
            _studentEntryUrlResult = Result<string>.Failure("Unable to determine LAN base URL.");
            _qrCodeDataUri = null;
            return;
        }

        var studentPath = ClassContextService.GetPathWithClassCode("/login", CurrentClass.Code);
        var studentEntryUrl = new Uri(new Uri(lanBaseUrl), studentPath).ToString();

        try
        {
            _qrCodeDataUri = QrCodeService.CreateSvgDataUri(studentEntryUrl);
            _studentEntryUrlResult = Result<string>.Success(studentEntryUrl);
        }
        catch (InvalidOperationException)
        {
            _qrCodeDataUri = null;
            _studentEntryUrlResult = Result<string>.Failure("The student connection URL is too long to display as a QR code.");
        }
    }

    private async Task ResetActiveStudentsAsync()
    {
        try
        {
            _isResettingStudents = true;
            _loginState = LoginStateService.CurrentLoginState ?? await SessionStorage.GetLoginStateAsync();
            LoginStateService.SetLoginState(_loginState);

            var result = await StudentService.ResetStudentsActiveStateAsync(_loginState, CurrentClass);

            Snackbar.Add(result.Message, result.Succeeded ? Severity.Success : Severity.Error);
        }
        finally
        {
            _isResettingStudents = false;
        }
    }
}

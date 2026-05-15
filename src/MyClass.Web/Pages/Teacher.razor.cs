using Microsoft.AspNetCore.Components;
using MyClass.Core.Models;

namespace MyClass.Web.Pages;

public partial class Teacher
{
    [CascadingParameter]
    public ClassContext CurrentClass { get; set; } = null!;

    private Result<string>? _studentEntryUrlResult;
    private string? _qrCodeDataUri;

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
}

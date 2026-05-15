namespace MyClass.Core.Services;

public interface IQrCodeService
{
    string CreateSvgDataUri(string text);
}

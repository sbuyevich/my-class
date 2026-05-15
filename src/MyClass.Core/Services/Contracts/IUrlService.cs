using MyClass.Core.Models;

namespace MyClass.Core.Services;

public interface IUrlService
{
    string? GetLanBaseUrl(string currentUri);
}

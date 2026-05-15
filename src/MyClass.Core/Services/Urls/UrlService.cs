using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public sealed class UrlService : IUrlService
{
    public string? GetLanBaseUrl(string currentUri)
    {
        var wifiAddress = GetWifiAddress();

        return wifiAddress is null ? null : BuildUrl(currentUri, wifiAddress);
    }

    private static string BuildUrl(string currentUri, string host)
    {
        var builder = new UriBuilder(currentUri)
        {
            Host = host
        };

        return builder.Uri.GetLeftPart(UriPartial.Authority);
    }

    private static string? GetWifiAddress()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
            .Where(IsCandidateInterface)
            .SelectMany(GetCandidateAddresses)
            .Select(address => address.ToString())
            .FirstOrDefault();
    }

    private static bool IsLanIpv4Address(IPAddress address)
    {
        return address.AddressFamily == AddressFamily.InterNetwork
            && !IPAddress.IsLoopback(address);
    }

    private static bool IsCandidateInterface(NetworkInterface networkInterface)
    {
        return networkInterface.OperationalStatus == OperationalStatus.Up
            && networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
            && !LooksVirtualOrNonLan(networkInterface);
    }

    private static IEnumerable<IPAddress> GetCandidateAddresses(NetworkInterface networkInterface)
    {
        return networkInterface.GetIPProperties()
            .UnicastAddresses
            .Select(unicastAddress => unicastAddress.Address)
            .Where(IsLanIpv4Address);
    }

    private static bool LooksVirtualOrNonLan(NetworkInterface networkInterface)
    {
        var text = $"{networkInterface.Name} {networkInterface.Description}";

        return ContainsAny(
            text,
            "vethernet",
            "virtual",
            "hyper-v",
            "wsl",
            "docker",
            "vmware",
            "virtualbox",
            "vpn",
            "bluetooth");
    }

    private static bool ContainsAny(string text, params string[] values)
    {
        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }
}

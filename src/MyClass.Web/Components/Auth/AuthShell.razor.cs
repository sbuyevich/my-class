using Microsoft.AspNetCore.Components;

namespace MyClass.Web.Components;

public partial class AuthShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public int ZIndex { get; set; } = 1;

    private string ShellStyle =>
        "position: fixed; inset: 0; " +
        $"z-index: {ZIndex}; " +
        "background: linear-gradient(90deg, rgba(15, 23, 42, 0.78), rgba(15, 23, 42, 0.38)), " +
        "url(\"/images/back.png\") center / cover no-repeat; " +
        "pointer-events: auto;";
}

using Microsoft.AspNetCore.Components;

namespace MyClass.Web.Pages;

public partial class QuizAnswer
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        Navigation.NavigateTo("/student", replace: true);
    }
}

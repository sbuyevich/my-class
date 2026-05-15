using MyClass.Core.Models;

namespace MyClass.Web.App;

public static class LandingRoutes
{
    public static string For(LoginState state)
    {
        return state.IsTeacher ? "/teacher" : "/student";
    }
}

using Microsoft.Extensions.DependencyInjection;
using MyClass.Core.Models;

namespace MyClass.Core.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoginStateService, LoginStateService>();
        services.AddScoped<IClassContextService, ClassContextService>();
        services.AddScoped<IClassContextState, ClassContextState>();
        services.AddScoped<IQuizContentService, QuizContentService>();
        services.AddScoped<IQuizSessionService, QuizSessionService>();
        services.AddScoped<IQuizAnswerService, QuizAnswerService>();
        services.AddScoped<IQuizResultService, QuizResultService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ISchoolClassService, SchoolClassService>();
        services.AddScoped<ISessionStorageService, SessionStorageService>();
        services.AddSingleton<IUrlService, UrlService>();
        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddSingleton<IActiveQuizSelectionService, ActiveQuizSelectionService>();
        services.AddSingleton<IPasswordHashService, PasswordHashService>();

        return services;
    }
}


using WebUI.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddControllersWithViews();
        services.AddSignalR();
        services.AddScoped<IViewRenderService, ViewRenderService>();
        services.AddHttpContextAccessor();
        return services;
    }
}

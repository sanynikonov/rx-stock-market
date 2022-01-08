using Business.StockPrice;
using Infrastructure;
using Infrastructure.Finance;
using Infrastructure.Storage;
using Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStockMarketServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpClient()
            .AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("StockMarket")))
            .AddIdentity<UserModel, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services
            .AddSingleton(typeof(IStorage<,>), typeof(KeyValueStorage<,>))
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IFinanceApiClient, FinanceApiApiClient>()
            .AddTransient<IStockService, StockService>();

        return services;
    }
}
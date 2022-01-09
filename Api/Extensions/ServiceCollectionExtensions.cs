using Api.Mapper;
using Business.Stock;
using Business.Stock.Price;
using Infrastructure;
using Infrastructure.Finance;
using Infrastructure.Storage;
using Infrastructure.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStockMarketServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAutoMapper(opt => opt.AddProfile(new MapperProfile()))
            .AddHttpClient()
            .AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("StockMarket")))
            .AddIdentity<UserModel, IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services
            .AddSingleton(typeof(IStorage<,>), typeof(KeyValueStorage<,>))
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IFinanceApiClient, FinanceApiApiClient>()
            .AddTransient<IStockService, StockService>()
            .AddTransient<IStockPriceSubscriptionService, StockPriceSubscriptionService>();

        return services;
    }
}
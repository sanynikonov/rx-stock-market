using System.Text;
using Api.Mapper;
using Business.News;
using Business.Stock;
using Business.Stock.Price;
using Business.Stock.Trend;
using Business.Users;
using Business.Users.Jwt;
using Infrastructure;
using Infrastructure.Finance;
using Infrastructure.Storage;
using Infrastructure.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            .AddTransient<IUserService, Business.Users.UserService>()
            .AddTransient<IFinanceApiClient, FinanceApiApiClient>()
            .AddTransient<IStockService, StockServiceRandomized>()
            .AddTransient<INewsService, NewsService>()
            .AddTransient<IStockPriceSubscriptionService, StockPriceSubscriptionService>()
            .AddTransient<IStockTrendSubscriptionService, StockTrendSubscriptionService>();

        services.AddScoped<IJwtGenerator, JwtGenerator>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        services.AddAuthorization();

        return services;
    }
}
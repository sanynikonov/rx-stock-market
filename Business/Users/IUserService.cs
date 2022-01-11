using System.Reactive;
using Microsoft.AspNetCore.Identity;

namespace Business.Users;

public interface IUserService
{
    Task<IdentityResult> Register(string username, string password);
    Task<string> Login(string username, string password);
    Task UpdateUserPreferences(UserPreferences model, string username);
}
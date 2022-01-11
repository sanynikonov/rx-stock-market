using System.Net;
using System.Reactive.Linq;
using Business.Users.Jwt;
using Infrastructure.Users;
using Microsoft.AspNetCore.Identity;

namespace Business.Users;

public class UserService : IUserService
{
    private readonly UserManager<UserModel> _userManager;
    private readonly SignInManager<UserModel> _signInManager;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUserRepository _userRepository;

    public UserService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IJwtGenerator jwtGenerator, IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtGenerator = jwtGenerator;
        _userRepository = userRepository;
    }

    public async Task<IdentityResult> Register(string username, string password)
    {
        return await _userManager.CreateAsync(new UserModel { UserName = username }, password);
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            throw new Exception("Unauthorized");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        if (result.Succeeded)
        {
            return _jwtGenerator.CreateToken(user);
        }

        throw new Exception("Unauthorized");
    }

    public async Task UpdateUserPreferences(UserPreferences model, string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        user.RequestedCompanies = model.Companies.Select(c => new CompanyModel { Name = c.Name, Tags = c.SearchTags.ToArray() }).ToArray();
        await _userRepository.UpdateUserPreferences(user);
    }
}
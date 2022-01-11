using Infrastructure.Users;

namespace Business.Users.Jwt;

public interface IJwtGenerator
{
    string CreateToken(UserModel user);
}
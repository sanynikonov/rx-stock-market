using System.Reactive;

namespace Infrastructure.Users;

public interface IUserRepository
{
    IObservable<UserModel> GetUserPreferences(string username);
    Task UpdateUserPreferences(UserModel model);
}
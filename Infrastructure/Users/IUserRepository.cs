using System.Reactive;

namespace Infrastructure.Users;

public interface IUserRepository
{
    IObservable<UserModel> GetUserPreferences(int userId);
    Task UpdateUserPreferences(UserModel model);
}
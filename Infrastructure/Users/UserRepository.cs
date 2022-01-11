using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly UserManager<UserModel> _userManager;
    private readonly IStorage<string, BehaviorSubject<UserModel>> _subjects;

    public UserRepository(UserManager<UserModel> userManager, IStorage<string, BehaviorSubject<UserModel>> subjects)
    {
        _userManager = userManager;
        _subjects = subjects;
    }

    public IObservable<UserModel> GetUserPreferences(string username)
    {
        //TODO: use PublishLast
        return Observable.DeferAsync(async token =>
        {
            if (_subjects.TryGetValue(username, out var subject))
            {
                return subject;
            }

            var user = await _userManager.FindByNameAsync(username);
            subject = new BehaviorSubject<UserModel>(user);
            _subjects.Add(username, new BehaviorSubject<UserModel>(user));
            return subject.AsObservable();
        });
    }

    public async Task UpdateUserPreferences(UserModel model)
    {
        await _userManager.UpdateAsync(model);
        if (_subjects.TryGetValue(model.UserName, out var subject))
        {
            subject.OnNext(model);
        }
    }
}
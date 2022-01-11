using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IStorage<string, BehaviorSubject<UserModel>> _subjects;

    public UserRepository(AppDbContext context, IStorage<string, BehaviorSubject<UserModel>> subjects)
    {
        _context = context;
        _subjects = subjects;
    }

    public IObservable<UserModel> GetUserPreferences(string username)
    {
        return Observable.Create<UserModel>(async (observer, token) =>
        {
            if (!_subjects.TryGetValue(username, out var subject))
            {
                try
                {
                    var user = await _context.Users
                        .Include(u => u.RequestedCompanies)
                        .SingleAsync(u => u.UserName == username, token);

                    subject = new BehaviorSubject<UserModel>(user);
                    _subjects.Add(username, new BehaviorSubject<UserModel>(user));
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }
            }

            observer.OnNext(subject.Value);
            observer.OnCompleted();
            return Disposable.Empty;
        });
    }

    public async Task UpdateUserPreferences(UserModel model)
    {
        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        if (_subjects.TryGetValue(model.UserName, out var subject))
        {
            subject.OnNext(model);
        }
    }
}
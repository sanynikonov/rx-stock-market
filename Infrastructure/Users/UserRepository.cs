using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<int, BehaviorSubject<UserModel>> _subjects = new();

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public IObservable<UserModel> GetUserPreferences(int userId)
    {
        //TODO: use PublishLast
        return Observable.DeferAsync(async token =>
        {
            if (_subjects.TryGetValue(userId, out var subject))
            {
                return subject;
            }

            var user = await _context.Users.FindAsync(userId, token);
            _subjects[userId] = new BehaviorSubject<UserModel>(user);
            return _subjects[userId].AsObservable();
        });
    }

    public IObservable<Unit> UpdateUserPreferences(UserModel model)
    {
        return Observable.FromAsync(async token =>
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync(token);
            _subjects[model.Id].OnNext(model);
            return Unit.Default;
        });
    }
}
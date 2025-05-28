using System.Collections.Concurrent;

public class LockerService
{
    private readonly ConcurrentDictionary<string, Locker> _lockers = new();

    public LockerService()
    {
        _lockers.TryAdd("L1", new Locker { Id = "L1", Location = "Main St 123", Available = true });
    }

    public bool Exists(string id) => _lockers.ContainsKey(id);

    public Locker GetLockerById(string id) => _lockers.TryGetValue(id, out var locker) ? locker : null;

    public bool AssignPackage(PackageAssignmentRequest request)
    {
        if (_lockers.TryGetValue(request.LockerId, out var locker) && locker.Available)
        {
            locker.Available = false;
            locker.AssignedTo = request.PackageCode;
            return true;
        }
        return false;
    }
}
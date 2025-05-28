public static class LockerHelper
{
    public static bool IsLockerAvailable(Locker locker)
    {
        return locker != null && locker.Available;
    }
}

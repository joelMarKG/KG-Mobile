namespace KG.Mobile.Helpers;

public static class AppDebug
{
#if DEBUG
    public const bool IsDebug = true;
#else
    public const bool IsDebug = false;
#endif
}

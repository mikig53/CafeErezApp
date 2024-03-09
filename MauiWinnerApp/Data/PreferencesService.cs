using BetModels.Interfaces;


namespace MauiTelesportApp.Data;

public class PreferencesService : IPreferencesService
{
    public T Get<T>(string key, T defaultValue)
    {
        return Preferences.Default.Get(key, defaultValue);
    }

    public void Set<T>(string key, T value)
    {
        Preferences.Default.Set(key, value);
    }
}

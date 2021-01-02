using Microsoft.Win32;
using System.Configuration;

namespace Otomasyon
{
    public static class SettingsExtensions
    {
        static RegistryKey settingConfigurations = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Settings");
        internal static void SaveChanges(this Properties.Settings settings)
        {
            foreach(SettingsPropertyValue pv in settings.PropertyValues)
                settingConfigurations.SetValue(pv.Name, pv.PropertyValue);
            settings.Save();
        }
        internal static void Load(this Properties.Settings settings)
        {
            settings.Reload();
            var names = new string[settings.PropertyValues.Count];
            int index = 0;
            foreach(SettingsPropertyValue pv in settings.PropertyValues)
                names[index++] = pv.Name;
            for(int i = 0; i < settings.PropertyValues.Count; i++)
                settings.PropertyValues[names[i]].PropertyValue = settingConfigurations.GetValue(names[i], settings.PropertyValues[names[i]].PropertyValue);
            settings.Save();
        }
    }
}

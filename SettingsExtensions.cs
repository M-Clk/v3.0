using System.Configuration;
using System.Windows.Forms;
using Microsoft.Win32;
using Otomasyon.Properties;

namespace Otomasyon
{
    public static class SettingsExtensions
    {
        private static readonly RegistryKey settingConfigurations =
            Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Settings");

        internal static void SaveChanges(this Settings settings)
        {
            foreach (SettingsPropertyValue pv in settings.PropertyValues)
                settingConfigurations.SetValue(pv.Name, pv.PropertyValue);
            settings.Save();
            settings.Upgrade();
            Application.Restart();
        }

        internal static void Load(this Settings settings)
        {
            settings.Reload();
            var names = new string[settings.PropertyValues.Count];
            var index = 0;
            foreach (SettingsPropertyValue pv in settings.PropertyValues)
                names[index++] = pv.Name;
            for (var i = 0; i < settings.PropertyValues.Count; i++)
                settings.PropertyValues[names[i]].PropertyValue =
                    settingConfigurations.GetValue(names[i], settings.PropertyValues[names[i]].PropertyValue);
            settings.Save();
        }
    }
}
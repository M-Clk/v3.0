using Microsoft.Win32;
using System;

namespace Otomasyon
{
    public static class ConfigurationManager
    {
        public static void SetKeyValueToRegistry<T>(ConfigurationCategory category, string valueName, T value)
        {
            Registry.SetValue(getKeyPath(category), valueName, value);
        }

        public static T GetValueFromRegistry<T>(ConfigurationCategory category, string valueName, Func<object, T> converterFunc, T defaultValue = default)
        {
            try
            {
                object value = Registry.GetValue(getKeyPath(category), valueName, null); 
                if(value != null)
                    return converterFunc(value);

            }
            catch (Exception)
            {
            }

            return defaultValue;
        }
        private static string getKeyPath(ConfigurationCategory category) => "HKEY_CURRENT_USER\\Software\\Otomasyon\\" + category.ToString();
    }

    public enum ConfigurationCategory
    {
        DbConf,
        PrinterConf,
        InitializeApp,
        Licence,
        General
    }
}

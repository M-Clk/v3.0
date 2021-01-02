using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Otomasyon
{
    static class Program
    {
        public enum Yetki
        {
            yonetici = 1,
            eleman = 0
        }
        public static RegistryKey SqlConfiguration = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\SqlServerConfiguration");
        static RegistryKey Yapilandirma = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Yazici");
        public static bool
            stok_calisiyor = false,
            giris = false,
            kritik = false,
            satisYapildi = false,
            lisans = false,
            sifreIstesin = true,
            herSatistaYazdir = false;
            public static bool sql_calisiyor { get; set; } = false;
        public static Yetki yetki = Yetki.eleman;
        public static string
            k_adi = "",
            sifre = "",
            isletmeAdi = "",
            telefon = "",
            yaziciAdi = "",
            email = "",
            adres = "",
            kayitliKagitTuru = "";
        public static CrystalDecisions.Shared.PaperSize
            kagiTuru = CrystalDecisions.Shared.PaperSize.PaperA4;

        public static int k_id = 1, odemeTipi = 0, serverType=0;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHadler);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Uyguluma açık mı diye kontrol et
            if (Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                MessageBox.Show(Application.ProductName + " zaten açık.");
            }
            else
            {
                Properties.Settings.Default.Load();
                if(!Properties.Settings.Default.IsDbCreated)
                    new DbOperations().CreateDb();
                if(!DbOperations.CheckSqlConnection())
                    throw new SqlServerManagementException("Veritabanına bağlanılamadı. Lütfen Sql Server servisinin açık olduğundan emin olun.");
                CheckAutoLogin();
                DefaultYapilandirma();
                if (SqlConfiguration.ValueCount > 0)
                     serverType = (int)SqlConfiguration.GetValue("ServerType");
                if(serverType==0)
                Application.Run(new frmAnaForm());
            }

        }
        static void CheckAutoLogin()
        {
            RegistryKey sifreIste = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
            try
            {
                sifreIstesin = Convert.ToBoolean(sifreIste.GetValue("SifreIste",1));
                if (!sifreIstesin)
                {
                    k_adi = sifreIste.GetValue("KayitliK_Adi").ToString();
                    sifre = sifreIste.GetValue("KayitliK_Sifre").ToString();
                }

            }
            catch (Exception ex)
            {
                sifreIstesin = false;
                sifreIste.SetValue("SifreIste", 1);
                sifreIste.SetValue("KayitliK_Adi", "");
                sifreIste.SetValue("KayitliK_Sifre", "");
            }
        }
        static void DefaultYapilandirma()
        {
            //TOO butun ayarlari RegistryKey den cek Properties.Settings e aktar. uygulamada oraya eris.
            //Yazıcı Adı
            try
            {
                yaziciAdi = Yapilandirma.GetValue("YaziciAdi").ToString();
            }
            catch (Exception)
            {

                Yapilandirma.SetValue("YaziciAdi", "");
            }
            //Kağıt Türü
            try
            {
                
                kayitliKagitTuru = Yapilandirma.GetValue("KagitTuru").ToString();
            }
            catch (Exception)
            {
                Yapilandirma.SetValue("KagitTuru", "");
            }
            Properties.Settings.Default.PrintEveryBills = herSatistaYazdir;
            //Ödeme Tipi
            try
            {
                odemeTipi = (int)Yapilandirma.GetValue("OdemeTipi");
            }
            catch (Exception)
            {
                Yapilandirma.SetValue("OdemeTipi", 0);
            }
            try
            {
                if (yaziciAdi == null || yaziciAdi == "") yaziciAdi = PrinterSettings.InstalledPrinters[0];
                Array paperDizi = Enum.GetValues(typeof(CrystalDecisions.Shared.PaperSize));

                foreach (var item in paperDizi)
                {
                    if (item.ToString() == kayitliKagitTuru)
                    {
                        kagiTuru = (CrystalDecisions.Shared.PaperSize)item;
                        break;
                    }

                }
            }
            catch { }
        }
        static void ExceptionHadler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            MessageBox.Show($"Bilinmeyen bir hata ile karşılaşıldı. Hata: {e.Message}", "Bir Problem Var!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

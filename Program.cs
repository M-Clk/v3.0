using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;

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
            herSatistaYazdir = false,
            sql_calisiyor=false;
        public static Yetki yetki = Yetki.eleman;
        public static string
            k_adi = "",
ConnectionString,
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Uyguluma açık mı diye kontrol et
            Process[] uyglama;
            uyglama = Process.GetProcessesByName(Application.ProductName);
            if (uyglama.Length > 1)
            {
                MessageBox.Show(Application.ProductName + " zaten açık.");
            }
            else
            {
                checkAutoLogin();
                DefaultYapilandirma();
                if (SqlConfiguration.ValueCount > 0)
                     serverType = (int)SqlConfiguration.GetValue("ServerType");
                if(serverType==0)
                checkSqlConnection();
                sql_calisiyor = DbOperations.BaglantiKontrol(Program.ConnectionString);
                Application.Run(new frmAnaForm());
            }

        }
        static void checkSqlConnection()
        {
            DbOperations dbOperations = new DbOperations();
            string server = dbOperations.FirstConnection();
            if (server != null)
            {
                SqlConfiguration.SetValue("SqlConnectString", @"Server=" + server + ";DataBase=OtomasyonDB;Trusted_Connection=True;");
                ConnectionString = SqlConfiguration.GetValue("SqlConnectString").ToString();
            }

            else
                MessageBox.Show("Veritabanı ile bağlantı sağlanamadı. Yapılandırma merkezinden ayarlarınızı yapılandırmayı deneyin.", "Uyarı Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        static void checkAutoLogin()
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
            //Yazdırma Seçeneği
            try
            {
                herSatistaYazdir = (bool)Yapilandirma.GetValue("HerSatisiYazdir");
            }
            catch
            {
                herSatistaYazdir = false;
                Yapilandirma.SetValue("HerSatisiYazdir", 0);
            }
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
    }
}

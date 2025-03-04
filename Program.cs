﻿using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.Win32;
using Otomasyon.Properties;

namespace Otomasyon
{
    internal static class Program
    {
        public enum Yetki
        {
            yonetici = 1,
            eleman = 0
        }

        public static RegistryKey Yapilandirma;

        public static bool
            stok_calisiyor = false,
            giris = false,
            kritik = false,
            satisYapildi = false,
            lisans = false,
            sifreIstesin = true,
            herSatistaYazdir = false;

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

        public static int k_id = 1, odemeTipi, quickProductsCount = 0;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionHadler;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            initialize();
            // Uyguluma açık mı diye kontrol et
            if (Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                MessageBox.Show(Application.ProductName + " zaten açık.");
            }
            else
            {
                //if (!DbOperations.IsDbCreated)
                //{
                //    Application.Run(new frmAnaForm(false));
                //    return;
                //}

                if (!DbOperations.Connection.IsAvailable())
                    throw new InvalidOperationException("Veritabanına bağlanılamadı. Lütfen Sql Server servisinin açık olduğundan emin olun.");
                CheckAutoLogin();
                DefaultYapilandirma();
                Application.Run(new frmAnaForm(true));
            }
        }

        private static void CheckAutoLogin()
        {
            var sifreIste = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
            try
            {
                sifreIstesin = Convert.ToBoolean(sifreIste.GetValue("SifreIste", 1));
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

        private static void DefaultYapilandirma()
        {
            //TOO butun ayarlari RegistryKey den cek Properties.Settings e aktar. uygulamada oraya eris.
            //Yazıcı Adı
            try
            {
                yaziciAdi = Yapilandirma.GetValue("YaziciAdi")?.ToString();
            }
            catch (Exception)
            {
                Yapilandirma.SetValue("YaziciAdi", "");
            }

            //Kağıt Türü
            try
            {
                kayitliKagitTuru = Yapilandirma.GetValue("KagitTuru")?.ToString();
            }
            catch (Exception)
            {
                Yapilandirma.SetValue("KagitTuru", "");
            }

            Settings.Default.PrintEveryBills = herSatistaYazdir;
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
                if (string.IsNullOrEmpty(yaziciAdi)) yaziciAdi = PrinterSettings.InstalledPrinters[0];
                var paperDizi = Enum.GetValues(typeof(PaperSize));

                foreach (var item in paperDizi)
                    if (item.ToString() == kayitliKagitTuru)
                    {
                        break;
                    }
            }
            catch
            {
            }
        }

        private static void ExceptionHadler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            MessageBox.Show($"Bilinmeyen bir hata ile karşılaşıldı. Hata: {e.Message}", "Bir Problem Var!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void initialize()
        {
            DbOperations.InitializeSqlConfiguration();
            Yapilandirma = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Yazici");
        }
    }
}
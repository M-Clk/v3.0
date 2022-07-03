using System;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Otomasyon
{
    public partial class frmGiris : Form
    {
        private static readonly RegistryKey _settingsKey =
            Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Settings");

        private string mclksiz = "";
        private readonly RegistryKey OtomasyonKeys = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
        private readonly RegistryKey SerialKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\License");
        private readonly DbOperations SqlOperation = new DbOperations();

        public frmGiris()
        {
            InitializeComponent();
        }

        private void GirisYap()
        {
            var parameterCollection = new SqlParameter[2];
            parameterCollection[0] = new SqlParameter();
            parameterCollection[0].ParameterName = "@Adi";
            parameterCollection[0].SqlDbType = SqlDbType.NVarChar;
            if (Program.sifreIstesin)
                parameterCollection[0].SqlValue = txtKullaniciAdi.Text;
            else
                parameterCollection[0].SqlValue = Program.k_adi;

            parameterCollection[1] = new SqlParameter();
            parameterCollection[1].ParameterName = "@Sifre";
            parameterCollection[1].SqlDbType = SqlDbType.NVarChar;
            if (Program.sifreIstesin)
                parameterCollection[1].SqlValue = txtSifre.Text;
            else
                parameterCollection[1].SqlValue = Program.sifre;

            Cursor = Cursors.WaitCursor;
            var dataRead = SqlOperation.OkuProcedure("KULLANICISORGULA", parameterCollection);
            Cursor = Cursors.Default;
            if (dataRead != null)
            {
                if (dataRead.NextResult()) //KULLANICISORGULA Stored Procedure göre eğer iki tablo sonuç geri döndüyse kullanıcı adı doğru girildi, şifre yanlış olabilir.
                {
                    if (dataRead.HasRows) //Eğer tablo boş değilse kullanıcı adı ve şifre doğru girildi. Kullanıcı adı ve şifreyi hafızda tut programa giriş yap.
                    {
                        dataRead.Read();
                        Program.k_adi = dataRead["Adi"].ToString();
                        Program.k_id = Convert.ToInt32(dataRead["Id"]);
                        Program.sifre = dataRead["Sifre"].ToString();
                        Program.yetki = (Program.Yetki)Convert.ToInt16(dataRead["Yetki"]);

                        DbOperations.Connection.Close();
                        //Eğer cbHatirla seçili ise kayıt defterindeki K_Adi kaydının değerini girilen kullanıcı adı ile değiştir. Seçili değilse kaydı sil.
                        if (Program.sifreIstesin)
                        {
                            if (cbHatirla.Checked)
                            {
                                OtomasyonKeys.SetValue("K_Adi", txtKullaniciAdi.Text);
                            }
                            else
                            {
                                OtomasyonKeys.SetValue("K_Adi", "");
                                OtomasyonKeys.DeleteValue("K_Adi");
                            }
                        }

                        Program.giris = true;
                        Close();
                    }
                    else
                    {
                        dataRead.NextResult(); //3. Tabloya git.
                        dataRead.Read();
                        MessageBox.Show(dataRead["Hata"].ToString(), "Kullanıcı Girişi Uyarısı", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        DbOperations.Connection.Close();
                    }
                }
                else
                {
                    MessageBox.Show(SqlOperation.bilgiMessage, "Kullanıcı Girişi Hatası", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                DbOperations.Connection.Close();
            }
            else
            {
                txtSifre.Text = "";
                return;
            }

            Program.sifreIstesin = true;
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            GirisKapisi();
        }

        private bool MailGonder()
        {
            var ePosta = new MailMessage();
            ePosta.From = new MailAddress("mclk.yzlm@gmail.com");
            ePosta.To.Add("mclk.yazilim.lisanshavuzu@gmail.com");
            ePosta.Subject = "Lisans İsteği";
            ePosta.Body =
                "<table style=\"width: 276.4px; \" border=\"1\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">İşletme Adı</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" +
                Program.isletmeAdi +
                "</td></tr><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">E-Mail</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" +
                Program.email +
                "</td></tr><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">Telefon</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" +
                Program.telefon +
                "</td></tr><tr style=\"height: 15.8px; \"><td style=\"width: 281.4px; text - align: center; height: 15.8px; \" colspan=\"2\"><center>" +
                mclksiz + "<center/></td></tr></tbody></table>";
            ePosta.IsBodyHtml = true;
            var smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential("mclk.yzlm@gmail.com", "m-clk123");
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            try
            {
                smtp.SendAsync(ePosta, ePosta);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void GirisKapisi()
        {
            try
            {
                Program.quickProductsCount = int.Parse(_settingsKey.GetValue("QuickProductsCount", "10").ToString());
            }
            catch (Exception)
            {
            }

            if (LisansKontrol())
            {
                Program.lisans = true;
            }
            else
            {
                Program.lisans = false;
                if (OtomasyonKeys.GetValue("MailGittiMi") == null || (int)OtomasyonKeys.GetValue("MailGittiMi") == 0)
                    if (MailGonder())
                        OtomasyonKeys.SetValue("MailGittiMi", 1);
                    else
                        OtomasyonKeys.SetValue("MailGittiMi", 0);
                else
                    OtomasyonKeys.SetValue("MailGittiMi", 0);
            }

            GirisYap();
        }

        private bool LisansKontrol()
        {
            try
            {
                if (SerialKey.ValueCount >= 1)
                {
                    var Key = SerialKey.GetValue("LicenseCode").ToString();
                    var mangnmt = new ManagementClass("Win32_LogicalDisk");
                    var mcol = mangnmt.GetInstances();
                    var result = "";
                    foreach (ManagementObject strt in mcol)
                        result += Convert.ToString(strt["VolumeSerialNumber"]);
                    result = MClkSifremele(result);
                    if (result == Key)
                        return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lisans kontrolü yapılırken bir sorun oluştu. Hata Mesajı : " + ex.Message);
            }

            return false;
        }

        private string MClkSifremele(string sifre)
        {
            var md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüştürdük.
            var dizi = Encoding.UTF8.GetBytes(sifre);
            //dizinin hash'ini hesaplattık.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiş verileri depolamak için StringBuilder nesnesi oluşturduk.
            var sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüştürdük.

            foreach (var ba in dizi)
                sb.Append(ba.ToString("x2").ToUpper());
            var md5li = sb.ToString();
            mclksiz = md5li;
            var sonHal = new char[24]; // "MCLKMXXXXCXXXXLXXXXKXXXX";
            //Kendi şifreleme yöntemim => MCLK-MXXXX-CXXXX-LXXXX-KXXXX  (24)
            sonHal[0] = 'M';
            sonHal[1] = 'C';
            sonHal[2] = 'L';
            sonHal[3] = 'K';
            sonHal[4] = 'M';
            sonHal[5] = md5li[6];
            sonHal[6] = md5li[9];
            sonHal[7] = md5li[29];
            sonHal[8] = md5li[13];
            sonHal[9] = 'C';
            sonHal[10] = md5li[15];
            sonHal[11] = md5li[5];
            sonHal[12] = md5li[10];
            sonHal[13] = md5li[16];
            sonHal[14] = 'L';
            sonHal[15] = md5li[17];
            sonHal[16] = md5li[19];
            sonHal[17] = md5li[18];
            sonHal[18] = md5li[1];
            sonHal[19] = 'K';
            sonHal[20] = md5li[0];
            sonHal[21] = md5li[30];
            sonHal[22] = md5li[27];
            sonHal[23] = md5li[25];
            md5li = "";
            for (var i = 0; i < 24; i++) md5li += sonHal[i];
            return md5li;
        }

        private void frmGiris_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.giris == false)
            {
                Application.ExitThread();
                Application.Exit();
            }
        }

        private void KullaniciKontrol()
        {
            //Software\Otomasyon dizininde adı K_Adi olan kaydın varlığını kontrol et. Eğer varsa txtKullaniciAdi textbox'ına K_Adi kaydının değerini yaz.
            var degerler = OtomasyonKeys.GetValueNames();
            for (var i = 0; i < degerler.Length; i++)
                if (degerler[i] == "K_Adi")
                {
                    txtKullaniciAdi.Text = OtomasyonKeys.GetValue("K_Adi").ToString();
                    cbHatirla.Checked = true;
                    txtSifre.Select();
                }
                else
                {
                    txtKullaniciAdi.Select();
                }
        }

        private void IsletmeKontrol()
        {
            var IsletmeOku = SqlOperation.SqlTextReader("SELECT Adi,Telefon,Email,Adres FROM Isletme");
            try
            {
                if (IsletmeOku.Read())
                {
                    Program.isletmeAdi = IsletmeOku[0].ToString();
                    Program.telefon = IsletmeOku[1].ToString();
                    Program.email = IsletmeOku[2].ToString();
                    Program.adres = IsletmeOku[3].ToString();
                }
            }
            catch
            {
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                txtSifre.PasswordChar = '\0';
            else if (checkBox1.Checked == false) txtSifre.PasswordChar = '•';
        }

        private void txtSifre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GirisKapisi();
        }

        private void frmGiris_Load(object sender, EventArgs e)
        {
            DefaultLoad();
        }

        private void DefaultLoad()
        {
            KullaniciKontrol();
            IsletmeKontrol();
            if (!Program.sifreIstesin)
                GirisKapisi();
            if (txtKullaniciAdi.TextLength > 0)
                txtSifre.Select();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            var frmCon = new frmConfiguration();
            frmCon.ShowDialog();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            var frmDes = new frmDestek();
            frmDes.Show();
        }
    }
}
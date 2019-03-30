using System;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Drawing.Printing;
using System.Drawing;
using System.Net.Mail;

namespace Otomasyon
{
    public partial class frmConfiguration : Form
    {
        DbOperations SqlCn = new DbOperations();

        RegistryKey SqlConfiguration = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\SqlServerConfiguration");
        RegistryKey SerialKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\License");
        RegistryKey Yapilandirma = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Yazici");
        RegistryKey Pass = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
        public frmConfiguration()
        {
            InitializeComponent();
        }       
        void IsletmeAdiGetır()
        {
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            {
                SqlDataReader IsletmeOku = SqlCn.SqlTextReader("SELECT Adi,Telefon,Email,Adres FROM Isletme");
                if (IsletmeOku.Read())
                {
                    txtIsletmeAdi.Text = IsletmeOku[0].ToString();
                    txtTelefon.Text = IsletmeOku[1].ToString();
                    txtEmail.Text = IsletmeOku[2].ToString();
                    txtAdres.Text = IsletmeOku[3].ToString();
                }
            }
            else
            {
                Program.sql_calisiyor = false;
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        string Key="";

        string SeriNoAl()
        {

                ManagementClass mangnmt = new ManagementClass("Win32_LogicalDisk");
                ManagementObjectCollection mcol = mangnmt.GetInstances();
                string result = "";
                foreach (ManagementObject strt in mcol)
                {
                    result += Convert.ToString(strt["VolumeSerialNumber"]);
                }
            return result;
                

        }
        bool LisansKontrol()
        {
            if (SerialKey.ValueCount >= 1)
            {
                Key = SerialKey.GetValue("LicenseCode").ToString();
                string result = MClkSifremele(SeriNoAl());
                if (result == Key)
                    return true;
            }
            else return false;
            return false;
        }
        void CBDoldur(ComboBox cb, string procString)
        {
                cb.DataSource = SqlCn.DisconnectedProcedure(procString, new SqlParameter[0]);
                cb.ValueMember = "Id";
                cb.DisplayMember = "Adi";        
        }

        string MClkSifremele(string sifre)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //Parametre olarak gelen veriyi byte dizisine dönüştürdük.
            byte[] dizi = Encoding.UTF8.GetBytes(sifre);
            //dizinin hash'ini hesaplattık.
            dizi = md5.ComputeHash(dizi);
            //Hashlenmiş verileri depolamak için StringBuilder nesnesi oluşturduk.
            StringBuilder sb = new StringBuilder();
            //Her byte'i dizi içerisinden alarak string türüne dönüştürdük.

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToUpper());
            }

            string md5li = sb.ToString();
            mclksiz = md5li;
            char[] sonHal = new char[24];  // "MCLKMXXXXCXXXXLXXXXKXXXX";
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

            for (int i = 0; i < 24; i++)
            {
                md5li += sonHal[i];
            }

            //İlk 4 karakteri dikkate alma
            //Dizenin [4] indexli karakteri M yap.
            //Dizenin [9] indexli karakteri C yap.
            //Dizenin [14] indexli karakteri L yap.
            //Dizenin [19] indexli karakteri K yap.
            //Aynı dizenin [24](dahil) indexli ve sonraki karakterlerini dikkate alma
            //Bunlardan yola çıkarak Yeni bir string oluştur.



            return md5li.ToString();
        }

        void IlkKullaniciKontrol()
        {
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            {
                SqlDataReader dr = SqlCn.OkuProcedure("KULLANICILARIGETIR", new SqlParameter[0]);
                if (dr.Read())
                {
                    txtKullaniciAdi.Text = dr["Adi"].ToString();
                    txtKullaniciAdi.Enabled = false;

                    txtKullaniciSifresi.Text = dr["Sifre"].ToString();
                    txtKullaniciSifresi.Enabled = false;
                    btnEkle.Enabled = false;
                    SqlCn.con.Dispose();
                    SqlCn.cmd.Dispose();
                }
            }
            else
            {
                Program.sql_calisiyor = false;
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
        }

        void AktiflikKontrol(bool durum)
        {
            try
            {
                if (durum)
                {
                    mskLisansAnahtari.Text = SerialKey.GetValue("LicenseCode").ToString();
                    mskLisansAnahtari.ReadOnly = true;
                    btnEtkinlestir.Enabled = false;
                    lblBilgi.Text = "Lisanslı Yazılım Kullanıyorsunuz. Teşekkür Ederiz.";
                    lblBilgi.ForeColor = System.Drawing.Color.LimeGreen;
                    btnLisansIste.Visible = false;
                }
                else
                {
                    mskLisansAnahtari.Text = "";
                    mskLisansAnahtari.ReadOnly = false;
                    btnEtkinlestir.Enabled = true;
                    lblBilgi.Text = "Bu yazılım lisanslı değil. Etkinleştirmek için lisans anahtarını girin.";
                    lblBilgi.ForeColor = System.Drawing.Color.Red;
                    btnLisansIste.Visible = true;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        bool MailGonder()
        {
            string pass="m-clk-123";
            try
            {
                pass = Pass.GetValue("Pass").ToString();
            }
            catch (Exception)
            {
                Pass.SetValue("Pass", "m-clk-123");
            }


            MClkSifremele(SeriNoAl());
            MailMessage ePosta = new MailMessage();
            ePosta.From = new MailAddress("mclk.yzlm@gmail.com");
            ePosta.To.Add("mclk.yazilim.lisanshavuzu@gmail.com");
            ePosta.Subject = "Lisans İsteği";
            ePosta.Body = "<table style=\"width: 276.4px; \" border=\"1\" cellspacing=\"0\" cellpadding=\"0\"><tbody><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">İşletme Adı</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" + Program.isletmeAdi + "</td></tr><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">E-Mail</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" + Program.email + "</td></tr><tr style=\"height: 17px; \"><td style=\"width: 281.4px; height: 17px; \">Telefon</td><td style=\"width: 281.4px; height: 17px; \">&nbsp;" + Program.telefon + "</td></tr><tr style=\"height: 15.8px; \"><td style=\"width: 281.4px; text - align: center; height: 15.8px; \" colspan=\"2\"><center>" + mclksiz + "<center/></td></tr></tbody></table>";
            ePosta.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new System.Net.NetworkCredential("mclk.yzlm@gmail.com",pass);
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            try
            {
                smtp.SendAsync(ePosta, (object)ePosta);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void ChangeGroupsVisible(bool status)
        {
            grpBirimler.Enabled = status;
            grpGenelYapilandirma.Enabled = status;
            grpIletisim.Enabled = status;
            grpIlkKullanici.Enabled = status;
            grpLisans.Enabled = status;
            grpYaziciYapilandirma.Enabled = status;
            grpLisans.Enabled = status;
            if (status) grpVeritbaniYapilandirma.BackColor = grpLisans.BackColor;
            else
                grpVeritbaniYapilandirma.BackColor = Color.GhostWhite;
        }

        void LoadControls()
        {
            try
            {
                AktiflikKontrol(LisansKontrol());
                if (Program.sql_calisiyor)
                {
                    CBDoldur(cbBirimler, "BIRIMSORGULAMA");
                    IlkKullaniciKontrol();
                    IsletmeAdiGetır();
                }
                foreach (string yazici in PrinterSettings.InstalledPrinters)
                {
                    cbYazici.Items.Add(yazici);
                }
                string yaziciAdi = "";
                try
                {
                    yaziciAdi = Yapilandirma.GetValue("YaziciAdi").ToString();
                }
                catch
                {

                }
                cbOdemeTipi.SelectedIndex = Program.odemeTipi;

                chkDevamliYazdir.Checked = Program.herSatistaYazdir;
                cbKagiTuru.DataSource = Enum.GetValues(typeof(CrystalDecisions.Shared.PaperSize));
                cbKagiTuru.DisplayMember = "Value";
                cbKagiTuru.SelectedItem = Program.kagiTuru;

                if (yaziciAdi != null && yaziciAdi != "")
                    cbYazici.SelectedItem = yaziciAdi;
                else cbYazici.SelectedIndex = 0;
                uygulaKontrol = true;
                cbSunucuTipi.SelectedIndex = 0;
                string[] degerler = SqlConfiguration.GetValueNames();
                if (degerler.Length == 6)
                {
                    txtIP.Text = SqlConfiguration.GetValue("ServerIP").ToString();
                    txtSunucuAdi.Text = SqlConfiguration.GetValue("ServerName").ToString();
                    txtKAdi.Text = SqlConfiguration.GetValue("DBUserName").ToString();
                    txtSifre.Text = SqlConfiguration.GetValue("DBUserPassword").ToString();
                    cbSunucuTipi.SelectedIndex = Convert.ToInt32(SqlConfiguration.GetValue("ServerType"));
                }
                else
                {
                    SqlConfiguration.SetValue("ServerIP", txtIP.Text);
                    SqlConfiguration.SetValue("ServerType", cbSunucuTipi.SelectedIndex);
                    SqlConfiguration.SetValue("ServerName", txtSunucuAdi.Text);
                    SqlConfiguration.SetValue("DBUserName", txtKAdi.Text);
                    SqlConfiguration.SetValue("DBUserPassword", txtSifre.Text);
                    SqlConfiguration.SetValue("SqlConnectString", "Server=" + @SqlConfiguration.GetValue("Server=" + @SqlConfiguration.GetValue("ServerName") + ";DataBase=OtomasyonDB;Trusted_Connection=True;"));
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmConfiguration_Load(object sender, EventArgs e)
        {
            if (Program.sql_calisiyor)
                LoadControls();
            else ChangeGroupsVisible(false);

        }
        
        private void btnUygula_Click(object sender, EventArgs e)
        {

            SqlConfiguration.SetValue("ServerType", cbSunucuTipi.SelectedIndex);
            SqlConfiguration.SetValue("ServerName", txtSunucuAdi.Text);

            SqlConfiguration.SetValue("SqlConnectString", @"Server=" + @SqlConfiguration.GetValue("ServerName") + ";DataBase=OtomasyonDB;Trusted_Connection=True;");
            Program.ConnectionString = "Server=" + @SqlConfiguration.GetValue("ServerName") + ";DataBase=OtomasyonDB;Trusted_Connection=True;";

            if (cbSunucuTipi.SelectedIndex == 1)
            {
                SqlConfiguration.SetValue("ServerIP", txtIP.Text);
                SqlConfiguration.SetValue("ServerType", cbSunucuTipi.SelectedIndex);
                SqlConfiguration.SetValue("DBUserName", txtKAdi.Text);
                SqlConfiguration.SetValue("DBUserPassword", txtSifre.Text);
                SqlConfiguration.SetValue("SqlConnectString", @"Server=" + @txtIP.Text + ";Database=OtomasyonDB;User ID=" + @txtKAdi.Text + ";Password = " + @txtSifre.Text + "; ");
                Program.ConnectionString = @"Server=" + @txtIP.Text + ";Database=OtomasyonDB;User ID=" + @txtKAdi.Text + ";Password = " + @txtSifre.Text + "; ";
            }
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            { 
            nfBasarili.BalloonTipText = "Veritabanı ayarlarınız yapılandırıldı. Veritabanı ile başarılı bir şekilde bağlantı kuruldu.";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(2000);
                Program.sql_calisiyor = true;
        }
            else MessageBox.Show("Veritabanı ayarlarınız yapılandırıldı. Ancak veritabanı ile bağlantı kurulamadı. Bu halde hiçbir işlem yapamazsınız. ", "Uyarı Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void cbSunucuTipi_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSunucuTipi.SelectedIndex == 1) AktifEt();
            else PasifEt();

        }

        private void AktifEt()
        {
            txtSunucuAdi.Enabled = false;
            txtIP.Enabled = true;
            txtKAdi.Enabled = true;
            txtSifre.Enabled = true;
        }

        private void PasifEt()
        {
            txtSunucuAdi.Enabled = true;
            txtIP.Enabled = false;
            txtKAdi.Enabled = false;
            txtSifre.Enabled = false;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
 
            if (txtIsletmeAdi.Text != "")
                {
                    if (DbOperations.BaglantiKontrol(Program.ConnectionString))
                    {
                        SqlCn.OkuScalar("Delete from Isletme; Insert Into Isletme Values ('" + txtIsletmeAdi.Text + "','" + txtTelefon.Text + "','" + txtEmail.Text + "','"+txtAdres.Text+"')", System.Data.CommandType.Text, new SqlParameter[0]);
                    Program.isletmeAdi = txtIsletmeAdi.Text;
                    Program.email = txtEmail.Text;
                    Program.telefon = txtTelefon.Text;
                    Program.adres = txtAdres.Text;
                        nfBasarili.BalloonTipText = "İşletme bilgileri başarılı bir şekilde güncellendi.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);
                    }
                    else
                        MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            {
                if (txtKullaniciSifresi.Text.Length >= 3 && txtKullaniciSifresi.Text.Length >= 6)
                {
                    SqlParameter[] EkleParam = new SqlParameter[3];
                    EkleParam[0] = new SqlParameter();
                    EkleParam[0].ParameterName = "@Adi";
                    EkleParam[0].SqlDbType = System.Data.SqlDbType.NVarChar;
                    EkleParam[0].SqlValue = txtKullaniciAdi.Text;

                    EkleParam[1] = new SqlParameter();
                    EkleParam[1].ParameterName = "@Sifre";
                    EkleParam[1].SqlDbType = System.Data.SqlDbType.NVarChar;
                    EkleParam[1].SqlValue = txtKullaniciSifresi.Text;

                    EkleParam[2] = new SqlParameter();
                    EkleParam[2].ParameterName = "@Yetki";
                    EkleParam[2].SqlDbType = System.Data.SqlDbType.TinyInt;
                    EkleParam[2].SqlValue = 1;
                    if (SqlCn.GuncelleProcedure("KULLANICIEKLE", EkleParam) == 1)
                    {

                        nfBasarili.BalloonTipText = "İşletme adı başarılı bir şekilde güncellendi.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);
                    }
                }
                else MessageBox.Show("Kullanıcı adınız en az 3, şifreniz de en az 6 karakter olmalıdır.", "Karakter Sayısı Az", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnBirimSil_Click(object sender, EventArgs e)
        {
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            {
                if (cbBirimler.Items.Count > 1)
                {
                    DialogResult silmeOnayi = MessageBox.Show("Seçili birimi silmek üzeresiniz. \nDevam etmek istiyor musunuz?", "Silgi Onayı", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (silmeOnayi == DialogResult.Yes)
                    {
                        try
                        {
                            Convert.ToInt32(SqlCn.OkuScalar("Delete from Birimler Where Id=" + cbBirimler.SelectedValue.ToString(), CommandType.Text, new SqlParameter[0]));
                                CBDoldur(cbBirimler, "BIRIMSORGULAMA");
                        
                            nfBasarili.BalloonTipText = "Seçili birim başarılı bir şekilde silindi.";
                            nfBasarili.Visible = true;
                            nfBasarili.ShowBalloonTip(2000);

                        }
                        catch
                        {
                            MessageBox.Show("Seçili birim bazı ürünler için kullanıldığından silinemiyor. Silebilmek için önce bu birimi kullanan bütün ürünleri silmelisiniz.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }
                }
                else
                    MessageBox.Show("En az 1 birim kalana kadar silebilirsiniz.", "Sınır Aşıldı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnBirimEkle_Click(object sender, EventArgs e)
        {
            if (DbOperations.BaglantiKontrol(Program.ConnectionString))
            {
                
                        try
                        {
                    for (int i = 0; i < cbBirimler.Items.Count; i++)
                    {
                        if (txtBirimEkle.Text.ToUpper().Trim() == cbBirimler.Items[i].ToString().ToUpper().Trim())
                        {
                            MessageBox.Show("Eklemek istediğiniz birim zaten var.","Uyarı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                            return;
                        }
                            
                    }
                    Convert.ToInt32(SqlCn.OkuScalar("Insert Into Birimler(Adi,Kisaltma) Values('" + txtBirimEkle.Text.ToUpper() + "','" + txtBirimKisaltma.Text.ToUpper() +"')", CommandType.Text, new SqlParameter[0]));
                            CBDoldur(cbBirimler, "BIRIMSORGULAMA");
                    txtBirimEkle.Text = "";
                    txtBirimKisaltma.Text = "";      
                    nfBasarili.BalloonTipText = "Birim başarılı bir şekilde eklendi.";
                    nfBasarili.Visible = true;
                    nfBasarili.ShowBalloonTip(2000);
                }
                        catch(Exception ex)
                        {

                    txtBirimEkle.Text = "";
                    txtBirimKisaltma.Text = "";
                    MessageBox.Show(ex+"Hatalı veri girişi yapıldı. Lütfen desteklenmeyen karakterleri girmeyin.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
            }
            else
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtBirimEkle_TextChanged(object sender, EventArgs e)
        {
            if (txtBirimEkle.Text.Length >= 3 && txtBirimKisaltma.Text.Trim().Length<=3 && txtBirimKisaltma.Text.Trim().Length>0) btnBirimEkle.Enabled = true;
            else btnBirimEkle.Enabled = false;
        }

        private void cbBirimler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBirimler.Items.Count > 1)
                btnBirimSil.Enabled = true;
            else btnBirimSil.Enabled = false;
        }

        string EksileriYokEt()
        {
            string  mskstring = mskLisansAnahtari.Text.ToUpper();
            mskstring = mskstring.Remove(4, 1);
            mskstring = mskstring.Remove(9, 1);
            mskstring = mskstring.Remove(14, 1);
            mskstring = mskstring.Remove(19, 1);
            return mskstring;
        }

        private void mskLisansAnahtari_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar=Convert.ToChar(e.KeyChar.ToString().ToUpper());
        }

        bool uygulaKontrol = false;

        private void btnEtkinlestir_Click(object sender, EventArgs e)
        {
            SerialKey.SetValue("LicenseCode", EksileriYokEt());
            if(LisansKontrol())
            {
                nfBasarili.BalloonTipText = "Lisans anahtarınız kabul edildi! Barkodlu sistemi hayırlı günlerde kullanmanız dileğiyle...";
                nfBasarili.BalloonTipTitle = "Lisans Onaylandı!";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
                AktiflikKontrol(true);
                Program.lisans = true;

            }
            else
            {
                mskLisansAnahtari.Text = "";
                MessageBox.Show("Lisans anahtarı onaylanmadı. Lütfen tekrar deneyin. Eğer ürün anahtarınız yoksa Muhammed ÇELİK (GSM : 0534 818 31 26) ile iletişime geçin.", "Lisans Onaylanmadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }

        private void btnYaziciUygula_Click(object sender, EventArgs e)
        {
            Yapilandirma.SetValue("HerSatisiYazdir", chkDevamliYazdir.Checked);
            Program.herSatistaYazdir = chkDevamliYazdir.Checked;
            Yapilandirma.SetValue("YaziciAdi", cbYazici.SelectedItem);
            Program.yaziciAdi = cbYazici.SelectedItem.ToString();
            Yapilandirma.SetValue("KagitTuru", (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue);
            Program.kagiTuru = (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue;
            btnYaziciUygula.Enabled = false;
        }

        private void cbYazici_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(uygulaKontrol)
            if (Program.yaziciAdi != cbYazici.SelectedItem.ToString() || Program.kagiTuru!= (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue || chkDevamliYazdir.Checked != Program.herSatistaYazdir)
                btnYaziciUygula.Enabled = true;
            else btnYaziciUygula.Enabled = false;

        }

        private void txtBirimEkle_Enter(object sender, EventArgs e)
        {
            if(txtBirimEkle.Text=="Adı")
            {
                txtBirimEkle.Text = "";
                txtBirimEkle.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(txtBirimKisaltma.Text=="Kısaltma")
            {
                txtBirimKisaltma.Text = "";
                txtBirimKisaltma.ForeColor = System.Drawing.Color.Black;
            }
        }

        private string mclksiz = "";

        private void cbYazici_DrawItem(object sender, DrawItemEventArgs e)
        {
           

        }

        private void btnLisansIste_Click(object sender, EventArgs e)
        {
            if (MailGonder()) { MessageBox.Show("Lisans anahtarı talebiniz iletildi. Geri dönüt bekleyin.","İşlem Başarılı",MessageBoxButtons.OK,MessageBoxIcon.Information); btnLisansIste.Visible = false; }
            else MessageBox.Show("Bir sorun oluştu lütfen internet bağlantınız olduğundan emin olun ve tekrar deneyin. Ya da \"mclk.yzlm@gmail.com\" e-posta adresi aracılığı ile lisans anahtarı talep edin.","Başarısız İşlem",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        private void cbYazici_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cbYazici, cbYazici.SelectedItem.ToString());
        }

        private void cbKagiTuru_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cbKagiTuru,cbKagiTuru.SelectedItem.ToString());
        }

        private void txtBirimEkle_Leave(object sender, EventArgs e)
        {
            if(txtBirimEkle.Text=="")
            {
 txtBirimEkle.ForeColor = System.Drawing.Color.DarkGray;
                txtBirimEkle.Text = "Adı";
               
            }
        }

        private void txtBirimKisaltma_Leave(object sender, EventArgs e)
        {
            if(txtBirimKisaltma.Text=="")
            {
                txtBirimKisaltma.ForeColor = System.Drawing.Color.DarkGray;
                txtBirimKisaltma.Text = "Kısaltma";
               
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.odemeTipi = cbOdemeTipi.SelectedIndex;
            Yapilandirma.SetValue("OdemeTipi", cbOdemeTipi.SelectedIndex);
        }
    }
}

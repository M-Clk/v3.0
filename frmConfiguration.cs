using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmConfiguration : Form
    {
        DbOperations _dbOperations = new DbOperations();
        RegistryKey SerialKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\License");
        RegistryKey _settingsKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon\\Settings");
        RegistryKey Pass = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
        private bool _isDbCreated;
        public frmConfiguration(bool isDbCreated = true)
        {
            InitializeComponent();
            _isDbCreated = isDbCreated;
            btnVeritabaniOlustur.Visible = !isDbCreated;
        }
        void IsletmeAdiGetir()
        {
            if (DbOperations.Connection.IsAvailable())
            {
                SqlDataReader IsletmeOku = _dbOperations.SqlTextReader("SELECT Adi,Telefon,Email,Adres FROM Isletme");
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
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        string Key = "";

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
                Key = SerialKey.GetValue("LicenseCode", "").ToString();
                string result = MClkSifremele(SeriNoAl());
                if (result == Key)
                    return true;
            }
            else
                return false;
            return false;
        }
        void CBDoldur(ComboBox cb, string procString)
        {
            cb.DataSource = _dbOperations.DisconnectedProcedure(procString, new SqlParameter[0]);
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
                sb.Append(ba.ToString("x2").ToUpper());
            string md5li = sb.ToString();
            mclksiz = md5li;
            char[] sonHal = new char[24];  // "MCLKMXXXXCXXXXLXXXXKXXXX"; Kendi şifreleme yöntemim => MCLK-MXXXX-CXXXX-LXXXX-KXXXX  (24)
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
                md5li += sonHal[i];

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
            if (DbOperations.Connection.IsAvailable())
            {
                SqlDataReader dr = _dbOperations.OkuProcedure("KULLANICILARIGETIR", new SqlParameter[0]);
                if (dr.Read())
                {
                    txtKullaniciAdi.Text = dr["Adi"].ToString();
                    txtKullaniciAdi.Enabled = false;

                    txtKullaniciSifresi.Text = dr["Sifre"].ToString();
                    txtKullaniciSifresi.Enabled = false;
                    btnEkle.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        void AktiflikKontrol(bool durum)
        {
            try
            {
                if (durum)
                {
                    mskLisansAnahtari.Text = SerialKey.GetValue("LicenseCode", "").ToString();
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
            string pass = "m-clk-123";
            try
            {
                pass = Pass.GetValue("Pass", "").ToString();
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
            smtp.Credentials = new System.Net.NetworkCredential("mclk.yzlm@gmail.com", pass);
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
            if (status)
                grpVeritbaniYapilandirma.BackColor = grpLisans.BackColor;
            else
                grpVeritbaniYapilandirma.BackColor = Color.GhostWhite;
        }

        void LoadControls()
        {
            try
            {
                AktiflikKontrol(LisansKontrol());
                CBDoldur(cbBirimler, "BIRIMSORGULAMA");
                IlkKullaniciKontrol();
                IsletmeAdiGetir();
                ChangeGroupsVisible(true);
                foreach (string yazici in PrinterSettings.InstalledPrinters)
                {
                    cbYazici.Items.Add(yazici);
                }
                string yaziciAdi = "";
                try
                {
                    yaziciAdi = Program.Yapilandirma.GetValue("YaziciAdi", "").ToString();
                    int count = (int)_settingsKey.GetValue("QuickProductsCount", 10);

                    rdHizliEkranUrunleriEnCokSatilan.Checked = count > 0;
                    comboBox1.SelectedIndex = rdHizliEkranUrunleriEnCokSatilan.Checked ? (count / 5) - 1 : 0;
                    rdHizliEkranUrunleriSadeceSectiklerim.Checked = !rdHizliEkranUrunleriEnCokSatilan.Checked;
                }
                catch
                {

                }
                cbOdemeTipi.SelectedIndex = Program.odemeTipi;

                chkDevamliYazdir.Checked = Program.herSatistaYazdir;
                //cbKagiTuru.DataSource = Enum.GetValues(typeof(CrystalDecisions.Shared.PaperSize));
                cbKagiTuru.DisplayMember = "Value";
                //cbKagiTuru.SelectedItem = Program.kagiTuru;

                if (yaziciAdi != null && yaziciAdi != "")
                    cbYazici.SelectedItem = yaziciAdi;
                else
                    cbYazici.SelectedIndex = 0;
                uygulaKontrol = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void frmConfiguration_Load(object sender, EventArgs e)
        {
            txtServer.Text = (string)DbOperations.SqlConfiguration.GetValue("Server", "");
            txtKAdi.Text = (string)DbOperations.SqlConfiguration.GetValue("DBUserName", "");
            txtSifre.Text = (string)DbOperations.SqlConfiguration.GetValue("DBUserPassword", "");
            if (DbOperations.IsDbCreated && DbOperations.Connection.IsAvailable())
                LoadControls();
            else
                ChangeGroupsVisible(false);

        }

        private void btnUygula_Click(object sender, EventArgs e)
        {                                                                                                                                                                                                           
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MessageBox.Show("Lütfen server adı girin.","Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DbOperations.SetSqlConfiguration(txtServer.Text, txtKAdi.Text, txtSifre.Text);
            if (DbOperations.Connection.IsAvailable())
            {
                nfBasarili.BalloonTipText = "Veritabanı ayarlarınız yapılandırıldı. " + (_isDbCreated ? "Ancak veritabanı henüz oluşmadı." : "Sistemi kullanabilirsiniz.");
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
                if (DbOperations.IsDbCreated)
                {

                }
                btnVeritabaniOlustur.Enabled = !DbOperations.IsDbCreated;
            }
            else
                MessageBox.Show("Veritabanı ayarlarınız yapılandırıldı. Ancak veritabanı ile bağlantı kurulamadı. Bu halde hiçbir işlem yapamazsınız. ", "Uyarı Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void SetEnable(bool enable)
        {
            txtServer.Enabled = enable;
            txtKAdi.Enabled = enable;
            txtSifre.Enabled = enable;
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (DbOperations.Connection.IsAvailable())
            {
                _dbOperations.OkuScalar("Delete from Isletme; Insert Into Isletme Values ('" + txtIsletmeAdi.Text + "','" + txtTelefon.Text + "','" + txtEmail.Text + "','" + txtAdres.Text + "')", System.Data.CommandType.Text, new SqlParameter[0]);
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

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (DbOperations.Connection.IsAvailable())
            {
                if (txtKullaniciSifresi.Text.Length >= 3 && txtKullaniciSifresi.Text.Length >= 6)
                {
                    SqlParameter[] EkleParam = new SqlParameter[3];
                    EkleParam[0] = new SqlParameter();
                    EkleParam[0].ParameterName = "@Adi";
                    EkleParam[0].SqlDbType = SqlDbType.NVarChar;
                    EkleParam[0].SqlValue = txtKullaniciAdi.Text;

                    EkleParam[1] = new SqlParameter();
                    EkleParam[1].ParameterName = "@Sifre";
                    EkleParam[1].SqlDbType = SqlDbType.NVarChar;
                    EkleParam[1].SqlValue = txtKullaniciSifresi.Text;

                    EkleParam[2] = new SqlParameter();
                    EkleParam[2].ParameterName = "@Yetki";
                    EkleParam[2].SqlDbType = SqlDbType.TinyInt;
                    EkleParam[2].SqlValue = 1;
                    if (_dbOperations.GuncelleProcedure("KULLANICIEKLE", EkleParam) == 1)
                    {

                        nfBasarili.BalloonTipText = "İlk kullanıcı başarılı bir şekilde eklendi. " + txtKullaniciAdi.Text + " kullanıcı adı ile sisteme giriş yapabilirsiniz.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);
                    }
                }
                else
                    MessageBox.Show("Kullanıcı adınız en az 3, şifreniz de en az 6 karakter olmalıdır.", "Karakter Sayısı Az", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnBirimSil_Click(object sender, EventArgs e)
        {
            if (DbOperations.Connection.IsAvailable())
            {
                if (cbBirimler.Items.Count > 1)
                {
                    DialogResult silmeOnayi = MessageBox.Show("Seçili birimi silmek üzeresiniz. \nDevam etmek istiyor musunuz?", "Silgi Onayı", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (silmeOnayi == DialogResult.Yes)
                    {
                        try
                        {
                            Convert.ToInt32(_dbOperations.OkuScalar("Delete from Birimler Where Id=" + cbBirimler.SelectedValue.ToString(), CommandType.Text, new SqlParameter[0]));
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
            if (DbOperations.Connection.IsAvailable())
            {

                try
                {
                    for (int i = 0; i < cbBirimler.Items.Count; i++)
                    {
                        if (txtBirimEkle.Text.ToUpper().Trim() == cbBirimler.Items[i].ToString().ToUpper().Trim())
                        {
                            MessageBox.Show("Eklemek istediğiniz birim zaten var.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                    }
                    Convert.ToInt32(_dbOperations.OkuScalar("Insert Into Birimler(Adi,Kisaltma) Values('" + txtBirimEkle.Text.ToUpper() + "','" + txtBirimKisaltma.Text.ToUpper() + "')", CommandType.Text, new SqlParameter[0]));
                    CBDoldur(cbBirimler, "BIRIMSORGULAMA");
                    txtBirimEkle.Text = "";
                    txtBirimKisaltma.Text = "";
                    nfBasarili.BalloonTipText = "Birim başarılı bir şekilde eklendi.";
                    nfBasarili.Visible = true;
                    nfBasarili.ShowBalloonTip(2000);
                }
                catch (Exception ex)
                {

                    txtBirimEkle.Text = "";
                    txtBirimKisaltma.Text = "";
                    MessageBox.Show(ex + "Hatalı veri girişi yapıldı. Lütfen desteklenmeyen karakterleri girmeyin.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Veritabanına bağlantı yapılamadı. Lütfen veritabanı yapılandırmasını doğru yaptığınızdan emin olun.", " Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtBirimEkle_TextChanged(object sender, EventArgs e)
        {
            if (txtBirimEkle.Text.Length >= 3 && txtBirimKisaltma.Text.Trim().Length <= 3 && txtBirimKisaltma.Text.Trim().Length > 0)
                btnBirimEkle.Enabled = true;
            else
                btnBirimEkle.Enabled = false;
        }

        private void cbBirimler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBirimler.Items.Count > 1)
                btnBirimSil.Enabled = true;
            else
                btnBirimSil.Enabled = false;
        }

        string EksileriYokEt()
        {
            string mskstring = mskLisansAnahtari.Text.ToUpper();
            mskstring = mskstring.Remove(4, 1);
            mskstring = mskstring.Remove(9, 1);
            mskstring = mskstring.Remove(14, 1);
            mskstring = mskstring.Remove(19, 1);
            return mskstring;
        }

        private void mskLisansAnahtari_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = Convert.ToChar(e.KeyChar.ToString().ToUpper());
        }

        bool uygulaKontrol = false;

        private void btnEtkinlestir_Click(object sender, EventArgs e)
        {
            SerialKey.SetValue("LicenseCode", EksileriYokEt());
            if (LisansKontrol())
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
                MessageBox.Show("Lisans anahtarı onaylanmadı. Lütfen tekrar deneyin.", "Lisans Onaylanmadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }

        private void btnYaziciUygula_Click(object sender, EventArgs e)
        {
            //Yapilandirma.SetValue("HerSatisiYazdir", chkDevamliYazdir.Checked);
            //Program.herSatistaYazdir = chkDevamliYazdir.Checked;
            //Yapilandirma.SetValue("YaziciAdi", cbYazici.SelectedItem);
            //Program.yaziciAdi = cbYazici.SelectedItem.ToString();
            ////Yapilandirma.SetValue("KagitTuru", (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue);
            ////Program.kagiTuru = (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue;
            //btnYaziciUygula.Enabled = false;
        }

        private void cbYazici_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (uygulaKontrol)
            //    if (Program.yaziciAdi != cbYazici.SelectedItem.ToString() || Program.kagiTuru != (CrystalDecisions.Shared.PaperSize)cbKagiTuru.SelectedValue || chkDevamliYazdir.Checked != Program.herSatistaYazdir)
            //        btnYaziciUygula.Enabled = true;
            //    else
            //        btnYaziciUygula.Enabled = false;

        }

        private void txtBirimEkle_Enter(object sender, EventArgs e)
        {
            if (txtBirimEkle.Text == "Adı")
            {
                txtBirimEkle.Text = "";
                txtBirimEkle.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (txtBirimKisaltma.Text == "Kısaltma")
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
            if (MailGonder())
            { MessageBox.Show("Lisans anahtarı talebiniz iletildi. Geri dönüt bekleyin.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information); btnLisansIste.Visible = false; }
            else
                MessageBox.Show("Bir sorun oluştu lütfen internet bağlantınız olduğundan emin olun ve tekrar deneyin. Ya da \"mclk.yzlm@gmail.com\" e-posta adresi aracılığı ile lisans anahtarı talep edin.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void cbYazici_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(cbYazici, cbYazici.SelectedItem.ToString());
        }

        private void cbKagiTuru_MouseHover(object sender, EventArgs e)
        {
            //toolTip1.SetToolTip(cbKagiTuru, cbKagiTuru.SelectedItem.ToString());
        }

        private void txtBirimEkle_Leave(object sender, EventArgs e)
        {
            if (txtBirimEkle.Text == "")
            {
                txtBirimEkle.ForeColor = System.Drawing.Color.DarkGray;
                txtBirimEkle.Text = "Adı";

            }
        }

        private void txtBirimKisaltma_Leave(object sender, EventArgs e)
        {
            if (txtBirimKisaltma.Text == "")
            {
                txtBirimKisaltma.ForeColor = System.Drawing.Color.DarkGray;
                txtBirimKisaltma.Text = "Kısaltma";

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Program.odemeTipi = cbOdemeTipi.SelectedIndex;
            Program.Yapilandirma.SetValue("OdemeTipi", cbOdemeTipi.SelectedIndex);
        }

        private void frmConfiguration_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.M)
            {
                txtHidden.Focus();
            }
        }

        private void txtHidden_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtHidden.Text.Equals("Clk"))
                grpVeritbaniYapilandirma.Enabled = true;
            else
                grpVeritbaniYapilandirma.Enabled = false;
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            setQuickProductsCount();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            setQuickProductsCount();
            comboBox1.Enabled = rdHizliEkranUrunleriEnCokSatilan.Checked;
        }
        private void setQuickProductsCount()
        {
            Program.quickProductsCount = rdHizliEkranUrunleriEnCokSatilan.Checked ? (comboBox1.SelectedIndex + 1) * 5 : 0;
            _settingsKey.SetValue("QuickProductsCount", Program.quickProductsCount);
        }

        private void btnVeritabaniOlustur_Click(object sender, EventArgs e)
        {
            var failedCommands = _dbOperations.CreateDb();
            if (!failedCommands.Any())
            {
                MessageBox.Show("TÜm komutlar çaşıştırıldı.", "Veritabanı Başarıyla Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnVeritabaniOlustur.Visible = false;
                LoadControls();
                return;
            }

            var commandsStr = string.Join("\n", failedCommands);
            Clipboard.SetText(commandsStr);
            MessageBox.Show("Hatalı komutlar ponoya kopyalandı. Başarısız Komutlar :\n" + commandsStr, "Veritabanı Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

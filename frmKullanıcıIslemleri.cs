using Microsoft.Win32;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmKullanıcıIslemleri : Form
    {
        public frmKullanıcıIslemleri()
        {
            InitializeComponent();
            if (Program.yetki == Program.Yetki.yonetici)
            {
                gbGuncelleSil.Visible = true;
                gbKullanicilar.Visible = true;
            }
            else
            {
                tableLayoutPanel1.Dock = DockStyle.None;
                groupBox1.Dock = DockStyle.None;
                tableLayoutPanel2.Dock = DockStyle.None;
                tableLayoutPanel3.Dock = DockStyle.None;
                tableLayoutPanel4.Dock = DockStyle.None;
                gbKullanicilar.Dock = DockStyle.None;
                gbGuncelleSil.Dock = DockStyle.None;
                this.Width = 425;
                this.Height = 290;
            }
        }
        DbOperations SqlOp = new DbOperations();
        string ilkKAdi="";
        private void frmKullanıcıIslemleri_Load(object sender, EventArgs e)
        {
            cbYetki.SelectedIndex = 1;
            KullanicilariGuruntule();
        }
        DataGridViewButtonColumn dgvBtn;
        bool uyariVer = true;
        void KullanicilariGuruntule()
        {
            lblK_Adi.Text = Program.k_adi;
            if (Program.yetki == Program.Yetki.eleman) return; 
            if (dgvBtn!=null)
            dgKullanicilar.Columns.Remove(dgvBtn);
            cbSifreIste.Visible = true;
            cbSifreIste.Checked = Convert.ToBoolean(sifreIste.GetValue("SifreIste", true));
            dgKullanicilar.Rows.Clear();
            SqlDataReader KullaniciOku = SqlOp.OkuProcedure("KULLANICILARIGETIR",  new SqlParameter[0]);
            while (KullaniciOku.Read())
            {
                    string[] rows = new string[dgKullanicilar.ColumnCount];
                    rows[0] = (dgKullanicilar.RowCount + 1).ToString();
                    rows[1] = KullaniciOku[0].ToString();
                    rows[2] = cbYetki.Items[Convert.ToInt16(KullaniciOku[1])].ToString();
                    rows[3] = KullaniciOku[2].ToString();
                    dgKullanicilar.Rows.Add(rows);
            }
            dgvBtn = new DataGridViewButtonColumn();
            //Kolon Başlığı
            dgvBtn.HeaderText = "Silgi";
            // Butonun Text
            dgvBtn.Text = "Sil";
            // Butonda Text Kullanılmasını aktifleştirme
            dgvBtn.UseColumnTextForButtonValue = true;
            //Kendine göre değştirme
            dgvBtn.FlatStyle = FlatStyle.Flat;
            dgvBtn.DefaultCellStyle.BackColor = Color.LightSkyBlue;
            // Buton seçiliykenki çerçeve rengi
            dgvBtn.DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            // Butonun genişiliği

            dgvBtn.Width = 45;
            // DataGridView e ekleme
            dgKullanicilar.Columns.Add(dgvBtn);
            dgKullanicilar.Columns[4].Width = 45;
        }
        bool KarakterSayisiKontrol(TextBox txt, int sinir)
        {
            if (txt.TextLength >= sinir) return true;
            else return false;
        }
        bool KarakterKontrol(TextBox txt)
        {
            if (txt.Text.IndexOf("'") ==-1) return true;
            else return false;
        }

        private void cbSifreGoster1_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSifreGoster1.Checked == true)
            {
                txtEski.PasswordChar = '\0';
                txtYeni.PasswordChar = '\0';
            }
            else
            {
                txtYeni.PasswordChar = '•';
                txtEski.PasswordChar = '•';
            }
        }

        private void cbSifreGoster2_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSifreGoster2.Checked == true) txt_Sifre.PasswordChar = '\0';
            else txt_Sifre.PasswordChar = '•';
        }

        private void dgKullanicilar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex>-1)
            {
                txtK_Ad.Text = dgKullanicilar.Rows[e.RowIndex].Cells[1].Value.ToString();
                ilkKAdi = dgKullanicilar.Rows[e.RowIndex].Cells[1].Value.ToString();
                txt_Sifre.Text = dgKullanicilar.Rows[e.RowIndex].Cells[3].Value.ToString();
                cbYetki.Text = dgKullanicilar.Rows[e.RowIndex].Cells[2].Value.ToString();

            }
        }

        private void dgKullanicilar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex>-1 && e.ColumnIndex==4)
            {
                if(dgKullanicilar.Rows[e.RowIndex].Cells[1].Value.ToString()==lblK_Adi.Text)
                {
                    MessageBox.Show("Kendinizi silemezsiniz. Ancak kullanıcı adınız dahil bilgilerinizi güncelleyebilirsiniz. Güncellemek için kullanıcı adınıza çift tıklayın. ", "Yetkisiz İşlem Girişimi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SqlParameter[] prmtr = new SqlParameter[1];
                prmtr[0] = new SqlParameter();
                prmtr[0].ParameterName = "@KullaniciAdi";
                prmtr[0].SqlDbType = SqlDbType.NVarChar;
                prmtr[0].SqlValue = dgKullanicilar.Rows[e.RowIndex].Cells[1].Value;
                SqlOp.GuncelleProcedure("KULLANICISIL",prmtr);
                dgKullanicilar.Rows.Remove(dgKullanicilar.Rows[e.RowIndex]);
                KullanicilariGuruntule();

            }
        }
        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }
        private void dgKullanicilar_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.Value != null)
            {
                e.Value = new String('•', e.Value.ToString().Length);
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (!IlkKontroller()) return;
            SqlParameter[] param = new SqlParameter[3];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@Adi";
            param[0].SqlDbType = SqlDbType.NVarChar;
            param[0].SqlValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtK_Ad.Text);
            param[1] = new SqlParameter();
            param[1].ParameterName = "@Sifre";
            param[1].SqlDbType = SqlDbType.NVarChar;
            param[1].SqlValue = txt_Sifre.Text;
            param[2] = new SqlParameter();
            param[2].ParameterName = "@Yetki";
            param[2].SqlDbType = SqlDbType.TinyInt;
            param[2].SqlValue = cbYetki.SelectedIndex;
            
            if (SqlOp.GuncelleProcedure("KULLANICIEKLE", param) != 0)
            {
                KullanicilariGuruntule();
                nfBasarili.BalloonTipText = "Kullanıcı başarılı bir şekilde eklendi.";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
                txtK_Ad.Text = "";
                txt_Sifre.Text = "";
                cbYetki.SelectedIndex = 1;
            }
        }
        bool IlkKontroller()
        {
            if (!KarakterKontrol(txtK_Ad) || !KarakterKontrol(txt_Sifre))
            {
                MessageBox.Show("Geçersiz karakter kullanıldı.", "Karakter Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!KarakterSayisiKontrol(txt_Sifre, 6))
            {
                MessageBox.Show("Şifre en az 6 karakter olmalıdır.", "Karakter Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!KarakterSayisiKontrol(txtK_Ad, 3))
            {
                MessageBox.Show("Kullanıcı adı en az 3 karakter olmalıdır.", "Karakter Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void btnKGuncelle_Click(object sender, EventArgs e)
        {
            bool bayrak = false;
            if (!IlkKontroller()) return;
            if (ilkKAdi == "" || ilkKAdi==lblK_Adi.Text)
            {
                
                DialogResult soru = MessageBox.Show("Kendi kullanıcı bilgilerinizi değiştirmeye çalışıyorsunuz. Eğer yetkinizi değiştirdiyseniz çıkış yapılacak, artık kullanıcıları ve müşteri işlemlerini yönetemeyeceksiniz. Devam edilsin mi?", "Kritik İşlem", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (soru == DialogResult.Yes)
                {
                    ilkKAdi = lblK_Adi.Text;
                    bayrak = true; //Eğer kendi bilgilerini değiştiriyorsa oturum bilgilerini de değiştirmek için flag kullan
                }
                else return;
            }
            
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@Adi";
            param[0].SqlDbType = SqlDbType.NVarChar;
            param[0].SqlValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtK_Ad.Text);
            param[1] = new SqlParameter();
            param[1].ParameterName = "@Sifre";
            param[1].SqlDbType = SqlDbType.NVarChar;
            param[1].SqlValue = txt_Sifre.Text;
            param[2] = new SqlParameter();
            param[2].ParameterName = "@Yetki";
            param[2].SqlDbType = SqlDbType.TinyInt;
            param[2].SqlValue = cbYetki.SelectedIndex;
            param[3] = new SqlParameter();
            param[3].ParameterName = "@IlkAdi";
            param[3].SqlDbType = SqlDbType.NVarChar;
            param[3].SqlValue = ilkKAdi;
            SqlOp.OkuProcedure("KULLANICIGUNCELLE", param).Read();
            if (SqlOp.bilgiMessage == "Aynı")
                MessageBox.Show("Girdiğiniz kullanıcı adına sahip bir kullanıcı zaten var. Lütfen tekrar deneyin.", "Kullanıcı Adı Çakışması", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (bayrak)
                {
                    lblK_Adi.Text = txtK_Ad.Text;
                    Program.k_adi = txtK_Ad.Text;
                    Program.sifre = txt_Sifre.Text;
                    if(sifreIste.GetValue("SifreIste") != null && (int)sifreIste.GetValue("SifreIste") == 0)
                    {
                        sifreIste.SetValue("KayitliK_Adi", txtK_Ad.Text);
                        sifreIste.SetValue("KayitliK_Sifre", txt_Sifre.Text);
                    }
                    Program.yetki = (Program.Yetki)cbYetki.SelectedIndex; //0. index = Diğer   , 1. index = Yönetici
                    if (cbYetki.SelectedIndex == 0)
                    {
                        sifreIste.SetValue("SifreIste", 1);
                        sifreIste.SetValue("KayitliK_Adi", "");
                        sifreIste.SetValue("KayitliK_Sifre", "");
                        MessageBox.Show("Yetkinizi değiştirdiniz. Artık yönetici değilsiniz. Başka kullanıcılara müdahale etme yetkiniz elinizden alındığı için kullanıcı işlemlerinden çıkılıyor.", "Çıkış Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.Close();
                        return;
                    }
                }
                KullanicilariGuruntule();
                nfBasarili.BalloonTipText = "Kullanıcı başarılı bir şekilde eklendi.";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
                txtK_Ad.Text = "";
                txt_Sifre.Text = "";
                cbYetki.SelectedIndex = 1;
            }
             
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {

            if (!KarakterKontrol(txtEski) || !KarakterKontrol(txtYeni))
            {
                MessageBox.Show("Geçersiz karakter kullanıldı.", "Karakter Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!KarakterSayisiKontrol(txtEski, 6) || !KarakterSayisiKontrol(txtYeni, 6))
            {
                MessageBox.Show("Şifre en az 6 karakter olmalıdır.", "Karakter Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlParameter[] prmtr = new SqlParameter[3];
            prmtr[0] = new SqlParameter();
            prmtr[0].ParameterName = "@Adi";
            prmtr[0].SqlDbType = SqlDbType.NVarChar;
            prmtr[0].SqlValue = lblK_Adi.Text;
            prmtr[1] = new SqlParameter();
            prmtr[1].ParameterName = "@EskiSifre";
            prmtr[1].SqlDbType = SqlDbType.NVarChar;
            prmtr[1].SqlValue = txtEski.Text;
            prmtr[2] = new SqlParameter();
            prmtr[2].ParameterName = "@YeniSifre";
            prmtr[2].SqlDbType = SqlDbType.NVarChar;
            prmtr[2].SqlValue = txtYeni.Text;

            SqlDataReader dr = SqlOp.OkuProcedure("KULLANICISIFRESI", prmtr);
            if (!dr.Read())
            {

                MessageBox.Show("Eski şifrenizi yanlış girdiniz. Lütfen tekrar deneyin.", "Şifre Uyuşmazlığı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                Program.sifre = txtYeni.Text;
                KullanicilariGuruntule();
                nfBasarili.BalloonTipText = "Şifreniz başarılı bir şekilde güncellendi.";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
            }
            txtEski.Text = "";
            txtYeni.Text = "";
        }
        RegistryKey sifreIste = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Otomasyon");
        private void chbSifreIste_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbSifreIste.Checked) 
           
            {
                cbSifreIste.ForeColor = Color.Red;
                sifreIste.SetValue("SifreIste",0);
                sifreIste.SetValue("KayitliK_Adi", Program.k_adi);
                sifreIste.SetValue("KayitliK_Sifre",Program.sifre);
                if(uyariVer)
                    MessageBox.Show("Artık şifre giriş ekranı açılmayacak. Ancak bunun pek güvenli bir seçenek olmadığını hatırlatmak isteriz. Tüm işlemler sizin kullanıcı adınız ile yapılacak. Siz ve sizin dışınızda yönetici yetkisine sahip herhangi bir kullanıcının bu seçeneği değiştirebileceğini unutmayın.","Güvenli Olmayan Seçenek Uyarısı",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                uyariVer = false;
                
            }
            else
            {
                cbSifreIste.ForeColor = Color.Green;
                sifreIste.SetValue("SifreIste", 1);
                sifreIste.SetValue("KayitliK_Adi", "");
                sifreIste.SetValue("KayitliK_Sifre", "");
            }
        }
    }
}

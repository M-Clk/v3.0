using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Threading;
using System.Globalization;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;

namespace Otomasyon
{
    public partial class frmGelirGider : Form
    {
        DataGridViewButtonColumn dgvBtn;
        OrtakIslemler islemYap = new OrtakIslemler();
        public frmGelirGider()
        {
            InitializeComponent();
        }
        DbOperations OpDb = new DbOperations();

        private void cbNumaraSor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbNumaraSor.Checked) txtSatisId.Enabled = true;
            else txtSatisId.Enabled = false;
        }
        void IadEt()
        {
            int satisId = -1;
            decimal miktar;
            if (cbNumaraSor.Checked)
            {
                try
                {
                    satisId = Convert.ToInt32(txtSatisId.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Lütfen sayısal bir değer girin. Elinizde satış numarası yoksa belirtiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                miktar = Convert.ToDecimal(cbMiktar.Text);
                if (miktar <= 0)
                {
                    MessageBox.Show("Miktar 0 veya negatif olamaz. Lütfen pozitif değer giriniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen sayısal bir değer girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SqlParameter[] IadeParam = new SqlParameter[3];
            IadeParam[0] = new SqlParameter();
            IadeParam[0].ParameterName = "@BarkodKodu";
            IadeParam[0].SqlDbType = SqlDbType.NVarChar;
            IadeParam[0].SqlValue = txtBarkodKodu.Text;

            IadeParam[1] = new SqlParameter();
            IadeParam[1].ParameterName = "@SatisId";
            IadeParam[1].SqlDbType = SqlDbType.Int;
            IadeParam[1].SqlValue = satisId;

            IadeParam[2] = new SqlParameter();
            IadeParam[2].ParameterName = "@Miktar";
            IadeParam[2].SqlDbType = SqlDbType.Decimal;
            IadeParam[2].SqlValue = miktar;

            try
            {

                if (!Program.lisans)
                {
                    int iadeSayisi = Convert.ToInt32(OpDb.ScalarTextCommand("Select COUNT(Barkod_Kodu) From Satis_Detayi Where Iade=1"));
                    if (iadeSayisi < 10)
                    {
                        islemYap.LisansUyarisi("Lisanssız yazılım kullanıyorsunuz. En fazla 10 ürün iade edebilirsiniz. " + (9 - iadeSayisi) + " hakkınız kaldı. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.");
                    }
                    else
                    {
                        islemYap.LisansUyarisiMesaj("Lisanssız yazılım kullanıyorsunuz. Maksimum ürün iade etme limitinize ulaştınız. Artık satış yapamazsınız. Eğer ürün anahtarınız varsa limitsiz kullanım için etkinleştirin.");
                        return;
                    }
                }
                int sonuc = OpDb.GuncelleProcedure("URUNIADET", IadeParam);
                decimal satisDus = 0;
                bool mesajVer = false;
                try { satisDus = Convert.ToDecimal(OpDb.bilgiMessage); }
                catch { mesajVer = true; }

                if (sonuc == 1)
                {
                    if (!cbNumaraSor.Checked)
                    {
                        SqlParameter[] IslemKaydetParam = new SqlParameter[3];
                        IslemKaydetParam[0] = new SqlParameter();
                        IslemKaydetParam[0].ParameterName = "@Adi";
                        IslemKaydetParam[0].SqlDbType = SqlDbType.NVarChar;
                        IslemKaydetParam[0].SqlValue = "Ürün iade edildi. Barkod Kodu : " + txtBarkodKodu.Text + " Miktarı : " + cbMiktar.Text;

                        IslemKaydetParam[2] = new SqlParameter();
                        IslemKaydetParam[2].ParameterName = "@KasiyerId";
                        IslemKaydetParam[2].SqlDbType = SqlDbType.Int;
                        IslemKaydetParam[2].SqlValue = Program.k_id;

                        IslemKaydetParam[1] = new SqlParameter();
                        IslemKaydetParam[1].ParameterName = "@Tutar";
                        IslemKaydetParam[1].SqlDbType = SqlDbType.Decimal;
                        IslemKaydetParam[1].Value = satisDus / 100;
                        OpDb.GuncelleProcedure("ISLEMEKLE", IslemKaydetParam);

                        IslemleriGetir();
                    }
                    if (mesajVer)
                        MessageBox.Show(OpDb.bilgiMessage, "Başarılı İşlem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    cbMiktar.SelectedIndex = 0;
                    txtSatisId.Text = "";
                    cbNumaraSor.Checked = false;
                }
                else if (sonuc == -1)
                {

                    MessageBox.Show(OpDb.bilgiMessage, "İade edilemedi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            catch (Exception)
            {
                return;
            }
            finally { txtBarkodKodu.Text = ""; }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            IadEt();
        }
        decimal topKar = 0, topTutar = 0;
        void IslemleriGetir()
        {

            dgIslemler.Rows.Clear();
            if (dgvBtn != null)
                dgIslemler.Columns.Remove(dgvBtn);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); //Sql sorgusunda datetime formatı ile aynı olması için kültür formatını ingiliz formatına çevir.
            DateTime parameterBaslangicTarihi = DateTime.Now, parameterBitisTarihi = DateTime.Now;
            if (cbSecenek.SelectedIndex == 1)
            {
                //dtBaslangicTarihi değeri geldiğinde geldiği zamanın saati ile gelecektir. Bu da tüm kayıtların gelmemesine sebep olabilir. Bunu engellemek sadece tarihi(.Date) almak gerek. Saat de otomatik olarak 00:00:00 olacaktır.
                parameterBaslangicTarihi = dtBaslangicTarihi.Value.Date;

            }
            else if (cbSecenek.SelectedIndex == 2)
            {
                //Yukarıdaki işlem tekrarlandı.
                parameterBaslangicTarihi = dtBaslangicTarihi.Value.Date;

                parameterBitisTarihi = dtBitisTarihi.Value.Date;
            }

            SqlParameter[] FilterParameter = new SqlParameter[3];
            FilterParameter[0] = new SqlParameter();
            FilterParameter[0].ParameterName = "@SorguTipi";
            FilterParameter[0].SqlDbType = SqlDbType.TinyInt;
            FilterParameter[0].SqlValue = cbSecenek.SelectedIndex;
            FilterParameter[1] = new SqlParameter();
            FilterParameter[1].ParameterName = "@BaslangicTarihi";
            FilterParameter[1].SqlDbType = SqlDbType.DateTime;
            FilterParameter[1].SqlValue = parameterBaslangicTarihi;
            FilterParameter[2] = new SqlParameter();
            FilterParameter[2].ParameterName = "@BitisTarihi";
            FilterParameter[2].SqlDbType = SqlDbType.DateTime;
            FilterParameter[2].SqlValue = parameterBitisTarihi;
            SqlDataReader FiltreOkuyucu = OpDb.OkuProcedure("GELIRGIDERGETIR", FilterParameter); Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR"); //Sorgu bittiğinde sonucu tekrar aynı formatta almak için türk formatına geri çevir.
            topKar = 0; topTutar = 0;
            string[] satir = new string[dgIslemler.ColumnCount];
            while (FiltreOkuyucu.Read())
            {
                satir[0] = FiltreOkuyucu[0].ToString();
                satir[1] = FiltreOkuyucu[1].ToString();
                satir[2] = FiltreOkuyucu[2].ToString();
                satir[3] = FiltreOkuyucu[3].ToString();
                satir[4] = FiltreOkuyucu[4].ToString();

                dgIslemler.Rows.Add(satir);
                decimal Tutar = Convert.ToDecimal(FiltreOkuyucu[3]);
                if (Tutar > 0)
                {
                    topTutar += Tutar;
                    dgIslemler.Rows[dgIslemler.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (Tutar < 0)
                {
                    topKar += Tutar * (-1);
                    dgIslemler.Rows[dgIslemler.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightPink;
                }
            }

            dgvBtn = new DataGridViewButtonColumn();
            //Kolon Başlığı
            dgvBtn.HeaderText = "Silgi";
            // Butonun Text
            dgvBtn.Text = "Sil";
            // Butonda Text Kullanılmasını aktifleştirme
            dgvBtn.UseColumnTextForButtonValue = true;
            //Kendine göre değştirme
            dgvBtn.FlatStyle = FlatStyle.Popup;

            // Buton çerçeve rengi
            dgvBtn.DefaultCellStyle.BackColor = Color.DeepSkyBlue;
            // Buton seçiliykenki çerçeve rengi
            //dgvBtn.DefaultCellStyle.SelectionBackColor = Color.PaleTurquoise;
            dgvBtn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            // Butonun genişiliği
            dgvBtn.Width = 45;
            dgIslemler.Columns.Add(dgvBtn);
            dgIslemler.Columns[5].DefaultCellStyle.BackColor = Color.DeepSkyBlue;
            dgvBtn.Dispose();
            lblTopTutar.Text = "Toplam Gelir Tutarı : " + topTutar.ToString() + " ₺";
            lblKar.Text = "Toplam Gider Tutarı : " + topKar.ToString() + " ₺";
        }
        private void cbSecenek_SelectedIndexChanged(object sender, EventArgs e)
        {

            dtBaslangicTarihi.Visible = false;
            dtBitisTarihi.Visible = false;
            lblTire.Visible = false;

            if (cbSecenek.SelectedIndex == 1)
            {
                dtBaslangicTarihi.Visible = true;

            }
            else if (cbSecenek.SelectedIndex == 2)
            {
                dtBaslangicTarihi.Visible = true;
                dtBitisTarihi.Visible = true;
                lblTire.Visible = true;
                if (dtBaslangicTarihi.Value.Day == dtBitisTarihi.Value.Day)
                    dtBaslangicTarihi.Value = dtBaslangicTarihi.Value.AddDays(-1);
            }
            IslemleriGetir();
        }
        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }
        private void frmGelirGider_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                cbMiktar.Items.Add(i);
            }

            cbMiktar.SelectedIndex = 0;
            cbSecenek.SelectedIndex = 0;
            cbIslemTutari.SelectedIndex = 0;
            lblMaxKarakter.Text = txtIslemAdi.TextLength.ToString();
            if (dgIslemler.RowCount > 0) btnExcel.Enabled = true;
            else btnExcel.Enabled = false;
        }
        bool a = true;
        private void cbMiktar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbMiktar.Text.IndexOf(',') == -1)
            {
                a = true;
            }
            if (e.KeyChar == (char)44 && a == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                a = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void txtBarkodKodu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) txtBarkodKodu.Text = "";
            if (e.KeyCode == Keys.Enter) IadEt();
        }

        private void dtBaslangicTarihi_ValueChanged(object sender, EventArgs e)
        {
            if (dtBaslangicTarihi.Visible) IslemleriGetir();
        }

        private void dtBitisTarihi_ValueChanged(object sender, EventArgs e)
        {
            if (dtBitisTarihi.Visible) IslemleriGetir();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            decimal islemTutar;
            try
            {
                islemTutar = Convert.ToDecimal(txtTutar.Text);
                if (islemTutar <= 0)
                {
                    MessageBox.Show("Lütfen işlem tutarına 0 veya negatif bir değer girmeyin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen ürün tutarına sayısal bir değer girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtIslemAdi.Text.Length < 3)
            {
                MessageBox.Show("İşlem adı en az 3 karakterli olmalıdır.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SqlParameter[] IslemEklePrm = new SqlParameter[3];
            IslemEklePrm[0] = new SqlParameter();
            IslemEklePrm[0].ParameterName = "@Adi";
            IslemEklePrm[0].SqlDbType = SqlDbType.NVarChar;
            IslemEklePrm[0].Value = txtIslemAdi.Text;

            IslemEklePrm[1] = new SqlParameter();
            IslemEklePrm[1].ParameterName = "@KasiyerId";
            IslemEklePrm[1].SqlDbType = SqlDbType.Int;
            IslemEklePrm[1].SqlValue = Program.k_id;

            IslemEklePrm[2] = new SqlParameter();
            IslemEklePrm[2].ParameterName = "@Tutar";
            IslemEklePrm[2].SqlDbType = SqlDbType.Decimal;
            if (cbIslemTutari.SelectedIndex == 1) islemTutar = -islemTutar;
            IslemEklePrm[2].SqlValue = islemTutar;
            try
            {
                if (!Program.lisans)
                {
                    int islemSayisi = Convert.ToInt32(OpDb.ScalarTextCommand("Select COUNT(Id) From Gelir_gider"));
                    if (islemSayisi < 10)
                    {
                        islemYap.LisansUyarisi("Lisanssız yazılım kullanıyorsunuz. En fazla 10 işlem kaydedebilirsiniz. " + (9 - islemSayisi) + " hakkınız kaldı. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.");
                    }
                    else
                    {
                        islemYap.LisansUyarisiMesaj("Lisanssız yazılım kullanıyorsunuz. Maksimum işlem kayıt limitinize ulaştınız. Artık işlem kaydedemezsiniz. Eğer ürün anahtarınız varsa limitsiz kullanım için etkinleştirin.");
                        return;
                    }

                }
                OpDb.GuncelleProcedure("ISLEMEKLE", IslemEklePrm);
                nfBasarili.BalloonTipText = "İşlem başarılı bir şekilde eklendi.";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
                IslemleriGetir();
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                txtIslemAdi.Text = "";
                txtTutar.Text = "";
                cbIslemTutari.SelectedIndex = 0;

            }

        }

        private void txtIslemAdi_TextChanged(object sender, EventArgs e)
        {
            lblMaxKarakter.Text = txtIslemAdi.TextLength.ToString();
            if (txtIslemAdi.TextLength < 25) lblMaxKarakter.ForeColor = Color.Lime;
            else if (txtIslemAdi.TextLength < 50) lblMaxKarakter.ForeColor = Color.ForestGreen;
            else if (txtIslemAdi.TextLength < 75) lblMaxKarakter.ForeColor = Color.Goldenrod;
            else if (txtIslemAdi.TextLength < 100) lblMaxKarakter.ForeColor = Color.DeepPink;
            else lblMaxKarakter.ForeColor = Color.Red;

        }
        bool b = true;
        private void txtTutar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtTutar.Text.IndexOf(',') == -1)
            {
                b = true;
            }
            if (e.KeyChar == (char)44 && b == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                b = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void dgIslemler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                DialogResult silmeOnayi = MessageBox.Show(" Bu işlemi silmek üzeresiniz. \nDevam etmek istiyor musunuz?", "Silgi Onayı", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (silmeOnayi == DialogResult.Yes)
                {
                    string a = dgIslemler.Rows[e.RowIndex].Cells[1].Value.ToString(), kod = "";
                    decimal miktar = -1 ;
                    if (a.IndexOf("Barkod Kodu :") > -1)
                    {
                        kod = a.Remove(a.IndexOf(" Miktarı : "), a.Length - a.IndexOf(" Miktarı :"));
                       kod= kod.Remove(0, "Ürün iade edildi. Barkod Kodu : ".Length); //Barkod Kodu
                    miktar = Convert.ToInt32(a.Remove(0, a.LastIndexOf(" Miktarı : ")+11)); // Miktar
                    }

                    SqlParameter[] silgiParameter = new SqlParameter[4];
                    silgiParameter[0] = new SqlParameter();
                    silgiParameter[0].ParameterName = "@Id";
                    silgiParameter[0].SqlDbType = SqlDbType.Int;
                    silgiParameter[0].SqlValue = dgIslemler.Rows[e.RowIndex].Cells[4].Value;

                    silgiParameter[1] = new SqlParameter();
                    silgiParameter[1].ParameterName = "@KasiyerId";
                    silgiParameter[1].SqlDbType = SqlDbType.Int;
                    silgiParameter[1].SqlValue = Program.k_id;

                    silgiParameter[2] = new SqlParameter();
                    silgiParameter[2].ParameterName = "@BarkodKodu";
                    silgiParameter[2].SqlDbType = SqlDbType.NVarChar;
                    silgiParameter[2].SqlValue = kod;

                    silgiParameter[3] = new SqlParameter();
                    silgiParameter[3].ParameterName = "@Miktar";
                    silgiParameter[3].SqlDbType = SqlDbType.Decimal;
                    silgiParameter[3].SqlValue = miktar;

                    if (OpDb.GuncelleProcedure("ISLEMSIL", silgiParameter) == 1)
                    {
                        IslemleriGetir();
                        nfBasarili.BalloonTipText = "Seçili işlem başarılı bir şekilde silindi.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);
                        if (dgIslemler.RowCount > 0) btnExcel.Enabled = true;
                        else btnExcel.Enabled = false;
                    }
                    else MessageBox.Show("Seçili işlem silinemedi. Başka kullanıcı tarafından silinmiş ya da veritabanında yetkisiz erişim olabilir.", "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExceleAktar();
        }
        struct excelTabloButunlugu
        {
            public string ilkSutun;
            public string sonSutun;
            public int ilkSatir;
            public int sonSatir;
            public int baslikSatiri;
            public int tabloBasligiSatiri;
        }
        struct altBilgiButunlugu
        {
            public string ilkSutun;
            public string sonSutun;
            public int ilkSatir;
        }
        private ExcelPackage _package;
        void ExceleAktar()
        {
            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Gelir-Gider_Listesi(" + DateTime.Now.ToShortDateString() + ")";
            DialogResult dialogResult = saveExceleKaydet.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (File.Exists(saveExceleKaydet.FileName))
                {
                    try
                    {
                        FileStream fs = File.Open(saveExceleKaydet.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                        fs.Close();
                        fs.Dispose();
                    }
                    catch
                    {
                        MessageBox.Show("Değiştirmek istediğiniz dosya şu anda başka bir uygulama tarafından kullanılıyor. Lütfen başka bir uygulama tarafından kullanılmadığından emin olun. Ya da farklı bir isimde kaydetmeyi deneyin.", "Dosya Meşgul", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                excelTabloButunlugu tablo = new excelTabloButunlugu();
                tablo.baslikSatiri = 1;
                tablo.ilkSatir = 4;
                tablo.sonSatir = 5;
                tablo.tabloBasligiSatiri = 3;
                tablo.ilkSutun = "A";
                tablo.sonSutun = "D";
                altBilgiButunlugu altBilgi = new altBilgiButunlugu();
                altBilgi.ilkSatir = 6;
                altBilgi.ilkSutun = "C";
                altBilgi.sonSutun = "D";


                _package = new ExcelPackage(new MemoryStream());
                ExcelWorksheet ws1 = _package.Workbook.Worksheets.Add("Gelir-Gider Listesi");
                if (cbSecenek.SelectedIndex == 0)
                    ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString()].Value = Program.isletmeAdi + " Tüm Gelir-Gider Listesi";
                else if (cbSecenek.SelectedIndex == 1)
                    ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString()].Value = Program.isletmeAdi + dtBaslangicTarihi.Value.ToShortDateString() + " Tarihli Gelir -Gider Listesi";
                else
                    ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString()].Value = Program.isletmeAdi + " (" + dtBaslangicTarihi.Value.ToShortDateString() + " - " + dtBitisTarihi.Value.ToShortDateString() + ") Tarihli Gelir -Gider Listesi";

                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Merge = true;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Font.Bold = true;
                ws1.Cells[tablo.ilkSutun + tablo.tabloBasligiSatiri.ToString() + ":" + tablo.sonSutun + tablo.tabloBasligiSatiri.ToString()].Style.Font.Bold = true;
                for (int i = 0; i < 4; i++)
                {
                    ws1.Cells[tablo.tabloBasligiSatiri, i + 1].Value = dgIslemler.Columns[i].HeaderText;
                }
                for (var kolon = 0; kolon < 4; kolon++)
                {

                    for (var satir = 0; satir < dgIslemler.RowCount; satir++)
                    {
                        if (kolon == 0) ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToInt32(dgIslemler.Rows[satir].Cells[kolon].Value);
                        else if (kolon == 3)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToDecimal(dgIslemler.Rows[satir].Cells[kolon].Value);
                            if (Convert.ToDecimal(dgIslemler.Rows[satir].Cells[kolon].Value) > 0)
                            {
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.BackgroundColor.SetColor(acikYesil);
                            }
                            else
                            {
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.BackgroundColor.SetColor(acikKirmizi);
                            }
                        }

                        else
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = dgIslemler.Rows[satir].Cells[kolon].Value;
                        tablo.sonSatir = satir + tablo.ilkSatir;

                    }
                    altBilgi.ilkSatir = tablo.sonSatir + 2;
                    if (kolon == 3)
                    {
                        ws1.Cells["D" + tablo.ilkSatir + ":D" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";

                    }

                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;

                    ws1.Column(kolon + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws1.Column(kolon + 1).AutoFit();
                }

                ws1.Cells["B" + tablo.ilkSatir + ":" + "B" + tablo.sonSatir].Style.WrapText = true;

                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Value = "Toplam Gelir";
                ws1.Cells["D" + altBilgi.ilkSatir].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["D" + altBilgi.ilkSatir].Value = topTutar;

                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir + ":" + altBilgi.sonSutun + altBilgi.ilkSatir].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir + ":" + altBilgi.sonSutun + altBilgi.ilkSatir].Style.Fill.BackgroundColor.SetColor(acikYesil);

                ws1.Cells[altBilgi.ilkSutun + (altBilgi.ilkSatir + 1)].Value = "Toplam Gider";
                ws1.Cells["D" + (altBilgi.ilkSatir + 1)].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["D" + (altBilgi.ilkSatir + 1)].Value = topKar;

                ws1.Cells[altBilgi.ilkSutun + (altBilgi.ilkSatir + 1) + ":" + altBilgi.sonSutun + (altBilgi.ilkSatir + 1)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws1.Cells[altBilgi.ilkSutun + (altBilgi.ilkSatir + 1) + ":" + altBilgi.sonSutun + (altBilgi.ilkSatir + 1)].Style.Fill.BackgroundColor.SetColor(acikKirmizi);

                ws1.Cells[altBilgi.ilkSutun + (altBilgi.ilkSatir + 2)].Value = "Tarih";
                ws1.Cells["D" + (altBilgi.ilkSatir + 2)].Value = DateTime.Now.ToString();
                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir + ":" + altBilgi.ilkSutun + (altBilgi.ilkSatir + 2)].Style.Font.Bold = true;


                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[altBilgi.ilkSutun + (tablo.sonSatir + 1) + ":" + altBilgi.sonSutun + (tablo.sonSatir + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;


                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                DialogResult ac = MessageBox.Show("Satışlar başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) System.Diagnostics.Process.Start(saveExceleKaydet.FileName);

                _package.Dispose();

            }
        }
        Color
            acikKirmizi = ColorTranslator.FromHtml("#ffe6e6"),
            acikYesil = ColorTranslator.FromHtml("#e6ffe6");

    }
}

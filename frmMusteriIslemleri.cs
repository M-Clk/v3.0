using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmMusteriIslemleri : Form
    {
        public frmMusteriIslemleri()
        {
            InitializeComponent();
        }
        DbOperations Sql = new DbOperations();
        OrtakIslemler islemYap = new OrtakIslemler();
        private void frmMusteriIslemleri_Load(object sender, EventArgs e)
        {
            MusteriYukle();
            if (dgMusteriler.RowCount > 0) btnExcel.Enabled = true;
            else btnExcel.Enabled = false;
        }
        DataGridViewButtonColumn dgvBtn;
        void MusteriYukle()
        {
            if (dgvBtn != null)
                dgMusteriler.Columns.Remove(dgvBtn);
            dgMusteriler.Rows.Clear();
            try
            {
                SqlDataReader MusterileriOku = Sql.OkuProcedure("MUSTERIGETIR", new SqlParameter[0]);
                if (MusterileriOku == null) return;
                while (MusterileriOku.Read())
                {
                    string[] rows = new string[dgMusteriler.ColumnCount];
                    rows[0] = MusterileriOku[5].ToString();
                    rows[1] = MusterileriOku[0].ToString();
                    rows[2] = MusterileriOku[1].ToString();
                    rows[3] = MusterileriOku[2].ToString();
                    rows[4] = MusterileriOku[3].ToString();
                    rows[5] = MusterileriOku[4].ToString();
                    dgMusteriler.Rows.Add(rows);

                    // DataGridView e ekleme

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
                dgvBtn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                dgvBtn.Width = 45;
                dgMusteriler.Columns.Add(dgvBtn);

            }
            catch (Exception)
            {
                MessageBox.Show("Müşteriler getirilemedi. Veritabanına ulaşılamıyor olabilir. Veritabanı sunucusunun açık olduğundan emin olun.", "Bilinmeyen Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            if (dgMusteriler.RowCount > 0) btnExcel.Enabled = true;
        }
        bool a = true;
        private void txtBorcu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtBorcu.Text.IndexOf(',') == -1)
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
        decimal borc = 0;
        private void dgMusteriler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                lblAdi.Text = txtMusteriAdi.Text = dgMusteriler.Rows[e.RowIndex].Cells[1].Value.ToString();
                mtxtTelefon.Text = dgMusteriler.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtBorcu.Text = dgMusteriler.Rows[e.RowIndex].Cells[4].Value.ToString();
                btnGuncelle.Enabled = true;
                borc = Convert.ToDecimal(txtBorcu.Text);
                txtMusteriAdi.Enabled = true;
                txtBorcu.Enabled = true;
                mtxtTelefon.Enabled = true;
                txtOdendi.Enabled = true;
                button1.Enabled = true;

            }
        }
        void MusteriyiGuncelle()
        {
            SqlParameter[] param = new SqlParameter[5];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@Adi";
            param[0].SqlDbType = SqlDbType.NVarChar;
            param[0].SqlValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtMusteriAdi.Text);

            param[1] = new SqlParameter();
            param[1].ParameterName = "@Telefon";
            param[1].SqlDbType = SqlDbType.NVarChar;
            param[1].SqlValue = mtxtTelefon.Text;

            param[2] = new SqlParameter();
            param[2].ParameterName = "@Borc";
            param[2].SqlDbType = SqlDbType.Decimal;
            param[2].SqlValue = txtBorcu.Text;

            param[3] = new SqlParameter();
            param[3].ParameterName = "@IlkAdi";
            param[3].SqlDbType = SqlDbType.NVarChar;
            param[3].SqlValue = lblAdi.Text;

            param[4] = new SqlParameter();
            param[4].ParameterName = "@KasiyerId";
            param[4].SqlDbType = SqlDbType.Int;
            param[4].SqlValue = Program.k_id;
            int sonuc = Sql.GuncelleProcedure("MUSTERIGUNCELLE", param);
            
            if(sonuc==-1) MessageBox.Show("Müşteri bulunamadı. Güncellemeden önce silmiş olabilirsiniz.", "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if ( sonuc!= 0)
            {
                MusteriYukle();
                MessageBox.Show("Müşteri başarılı bir şekilde güncellendi.", "Başarılı İşlem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblAdi.Text = "";
                txtMusteriAdi.Text = "";
                mtxtTelefon.Text = "";
                txtBorcu.Text = "";
                btnGuncelle.Enabled = false;
                txtMusteriAdi.Enabled = false;
                txtBorcu.Enabled = false;
                mtxtTelefon.Enabled = false;
                txtOdendi.Enabled = false;
                button1.Enabled = false;
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            MusteriyiGuncelle();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BorcHesapla();
        }
        void BorcHesapla()
        {
            try
            {
                decimal kalan;
                
                if (txtOdendi.Text == "") kalan = 0; 
                else
                kalan = Convert.ToDecimal(txtOdendi.Text);
                if (kalan <= borc)
                {
                    txtBorcu.Text = (borc - kalan).ToString();
                    txtOdendi.Text = "";
                }
                else MessageBox.Show("Verilen tutar toplam tutardan fazla olamaz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Lütfen sayısal bir değer girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtOdendi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BorcHesapla();
        }
        bool c = true;
        private void txtOdendi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtOdendi.Text.IndexOf(',') == -1)
            {
                c = true;
            }
            if (e.KeyChar == (char)44 && c == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                c = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void dgMusteriler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.ColumnIndex ==6 && e.RowIndex>=0)
            {
                DialogResult silmeOnayi = MessageBox.Show(" Bu müşteriyi silmek üzeresiniz. \nDevam etmek istiyor musunuz?", "Silgi Onayı", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (silmeOnayi == DialogResult.Yes)
                {
                    try
                    {
                        SqlParameter[] silgiParameter = new SqlParameter[2];
                        silgiParameter[0] = new SqlParameter();
                        silgiParameter[0].ParameterName = "@Telefon";
                        silgiParameter[0].SqlDbType = SqlDbType.NVarChar;
                        silgiParameter[0].SqlValue = dgMusteriler.Rows[e.RowIndex].Cells[2].Value;

                        silgiParameter[1] = new SqlParameter();
                        silgiParameter[1].ParameterName = "@KasiyerId";
                        silgiParameter[1].SqlDbType = SqlDbType.Int;
                        silgiParameter[1].SqlValue = Program.k_id;
                        if (Sql.GuncelleProcedure("MUSTERISIL", silgiParameter) == 1)
                        {
                            MusteriYukle();
                            nfBasarili.BalloonTipText = "Müşteri başarıyla silindi.";
                            nfBasarili.Visible = true;
                            nfBasarili.ShowBalloonTip(2000);
                            if (dgMusteriler.RowCount > 0) btnExcel.Enabled = true;
                            else btnExcel.Enabled = false;
                        }
                        else MessageBox.Show("Seçili müşteri silinemedi. Başka kullanıcı tarafından silinmiş ya da veritabanında yetkisiz erişim olabilir.", "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Müşteri Silinemedi. Bilinmeyen bir hata ile karlşılaşıldı. Lütfen tekrar deneyin ya da yetkili kişi ile iletişime geçin.","Bilinmeyen Hata", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                    
                }
            }
        }
        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
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
            saveExceleKaydet.FileName = "Musteri_Listesi(" + DateTime.Now.ToShortDateString() + ")";
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
                tablo.sonSutun = "F";
                altBilgiButunlugu altBilgi = new altBilgiButunlugu();
                altBilgi.ilkSatir = 6;
                altBilgi.ilkSutun = "D";
                altBilgi.sonSutun = "F";


                _package = new ExcelPackage(new MemoryStream());
                ExcelWorksheet ws1 = _package.Workbook.Worksheets.Add("Müşteri Listesi");
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString()].Value = Program.isletmeAdi + " Müşteri Listesi";
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Merge = true;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Font.Bold = true;
                ws1.Cells[tablo.ilkSutun + tablo.tabloBasligiSatiri.ToString() + ":" + tablo.sonSutun + tablo.tabloBasligiSatiri.ToString()].Style.Font.Bold = true;
                for (int i = 0; i < 6; i++)
                {
                    ws1.Cells[tablo.tabloBasligiSatiri, i + 1].Value = dgMusteriler.Columns[i].HeaderText;
                }
                for (var kolon = 0; kolon < 6; kolon++)
                {

                    for (var satir = 0; satir < dgMusteriler.RowCount; satir++)
                    {
                        if (kolon == 0)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToInt32(dgMusteriler.Rows[satir].Cells[kolon].Value);
                            if ((satir) % 2 == 0)
                            {
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.BackgroundColor.SetColor(acikGri);
                            }
                        }
                        else if (kolon == 4 || kolon == 5) ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToDecimal(dgMusteriler.Rows[satir].Cells[kolon].Value);

                        else
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = dgMusteriler.Rows[satir].Cells[kolon].Value;
                        tablo.sonSatir = satir + tablo.ilkSatir;
                        
                    }
                    altBilgi.ilkSatir = tablo.sonSatir + 2;
                    if (kolon == 4)
                    {
                        ws1.Cells["E" + tablo.ilkSatir + ":E" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";
                        
                    }
                    if (kolon == 5)
                    {
                        ws1.Cells["F" + tablo.ilkSatir + ":F" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";
                    }

                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;

                    ws1.Column(kolon + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws1.Column(kolon + 1).AutoFit();
                }
                ws1.Cells["B" + tablo.ilkSatir + ":" + "B" + tablo.sonSatir].Style.WrapText = true;

                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Value = "Toplam";
                ws1.Cells["E" + altBilgi.ilkSatir].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["E" + altBilgi.ilkSatir].Formula = "SUM(E" + tablo.ilkSatir + ":E" +tablo.sonSatir+")";

                ws1.Cells["F" + altBilgi.ilkSatir].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["F" + altBilgi.ilkSatir].Formula = "SUM(F" + tablo.ilkSatir + ":F" + tablo.sonSatir + ")";
                _package.Workbook.Calculate();

                ws1.Cells[altBilgi.ilkSutun + (altBilgi.ilkSatir + 1)].Value = "Tarih";
                ws1.Cells["E" + (altBilgi.ilkSatir+1) + ":" + altBilgi.sonSutun + (altBilgi.ilkSatir+1)].Merge = true;
                ws1.Cells["E" + (altBilgi.ilkSatir+1)].Value = DateTime.Now.ToString();
                ws1.Cells[altBilgi.ilkSutun+altBilgi.ilkSatir+":"+ altBilgi.ilkSutun + (altBilgi.ilkSatir+1)].Style.Font.Bold = true;


                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[altBilgi.ilkSutun + (tablo.sonSatir + 1) + ":" + altBilgi.sonSutun + (tablo.sonSatir + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                //ws1.Column(1).Width = 6;
                //ws1.Column(2).Width = 10;
                //ws1.Column(3).Width = 40;
                //ws1.Column(4).Width = 7;
                //ws1.Column(5).Width = 7;
                //ws1.Column(6).Width = 6;
                //ws1.Column(7).Width = 6;

                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                DialogResult ac = MessageBox.Show("Satışlar başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) System.Diagnostics.Process.Start(saveExceleKaydet.FileName);

                _package.Dispose();

            }
        }
        Color acikGri = ColorTranslator.FromHtml("#f2f2f2");
    }
}

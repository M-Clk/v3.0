using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Otomasyon
{
    public partial class frmStokIslemleri : Form
    {
        private static frmStokIslemleri _frmStokIslemleri;
        private readonly Color acikGri = ColorTranslator.FromHtml("#f2f2f2");
        private readonly DbOperations SqlBaglantisi = new DbOperations();
        private ExcelPackage _package;
        private bool a = true;
        private DataView adAra;
        private bool b = true;
        private bool c = true;
        private bool clearCalisiyor;
        private bool d = true;
        private DataGridViewButtonColumn dgvBtn;
        private bool f = true, ff = true;
        private bool g = true;
        private int guncellenecekRowIndex = -1;
        private OrtakIslemler islemYap = new OrtakIslemler();
        private bool urunAraniyor;
        private DataTable varsayilanTblo;

        private frmStokIslemleri()
        {
            InitializeComponent();
            Load();
        }

        public static frmStokIslemleri SingletonStokFrmGetir()
        {
            if (_frmStokIslemleri == null)
                _frmStokIslemleri = new frmStokIslemleri();
            
            return _frmStokIslemleri;
        }

        public DialogResult Ac()
        {
            if(varsayilanTblo != null && varsayilanTblo.Rows.Count > 0)
                varsayilanTblo.DefaultView.RowFilter = "";
            txtAdaGoreAra.Text = "";
            txtBarkodSorgula.Text = "";
            return ShowDialog();
        }

        private void tmYanipSonme_Tick(object sender, EventArgs e)
        {
            label11.Visible = !label11.Visible; //Şu anki visible durumunun tam tersi durumuna getir.
            tmYanipSonme.Interval = tmYanipSonme.Interval + 10; //Her yanıp söndüğünde bu süreyi 10 ms arttır.
            if (tmYanipSonme.Interval == 500) //500 olduğunda yanıp sönme dursun yazı aktif kalsın.
            {
                label11.Visible = true;
                tmYanipSonme.Enabled = false;
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            var urunKaydetPopup = frmUrunKaydet.SingletonFrmGetir();
            urunKaydetPopup.ShowDialog(null);
            if(urunKaydetPopup.EklenenUrunBarkodKodu != null)
            {
                var sqlParams = new SqlParameter[1];

                sqlParams[0] = new SqlParameter();
                sqlParams[0].ParameterName = "@BarkodKodu";
                sqlParams[0].SqlDbType = SqlDbType.NVarChar;
                sqlParams[0].SqlValue = urunKaydetPopup.EklenenUrunBarkodKodu;

                var rd = SqlBaglantisi.OkuProcedure("EKLENENURUNUGETIR", sqlParams);

                rd.Read();
                var yeniUrun = new string[12];
                yeniUrun[0] = rd["No"].ToString();
                yeniUrun[1] = rd["Bakod_kodu"].ToString();
                yeniUrun[2] = rd["Marka_Adi"].ToString();
                yeniUrun[3] = rd["Adi"].ToString();
                yeniUrun[4] = rd["Maliyet"].ToString();
                yeniUrun[5] = rd["Satis_fiyati"].ToString();
                var ondalik = Convert.ToInt32(rd["Stok"]);
                var kesirli = Convert.ToDecimal(rd["Stok"]);

                if (ondalik - kesirli == 0)
                    yeniUrun[6] = Convert.ToInt32(rd["Stok"]).ToString();
                else
                    yeniUrun[6] = rd["Stok"].ToString();
                yeniUrun[7] = rd["Birim_Adi"].ToString();
                yeniUrun[8] = rd["Stok_birimi"].ToString();
                yeniUrun[9] = rd["Kritik_miktar"].ToString();
                yeniUrun[10] = rd["Hizli_urun"].ToString();
                yeniUrun[11] = rd["Marka_Id"].ToString();
                varsayilanTblo.Rows.Add(yeniUrun);

                nfBasarili.BalloonTipText = "Ürün başarılı bir şekilde eklendi.";
                nfBasarili.Visible = true;
                nfBasarili.ShowBalloonTip(2000);
            }
        }


        private void frmAnaForm_AllControls(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1) txtBarkodSorgula.Select();
        }

        public void Load()
        {
            if (Program.kritik) tmYanipSonme.Enabled = true;

            UrunleriAl();
            TabloyuDuzenle();

            UrunSayisiKontrolEt();
            if (dgUrunler.RowCount > 0) 
                btnExcel.Enabled = true;
        }

        public void frmStokIslemleri_Load(object sender, EventArgs e)
        {
        }

        public void UrunleriAl()
        {
            var satisParameter = new SqlParameter[1];
            satisParameter[0] = new SqlParameter();
            satisParameter[0].ParameterName = "@Kritik";
            satisParameter[0].SqlDbType = SqlDbType.Bit;
            satisParameter[0].SqlValue = Program.kritik;
            varsayilanTblo = SqlBaglantisi.DisconnectedProcedure("URUNLERIGETIR", satisParameter);
            clearCalisiyor = true;
            dgUrunler.Columns.Clear();
            dgUrunler.DataSource = varsayilanTblo;
            btnExcel.Visible = dgUrunler.RowCount > 0;
            for (var i = 0; i < varsayilanTblo.Rows.Count; i++)
            {
                varsayilanTblo.Rows[i]["Stok"] = Convert.ToDecimal(varsayilanTblo.Rows[i]["Stok"]).ToString("0.##");
                varsayilanTblo.Rows[i]["Kritik_miktar"] = Convert.ToDecimal(varsayilanTblo.Rows[i]["Kritik_miktar"]).ToString("0.##");
                varsayilanTblo.Rows[i]["Satis_fiyati"] = Convert.ToDecimal(varsayilanTblo.Rows[i]["Satis_fiyati"]).ToString("0.##");
                varsayilanTblo.Rows[i]["Maliyet"] = Convert.ToDecimal(varsayilanTblo.Rows[i]["Maliyet"]).ToString("0.##");
            }

        }

        public void TabloyuDuzenle()
        {
            urunAraniyor = false;
            if (dgUrunler.Columns.Count > 0)
            {
                for (var i = 0; i < dgUrunler.ColumnCount; i++)
                {
                    dgUrunler.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgUrunler.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                dgUrunler.Columns["Adi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;  
                dgUrunler.Columns["Adi"].HeaderText = "Adı";
                dgUrunler.Columns["No"].Width = 40;
                dgUrunler.Columns["Bakod_kodu"].Width = 120;
                dgUrunler.Columns["Bakod_kodu"].HeaderText = "Barkod";
                
                dgUrunler.Columns["Marka_Adi"].HeaderText = "Marka";
                dgUrunler.Columns["Marka_Adi"].Width = 200;

                dgUrunler.Columns["Maliyet"].HeaderText = "Maliyeti (₺)";
                dgUrunler.Columns["Satis_fiyati"].HeaderText = "Fiyatı (₺)";
                dgUrunler.Columns["Stok"].HeaderText = "Miktarı";
                dgUrunler.Columns["Birim_Adi"].HeaderText = "Birimi";

                dgUrunler.Columns["Maliyet"].Width = dgUrunler.Columns["Satis_fiyati"].Width = 110;
                dgUrunler.Columns["Maliyet"].Visible = !ConfigurationManager.GetValueFromRegistry(ConfigurationCategory.General, "SadeceSatisFiyatiGoster", Convert.ToBoolean, false);
                
                dgUrunler.Columns["Stok"].Width = dgUrunler.Columns["Birim_Adi"].Width = 65;
                dgUrunler.Columns["Stok_birimi"].Visible = false;
                dgUrunler.Columns["Kritik_miktar"].Visible = false;
                dgUrunler.Columns["Hizli_urun"].Visible = false;
                dgUrunler.Columns["Marka_Id"].Visible = false;
                dgvBtn = new DataGridViewButtonColumn();
                //Kolon Başlığı
                dgvBtn.HeaderText = "";
                // Butonun Text
                dgvBtn.Text = "Sil";
                // Butonda Text Kullanılmasını aktifleştirme
                dgvBtn.UseColumnTextForButtonValue = true;
                // Buton çerçeve rengi

                //Kendine göre değştirme
                dgvBtn.FlatStyle = FlatStyle.Flat;

                dgvBtn.DefaultCellStyle.BackColor = Color.LightSkyBlue;

                // Buton seçiliykenki çerçeve rengi
                dgvBtn.DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;

                dgvBtn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                // Butonun genişiliği
                dgvBtn.Width = 45;

                dgUrunler.Columns.Add(dgvBtn);

                dgvBtn.Dispose();
            }
        }

        private void UrunSayisiKontrolEt()
        {
            if (dgUrunler.Rows.Count <= 0 && clearCalisiyor == false)
            {
                txtAdaGoreAra.Enabled = false;
                txtBarkodSorgula.Enabled = false;
                btnExcel.Enabled = false;
            }
            else
            {
                clearCalisiyor = true;
            }
        }

        private void txtAdaGoreAra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1) txtBarkodSorgula.Select();
            if (e.KeyCode == Keys.Enter)
            {
                txtAdaGoreAra.Text = "";
                txtAdaGoreAra_TextChanged(0, e);
            }
        }
        
        private void txtBarkodSorgula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtBarkodSorgula.Text != "")
            {
                var sorgula = varsayilanTblo.DefaultView;
                sorgula.RowFilter = "Bakod_kodu = '" + txtBarkodSorgula.Text + "'";
                if (sorgula.Count > 0)
                {
                    txtBarkodSorgula.Text = "";
                    dgUrunler.DataSource = sorgula;
                    btnExcel.Visible = dgUrunler.RowCount > 0;
                    urunAraniyor = true;
                }

                else
                {
                    txtBarkodSorgula.Text = "";
                    sorgula.RowFilter = "";
                    MessageBox.Show("Aradığınız ürün bulunamadı.", "Hata Mesajı", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void txtAdaGoreAra_TextChanged(object sender, EventArgs e)
        {
            adAra = varsayilanTblo.DefaultView;
            adAra.RowFilter = "Adi like '%" + txtAdaGoreAra.Text + "%'";
            dgUrunler.DataSource = adAra;
            urunAraniyor = true;
            btnExcel.Visible = dgUrunler.RowCount > 0;
        }

        private void dgUrunler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != 10)
            {
                var urunKaydetPopup = frmUrunKaydet.SingletonFrmGetir();
                urunKaydetPopup.ShowDialog(dgUrunler.Rows[e.RowIndex]);
                
                var kapat = false;
                if (urunKaydetPopup.IslemYapildi)
                {
                    if (Program.kritik && Convert.ToDecimal(urunKaydetPopup.DgRow.Cells["Kritik_miktar"].Value) < Convert.ToDecimal(urunKaydetPopup.DgRow.Cells["Stok"].Value))
                    {
                        UrunleriAl();
                        TabloyuDuzenle();
                        if (dgUrunler.RowCount == 0) 
                            kapat = true;
                    }
                    ff = true;

                    nfBasarili.BalloonTipText = "Ürün başarılı bir şekilde güncellendi.";
                    nfBasarili.Visible = true;
                    nfBasarili.ShowBalloonTip(2000);

                }           
                if (kapat) 
                    Close();
            }

            //btnGuncelle.Enabled = true;
            guncellenecekRowIndex = e.RowIndex;
                if (Convert.ToInt16(dgUrunler.Rows[e.RowIndex].Cells["Stok_birimi"].Value) == 1) ff = false;
                //groupBox1.Visible = true;
            
        }

        private void dgUrunler_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UrunSayisiKontrolEt();
        }

        private void frmStokIslemleri_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.stok_calisiyor = false;
            Program.kritik = false;
        }

        private void txtAdi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = char.IsPunctuation(e.KeyChar);
        }

        private void dgUrunler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && dgUrunler.Columns[e.ColumnIndex].GetType()
                    .IsEquivalentTo(typeof(DataGridViewButtonColumn)))
            {
                var silmeOnayi = MessageBox.Show(" Bu ürünü silmek üzeresiniz. \nDevam etmek istiyor musunuz?",
                    "Silgi Onayı", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (silmeOnayi == DialogResult.Yes)
                {
                    var silgiParameter = new SqlParameter[1];
                    silgiParameter[0] = new SqlParameter();
                    silgiParameter[0].ParameterName = "@Barkod";
                    silgiParameter[0].SqlDbType = SqlDbType.NVarChar;
                    silgiParameter[0].SqlValue = dgUrunler.Rows[e.RowIndex].Cells["Bakod_kodu"].Value;
                    if (SqlBaglantisi.GuncelleProcedure("URUNSIL", silgiParameter) == 1)
                    {
                        if (urunAraniyor)
                            dgUrunler.Rows.Remove(dgUrunler.Rows[e.RowIndex]);

                        else varsayilanTblo.Rows[e.RowIndex].Delete();

                        nfBasarili.BalloonTipText = "Ürün başarılı bir şekilde silindi.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);

                        if (dgUrunler.Rows.Count < 1)
                        {
                            btnExcel.Enabled = false;
                            adAra = varsayilanTblo.DefaultView;
                            adAra.RowFilter = "Adi like '%%'";
                            dgUrunler.DataSource = adAra;
                            urunAraniyor = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Seçili ürün silinemedi. Başka kullanıcı tarafından silinmiş ya da veritabanında yetkisiz erişim olabilir.",
                            "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            btnExcel.Visible = dgUrunler.RowCount > 0;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExceleAktar();
        }

        private void ExceleAktar()
        {
            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Urun_Listesi(" + DateTime.Now.ToShortDateString() + ")";
            var dialogResult = saveExceleKaydet.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                if (File.Exists(saveExceleKaydet.FileName))
                    try
                    {
                        var fs = File.Open(saveExceleKaydet.FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                        fs.Close();
                        fs.Dispose();
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Değiştirmek istediğiniz dosya şu anda başka bir uygulama tarafından kullanılıyor. Lütfen başka bir uygulama tarafından kullanılmadığından emin olun. Ya da farklı bir isimde kaydetmeyi deneyin.",
                            "Dosya Meşgul", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                var tablo = new excelTabloButunlugu();
                tablo.baslikSatiri = 1;
                tablo.ilkSatir = 4;
                tablo.sonSatir = 5;
                tablo.tabloBasligiSatiri = 3;
                tablo.ilkSutun = "A";
                tablo.sonSutun = "G";
                var altBilgi = new altBilgiButunlugu();
                altBilgi.ilkSatir = 6;
                altBilgi.ilkSutun = "E";
                altBilgi.sonSutun = "G";


                _package = new ExcelPackage(new MemoryStream());
                var ws1 = _package.Workbook.Worksheets.Add("Ürün Listesi");
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri].Value = Program.isletmeAdi + " Ürün Listesi";
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri + ":" + tablo.sonSutun + tablo.baslikSatiri].Merge = true;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri + ":" + tablo.sonSutun + tablo.baslikSatiri].Style.Font
                    .Bold = true;
                ws1.Cells[tablo.ilkSutun + tablo.tabloBasligiSatiri + ":" + tablo.sonSutun + tablo.tabloBasligiSatiri]
                    .Style.Font.Bold = true;
                for (var i = 0; i < 7; i++)
                    ws1.Cells[tablo.tabloBasligiSatiri, i + 1].Value = dgUrunler.Columns[i].HeaderText;
                for (var kolon = 0; kolon < 7; kolon++)
                {
                    for (var satir = 0; satir < dgUrunler.RowCount; satir++)
                    {
                        if (kolon == 0)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value =
                                Convert.ToInt32(dgUrunler.Rows[satir].Cells[kolon].Value);
                            if (satir % 2 == 0)
                            {
                                ws1.Cells[
                                    tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun +
                                    (satir + tablo.ilkSatir)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws1.Cells[
                                    tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun +
                                    (satir + tablo.ilkSatir)].Style.Fill.BackgroundColor.SetColor(acikGri);
                            }
                        }
                        else if (kolon == 3 || kolon == 4)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value =
                                Convert.ToDecimal(dgUrunler.Rows[satir].Cells[kolon].Value);
                        }
                        else if (kolon == 5)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value =
                                Convert.ToDecimal(dgUrunler.Rows[satir].Cells[kolon].Value);
                        }
                        else
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value =
                                dgUrunler.Rows[satir].Cells[kolon].Value;
                        }

                        tablo.sonSatir = satir + tablo.ilkSatir;
                    }

                    altBilgi.ilkSatir = tablo.sonSatir + 2;
                    if (kolon == 3)
                        ws1.Cells["D" + tablo.ilkSatir + ":D" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";
                    if (kolon == 4)
                        ws1.Cells["E" + tablo.ilkSatir + ":E" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";

                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;

                    ws1.Column(kolon + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws1.Column(kolon + 1).AutoFit();
                }

                ws1.Cells["B" + tablo.ilkSatir + ":" + "B" + tablo.sonSatir].Style.WrapText = true;
                ws1.Cells["C" + tablo.ilkSatir + ":" + "C" + tablo.sonSatir].Style.WrapText = true;

                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Value = "Tarih";
                ws1.Cells[
                    Convert.ToString(Convert.ToChar(Convert.ToInt32(altBilgi.ilkSutun[0]) + 1)) + altBilgi.ilkSatir +
                    ":" + altBilgi.sonSutun + altBilgi.ilkSatir].Merge = true;
                ws1.Cells[
                        Convert.ToString(Convert.ToChar(Convert.ToInt32(altBilgi.ilkSutun[0]) + 1)) + altBilgi.ilkSatir]
                    .Value = DateTime.Now.ToString();
                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Style.Font.Bold = true;


                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri + ":" + tablo.sonSutun + tablo.baslikSatiri].Style
                    .HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri + ":" + tablo.sonSutun + tablo.baslikSatiri].Style.Border
                    .Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri + ":" + tablo.sonSutun + tablo.baslikSatiri].Style.Border
                    .Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[altBilgi.ilkSutun + (tablo.sonSatir + 1) + ":" + altBilgi.sonSutun + (tablo.sonSatir + 1)]
                    .Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Column(1).Width = 6;
                ws1.Column(2).Width = 10;
                ws1.Column(3).Width = 40;
                ws1.Column(4).Width = 8;
                ws1.Column(5).Width = 8;
                ws1.Column(6).Width = 6;
                ws1.Column(7).Width = 6;

                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                var ac = MessageBox.Show("Satışlar başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?",
                    "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) Process.Start(saveExceleKaydet.FileName);

                _package.Dispose();
            }
        }

        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }

        private struct excelTabloButunlugu
        {
            public string ilkSutun;
            public string sonSutun;
            public int ilkSatir;
            public int sonSatir;
            public int baslikSatiri;
            public int tabloBasligiSatiri;
        }

        private void btnMarka_Click(object sender, EventArgs e)
        {
            var markaYonetimPopup = frmMarkaYonetim.SingletonFrmGetir();
            markaYonetimPopup.ShowDialog();
        }

        private void btnTopluFiyatGuncelle_Click(object sender, EventArgs e)
        {
            dgUrunler.Visible = false;
            var topluFiyatGuncelleme = frmTopluFiyatGuncelleme.SingletonFrmGetir();
            topluFiyatGuncelleme.ShowDialog(varsayilanTblo);
            varsayilanTblo.DefaultView.RowFilter = "";
            dgUrunler.Visible = true;
        }

        private struct altBilgiButunlugu
        {
            public string ilkSutun;
            public string sonSutun;
            public int ilkSatir;
        }
    }
}
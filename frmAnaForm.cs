using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Otomasyon
{
    public partial class frmAnaForm : Form
    {
        public enum miktarDurum
        {
            sepette_yok_yetersiz,
            sepette_var_yetersiz,
            sepette_yok_yeterli,
            sepette_var_yeterli
        }

        private readonly Color acikGri = ColorTranslator.FromHtml("#f2f2f2");

        private readonly OrtakIslemler islemYap = new OrtakIslemler();
        private readonly DbOperations SqlOperation = new DbOperations();
        private readonly int[] taksitAylari = new int[5];

        private ExcelPackage _package;
        private bool _searchingProducs;
        private bool a = true, nf_cagirdi;
        private bool b = true;
        private bool c = true;
        private bool d = true;
        private bool f = true;
        private frmStokIslemleri frmStok;
        public int musteriId, sonSatisId;
        private decimal odenenNakit, odenenKredi;
        private decimal paraUstu;
        private int satirIndex;
        private string sonMusteriAdi;
        private DataTable StokKontrolMerkezi = new DataTable();

        private decimal toplam, Borc, seciliMiktar;
        private decimal varsayilanMiktar = 1;

        public frmAnaForm(bool isDbCreated)
        {
            InitializeComponent();
            if (!isDbCreated)
            {
                var yapilandirma = new frmConfiguration(false);
                yapilandirma.ShowDialog();
            }

            frmStok = frmStokIslemleri.SingletonStokFrmGetir();
        }

        private void btnStokIslemleri_Click(object sender, EventArgs e)
        {
            if (dgSepet.RowCount > 0)
            {
                MessageBox.Show("Satış işlemini tamamlamadan stok işlemleri yapılamaz!", "Hata", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                if (!nfUyari.Visible && !nf_cagirdi)
                    if (Convert.ToInt32(SqlOperation.OkuScalar("KRITIKURUNSORGULA", CommandType.StoredProcedure,
                            new SqlParameter[0])) > 0)
                    {
                        nfUyari.BalloonTipIcon = ToolTipIcon.None;
                        nfUyari.Visible = true;
                        nfUyari.ShowBalloonTip(4000);
                    }

                if (nf_cagirdi)
                    nf_cagirdi = false;
                Program.stok_calisiyor = true;
                frmStok = frmStokIslemleri.SingletonStokFrmGetir();
                frmStok.Ac();
            }

            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnSatisIslemleri_Click(object sender, EventArgs e)
        {
            var satis = new frmSatisIslemleri();
            satis.ShowDialog();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnMusteriIslemleri_Click(object sender, EventArgs e)
        {
            if (Program.yetki == Program.Yetki.yonetici)
            {
                var musteri = new frmMusteriIslemleri();
                musteri.ShowDialog();
            }
            else
            {
                MessageBox.Show(
                    "Yönetici olmadığınızdan müşteri işlemlerine giriş yapamazsınız. Lütfen işletme yöneticisiyle görüşün.",
                    "Yetkisiz İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnKullaniciIslemleri_Click(object sender, EventArgs e)
        {
            var kullanici = new frmKullanıcıIslemleri();
            if (Program.yetki == Program.Yetki.yonetici)
            {
                kullanici.Width = 900;
                kullanici.Height = 560;
                kullanici.StartPosition = FormStartPosition.CenterScreen;
            }

            kullanici.ShowDialog();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            if (dgSepet.RowCount > 0)
            {
                var Sor = MessageBox.Show(
                    "Sepette satılmamış ürün var. Eğer çıkış yaparsanız ürünler satılmayacak. Çıkmak istediğinizden emin misiniz?",
                    "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (Sor == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    txtBarkodOku.Text = "";
                    txtBarkodOku.Select();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void frmAnaForm_Load(object sender, EventArgs e)
        {
            var login = new frmGiris();

            login.ShowDialog();
            login.Dispose();
            if (!Program.giris)
                return;
            SonSatisGoruntule();
            while (cbMiktar.Items.Count < 100)
                cbMiktar.Items.Add(cbMiktar.Items.Count + 1);
            cbMiktar.SelectedIndex = 0;
            lblSistemAdi.Text = Program.isletmeAdi + " Barkodlu Satış Sistemi";
            if (Convert.ToInt32(SqlOperation.OkuScalar("KRITIKURUNSORGULA", CommandType.StoredProcedure,
                    new SqlParameter[0])) > 0)
            {
                nfUyari.BalloonTipIcon = ToolTipIcon.None;
                nfUyari.Visible = true;
                nfUyari.ShowBalloonTip(3500);
            }

            taksitAylari[0] = 0;
            taksitAylari[1] = 6;
            taksitAylari[2] = 12;
            taksitAylari[3] = 24;
            taksitAylari[4] = 36;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            var destek = new frmDestek();
            destek.ShowDialog();
        }

        private void txtBarkodOku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                cbMiktar.Select();
            if (e.KeyCode == Keys.Enter && txtBarkodOku.Text != "" && !_searchingProducs)
                UrunSorgula();
        }

        private miktarDurum
            MiktarHesapla(string kod, decimal miktar,
                decimal istenenMiktar) //Sorgulanan ürünün miktarının yeterliliğini kontrol et ve sepette varsa seçilen miktarın eklemesi için yoksa sepete eklemesi için enum gönder
        {
            for (var i = 0; i < dgSepet.RowCount; i++) //Barkod kodunu sepettekilerle karşılaştır
                if (dgSepet.Rows[i].Cells[1].Value.ToString() == kod) //Kod sepette varsa işlem yap
                {
                    if (Convert.ToDecimal(dgSepet.Rows[i].Cells["Miktar"].Value) + istenenMiktar >
                        miktar) //istenen miktar stoktaki ve sepettikinin toplamında fazla ise onun anlamına gelen enumu gönder
                        return miktarDurum.sepette_var_yetersiz;

                    satirIndex = i;
                    return miktarDurum.sepette_var_yeterli;
                }

            if (istenenMiktar > miktar)
                return
                    miktarDurum
                        .sepette_yok_yetersiz; //istenen miktar sepette yok ve yetersizse onun anlamına gelen enumu gönder
            return miktarDurum.sepette_yok_yeterli; //bunlardan hiçbiri değilse tek enum kaldı onu gönder
        }

        private void ToplamHesapla() //Toplam fiyat tutarını hesapla
        {
            toplam = 0;
            for (var i = 0; i < dgSepet.Rows.Count; i++)
            {
                toplam += Convert.ToDecimal(dgSepet.Rows[i].Cells["TopTutar"].Value);
                if (i % 2 == 0)
                    dgSepet.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                else
                    dgSepet.Rows[i].DefaultCellStyle.BackColor = Color.LavenderBlush;
            }


            lblToplam.Text = "Toplam : " + toplam.ToString("F2") + " ₺";
            if (Program.odemeTipi == 1)
            {
                odenenKredi = 0;
                rbNakit.Checked = true;
                txtOdenenKredi.Text = "0";
                cbOdenenNakit.Text = toplam.ToString();
            }
            else if (Program.odemeTipi == 2)
            {
                odenenNakit = 0;
                rbKrediKarti.Checked = true;
                cbOdenenNakit.Text = "0";
                txtOdenenKredi.Text = toplam.ToString();
            }

            ParaUstuHesapla();
        }

        private void dgSepet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != 4)
            {
                if (e.RowIndex < dgSepet.Rows.Count)
                    for (var i = e.RowIndex; i < dgSepet.Rows.Count; i++)
                        dgSepet.Rows[i].Cells[0].Value = i; //Bu satırın altındaki satırların numarasını 1 azalt.
                dgSepet.Rows.Remove(dgSepet.Rows[e.RowIndex]);
                ToplamHesapla(); //Toplam tutarı hesapla
                if (dgSepet.RowCount <= 0) //Eğer satır kalmadıysa herseyi pasifize et
                    Etkisizlestir();
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 &&
                e.ColumnIndex == 4) // Miktar tıklandı ise yanlış işlem sonucu eski değerini girmek için kaydet
            {
                varsayilanMiktar = Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                dgSepet.Focus();
            }
            else
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == 4) //Kayıt var mı kontrol et ve kayıt eklenme durumu değilse
            {
                var parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter();
                parameter[0].ParameterName = "@BarkodKodu";
                parameter[0].SqlDbType = SqlDbType.NVarChar;
                parameter[0].SqlValue = dgSepet.Rows[e.RowIndex].Cells[1].Value;
                using (var dReader = SqlOperation.OkuProcedure("URUNSORGULAMA", parameter)) //...Seçili ürünü getir
                {
                    try //Sayı dışında bir değer girilirse hata verir. Bunu yakala ve kullanıcıyı uyar.
                    {
                        dReader.Read();
                        if (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) >
                            Convert.ToDecimal(dReader[3])) //Yeni değer stoktan büyükse uyar, max ile değiştir.
                        {
                            MessageBox.Show(
                                "Bu üründen stokta " + dReader[3] + " " + dReader[4].ToString().ToLower() +
                                " var. Daha fazla seçilemez. ", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dReader[3].ToString();
                            dgSepet.Rows[e.RowIndex].Selected = true;
                        }
                        else if (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) <=
                                 0) //Yeni değer 0 veya negatif ise hata mesajı göster 
                        {
                            MessageBox.Show(
                                "Miktar negatif veya 0 girilemez. Ürünü sepetten çıkarmak için ürünü seçip çift tıklayabilirsiniz.",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = varsayilanMiktar; //Eski değerini gir.
                        }
                        else //Bunlardan hiçbiri değilse hatasız veri girişi yapılmıştır toplamı güncelle
                        {
                            dgSepet.Rows[e.RowIndex].Cells["TopTutar"].Value =
                                (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells["Miktar"].Value) *
                                 Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells["BirimFiyat"].Value)).ToString("F2");
                            ToplamHesapla();
                        }
                    }
                    catch (Exception)
                    {
                        if (dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString() == "Adet")
                            //Birimi adet ise doğal sayı gir uyar eski değer ile değiştir
                        {
                            MessageBox.Show("Doğal sayı dışında veri girildi. Adet yalnız doğal sayı olmalıdır.",
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = varsayilanMiktar;
                        }
                        else
                            //
                        {
                            MessageBox.Show("Miktar sayısal olmalıdır. ", "Hata", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = varsayilanMiktar;
                        }
                    }
                    finally
                    {
                        DbOperations.Connection.Close();
                        SqlOperation.cmd.Dispose();
                    }
                }

                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void
            cbMiktar_KeyPress(object sender,
                KeyPressEventArgs e) //cbMiktar combobox seçiliyken girilen karakterleri kontrol et yalnız sayı ve sadece bir kez virgül(,) girmesini sağla
        {
            if (cbMiktar.Text.IndexOf(',') == -1) a = true;
            if (e.KeyChar == (char)44 && a)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                a = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void cbMiktar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Left || e.KeyCode == Keys.Space ||
                e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.Left)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void frmAnaForm_AllControls(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Enter)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (dgSepet.Rows.Count == 1) Etkinlestir();
        }

        private void Etkinlestir()
        {
            rbNakit.Checked = true;
            tableLayoutPanel25.Visible = true;
            tableLayoutPanel26.Visible = true;
            rbKrediKarti.Visible = true;
            rbNakit.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            lblTaksit.Visible = true;
            lblVerileNakit.Visible = true;
            lblVerilenKredi.Visible = true;
            txtOdenenKredi.Visible = true;
            cbOdenenNakit.Visible = true;
            cbTaksit.Visible = true;
            txtMusteriAdi.Enabled = true;
            mtxtTelefon.Enabled = true;
            chckMusteriKaydet.Enabled = true;
            btnSatisYap.Enabled = true;
            btnSatisIptalEt.Enabled = true;
            cbYazdir.Visible = true;
            cbYazdir.Checked = Program.herSatistaYazdir;
            lblParaUstu.Visible = true;
            cbTaksit.SelectedIndex = 0;
            cbOdenenNakit.SelectedIndex = 0;
            txtOdenenKredi.Text = "0";
        }

        private void Etkisizlestir()
        {
            tableLayoutPanel25.Visible = false;
            tableLayoutPanel26.Visible = false;
            rbNakit.Visible = false;
            button3.Visible = false;
            button2.Visible = false;
            rbKrediKarti.Visible = false;
            lblTaksit.Visible = false;
            lblVerileNakit.Visible = false;
            lblVerilenKredi.Visible = false;
            txtOdenenKredi.Visible = false;
            cbOdenenNakit.Visible = false;
            cbTaksit.Visible = false;
            lblParaUstu.Visible = false;
            txtMusteriAdi.Enabled = false;
            txtMusteriAdi.Text = "";
            mtxtTelefon.Enabled = false;
            mtxtTelefon.Text = "";
            chckMusteriKaydet.Enabled = false;
            chckMusteriKaydet.Checked = false;
            btnSatisYap.Enabled = false;
            btnSatisIptalEt.Enabled = false;
            label4.Visible = false;
            txtBorc.Visible = false;
            btnHepsi.Visible = false;
            cbYazdir.Visible = false;
            odenenKredi = 0;
            odenenNakit = 0;
            paraUstu = 0;
        }

        private void dgSepet_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dgSepet.Rows.Count == 0) Etkisizlestir();
        }

        private void txtMusteriAdi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                mtxtTelefon.Select();
            if (e.KeyCode == Keys.F1)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void mtxtTelefon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                chckMusteriKaydet.Select();
            if (e.KeyCode == Keys.Up)
                txtMusteriAdi.Select();
            if (e.KeyCode == Keys.F1)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void chckMusteriKaydet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                mtxtTelefon.Select();
            if (e.KeyCode == Keys.F1)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }

            if (e.KeyCode == Keys.Enter)
                chckMusteriKaydet.Checked = !chckMusteriKaydet.Checked;
        }

        private void chkBorc_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBorc.Checked)
            {
                if (txtMusteriAdi.Text == "" || mtxtTelefon.Text == "")
                {
                    chkBorc.Checked = false;
                    MessageBox.Show(
                        "Lütfen müşteri bilgilerini eksiksiz girin. Borç yapılacaksa müşterinin adı ve telefon numarası girilmek zorundadır.",
                        "Başarısız İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                BorcHesabi(true);
            }
            else
            {
                txtBorc.Text = "";
                txtOdendi.Text = "";
                BorcHesabi(false);
            }

            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void BorcHesabi(bool durum)
        {
            label2.Visible = durum;
            label4.Visible = durum;
            txtBorc.Visible = durum;
            txtOdendi.Visible = durum;
            btnHepsi.Visible = durum;
            btnHesapla.Visible = durum;
        }

        private void chckMusteriKaydet_CheckedChanged(object sender, EventArgs e)
        {
            if (chckMusteriKaydet.Checked)
                chkBorc.Visible = true;
            else
                chkBorc.Visible = false;
            chkBorc.Checked = false;
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnSatisIptalEt_Click(object sender, EventArgs e)
        {
            var sil = MessageBox.Show(
                "Satış iptal edilirse sepet boşaltılacak. İşleminiz sıfırlanacak. Satışı iptal etmek istediğinizden emin misiniz?",
                "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sil == DialogResult.Yes)
            {
                Etkisizlestir();
                dgSepet.Rows.Clear();
                ToplamHesapla();
            }

            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            if (paraUstu < 0)
            {
                MessageBox.Show(
                    "Ücret tamamen ödenmedi. " + -paraUstu +
                    " TL ödenmesi gerek. Lütfen satış tutarı tamamen ödendikten sonra tekrar deneyin.",
                    "Satış Tutarı Tamamen Ödenmedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rbNakit.Checked = true;
                return;
            }

            if (!Program.lisans)
            {
                var satisSayisi = Convert.ToInt32(SqlOperation.ScalarTextCommand("Select COUNT(Id) From Satislar"));
                try
                {
                    if (satisSayisi < 10)
                    {
                        islemYap.LisansUyarisi("Lisanssız yazılım kullanıyorsunuz. En fazla 10 satış yapabilirsiniz. " +
                                               (9 - satisSayisi) +
                                               " hakkınız kaldı. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.");
                    }
                    else
                    {
                        islemYap.LisansUyarisiMesaj(
                            "Lisanssız yazılım kullanıyorsunuz. Maksimum satış limitinize ulaştınız. Artık satış yapamazsınız. Eğer ürün anahtarınız varsa limitsiz kullanım için etkinleştirin.");
                        return;
                    }
                }
                catch
                {
                }
            }

            sonMusteriAdi = "Kaydedilmedi";
            musteriId = 0;
            if (!MusteriKontrolEt())
                return;
            SatisiYap();
            SatisDetayiEkle();
            //if(cbYazdir.Checked)
            //    DetayiYazdir();
            //DetayiExceleAktar();
            //Stok güncellemesi satış detayı eklendiğinde trigger tetiklenecek ve otomatik güncelleyecektir.
            //İşin çoğu bölümü SQL Server da yapılacak
            //Eğer yapılabilirse datagrid deki tüm parametlereler SqlParameterCollection nesnesi yardımı ile birden gönderilecek.
            //Satış ekleme procedure ünde başlayacak herşey. Satış id si ExecuteScalar yardımı ile alınıp satış detayına parametre olarak gönderilecek. 
            dgSepet.Rows.Clear();
            ToplamHesapla();
            Etkisizlestir();
            SonSatisGoruntule();
            //frmMesaj mesajVer = new frmMesaj("Satış başarıyla gerçekleşti.", "Başarılı İşlem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //mesajVer.Show();
            nfBasarili.BalloonTipText = "Satış başarıyla gerçekleşti.";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(2000);
            cbYazdir.Checked = false;
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void DetayiExceleAktar()
        {
            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Satis_Detayi(Satis_No=" + sonSatisId + ")";
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

                _package = new ExcelPackage(new MemoryStream());
                var ws1 = _package.Workbook.Worksheets.Add(sonSatisId + " Numaralı Satış");
                ws1.Cells[1, 1].Value = Program.isletmeAdi + " Satış Detayı";
                ws1.Cells["A1:G1"].Merge = true;
                ws1.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells["A1:G1"].Style.Font.Bold = true;
                ws1.Cells["A2:G2"].Style.Font.Bold = true;
                ws1.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells["A2:G2"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                for (var i = 0; i < dgSepet.ColumnCount; i++)
                    if (i < 6)
                        ws1.Cells[2, i + 1].Value = dgSepet.Columns[i].HeaderText;
                    else if (i > 7)
                        ws1.Cells[2, i - 1].Value = dgSepet.Columns[i].HeaderText;
                var sonSatir = 0;
                for (var kolon = 0; kolon < dgSepet.ColumnCount; kolon++)
                {
                    for (var satir = 0; satir < dgSepet.RowCount; satir++)
                    {
                        if (kolon == 0)
                        {
                            ws1.Cells[satir + 3, kolon + 1].Value =
                                Convert.ToInt32(dgSepet.Rows[satir].Cells[kolon].Value);
                            if (satir % 2 == 0)
                            {
                                ws1.Cells["A" + (satir + 3) + ":G" + (satir + 3)].Style.Fill.PatternType =
                                    ExcelFillStyle.Solid;
                                ws1.Cells["A" + (satir + 3) + ":G" + (satir + 3)].Style.Fill.BackgroundColor
                                    .SetColor(acikGri);
                            }
                        }
                        else if (kolon == 3 || kolon == 4)
                        {
                            ws1.Cells[satir + 3, kolon + 1].Value =
                                Convert.ToDecimal(dgSepet.Rows[satir].Cells[kolon].Value);
                        }
                        else if (kolon < 6)
                        {
                            ws1.Cells[satir + 3, kolon + 1].Value = dgSepet.Rows[satir].Cells[kolon].Value;
                        }
                        else if (kolon > 7)
                        {
                            ws1.Cells[satir + 3, kolon - 1].Value =
                                Convert.ToDecimal(dgSepet.Rows[satir].Cells[kolon].Value);
                        }

                        sonSatir = satir + 3;
                    }


                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
                }

                string aciklamaSutunu = "E", icerikSutunu = "F";

                ws1.Cells[aciklamaSutunu + (sonSatir + 2)].Value = "Toplam";
                ws1.Cells[icerikSutunu + (sonSatir + 2) + ":G" + (sonSatir + 2)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 2)].Value = HarfleriSil();
                ws1.Cells[icerikSutunu + (sonSatir + 2)].Style.Numberformat.Format = "₺#,0.00";

                ws1.Cells[aciklamaSutunu + (sonSatir + 3)].Value = "Tarih";
                ws1.Cells[icerikSutunu + (sonSatir + 3) + ":G" + (sonSatir + 3)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 3)].Value = DateTime.Now.ToShortDateString();

                ws1.Cells[aciklamaSutunu + (sonSatir + 4)].Value = "Saat";
                ws1.Cells[icerikSutunu + (sonSatir + 4) + ":G" + (sonSatir + 4)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 4)].Value = DateTime.Now.ToLongTimeString();

                ws1.Cells[aciklamaSutunu + (sonSatir + 5)].Value = "Satış No";
                ws1.Cells[icerikSutunu + (sonSatir + 5) + ":G" + (sonSatir + 5)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 5)].Value = sonSatisId.ToString("D9");

                ws1.Cells[aciklamaSutunu + (sonSatir + 6)].Value = "Müşteri";
                ws1.Cells[icerikSutunu + (sonSatir + 6) + ":G" + (sonSatir + 6)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 6)].Value = sonMusteriAdi;

                ws1.Cells[aciklamaSutunu + (sonSatir + 7)].Value = "Kasiyer";
                ws1.Cells[icerikSutunu + (sonSatir + 7) + ":G" + (sonSatir + 7)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 7)].Value = Program.k_adi;

                ws1.Cells["D3:D" + sonSatir].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["G3:G" + sonSatir].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["E3:E" + sonSatir].Style.Numberformat.Format = "0.0";

                ws1.Cells[aciklamaSutunu + (sonSatir + 2) + ":G" + (sonSatir + 2)].Style.Border.Top.Style =
                    ExcelBorderStyle.Medium;
                ws1.Cells[aciklamaSutunu + (sonSatir + 2) + ":" + aciklamaSutunu + (sonSatir + 7)].Style.Font.Bold =
                    true;

                ws1.Cells["$A$2:$G$" + (sonSatir + 7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws1.Cells.Style.Font.Name = "Courier New";
                for (var i = 1; i <= 7; i++) ws1.Column(i).AutoFit();

                ws1.PrinterSettings.FitToPage = true;
                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                var ac = MessageBox.Show(
                    "Satış detayı başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes)
                    Process.Start(saveExceleKaydet.FileName);
                _package.Dispose();
            }
        }

        private void SonSatisGoruntule()
        {
            dgSonSatislar.Rows.Clear();
            var SonKayitlar = SqlOperation.OkuProcedure("SONSATISLARIGETIR", new SqlParameter[0]);
            try
            {
                var satir = new string[10];
                while (SonKayitlar.Read())
                {
                    satir[0] = SonKayitlar[0].ToString();
                    satir[1] = SonKayitlar[1].ToString();
                    satir[2] = SonKayitlar[2].ToString();
                    satir[3] = SonKayitlar[3].ToString();
                    satir[4] = SonKayitlar[4].ToString();
                    satir[5] = SonKayitlar[5].ToString();
                    satir[6] = SonKayitlar[6].ToString();
                    satir[7] = SonKayitlar[7].ToString();
                    satir[8] = SonKayitlar[8].ToString();
                    satir[9] = SonKayitlar[9].ToString();
                    dgSonSatislar.Rows.Add(satir);
                }
            }
            catch
            {
            }
            finally
            {
                DbOperations.Connection.Close();
                SqlOperation.cmd.Dispose();
            }
        }

        private void SatisDetayiEkle()
        {
            var detayParametresi = new SqlParameter[4];
            detayParametresi[0] = new SqlParameter();
            detayParametresi[0].ParameterName = "@Id";
            detayParametresi[0].SqlDbType = SqlDbType.Int;
            detayParametresi[0].SqlValue = sonSatisId;
            detayParametresi[1] = new SqlParameter();
            detayParametresi[1].ParameterName = "@BarkodKodu";
            detayParametresi[1].SqlDbType = SqlDbType.NVarChar;
            detayParametresi[2] = new SqlParameter();
            detayParametresi[2].ParameterName = "@Miktar";
            detayParametresi[2].SqlDbType = SqlDbType.Decimal;
            detayParametresi[3] = new SqlParameter();
            detayParametresi[3].ParameterName = "@Birimi";
            detayParametresi[3].SqlDbType = SqlDbType.TinyInt;

            foreach (DataGridViewRow dgvRow in dgSepet.Rows)
            {
                detayParametresi[1].SqlValue = dgvRow.Cells[1].Value;
                detayParametresi[2].SqlValue = dgvRow.Cells["Miktar"].Value;
                detayParametresi[3].SqlValue = dgvRow.Cells[7].Value;
                SqlOperation.GuncelleProcedure("SATISDETAYIEKLE", detayParametresi);
            }
        }

        private void SatisiYap()
        {
            decimal toplamMaliyet = 0;
            for (var i = 0; i < dgSepet.Rows.Count; i++)
                toplamMaliyet += Convert.ToDecimal(dgSepet.Rows[i].Cells[6].Value) *
                                 Convert.ToDecimal(dgSepet.Rows[i].Cells["Miktar"].Value);
            var paramtr = new SqlParameter[9];

            paramtr[0] = new SqlParameter();
            paramtr[0].SqlDbType = SqlDbType.Int;
            paramtr[0].ParameterName = "@MusteriId";
            paramtr[0].SqlValue = musteriId;

            paramtr[1] = new SqlParameter();
            paramtr[1].ParameterName = "@Tarih";
            paramtr[1].SqlDbType = SqlDbType.DateTime;
            paramtr[1].SqlValue = DateTime.Now;

            paramtr[2] = new SqlParameter();
            paramtr[2].ParameterName = "@SatisBedeli";
            paramtr[2].SqlDbType = SqlDbType.Decimal;
            paramtr[2].SqlValue = HarfleriSil();

            paramtr[3] = new SqlParameter();
            paramtr[3].ParameterName = "@SatisKari";
            paramtr[3].SqlDbType = SqlDbType.Decimal;
            paramtr[3].SqlValue = HarfleriSil() - toplamMaliyet;

            paramtr[4] = new SqlParameter();
            paramtr[4].ParameterName = "@Kasiyer_Id";
            paramtr[4].SqlDbType = SqlDbType.Int;
            paramtr[4].SqlValue = Program.k_id;

            paramtr[5] = new SqlParameter();
            paramtr[5].ParameterName = "@Borc";
            paramtr[5].SqlDbType = SqlDbType.Decimal;
            paramtr[5].SqlValue = Borc;

            paramtr[6] = new SqlParameter();
            paramtr[6].ParameterName = "@Nakit";
            paramtr[6].SqlDbType = SqlDbType.Decimal;
            paramtr[6].SqlValue = odenenNakit;

            paramtr[7] = new SqlParameter();
            paramtr[7].ParameterName = "@Kredi";
            paramtr[7].SqlDbType = SqlDbType.Decimal;
            paramtr[7].SqlValue = odenenKredi;

            paramtr[8] = new SqlParameter();
            paramtr[8].ParameterName = "@Taksit";
            paramtr[8].SqlDbType = SqlDbType.TinyInt;
            paramtr[8].SqlValue = taksitAylari[cbTaksit.SelectedIndex];

            sonSatisId = Convert.ToInt32(SqlOperation.OkuScalar("SATISEKLE", CommandType.StoredProcedure, paramtr));
            Program.satisYapildi = true;
        }

        private void label8_Click_1(object sender, EventArgs e)
        {
            var destek = new frmDestek();
            destek.Show();
        }

        private void nfUyari_BalloonTipClicked(object sender, EventArgs e)
        {
            var en = new EventArgs();
            if (Program.stok_calisiyor)
            {
                Program.kritik = true;
                frmStok.frmStokIslemleri_Load(0, en);
            }
            else
            {
                Program.kritik = true;
                nf_cagirdi = true;
                btnStokIslemleri_Click(0, en);
            }
        }

        private void txtKalan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtOdendi.Text.IndexOf(',') == -1) b = true;
            if (e.KeyChar == (char)44 && b)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                b = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void txtBorc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtBorc.Text.IndexOf(',') == -1) c = true;
            if (e.KeyChar == (char)44 && c)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                c = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void BorcHesapla()
        {
            try
            {
                var kalan = Convert.ToDecimal(txtOdendi.Text);
                if (kalan <= HarfleriSil())
                {
                    txtBorc.Text = (HarfleriSil() - kalan).ToString();
                    txtOdendi.Text = "";
                }
                else
                {
                    MessageBox.Show("Verilen tutar toplam tutardan fazla olamaz.", "Veri Girişi Hatası",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("Lütfen sayısal bir değer girin.", "Veri Girişi Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void txtKalan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BorcHesapla();
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void btnHesapla_Click(object sender, EventArgs e)
        {
            BorcHesapla();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Program.yetki == Program.Yetki.yonetici)
            {
                var gel = new frmGelirGider();
                gel.ShowDialog();
            }
            else
            {
                MessageBox.Show(
                    "Yönetici olmadığınızdan gelir-gider işlemlerine giriş yapamazsınız. Lütfen işletme yöneticisiyle görüşün.",
                    "Yetkisiz İşlem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }

        private void nfUyari_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var en = new EventArgs();

            if (!Program.stok_calisiyor)
            {
                if (Convert.ToInt32(SqlOperation.OkuScalar("KRITIKURUNSORGULA", CommandType.StoredProcedure,
                        new SqlParameter[0])) > 0)
                {
                    Program.kritik = true;
                    nf_cagirdi = true;
                    btnStokIslemleri_Click(0, en);
                }
                else
                {
                    Program.kritik = false;
                }
            }
            else
            {
                if (Convert.ToInt32(SqlOperation.OkuScalar("KRITIKURUNSORGULA", CommandType.StoredProcedure,
                        new SqlParameter[0])) > 0)
                {
                    Program.kritik = true;
                    frmStok.frmStokIslemleri_Load(0, en);
                }
                else
                {
                    Program.kritik = false;
                }
            }

            nfUyari.Visible = false;
        }

        private void prntDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var myFontBaslik = new Font("Calibri", 35);
            var sbrush = new SolidBrush(Color.Black);
            var myPen = new Pen(Color.Black);
            var genislik = e.PageSettings.PaperSize.Width;
            e.Graphics.DrawString(Program.isletmeAdi + " Satış Sistemi", myFontBaslik, sbrush, 200, 120);
            myFontBaslik.Dispose();
            var myFontIcerik = new Font("Calibri", 15);
        }

        private void dgSepet_Click(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void cbYazdir_CheckedChanged(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_Enter(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            var conf = new frmConfiguration();
            conf.ShowDialog();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void label6_MouseHover(object sender, EventArgs e)
        {
            label6.ForeColor = Color.Black;
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.ForeColor = Color.SteelBlue;
        }

        private void label8_MouseHover(object sender, EventArgs e)
        {
            label8.ForeColor = Color.Black;
        }

        private void label8_MouseLeave(object sender, EventArgs e)
        {
            label8.ForeColor = Color.SteelBlue;
        }

        private void dgSepet_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
                dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
        }

        private void dgSepet_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
                dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor =
                    dgSepet.Rows[e.RowIndex].Cells[0].Style.BackColor;
        }

        private void dgSepet_MouseHover(object sender, EventArgs e)
        {
            dgSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            label7.Visible = true;
        }

        private void dgSepet_MouseLeave(object sender, EventArgs e)
        {
            dgSepet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            label7.Visible = false;
        }

        private void dgSonSatislar_Click(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSonSatislar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var SonSatis = new frmSatisIslemleri(Convert.ToInt32(dgSonSatislar.Rows[e.RowIndex].Cells["Id"].Value),
                Convert.ToDecimal(dgSonSatislar.Rows[e.RowIndex].Cells["Tutar"].Value),
                Convert.ToDateTime(dgSonSatislar.Rows[e.RowIndex].Cells["Tarih"].Value),
                dgSonSatislar.Rows[e.RowIndex].Cells["MusteriAdi"].Value.ToString(),
                dgSonSatislar.Rows[e.RowIndex].Cells["KasiyerAdi"].Value.ToString(),
                Convert.ToDecimal(dgSonSatislar.Rows[e.RowIndex].Cells["Nakit"].Value),
                Convert.ToDecimal(dgSonSatislar.Rows[e.RowIndex].Cells["Kredi"].Value),
                Convert.ToInt32(dgSonSatislar.Rows[e.RowIndex].Cells["Taksit"].Value));

            SonSatis.ShowDialog();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void label10_MouseHover(object sender, EventArgs e)
        {
            label10.Visible = true;
        }

        private void label10_MouseLeave(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void rbNakit_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNakit.Checked)
            {
                rbKrediKarti.Checked = false;
                rbNakit.BackColor = lblSistemAdi.BackColor;
                tableLayoutPanel25.BackColor = Color.AliceBlue;
                cbOdenenNakit.Enabled = true;
                txtOdenenKredi.Enabled = false;
                cbTaksit.Enabled = false;
                cbOdenenNakit.Focus();
            }
            else
            {
                rbNakit.BackColor = tableLayoutPanel25.BackColor = rbKrediKarti.BackColor;
            }
        }

        private void rbKrediKarti_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKrediKarti.Checked)
            {
                rbNakit.Checked = false;
                rbKrediKarti.BackColor = lblSistemAdi.BackColor;
                tableLayoutPanel26.BackColor = Color.AliceBlue;
                cbOdenenNakit.Enabled = false;
                txtOdenenKredi.Enabled = true;
                cbTaksit.Enabled = true;
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }
            else
            {
                rbKrediKarti.BackColor = tableLayoutPanel26.BackColor = rbNakit.BackColor;
            }
        }

        private void txtOdenenKredi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtOdenenKredi.Text.IndexOf(',') == -1) d = true;
            if (e.KeyChar == (char)44 && d)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                d = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void cbOdenenNakit_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (cbOdenenNakit.Text.IndexOf(',') == -1) f = true;
            if (e.KeyChar == (char)44 && f)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                f = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void ParaUstuHesapla()
        {
            var odenen = odenenNakit + odenenKredi;
            paraUstu = odenen - toplam;

            if (paraUstu < 0)
                lblParaUstu.Text = (-paraUstu).ToString("F2") + " TL alacaklısınız.";
            else
                lblParaUstu.Text = " Para Üstü : " + paraUstu.ToString("F2") + " TL";
        }

        private void UrunSorgula()
        {
            _searchingProducs = true;
            try //Her ihtimale karşı miktarın sayısal girilip girilmediğini kontrol et
            {
                if (Convert.ToDecimal(cbMiktar.Text) > 0) //Miktar 0'dan büyükse işlem yap değilse uyar ve çık
                {
                    seciliMiktar = Convert.ToDecimal(cbMiktar.Text);
                }
                else
                {
                    MessageBox.Show("Lütfen miktarı pozitif tamsayı olarak girin.", "Veri Girişi Hatası",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbMiktar.Text = "1";
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen miktarı sayısal olarak girin.", "Veri Girişi Hatası", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var param = new SqlParameter[1];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@BarkodKodu";
            param[0].SqlDbType = SqlDbType.NVarChar;
            param[0].SqlValue = txtBarkodOku.Text;
            var dReader = SqlOperation.OkuProcedure("URUNSORGULAMA", param);
            try
            {
                if (dReader != null)
                {
                    if (dReader.HasRows)
                    {
                        dReader.Read();
                        var durum = MiktarHesapla(txtBarkodOku.Text, Convert.ToDecimal(dReader[3]),
                            seciliMiktar); //Seçilen miktarın yeterli mi fazla mı olduğunu kontrol et

                        if (durum == miktarDurum.sepette_yok_yeterli) //Sepette yoksa ekle
                        {
                            var satir = new string[dgSepet.ColumnCount];
                            satir[0] = (dgSepet.RowCount + 1).ToString(); //Satır numarası
                            satir[1] = txtBarkodOku.Text; //Barkod kodu
                            satir[2] = dReader[1].ToString(); // Ürün Adı
                            satir[3] = dReader[2].ToString(); //Birim Fiyatı
                            satir[4] = seciliMiktar.ToString(); //Miktarı
                            satir[5] = dReader[4].ToString(); //Birimi
                            satir[6] = dReader[5]
                                .ToString(); //Maliyeti   (Visible özelliği false olan -Görünmez olan- kolona gir.)
                            satir[7] = dReader[6]
                                .ToString(); //Birim Id'si(Visible özelliği false olan -Görünmez olan- kolona gir.)
                            satir[8] = (seciliMiktar * Convert.ToDecimal(dReader[2])).ToString("F2"); //Toplam Tutar
                            dgSepet.Rows.Add(satir); //Satır dizisini ekle
                            dgSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                            ToplamHesapla(); //Toplam tutarı hesapla
                            cbMiktar.Text = "1"; //İlk stok sayısını gir
                        }
                        else if
                            (durum == miktarDurum.sepette_var_yeterli) //Sepette varsa miktarı ve toplam fiyatı güncelle
                        {
                            dgSepet.Rows[satirIndex].Cells["Miktar"].Value =
                                Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value) + seciliMiktar;
                            var ondalik = Convert.ToInt32(dgSepet.Rows[satirIndex].Cells["Miktar"].Value);
                            var kesirli = Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value);

                            if (ondalik - kesirli == 0)
                                dgSepet.Rows[satirIndex].Cells["Miktar"].Value = Convert
                                    .ToInt32(dgSepet.Rows[satirIndex].Cells["Miktar"].Value).ToString();
                            dgSepet.Rows[satirIndex].Cells["TopTutar"].Value =
                                (Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value) *
                                 Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["BirimFiyat"].Value)).ToString("F2");
                            ToplamHesapla(); //Toplam tutarı hesapla
                            cbMiktar.Text = "1"; //İlk stok sayısını gir
                        }
                        else if
                            (durum == miktarDurum
                                .sepette_var_yetersiz) //Yoksa sepette var ve miktar yetersizse diye uyar ve en fazla ne kadar girmesi gerektiğini söyle
                        {
                            if (Convert.ToDecimal(dReader[3]) -
                                Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value) >
                                0) //Eğer kalan miktar 0 dan büyükse, farklı 0'a eşitse farklı uyarı ver
                            {
                                MessageBox.Show(
                                    "İstenilen ürün yeterli miktarda yok. En fazla " +
                                    (Convert.ToDecimal(dReader[3]) -
                                     Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value)) + " " +
                                    dReader[4].ToString().ToLower() + " seçebilirsiniz.", "Stok Uyarısı",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                cbMiktar.Text = (Convert.ToDecimal(dReader[3]) -
                                                 Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value))
                                    .ToString();
                            }
                            else
                            {
                                MessageBox.Show(
                                    "Bu ürün zaten sepete eklendi. Bu üründen daha fazla ekleyebilmeniz için stokta kalmadı.",
                                    "Stok Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else //Bunların hiçbiri değilse sepette yok ve yetersizdir. Uyar ve en fazla seçebileceği miktarı yaz.
                        {
                            if (Convert.ToDecimal(dReader[3]) >
                                0) //Sepette yok,0'dan büyük ve istenilen miktar kadar stokta yoksa uyar ve combobox text özelliğine max miktarı gir.
                            {
                                MessageBox.Show(
                                    "İstenilen ürün yeterli miktarda yok. En fazla " + dReader[3] + " " +
                                    dReader[4].ToString().ToLower() + " seçebilirsiniz.", "Stok Uyarısı",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                cbMiktar.Text = dReader[3].ToString();
                            }
                            else //Sepette yok ve stok 0'a eşitse.
                            {
                                MessageBox.Show("Bu ürün stokta kalmadı.", "Stok Hatası", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(txtBarkodOku.Text + " kodlu ürün bulunamadı.", "Dikkat!", MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    txtBarkodOku.Text = ""; //Barkod kodu girişini sıfırla
                    DbOperations.Connection.Close();
                    SqlOperation.cmd.Dispose();
                }
                else
                {
                    DbOperations.Connection.Close();
                    SqlOperation.cmd.Dispose();
                }
            }
            catch
            {
            }
            finally
            {
                DbOperations.Connection.Close();
                SqlOperation.cmd.Dispose();
                _searchingProducs = false;
            }
        }

        private void txtOdenenKredi_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtOdenenKredi.Text == "") odenenKredi = 0;

                if (txtOdenenKredi.Text == ",")
                {
                    txtOdenenKredi.Text = "0,";
                    txtOdenenKredi.SelectionStart = txtOdenenKredi.Text.Length;
                }

                odenenKredi = Convert.ToDecimal(txtOdenenKredi.Text);
            }
            catch
            {
                txtOdenenKredi.Text = "";
                odenenKredi = 0;
            }

            var diger = toplam - odenenNakit;
            if (odenenKredi > diger)
            {
                if (odenenKredi != 0 && odenenNakit != 1)
                    txtOdenenKredi.Text = diger.ToString();
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }

            if (txtOdenenKredi.Text == "0")

            {
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }

            ParaUstuHesapla();
        }

        private void cbOdenenNakit_KeyDown(object sender, KeyEventArgs e)
        {
            if (paraUstu < 0 && e.KeyCode == Keys.Enter)
            {
                txtOdenenKredi.Text = (odenenKredi - paraUstu).ToString();
                rbKrediKarti.Checked = true;
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }
            else if (e.KeyCode == Keys.Right)
            {
                rbKrediKarti.Checked = true;
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }

            else if (e.KeyCode == Keys.F1)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            odenenKredi = 0;
            rbNakit.Checked = true;
            txtOdenenKredi.Text = "0";
            cbOdenenNakit.Text = toplam.ToString();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            odenenNakit = 0;
            rbKrediKarti.Checked = true;
            cbOdenenNakit.Text = "0";
            txtOdenenKredi.Text = toplam.ToString();
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void cbOdenenNakit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbOdenenNakit.Text == "") odenenNakit = 0;
                if (cbOdenenNakit.Text == ",")
                {
                    cbOdenenNakit.Text = "0,";
                    cbOdenenNakit.SelectionStart = cbOdenenNakit.Text.Length;
                }

                odenenNakit = Convert.ToDecimal(cbOdenenNakit.Text);
            }
            catch
            {
                foreach (var item in cbOdenenNakit.Items)
                {
                    if (Convert.ToInt32(item.ToString()) > toplam)
                    {
                        cbOdenenNakit.SelectedItem = item;
                        break;
                    }

                    cbOdenenNakit.SelectedItem = item;

                    ;
                }

                odenenNakit = Convert.ToDecimal(cbOdenenNakit.Text);
                cbOdenenNakit.Focus();
            }

            ParaUstuHesapla();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var satis = frmSatis.GetFrmSatis(this);
            satis.Show();
            satis.WindowState = FormWindowState.Maximized;
            Hide();
        }

        private void txtBarkodOku_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                cbMiktar.Select();
            if (e.KeyCode == Keys.Enter && txtBarkodOku.Text != "" && !_searchingProducs)
                UrunSorgula();
        }

        private void txtOdenenKredi_KeyDown(object sender, KeyEventArgs e)
        {
            if (paraUstu < 0 && e.KeyCode == Keys.Enter)
            {
                cbOdenenNakit.Text = (-paraUstu + odenenNakit).ToString();
                rbNakit.Checked = true;
                cbOdenenNakit.Focus();
                cbOdenenNakit.SelectAll();
            }
            else if (e.KeyCode == Keys.Left)
            {
                rbNakit.Checked = true;
                cbOdenenNakit.Focus();
            }
            else if (e.KeyCode == Keys.F1)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void btnHepsi_Click(object sender, EventArgs e)
        {
            txtBorc.Text = Convert.ToDecimal(HarfleriSil()).ToString();
        }

        private bool MusteriKontrolEt()
        {
            if (chckMusteriKaydet.Checked)
            {
                if (txtMusteriAdi.Text != "" && mtxtTelefon.Text != "")
                {
                    if (chkBorc.Visible && txtBorc.Text != "")
                        try
                        {
                            if (Convert.ToDecimal(txtBorc.Text) > 0)
                                Borc = Convert.ToDecimal(txtBorc.Text);
                            else
                                Borc = 0;
                        }
                        catch
                        {
                            MessageBox.Show("Lütfen sayısal bir değer girin.", "Veri Girişi Hatası",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    else
                        Borc = 0;
                }
                else
                {
                    Borc = 0;
                }

                var MusteriBilgisi = new SqlParameter[4];
                MusteriBilgisi[0] = new SqlParameter();
                MusteriBilgisi[0].ParameterName = "@Adi";
                MusteriBilgisi[0].SqlDbType = SqlDbType.NVarChar;
                MusteriBilgisi[0].SqlValue = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtMusteriAdi.Text);
                MusteriBilgisi[1] = new SqlParameter();
                MusteriBilgisi[1].ParameterName = "@Telefon";
                MusteriBilgisi[1].SqlDbType = SqlDbType.NVarChar;
                MusteriBilgisi[1].SqlValue = mtxtTelefon.Text;
                MusteriBilgisi[2] = new SqlParameter();
                MusteriBilgisi[2].ParameterName = "@AlisverisTutari";
                MusteriBilgisi[2].SqlDbType = SqlDbType.Decimal;
                MusteriBilgisi[2].SqlValue = HarfleriSil();
                MusteriBilgisi[3] = new SqlParameter();
                MusteriBilgisi[3].ParameterName = "@Borc";
                MusteriBilgisi[3].SqlDbType = SqlDbType.Decimal;
                MusteriBilgisi[3].SqlValue = Borc;
                string sonuc;

                sonuc = SqlOperation.OkuScalar("MUSTERIEKLE", CommandType.StoredProcedure, MusteriBilgisi).ToString();
                try
                {
                    sonMusteriAdi = sonuc.Remove(0, sonuc.IndexOf('_') + 1);
                    musteriId = Convert.ToInt32(sonuc.Remove(sonuc.IndexOf('_')));
                }
                catch (Exception e)
                {
                    musteriId = Convert.ToInt32(sonuc);
                    if (e.Message.IndexOf("StartIndex") > -1)
                        SqlOperation.TextCommand(
                            "ALTER TRIGGER  [dbo].[MUSTERIIDSINIGETIR] ON  [dbo].[Musteriler] AFTER INSERT,UPDATE AS BEGIN SET NOCOUNT ON;Select CAST((Select Id from Inserted)AS nvarchar(4))+'_'+Adi From Inserted;SET NOCOUNT OFF;END");
                }
                finally
                {
                    DbOperations.Connection.Close();
                    SqlOperation.cmd.Dispose();
                }

                return true;
            }

            Borc = 0;
            return true;
        }

        private decimal HarfleriSil()
        {
            var toplam = lblToplam.Text.Remove(lblToplam.Text.Length - 2, 2);
            toplam = toplam.Remove(0, 9);
            return Convert.ToDecimal(toplam);
        }
    }
}
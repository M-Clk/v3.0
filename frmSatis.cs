using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using static Otomasyon.frmAnaForm;

namespace Otomasyon
{
    public partial class frmSatis :Form
    {
        int satirIndex;
        private bool a = true;
        OrtakIslemler islemYap = new OrtakIslemler();
        private static frmSatis singletonSatis;
        decimal toplam = 0, Borc = 0, seciliMiktar, odenenNakit = 0, odenenKredi = 0, paraUstu = 0, varsayilanMiktar = 1;
        private static frmAnaForm _frmAna;
        int[] taksitAylari = new int[5];
        private frmSatis()
        {
            InitializeComponent();
            taksitAylari[0] = 0;
            taksitAylari[1] = 6;
            taksitAylari[2] = 12;
            taksitAylari[3] = 24;
            taksitAylari[4] = 36;
        }
        public static frmSatis GetFrmSatis(frmAnaForm frmAna)
        {
            if(singletonSatis == null || (singletonSatis?.IsDisposed ?? true))
                singletonSatis = new frmSatis();
            _frmAna = frmAna;
            return singletonSatis;
        }
        DbOperations SqlOperation = new DbOperations();

        private void btnSatisYap_Click(object sender, EventArgs e)
        {
            if (paraUstu < 0)
            {
                MessageBox.Show("Ücret tamamen ödenmedi. " + (-paraUstu).ToString() + " TL ödenmesi gerek. Lütfen satış tutarı tamamen ödendikten sonra tekrar deneyin.", "Satış Tutarı Tamamen Ödenmedi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                rbNakit.Checked = true;
                return;
            }
            if (!Program.lisans)
            {
                int satisSayisi = Convert.ToInt32(SqlOperation.ScalarTextCommand("Select COUNT(Id) From Satislar"));
                try
                {
                    if (satisSayisi < 10)
                    {
                        islemYap.LisansUyarisi("Lisanssız yazılım kullanıyorsunuz. En fazla 10 satış yapabilirsiniz. " + (9 - satisSayisi) + " hakkınız kaldı. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.");
                    }
                    else
                    {
                        islemYap.LisansUyarisiMesaj("Lisanssız yazılım kullanıyorsunuz. Maksimum satış limitinize ulaştınız. Artık satış yapamazsınız. Eğer ürün anahtarınız varsa limitsiz kullanım için etkinleştirin.");
                        return;
                    }
                }
                catch
                {
                }
            }

            var sonMusteriAdi = "Kaydedilmedi";
            var musteriId = 0;
            SatisiYap();
            SatisDetayiEkle();
            //if (cbYazdir.Checked)
            //    DetayiYazdir();
            //DetayiExceleAktar();
            //Stok güncellemesi satış detayı eklendiğinde trigger tetiklenecek ve otomatik güncelleyecektir.
            //İşin çoğu bölümü SQL Server da yapılacak
            //Eğer yapılabilirse datagrid deki tüm parametlereler SqlParameterCollection nesnesi yardımı ile birden gönderilecek.
            //Satış ekleme procedure ünde başlayacak herşey. Satış id si ExecuteScalar yardımı ile alınıp satış detayına parametre olarak gönderilecek. 
            dgSepet.Rows.Clear();
            ToplamHesapla();
            Etkisizlestir();
            //frmMesaj mesajVer = new frmMesaj("Satış başarıyla gerçekleşti.", "Başarılı İşlem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //mesajVer.Show();
            _frmAna.nfBasarili.BalloonTipText = "Satış başarıyla gerçekleşti.";
            _frmAna.nfBasarili.Visible = true;
            _frmAna.nfBasarili.ShowBalloonTip(2000);
            cbYazdir.Checked = false;
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }
        void SatisDetayiEkle()
        {
            SqlParameter[] detayParametresi = new SqlParameter[4];
            detayParametresi[0] = new SqlParameter();
            detayParametresi[0].ParameterName = "@Id";
            detayParametresi[0].SqlDbType = SqlDbType.Int;
            detayParametresi[0].SqlValue = _frmAna.sonSatisId;
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
        void SatisiYap()
        {
            decimal toplamMaliyet = 0;
            for (int i = 0; i < dgSepet.Rows.Count; i++)
            {
                toplamMaliyet += Convert.ToDecimal(dgSepet.Rows[i].Cells[6].Value) * Convert.ToDecimal(dgSepet.Rows[i].Cells["Miktar"].Value);
            }
            SqlParameter[] paramtr = new SqlParameter[9];

            paramtr[0] = new SqlParameter();
            paramtr[0].SqlDbType = SqlDbType.Int;
            paramtr[0].ParameterName = "@MusteriId";
            paramtr[0].SqlValue = 0;

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

            _frmAna.sonSatisId = Convert.ToInt32(SqlOperation.OkuScalar("SATISEKLE", CommandType.StoredProcedure, paramtr));
            Program.satisYapildi = true;
        }

        private void frmSatis_Load(object sender, EventArgs e)
        {
            var nameId = 1;
            while (cbMiktar.Items.Count < 100)
                cbMiktar.Items.Add(cbMiktar.Items.Count + 1);
            cbMiktar.SelectedIndex = 0;
            SqlParameter[] parameter = new SqlParameter[1];
            parameter[0] = new SqlParameter();
            parameter[0].ParameterName = "@UrunSayisi";
            parameter[0].SqlDbType = SqlDbType.TinyInt;
            parameter[0].SqlValue = Program.quickProductsCount;
            SqlDataReader dReader = SqlOperation.OkuProcedure("HIZLIURUNLERIGETIR", parameter);
            while(dReader.Read())
            {
                var newButton = new Button();
                newButton.Text = dReader["Adi"].ToString();
                newButton.Tag = dReader["Bakod_kodu"].ToString();
                newButton.Name = $"button{nameId++}";
                newButton.Size = new Size(150, 80);
                newButton.Click += new EventHandler(ProductClick);
                flowLayoutPanel1.Controls.Add(newButton);
            }
        }
        decimal HarfleriSil()
        {
            string toplam = lblToplam.Text.Remove(lblToplam.Text.Length - 2, 2);
            toplam = toplam.Remove(0, 9);
            return Convert.ToDecimal(toplam);
        }
        private void frmSatis_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.F1 || e.KeyCode == Keys.Enter)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }
        void Etkinlestir()
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
            //txtMusteriAdi.Enabled = true;
            //mtxtTelefon.Enabled = true;
            //chckMusteriKaydet.Enabled = true;
            btnSatisYap.Enabled = true;
            btnSatisIptalEt.Enabled = true;
            cbYazdir.Visible = true;
            cbYazdir.Checked = Program.herSatistaYazdir;
            lblParaUstu.Visible = true;
            cbTaksit.SelectedIndex = 0;
            cbOdenenNakit.SelectedIndex = 0;
            txtOdenenKredi.Text = "0";
        }
        void Etkisizlestir()
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
            //txtMusteriAdi.Enabled = false;
            //txtMusteriAdi.Text = "";
            //mtxtTelefon.Enabled = false;
            //mtxtTelefon.Text = "";
            //chckMusteriKaydet.Enabled = false;
            //chckMusteriKaydet.Checked = false;
            btnSatisYap.Enabled = false;
            btnSatisIptalEt.Enabled = false;
            cbYazdir.Visible = false;
            odenenKredi = 0;
            odenenNakit = 0;
            paraUstu = 0;
        }
        private void dgSepet_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex != -1 && e.ColumnIndex != 4)
            {
                if(e.RowIndex < dgSepet.Rows.Count)
                    for(int i = e.RowIndex; i < dgSepet.Rows.Count; i++)
                        dgSepet.Rows[i].Cells[0].Value = i;//Bu satırın altındaki satırların numarasını 1 azalt.
                dgSepet.Rows.Remove(dgSepet.Rows[e.RowIndex]);
                ToplamHesapla(); //Toplam tutarı hesapla
                if(dgSepet.RowCount <= 0) //Eğer satır kalmadıysa herseyi pasifize et
                    Etkisizlestir();
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if(dgSepet.Rows.Count == 1)
                Etkinlestir();
        }

        private void frmSatis_FormClosing(object sender, FormClosingEventArgs e)
        {
            _frmAna.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgSepet_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != 4)
            {
                if (e.RowIndex < dgSepet.Rows.Count)
                    for (int i = e.RowIndex; i < dgSepet.Rows.Count; i++)
                        dgSepet.Rows[i].Cells[0].Value = i;//Bu satırın altındaki satırların numarasını 1 azalt.
                dgSepet.Rows.Remove(dgSepet.Rows[e.RowIndex]);
                ToplamHesapla(); //Toplam tutarı hesapla
                if (dgSepet.RowCount <= 0) //Eğer satır kalmadıysa herseyi pasifize et
                    Etkisizlestir();
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgSepet_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
                dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
        }

        private void dgSepet_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex == 4) //Kayıt var mı kontrol et ve kayıt eklenme durumu değilse
            {
                SqlParameter[] parameter = new SqlParameter[1];
                parameter[0] = new SqlParameter();
                parameter[0].ParameterName = "@BarkodKodu";
                parameter[0].SqlDbType = SqlDbType.NVarChar;
                parameter[0].SqlValue = dgSepet.Rows[e.RowIndex].Cells[1].Value;
                using (SqlDataReader dReader = SqlOperation.OkuProcedure("URUNSORGULAMA", parameter)) //...Seçili ürünü getir
                {

                    try //Sayı dışında bir değer girilirse hata verir. Bunu yakala ve kullanıcıyı uyar.
                    {
                        dReader.Read();
                        if (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) > Convert.ToDecimal(dReader[3])) //Yeni değer stoktan büyükse uyar, max ile değiştir.
                        {
                            MessageBox.Show("Bu üründen stokta " + dReader[3].ToString() + " " + dReader[4].ToString().ToLower() + " var. Daha fazla seçilemez. ", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dReader[3].ToString();
                            dgSepet.Rows[e.RowIndex].Selected = true;
                        }
                        else if (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) <= 0) //Yeni değer 0 veya negatif ise hata mesajı göster 
                        {
                            MessageBox.Show("Miktar negatif veya 0 girilemez. Ürünü sepetten çıkarmak için ürünü seçip çift tıklayabilirsiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = varsayilanMiktar;//Eski değerini gir.
                        }
                        else //Bunlardan hiçbiri değilse hatasız veri girişi yapılmıştır toplamı güncelle
                        {
                            dgSepet.Rows[e.RowIndex].Cells["TopTutar"].Value = (Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells["Miktar"].Value) * Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells["BirimFiyat"].Value)).ToString("F2");
                            ToplamHesapla();
                        }
                    }
                    catch (Exception)
                    {
                        if (dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString() == "Adet")
                        //Birimi adet ise doğal sayı gir uyar eski değer ile değiştir
                        {
                            MessageBox.Show("Doğal sayı dışında veri girildi. Adet yalnız doğal sayı olmalıdır.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = varsayilanMiktar;
                        }
                        else
                        //
                        {
                            MessageBox.Show("Miktar sayısal olmalıdır. ", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dgSepet_Click(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dgSepet.Rows.Count == 0)
            {
                Etkisizlestir();
            }
        }

        private void dgSepet_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void dgSepet_MouseLeave(object sender, EventArgs e)
        {
            dgSepet.SelectionMode = DataGridViewSelectionMode.CellSelect;
            label7.Visible = false;
        }

        private void dgSepet_MouseHover(object sender, EventArgs e)
        {
            dgSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            label7.Visible = true;
        }

        private void dgSepet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.Enter)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

        private void dgSepet_Enter(object sender, EventArgs e)
        {
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void txtBarkodOku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                cbMiktar.Select();
            if (e.KeyCode == Keys.Enter && txtBarkodOku.Text != "")
            UrunSorgula();
        }

        void UrunSorgula()
        {
            try //Her ihtimale karşı miktarın sayısal girilip girilmediğini kontrol et
            {
                if (Convert.ToDecimal(cbMiktar.Text) > 0) //Miktar 0'dan büyükse işlem yap değilse uyar ve çık
                {
                    seciliMiktar = Convert.ToDecimal(cbMiktar.Text);
                }
                else
                {
                    MessageBox.Show("Lütfen miktarı pozitif tamsayı olarak girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbMiktar.Text = "1";
                    return;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Lütfen miktarı sayısal olarak girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UrunSorgula(txtBarkodOku.Text);
        }

        private void txtBarkodOku_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
                cbMiktar.Select();
            if (e.KeyCode == Keys.Enter && txtBarkodOku.Text != "")
                UrunSorgula();
        }

        private void cbMiktar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Left || e.KeyCode == Keys.Space || e.KeyCode == Keys.RShiftKey || e.KeyCode == Keys.Left)
            {
                txtBarkodOku.Text = "";
                txtBarkodOku.Select();
            }
        }

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

        private void btnSatisIptalEt_Click(object sender, EventArgs e)
        {
            DialogResult sil = MessageBox.Show("Satış iptal edilirse sepet boşaltılacak. İşleminiz sıfırlanacak. Satışı iptal etmek istediğinizden emin misiniz?", "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sil == DialogResult.Yes)
            {
                Etkisizlestir();
                dgSepet.Rows.Clear();
                ToplamHesapla();
            }
            txtBarkodOku.Text = "";
            txtBarkodOku.Select();
        }

        private void rbNakit_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNakit.Checked)
            {
                rbKrediKarti.Checked = false;
                tableLayoutPanel25.BackColor = Color.AliceBlue;
                cbOdenenNakit.Enabled = true;
                txtOdenenKredi.Enabled = false;
                cbTaksit.Enabled = false;
                cbOdenenNakit.Focus();
            }
            else
                rbNakit.BackColor = tableLayoutPanel25.BackColor = rbKrediKarti.BackColor;
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

        private void rbKrediKarti_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKrediKarti.Checked)
            {
                rbNakit.Checked = false;
                tableLayoutPanel26.BackColor = Color.AliceBlue;
                cbOdenenNakit.Enabled = false;
                txtOdenenKredi.Enabled = true;
                cbTaksit.Enabled = true;
                txtOdenenKredi.Focus();
                txtOdenenKredi.SelectAll();
            }
            else
                rbKrediKarti.BackColor = tableLayoutPanel26.BackColor = rbNakit.BackColor;
        }

        bool d = true;
        private void txtOdenenKredi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtOdenenKredi.Text.IndexOf(',') == -1)
            {
                d = true;

            }
            if (e.KeyChar == (char)44 && d == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                d = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }
        bool f = true;
        private void cbOdenenNakit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbOdenenNakit.Text.IndexOf(',') == -1)
            {
                f = true;
            }
            if (e.KeyChar == (char)44 && f == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                f = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }

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
            { txtBarkodOku.Text = ""; txtBarkodOku.Select(); }

        }

        private void cbOdenenNakit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbOdenenNakit.Text == "")
                {
                    odenenNakit = 0;
                }
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
                    { cbOdenenNakit.SelectedItem = item; break; }
                    else
                        cbOdenenNakit.SelectedItem = item;
                    ;
                }
                odenenNakit = Convert.ToDecimal(cbOdenenNakit.Text);
                cbOdenenNakit.Focus();
            }
            ParaUstuHesapla();
        }

        private void txtOdenenKredi_TextChanged(object sender, EventArgs e)
        {

            try
            {
                if (txtOdenenKredi.Text == "")
                {
                    odenenKredi = 0;
                }

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
            decimal diger = toplam - odenenNakit;
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
            { rbNakit.Checked = true; cbOdenenNakit.Focus(); }
            else if (e.KeyCode == Keys.F1)
            { txtBarkodOku.Text = ""; txtBarkodOku.Select(); }
        }

        private void txtOdenenKredi_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (txtOdenenKredi.Text.IndexOf(',') == -1)
            {
                d = true;

            }
            if (e.KeyChar == (char)44 && d == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                d = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void dgSepet_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1 && e.ColumnIndex == 4) // Miktar tıklandı ise yanlış işlem sonucu eski değerini girmek için kaydet
            {
                varsayilanMiktar = Convert.ToDecimal(dgSepet.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                dgSepet.Focus();
            }
            else
            { txtBarkodOku.Text = ""; txtBarkodOku.Select(); }
        }

        private void ProductClick(object sender, EventArgs e)
        {
            seciliMiktar = 1;
            var barcode = (string)((Button)sender).Tag;
            UrunSorgula(barcode);
        }
        miktarDurum MiktarHesapla(string kod, decimal miktar, decimal istenenMiktar) //Sorgulanan ürünün miktarının yeterliliğini kontrol et ve sepette varsa seçilen miktarın eklemesi için yoksa sepete eklemesi için enum gönder
        {
            for(int i = 0; i < dgSepet.RowCount; i++)//Barkod kodunu sepettekilerle karşılaştır
            {
                if(dgSepet.Rows[i].Cells[1].Value.ToString() == kod) //Kod sepette varsa işlem yap
                {
                    if(Convert.ToDecimal(dgSepet.Rows[i].Cells["Miktar"].Value) + istenenMiktar > miktar)//istenen miktar stoktaki ve sepettikinin toplamında fazla ise onun anlamına gelen enumu gönder
                    {
                        return miktarDurum.sepette_var_yetersiz;
                    }
                    else //Sepette var yeterliyse onun anlamına gelen enumu gönder
                    {
                        satirIndex = i;
                        return miktarDurum.sepette_var_yeterli;
                    }
                }
            }

            if(istenenMiktar > miktar)
                return miktarDurum.sepette_yok_yetersiz;//istenen miktar sepette yok ve yetersizse onun anlamına gelen enumu gönder
            else
                return miktarDurum.sepette_yok_yeterli; //bunlardan hiçbiri değilse tek enum kaldı onu gönder
        }
        void ToplamHesapla() //Toplam fiyat tutarını hesapla
        {

            toplam = 0;
            for(int i = 0; i < dgSepet.Rows.Count; i++)
            {
                toplam += Convert.ToDecimal(dgSepet.Rows[i].Cells["TopTutar"].Value);
                if(i % 2 == 0)
                    dgSepet.Rows[i].DefaultCellStyle.BackColor = Color.LightPink;
                else
                    dgSepet.Rows[i].DefaultCellStyle.BackColor = Color.LavenderBlush;
            }


            lblToplam.Text = "Toplam : " + toplam.ToString("F2") + " ₺";
            if(Program.odemeTipi == 1)
            {
                odenenKredi = 0;
                rbNakit.Checked = true;
                txtOdenenKredi.Text = "0";
                cbOdenenNakit.Text = toplam.ToString();
            }
            else if(Program.odemeTipi == 2)
            {
                odenenNakit = 0;
                rbKrediKarti.Checked = true;
                cbOdenenNakit.Text = "0";
                txtOdenenKredi.Text = toplam.ToString();
            }
            ParaUstuHesapla();


        }
        void ParaUstuHesapla()
        {

            decimal odenen = odenenNakit + odenenKredi;
            paraUstu = odenen - toplam;

            if(paraUstu < 0)
                lblParaUstu.Text = (-paraUstu).ToString("F2") + " TL alacaklısınız.";
            else
                lblParaUstu.Text = " Para Üstü : " + paraUstu.ToString("F2") + " TL";

        }
        void UrunSorgula(string barcode)
        {
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter();
            param[0].ParameterName = "@BarkodKodu";
            param[0].SqlDbType = SqlDbType.NVarChar;
            param[0].SqlValue = barcode;
            SqlDataReader dReader = SqlOperation.OkuProcedure("URUNSORGULAMA", param);
            try
            {
                if(dReader != null)
                {
                    if(dReader.HasRows)
                    {
                        dReader.Read();
                        miktarDurum durum = MiktarHesapla(barcode, Convert.ToDecimal(dReader[3]), seciliMiktar); //Seçilen miktarın yeterli mi fazla mı olduğunu kontrol et

                        if(durum == miktarDurum.sepette_yok_yeterli) //Sepette yoksa ekle
                        {
                            string[] satir = new string[dgSepet.ColumnCount];
                            satir[0] = (dgSepet.RowCount + 1).ToString(); //Satır numarası
                            satir[1] = barcode; //Barkod kodu
                            satir[2] = dReader[1].ToString(); // Ürün Adı
                            satir[3] = dReader[2].ToString(); //Birim Fiyatı
                            satir[4] = seciliMiktar.ToString(); //Miktarı
                            satir[5] = dReader[4].ToString(); //Birimi
                            satir[6] = dReader[5].ToString(); //Maliyeti   (Visible özelliği false olan -Görünmez olan- kolona gir.)
                            satir[7] = dReader[6].ToString(); //Birim Id'si(Visible özelliği false olan -Görünmez olan- kolona gir.)
                            satir[8] = (seciliMiktar * (Convert.ToDecimal(dReader[2]))).ToString("F2"); //Toplam Tutar
                            dgSepet.Rows.Add(satir); //Satır dizisini ekle
                            dgSepet.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                            ToplamHesapla(); //Toplam tutarı hesapla
                            cbMiktar.Text = "1"; //İlk stok sayısını gir

                        }
                        else if(durum == miktarDurum.sepette_var_yeterli)//Sepette varsa miktarı ve toplam fiyatı güncelle
                        {
                            dgSepet.Rows[satirIndex].Cells["Miktar"].Value = Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value) + seciliMiktar;
                            int ondalik = Convert.ToInt32(dgSepet.Rows[satirIndex].Cells["Miktar"].Value);
                            decimal kesirli = Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value);

                            if(ondalik - kesirli == 0)
                                dgSepet.Rows[satirIndex].Cells["Miktar"].Value = Convert.ToInt32(dgSepet.Rows[satirIndex].Cells["Miktar"].Value).ToString();
                            dgSepet.Rows[satirIndex].Cells["TopTutar"].Value = (Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value) * Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["BirimFiyat"].Value)).ToString("F2");
                            ToplamHesapla(); //Toplam tutarı hesapla
                            cbMiktar.Text = "1"; //İlk stok sayısını gir

                        }
                        else if(durum == miktarDurum.sepette_var_yetersiz)//Yoksa sepette var ve miktar yetersizse diye uyar ve en fazla ne kadar girmesi gerektiğini söyle
                        {
                            if((Convert.ToDecimal(dReader[3]) - Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value)) > 0) //Eğer kalan miktar 0 dan büyükse, farklı 0'a eşitse farklı uyarı ver
                            {
                                MessageBox.Show("İstenilen ürün yeterli miktarda yok. En fazla " + (Convert.ToDecimal(dReader[3]) - Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value)).ToString() + " " + dReader[4].ToString().ToLower() + " seçebilirsiniz.", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                cbMiktar.Text = (Convert.ToDecimal(dReader[3]) - Convert.ToDecimal(dgSepet.Rows[satirIndex].Cells["Miktar"].Value)).ToString();

                            }
                            else
                                MessageBox.Show("Bu ürün zaten sepete eklendi. Bu üründen daha fazla ekleyebilmeniz için stokta kalmadı.", "Stok Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                        else //Bunların hiçbiri değilse sepette yok ve yetersizdir. Uyar ve en fazla seçebileceği miktarı yaz.
                        {
                            if(Convert.ToDecimal(dReader[3]) > 0) //Sepette yok,0'dan büyük ve istenilen miktar kadar stokta yoksa uyar ve combobox text özelliğine max miktarı gir.
                            {
                                MessageBox.Show("İstenilen ürün yeterli miktarda yok. En fazla " + dReader[3].ToString() + " " + dReader[4].ToString().ToLower() + " seçebilirsiniz.", "Stok Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                cbMiktar.Text = dReader[3].ToString();

                            }
                            else //Sepette yok ve stok 0'a eşitse.
                            {
                                MessageBox.Show("Bu ürün stokta kalmadı.", "Stok Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                    }
                    else
                    {
                        MessageBox.Show(txtBarkodOku.Text + " kodlu ürün bulunamadı.", "Dikkat!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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
            }
        }
    }
}

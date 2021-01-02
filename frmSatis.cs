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
using static Otomasyon.frmAnaForm;

namespace Otomasyon
{
    public partial class frmSatis :Form
    {
        int satirIndex;
        private static frmSatis singletonSatis;
        decimal toplam = 0, Borc = 0, seciliMiktar, odenenNakit = 0, odenenKredi = 0, paraUstu = 0, varsayilanMiktar = 1;
        private frmSatis()
        {
            InitializeComponent();
        }
        public static frmSatis GetFrmSatis()
        {
            if(singletonSatis == null || (singletonSatis?.IsDisposed ?? true))
                singletonSatis = new frmSatis();
            return singletonSatis;
        }
        DbOperations SqlOperation = new DbOperations();

        private void btnSatisYap_Click(object sender, EventArgs e)
        {

        }

        private void frmSatis_Load(object sender, EventArgs e)
        {
            var nameId = 1;
            SqlDataReader dReader = SqlOperation.OkuProcedure("HIZLIURUNLERIGETIR", null);
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
            label4.Visible = false;
            txtBorc.Visible = false;
            btnHepsi.Visible = false;
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
                    SqlOperation.con.Dispose();
                    SqlOperation.cmd.Dispose();

                }
                else
                {
                    SqlOperation.con.Dispose();
                    SqlOperation.cmd.Dispose();
                    return;
                }
            }
            catch
            {
            }
            finally
            {
                SqlOperation.con.Dispose();
                SqlOperation.cmd.Dispose();
            }
        }
    }
}

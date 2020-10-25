﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Win32;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;

namespace Otomasyon
{
    public partial class frmStokIslemleri : Form
    {
        private ExcelPackage _package;
        public frmStokIslemleri()
        {
            InitializeComponent();
        }
        DbOperations SqlBaglantisi = new DbOperations();
        private void tmYanipSonme_Tick(object sender, EventArgs e)
        {
            label11.Visible = !label11.Visible;//Şu anki visible durumunun tam tersi durumuna getir.
            tmYanipSonme.Interval = tmYanipSonme.Interval + 10;//Her yanıp söndüğünde bu süreyi 10 ms arttır.
            if (tmYanipSonme.Interval == 500)//500 olduğunda yanıp sönme dursun yazı aktif kalsın.
            {
                label11.Visible = true;
                tmYanipSonme.Enabled = false;
            }
        }
        OrtakIslemler islemYap = new OrtakIslemler();
        private void btnEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (EklemeKontrolEt())
                {
                    SqlParameter[] parameterEkleme = new SqlParameter[7];
                    parameterEkleme[0] = new SqlParameter();
                    parameterEkleme[0].ParameterName = "@Adi";
                    parameterEkleme[0].SqlDbType = SqlDbType.NVarChar;
                    parameterEkleme[0].SqlValue = txtAdiEkle.Text.ToUpper();

                    parameterEkleme[1] = new SqlParameter();
                    parameterEkleme[1].ParameterName = "@Stok";
                    parameterEkleme[1].SqlDbType = SqlDbType.Decimal;
                    parameterEkleme[1].SqlValue = txtStokEkle.Text;

                    parameterEkleme[2] = new SqlParameter();
                    parameterEkleme[2].ParameterName = "@Maliyet";
                    parameterEkleme[2].SqlDbType = SqlDbType.Decimal;
                    parameterEkleme[2].SqlValue = txtMaliyetiEkle.Text;

                    parameterEkleme[3] = new SqlParameter();
                    parameterEkleme[3].ParameterName = "@SatisFiyati";
                    parameterEkleme[3].SqlDbType = SqlDbType.Decimal;
                    parameterEkleme[3].SqlValue = txtSatisFiyatiEkle.Text;

                    parameterEkleme[4] = new SqlParameter();
                    parameterEkleme[4].ParameterName = "@KritikMiktar";
                    parameterEkleme[4].SqlDbType = SqlDbType.Decimal;
                    parameterEkleme[4].SqlValue = txtEkleKritikMiktar.Text;

                    parameterEkleme[5] = new SqlParameter();
                    parameterEkleme[5].ParameterName = "@BarkodKodu";
                    parameterEkleme[5].SqlDbType = SqlDbType.NVarChar;
                    parameterEkleme[5].SqlValue = txtBarkodEkle.Text;

                    parameterEkleme[6] = new SqlParameter();
                    parameterEkleme[6].ParameterName = "@StokBirimi";
                    parameterEkleme[6].SqlDbType = SqlDbType.TinyInt;
                    parameterEkleme[6].SqlValue = cbBirimler.SelectedValue;

                    if (SqlBaglantisi.GuncelleProcedure("URUNEKLE", parameterEkleme) == 1)
                    {

                        SqlParameter[] prm = new SqlParameter[1];
                        prm[0] = parameterEkleme[5];
                        SqlDataReader rd = SqlBaglantisi.OkuProcedure("EKLENENURUNUGETIR", prm);

                        rd.Read();
                        string[] yeniUrun = new string[9];
                        yeniUrun[0] = rd["No"].ToString();
                        yeniUrun[1] = rd["Bakod_kodu"].ToString();
                        yeniUrun[2] = rd["Adi"].ToString();
                        yeniUrun[3] = rd["Maliyet"].ToString();
                        yeniUrun[4] = rd["Satis_fiyati"].ToString();
                        int ondalik = Convert.ToInt32(rd["Stok"]);
                        decimal kesirli = Convert.ToDecimal(rd["Stok"]);

                        if (ondalik - kesirli == 0)
                            yeniUrun[5] = Convert.ToInt32(rd["Stok"]).ToString();
                        else
                            yeniUrun[5] = rd["Stok"].ToString();
                        yeniUrun[6] = rd[6].ToString();
                        yeniUrun[7] = rd["Id"].ToString();
                        yeniUrun[8] = rd["Kritik_miktar"].ToString();
                        varsayilanTblo.Rows.Add(yeniUrun);
                        txtAdiEkle.Text = "";
                        txtStokEkle.Text = "";
                        txtMaliyetiEkle.Text = "";
                        txtSatisFiyatiEkle.Text = "";
                        txtEkleKritikMiktar.Text = "";
                        txtBarkodEkle.Text = "";
                        cbBirimler.SelectedIndex = 0;
                        nfBasarili.BalloonTipText = "Ürün başarılı bir şekilde eklendi.";
                        nfBasarili.Visible = true;
                        nfBasarili.ShowBalloonTip(2000);
                    }
                }
                else MessageBox.Show("Lütfen verileri doğru girin. Sayısal veri girilmesi gereken yerler boş bırakılamaz. Ancak 0 girebilirsiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                return;
            }
        }
        bool EklemeKontrolEt()
        {
            
            try
            {
                if (txtMaliyetiEkle.Text == "")
                    txtMaliyetiEkle.Text = txtSatisFiyatiEkle.Text;
                if (txtEkleKritikMiktar.Text == "") txtEkleKritikMiktar.Text = "10";
                Convert.ToDecimal(txtMaliyetiEkle.Text);
                Convert.ToDecimal(txtSatisFiyatiEkle.Text);
                Convert.ToDecimal(txtStokEkle.Text);
                Convert.ToDecimal(txtEkleKritikMiktar.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void frmAnaForm_AllControls(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                txtBarkodSorgula.Select();
            }
        }
        public void frmStokIslemleri_Load(object sender, EventArgs e)
        {
            if (Program.kritik)
            {
                tmYanipSonme.Enabled = true;
            }
            
                UrunleriAl();
                TabloyuDuzenle();
            
            UrunSayisiKontrolEt();
            if (dgUrunler.RowCount > 0) btnExcel.Enabled = true;
            cbBirimler.DataSource = SqlBaglantisi.DisconnectedProcedure("BIRIMSORGULAMA", new SqlParameter[0]);
            cbBirimler.ValueMember = "Id";
            cbBirimler.DisplayMember = "Adi";

        }
        DataGridViewButtonColumn dgvBtn;
        DataTable varsayilanTblo;
        bool clearCalisiyor = false;
        public void UrunleriAl()
        {
            SqlParameter[] satisParameter = new SqlParameter[1];
            satisParameter[0] = new SqlParameter();
            satisParameter[0].ParameterName = "@Kritik";
            satisParameter[0].SqlDbType = SqlDbType.Bit;
            satisParameter[0].SqlValue = Program.kritik;
            varsayilanTblo = SqlBaglantisi.DisconnectedProcedure("URUNLERIGETIR", satisParameter);
            clearCalisiyor = true;
            dgUrunler.Columns.Clear();
            dgUrunler.DataSource = varsayilanTblo;
            for (int i = 0; i < varsayilanTblo.Rows.Count; i++)
            {
                if (Convert.ToInt16(varsayilanTblo.Rows[i][7]) == 1)
                {
                    varsayilanTblo.Rows[i][5] = Convert.ToInt32(varsayilanTblo.Rows[i][5]);
                    varsayilanTblo.Rows[i]["Kritik_miktar"] = Convert.ToInt32(varsayilanTblo.Rows[i]["Kritik_miktar"]);
                }
            }
        }
        public void TabloyuDuzenle()
        {
            urunAraniyor = false;
            if (dgUrunler.Columns.Count > 0)
            {
                for (int i = 0; i < dgUrunler.ColumnCount; i++)
                {
                    dgUrunler.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgUrunler.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dgUrunler.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                dgUrunler.Columns[0].Width = 40;
                dgUrunler.Columns[1].Width = 120;
                dgUrunler.Columns[1].HeaderText = "Barkod";
                dgUrunler.Columns[2].HeaderText = "Adı";
                dgUrunler.Columns[3].HeaderText = "Maliyeti (₺)";
                dgUrunler.Columns[4].HeaderText = "Fiyatı (₺)";
                dgUrunler.Columns[5].HeaderText = "Miktarı";
                dgUrunler.Columns[6].HeaderText = "Birimi";

                dgUrunler.Columns[3].Width = dgUrunler.Columns[4].Width = 110;
                dgUrunler.Columns[5].Width = 65;
                dgUrunler.Columns[6].Width = 65;
                dgUrunler.Columns[7].Width = 0;
                dgUrunler.Columns[8].Width = 0;
                dgUrunler.Columns[7].Visible = false;
                dgUrunler.Columns[8].Visible = false;
                dgvBtn = new DataGridViewButtonColumn();
                //Kolon Başlığı
                dgvBtn.HeaderText = "Silgi";
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
        void UrunSayisiKontrolEt()
        {
            if (dgUrunler.Rows.Count <= 0 && clearCalisiyor == false)
            {
                btnGuncelle.Enabled = false;
                txtAdaGoreAra.Enabled = false;
                txtBarkodSorgula.Enabled = false;
                btnExcel.Enabled = false;
            }
            else clearCalisiyor = true;
            
        }
        private void txtAdaGoreAra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                txtBarkodSorgula.Select();
            }
            if(e.KeyCode==Keys.Enter)
            {
                txtAdaGoreAra.Text = "";
                txtAdaGoreAra_TextChanged(0, e);
            }
        }
        bool a = true;
        private void txtSatisFiyati_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSatisFiyati.Text.IndexOf(',') == -1)
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
        bool b = true;
        private void txtMaliyeti_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtMaliyeti.Text.IndexOf(',') == -1)
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
        bool c = true;
        private void txtSatisFiyatiEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSatisFiyatiEkle.Text.IndexOf(',') == -1)
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
        bool d = true;
        private void txtMaliyetiEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtMaliyetiEkle.Text.IndexOf(',') == -1)
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
        private void txtStokSayisi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        private void txtStokEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
        bool f = true, ff = true;
        private void txtKritikMiktar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtKritikMiktar.Text.IndexOf(',') == -1 )
            {
                f = true;
            }
            if (e.KeyChar == (char)44 && f == true && ff==true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
               f = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }
        bool g = true;
        private void txtEkleKritikMiktar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtEkleKritikMiktar.Text.IndexOf(',') == -1)
            {
                g = true;
            }
            if (e.KeyChar == (char)44 && g == true)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != (char)44;
                g = false;
            }
            else
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void txtBarkodSorgula_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter && txtBarkodSorgula.Text != "")
            {

                DataView sorgula = varsayilanTblo.DefaultView;
                sorgula.RowFilter = "Bakod_kodu = '" + txtBarkodSorgula.Text + "'";
                if (sorgula.Count > 0)
                {
                    txtBarkodSorgula.Text = "";
                    dgUrunler.DataSource = sorgula;
                    urunAraniyor = true;
                }

                else
                {
                    txtBarkodSorgula.Text = "";
                    sorgula.RowFilter = "";
                    MessageBox.Show("Aradığınız ürün bulunamadı.", "Hata Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
                
            }
        }
        DataView adAra;
        bool urunAraniyor = false;
        private void txtAdaGoreAra_TextChanged(object sender, EventArgs e)
        {
            adAra = varsayilanTblo.DefaultView;
            adAra.RowFilter = "Adi like '%"+txtAdaGoreAra.Text+"%'";
            dgUrunler.DataSource = adAra;
            urunAraniyor = true;
        }
        int guncellenecekRowIndex = -1;
        private void dgUrunler_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex!=-1 && e.ColumnIndex!=9)
            {
                txtBarkodKodu.Text = dgUrunler.Rows[e.RowIndex].Cells["Bakod_kodu"].Value.ToString();
                txtAdi.Text = dgUrunler.Rows[e.RowIndex].Cells["Adi"].Value.ToString();
                txtMaliyeti.Text = dgUrunler.Rows[e.RowIndex].Cells["Maliyet"].Value.ToString();
                txtSatisFiyati.Text = dgUrunler.Rows[e.RowIndex].Cells["Satis_fiyati"].Value.ToString();
                txtStokSayisi.Text = dgUrunler.Rows[e.RowIndex].Cells["Stok"].Value.ToString();
                txtKritikMiktar.Text = dgUrunler.Rows[e.RowIndex].Cells["Kritik_miktar"].Value.ToString();
                btnGuncelle.Enabled = true;
                guncellenecekRowIndex = e.RowIndex;
                if (Convert.ToInt16(dgUrunler.Rows[e.RowIndex].Cells["Id"].Value) == 1) ff = false;
            }
            
        }

        private void dgUrunler_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UrunSayisiKontrolEt();
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (KontrolEt())
            {
                SqlParameter[] parameterGuncelleme = new SqlParameter[6];
                parameterGuncelleme[0] = new SqlParameter();
                parameterGuncelleme[0].ParameterName = "@Adi";
                parameterGuncelleme[0].SqlDbType = SqlDbType.NVarChar;
                parameterGuncelleme[0].SqlValue = txtAdi.Text.ToUpper();

                parameterGuncelleme[1] = new SqlParameter();
                parameterGuncelleme[1].ParameterName = "@Miktar";
                parameterGuncelleme[1].SqlDbType = SqlDbType.Decimal;
                parameterGuncelleme[1].SqlValue = Convert.ToDecimal(txtStokSayisi.Text);

                parameterGuncelleme[2] = new SqlParameter();
                parameterGuncelleme[2].ParameterName = "@Maliyet";
                parameterGuncelleme[2].SqlDbType = SqlDbType.Decimal;
                parameterGuncelleme[2].SqlValue = Convert.ToDecimal(txtMaliyeti.Text);

                parameterGuncelleme[3] = new SqlParameter();
                parameterGuncelleme[3].ParameterName = "@Fiyat";
                parameterGuncelleme[3].SqlDbType = SqlDbType.Decimal;
                parameterGuncelleme[3].SqlValue = Convert.ToDecimal(txtSatisFiyati.Text);

                parameterGuncelleme[4] = new SqlParameter();
                parameterGuncelleme[4].ParameterName = "@KritikMiktar";
                parameterGuncelleme[4].SqlDbType = SqlDbType.Decimal;
                parameterGuncelleme[4].SqlValue = Convert.ToDecimal(txtKritikMiktar.Text);

                parameterGuncelleme[5] = new SqlParameter();
                parameterGuncelleme[5].ParameterName = "@Barkod";
                parameterGuncelleme[5].SqlDbType = SqlDbType.NVarChar;
                parameterGuncelleme[5].SqlValue = txtBarkodKodu.Text;
                bool kapat = false;
                if (SqlBaglantisi.GuncelleProcedure("URUNGUNCELLE", parameterGuncelleme) == 1)
                {
                    if (Program.kritik && Convert.ToDecimal(txtKritikMiktar.Text) < Convert.ToDecimal(txtStokSayisi.Text))
                    {
                        UrunleriAl();
                        TabloyuDuzenle();
                        if (dgUrunler.RowCount == 0) kapat = true;
                    }
                    else
                    {

                        DataRow[] updateRow = varsayilanTblo.Select("Bakod_kodu='" + txtBarkodKodu.Text + "'");
                        
                        varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Adi"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Adi"].Value = txtAdi.Text;
                        varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Maliyet"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Maliyet"].Value = Convert.ToDecimal(txtMaliyeti.Text).ToString("F");
                        varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Satis_fiyati"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Satis_fiyati"].Value = Convert.ToDecimal(txtSatisFiyati.Text).ToString("F");
                        if (Convert.ToInt16(varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Id"]) != 1)
                        {
                           varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Stok"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Stok"].Value = Convert.ToDecimal(txtStokSayisi.Text).ToString("F1");
                            varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Kritik_miktar"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Kritik_miktar"].Value = Convert.ToDecimal(txtKritikMiktar.Text).ToString("F1");
                        }
                        else
                        {
                           varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Stok"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Stok"].Value = txtStokSayisi.Text;
                            varsayilanTblo.Rows[varsayilanTblo.Rows.IndexOf(updateRow[0])]["Kritik_miktar"] = dgUrunler.Rows[guncellenecekRowIndex].Cells["Kritik_miktar"].Value = txtKritikMiktar.Text;
                        }
                    }

                    txtBarkodKodu.Text = "";
                    txtAdi.Text = "";
                    txtMaliyeti.Text = "";
                    txtSatisFiyati.Text = "";
                    txtStokSayisi.Text = "";
                    txtKritikMiktar.Text = "";
                    btnGuncelle.Enabled = false;
                    guncellenecekRowIndex = -1;
                    ff = true;

                    nfBasarili.BalloonTipText = "Ürün başarılı bir şekilde güncellendi.";
                    nfBasarili.Visible = true;
                    nfBasarili.ShowBalloonTip(2000);
                    if (kapat) this.Close();

                }
                else MessageBox.Show("Lütfen verileri doğru girin. Sayısal veri girilmesi gereken yerler boş bırakılamaz. Ancak 0 girebilirsiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Lütfen verileri doğru girin. Sayısal veri girilmesi gereken yerler boş bırakılamaz. Ancak 0 girebilirsiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        bool KontrolEt()
        {
            
            try
            {
                
                Convert.ToDecimal(txtMaliyeti.Text);
                Convert.ToDecimal(txtSatisFiyati.Text);
                Convert.ToDecimal(txtStokSayisi.Text);
                Convert.ToDecimal(txtKritikMiktar.Text);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        private void frmStokIslemleri_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.stok_calisiyor = false;
            Program.kritik = false;
        }

        private void txtAdi_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = Char.IsPunctuation(e.KeyChar);
        }

        private void txtAdi_TextChanged(object sender, EventArgs e)
        {
            lblMaxKarakter.Text = txtAdi.TextLength.ToString();
            if (txtAdi.TextLength < 25) lblMaxKarakter.ForeColor = Color.Lime;
            else if (txtAdi.TextLength < 50) lblMaxKarakter.ForeColor = Color.ForestGreen;
            else if (txtAdi.TextLength < 75) lblMaxKarakter.ForeColor = Color.Goldenrod;
            else if (txtAdi.TextLength < 100) lblMaxKarakter.ForeColor = Color.DeepPink;
            else lblMaxKarakter.ForeColor = Color.Red;

        }

        private void txtAdiEkle_TextChanged(object sender, EventArgs e)
        {
            lblMaxKarakterEkle.Text = txtAdiEkle.TextLength.ToString();
            if (txtAdiEkle.TextLength < 25) lblMaxKarakterEkle.ForeColor = Color.Lime;
            else if (txtAdiEkle.TextLength < 50) lblMaxKarakterEkle.ForeColor = Color.ForestGreen;
            else if (txtAdiEkle.TextLength < 75) lblMaxKarakterEkle.ForeColor = Color.Goldenrod;
            else if (txtAdiEkle.TextLength < 100) lblMaxKarakterEkle.ForeColor = Color.DeepPink;
            else lblMaxKarakterEkle.ForeColor = Color.Red;
        }

        private void dgUrunler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int column;
            if (urunAraniyor)
                column = 0;
            else column = 9;
            if (e.RowIndex >-1 && e.ColumnIndex == column)
            {
                DialogResult silmeOnayi = MessageBox.Show(" Bu ürünü silmek üzeresiniz. \nDevam etmek istiyor musunuz?","Silgi Onayı",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
                
                if (silmeOnayi == DialogResult.Yes)
                {
                    SqlParameter[] silgiParameter = new SqlParameter[1];
                    silgiParameter[0] = new SqlParameter();
                    silgiParameter[0].ParameterName = "@Barkod";
                    silgiParameter[0].SqlDbType = SqlDbType.NVarChar;
                    silgiParameter[0].SqlValue = dgUrunler.Rows[e.RowIndex].Cells["Bakod_kodu"].Value;
                    if (SqlBaglantisi.GuncelleProcedure("URUNSIL", silgiParameter) == 1)
                    {
                        if (urunAraniyor)
                        {
                            dgUrunler.Rows.Remove(dgUrunler.Rows[e.RowIndex]);                           
                        }

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
                    else MessageBox.Show("Seçili ürün silinemedi. Başka kullanıcı tarafından silinmiş ya da veritabanında yetkisiz erişim olabilir.", "İşlem Yapılamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExceleAktar();
        }
        struct  excelTabloButunlugu
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
        void ExceleAktar()
        {

            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Urun_Listesi(" + DateTime.Now.ToShortDateString() + ")";
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
                tablo.sonSutun = "G";
                altBilgiButunlugu altBilgi = new altBilgiButunlugu();
                altBilgi.ilkSatir = 6;
                altBilgi.ilkSutun = "E";
                altBilgi.sonSutun = "G";
                

                _package = new ExcelPackage(new MemoryStream());
                ExcelWorksheet ws1 = _package.Workbook.Worksheets.Add("Ürün Listesi");
                ws1.Cells[tablo.ilkSutun+tablo.baslikSatiri.ToString()].Value = Program.isletmeAdi + " Ürün Listesi";
                ws1.Cells[tablo.ilkSutun+tablo.baslikSatiri.ToString()+":"+tablo.sonSutun+tablo.baslikSatiri.ToString()].Merge = true;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Font.Bold = true;
                ws1.Cells[tablo.ilkSutun + tablo.tabloBasligiSatiri.ToString() + ":" + tablo.sonSutun + tablo.tabloBasligiSatiri.ToString()].Style.Font.Bold = true;
                for (int i = 0; i < 7; i++)
                {
                    ws1.Cells[tablo.tabloBasligiSatiri, i + 1].Value = dgUrunler.Columns[i].HeaderText;
                }
                for (var kolon = 0; kolon < 7; kolon++)
                {

                    for (var satir = 0; satir < dgUrunler.RowCount; satir++)
                    {
                        if (kolon == 0)
                        {
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToInt32(dgUrunler.Rows[satir].Cells[kolon].Value);
                            if ((satir) % 2 == 0)
                            {
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws1.Cells[tablo.ilkSutun + (satir + tablo.ilkSatir) + ":" + tablo.sonSutun + (satir + tablo.ilkSatir)].Style.Fill.BackgroundColor.SetColor(acikGri);
                            }
                        }
                        else if (kolon == 3 || kolon == 4) ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToDecimal(dgUrunler.Rows[satir].Cells[kolon].Value);
                        else if (kolon == 5)
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = Convert.ToDecimal(dgUrunler.Rows[satir].Cells[kolon].Value);
                        else
                            ws1.Cells[satir + tablo.ilkSatir, kolon + 1].Value = dgUrunler.Rows[satir].Cells[kolon].Value;
                        tablo.sonSatir = satir + tablo.ilkSatir;
                        
                    }
                    altBilgi.ilkSatir = tablo.sonSatir + 2;
                    if (kolon == 3)
                    {
                        ws1.Cells["D"+tablo.ilkSatir+":D" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";

                    }
                    if (kolon == 4)
                    {
                        ws1.Cells["E" + tablo.ilkSatir + ":E" + tablo.sonSatir].Style.Numberformat.Format = "₺#,0.00";
                    }
                    
                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
                    
                    ws1.Column(kolon + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws1.Column(kolon + 1).AutoFit();
                }
                ws1.Cells["B" + tablo.ilkSatir + ":" + "B" + tablo.sonSatir].Style.WrapText = true;
                ws1.Cells["C" + tablo.ilkSatir + ":" + "C" + tablo.sonSatir].Style.WrapText = true;

                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Value = "Tarih";
                ws1.Cells[Convert.ToString(Convert.ToChar(Convert.ToInt32(altBilgi.ilkSutun[0])+1)) + altBilgi.ilkSatir + ":"+altBilgi.sonSutun + altBilgi.ilkSatir].Merge = true;
                ws1.Cells[Convert.ToString(Convert.ToChar(Convert.ToInt32(altBilgi.ilkSutun[0]) + 1)) + altBilgi.ilkSatir].Value = DateTime.Now.ToString();
                ws1.Cells[altBilgi.ilkSutun + altBilgi.ilkSatir].Style.Font.Bold = true;


                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[tablo.ilkSutun + tablo.baslikSatiri.ToString() + ":" + tablo.sonSutun + tablo.baslikSatiri.ToString()].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells[altBilgi.ilkSutun + (tablo.sonSatir + 1) + ":"+altBilgi.sonSutun+ (tablo.sonSatir + 1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Column(1).Width = 6;
                ws1.Column(2).Width = 10;
                ws1.Column(3).Width = 40;
                ws1.Column(4).Width = 8;
                ws1.Column(5).Width = 8;
                ws1.Column(6).Width = 6;
                ws1.Column(7).Width = 6;

                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                DialogResult ac = MessageBox.Show("Satışlar başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) System.Diagnostics.Process.Start(saveExceleKaydet.FileName);

                _package.Dispose();

            }
        }
        Color acikGri = ColorTranslator.FromHtml("#f2f2f2");
        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }
    }
}

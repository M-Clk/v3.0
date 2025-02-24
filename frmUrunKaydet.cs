using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmUrunKaydet : Form
    {
        public static frmUrunKaydet _frmUrunKaydet;
        private readonly DbOperations _dbOperations = new DbOperations();
        public DataGridViewRow DgRow { get; set; }
        public string EklenenUrunBarkodKodu { get; set; }
        public bool IslemYapildi { get; set; }
        
        private bool _adding = true;
        public frmUrunKaydet()
        {
            InitializeComponent();
            Load();
        }
        public static frmUrunKaydet SingletonFrmGetir()
        {
            if (_frmUrunKaydet == null)
                _frmUrunKaydet = new frmUrunKaydet();
            return _frmUrunKaydet;
        }

        public DialogResult ShowDialog(DataGridViewRow dgRow)
        {
            urunBilgileriniYaz(dgRow);
            return ShowDialog();
        }

        public void Load()
        {
            _dbOperations.LoadComboBox(cbBirimler, "BIRIMSORGULAMA");

            cbBirimler.DataSource = _dbOperations.DisconnectedProcedure("BIRIMSORGULAMA", new SqlParameter[0]);
            cbBirimler.ValueMember = "Id";
            cbBirimler.DisplayMember = "Adi";
        }

        void urunBilgileriniYaz(DataGridViewRow dgRow)
        {            
            _dbOperations.LoadComboBox(cbMarkalar, "MARKASORGULAMA");
            txtBarkodKodu.Text = dgRow?.Cells["Bakod_kodu"].Value.ToString() ?? "";
            txtAdi.Text = dgRow?.Cells["Adi"].Value.ToString() ?? "";
            txtMaliyet.Text = dgRow?.Cells["Maliyet"].Value.ToString() ?? "";
            txtSatisFiyati.Text = dgRow?.Cells["Satis_fiyati"].Value.ToString() ?? "";
            txtStok.Text = dgRow?.Cells["Stok"].Value.ToString() ?? "";
            txtKritikMiktar.Text = dgRow?.Cells["Kritik_miktar"].Value.ToString() ?? "";
            chkHizliEkrandaGoster.Checked = (bool)(dgRow?.Cells["Hizli_urun"].Value ?? false);
            _adding = dgRow == null;
            if (!_adding)
            {
                cbMarkalar.SelectedValue = dgRow.Cells["Marka_Id"].Value;
                if (cbMarkalar.SelectedValue == null)
                    cbMarkalar.SelectedValue = 0;
            }
            txtBarkodKodu.ReadOnly = !_adding;
            EklenenUrunBarkodKodu = null;
            DgRow = dgRow;
            IslemYapildi = false;
            txtBarkodKodu.Focus();
            txtBarkodKodu.Select();
            if (ConfigurationManager.GetValueFromRegistry(ConfigurationCategory.General, "SadeceSatisFiyatiGoster", Convert.ToBoolean, false))
            {
                tableLayoutPanel9.RowStyles[3].Height = 0;
                txtMaliyet.Visible = false;
            }
        }

        private bool KontrolEt()
        {
            try
            {
                if(txtAdi.Text.Length < 3)
                {
                    MessageBox.Show("Lütfen ürün adını en az 3 karakter olacak şekilde girin.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (_adding)
                {
                    var urunAdi = tekUrunAdiGetir(txtBarkodKodu.Text);
                    if (!string.IsNullOrWhiteSpace(urunAdi))
                    {
                        MessageBox.Show($"Bu ürün {urunAdi} adıyla kayıtlı. Aynı ürün ikinci defa kaydedilemez.", "Ekleme Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        eklemeAlaniTemizle();
                        return false;
                    }
                }
                
                if (txtMaliyet.Text == "")
                    txtMaliyet.Text = txtSatisFiyati.Text;
                if (txtKritikMiktar.Text == "")
                    txtKritikMiktar.Text = "10";

                Convert.ToDecimal(txtMaliyet.Text);
                Convert.ToDecimal(txtSatisFiyati.Text);
                Convert.ToDecimal(txtStok.Text);
                Convert.ToDecimal(txtKritikMiktar.Text);

                return true;
            }
            catch
            {
                MessageBox.Show("Lütfen verileri doğru girin. Sayısal veri girilmesi gereken yerler boş bırakılamaz. Ancak 0 girebilirsiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void eklemeAlaniTemizle()
        {
            txtAdi.Text = "";
            txtStok.Text = "";
            txtMaliyet.Text = "";
            txtSatisFiyati.Text = "";
            txtKritikMiktar.Text = "";
            txtBarkodKodu.Text = "";
            cbBirimler.SelectedIndex = 0;
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            if (!KontrolEt())
                return;
            var sqlParams = new SqlParameter[9];
            sqlParams[0] = new SqlParameter();
            sqlParams[0].ParameterName = "@Adi";
            sqlParams[0].SqlDbType = SqlDbType.NVarChar;
            sqlParams[0].SqlValue = txtAdi.Text.ToUpper();

            sqlParams[1] = new SqlParameter();
            sqlParams[1].ParameterName = "@Miktar";
            sqlParams[1].SqlDbType = SqlDbType.Decimal;
            sqlParams[1].SqlValue = Convert.ToDecimal(txtStok.Text);

            sqlParams[2] = new SqlParameter();
            sqlParams[2].ParameterName = "@Maliyet";
            sqlParams[2].SqlDbType = SqlDbType.Decimal;
            sqlParams[2].SqlValue = Convert.ToDecimal(txtMaliyet.Text);

            sqlParams[3] = new SqlParameter();
            sqlParams[3].ParameterName = "@Fiyat";
            sqlParams[3].SqlDbType = SqlDbType.Decimal;
            sqlParams[3].SqlValue = Convert.ToDecimal(txtSatisFiyati.Text);

            sqlParams[4] = new SqlParameter();
            sqlParams[4].ParameterName = "@KritikMiktar";
            sqlParams[4].SqlDbType = SqlDbType.Decimal;
            sqlParams[4].SqlValue = Convert.ToDecimal(txtKritikMiktar.Text);

            sqlParams[5] = new SqlParameter();
            sqlParams[5].ParameterName = "@Barkod";
            sqlParams[5].SqlDbType = SqlDbType.NVarChar;
            sqlParams[5].SqlValue = txtBarkodKodu.Text;

            sqlParams[6] = new SqlParameter();
            sqlParams[6].ParameterName = "@HizliUrun";
            sqlParams[6].SqlDbType = SqlDbType.Bit;
            sqlParams[6].SqlValue = chkHizliEkrandaGoster.Checked;

            sqlParams[7] = new SqlParameter();
            sqlParams[7].ParameterName = "@StokBirimi";
            sqlParams[7].SqlDbType = SqlDbType.TinyInt;
            sqlParams[7].SqlValue = cbBirimler.SelectedValue;

            sqlParams[8] = new SqlParameter();
            sqlParams[8].ParameterName = "@MarkaId";
            sqlParams[8].SqlDbType = SqlDbType.TinyInt;
            sqlParams[8].SqlValue = cbMarkalar.SelectedValue ?? 0;

            if (_dbOperations.GuncelleProcedure("URUNKAYDET", sqlParams) == 1)
            {
                if (_adding)
                    EklenenUrunBarkodKodu = txtBarkodKodu.Text;
                else
                    guncellemeIcinAyarla();
                IslemYapildi = true;
                Close();
            }
            else
                MessageBox.Show("Lütfen verileri doğru girin. Sayısal veri girilmesi gereken yerler boş bırakılamaz. Ancak 0 girebilirsiniz.", "Veri Girişi Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }

        private void guncellemeIcinAyarla()
        {
            DgRow.Cells["Bakod_kodu"].Value = txtBarkodKodu.Text;
            DgRow.Cells["Adi"].Value = txtAdi.Text;
            DgRow.Cells["Maliyet"].Value = Convert.ToDecimal(txtMaliyet.Text);
            DgRow.Cells["Satis_fiyati"].Value = Convert.ToDecimal(txtSatisFiyati.Text);
            DgRow.Cells["Hizli_urun"].Value = chkHizliEkrandaGoster.Checked;
            DgRow.Cells["Stok"].Value = Convert.ToDecimal(txtStok.Text);
            DgRow.Cells["Kritik_miktar"].Value = Convert.ToDecimal(txtKritikMiktar.Text);
            DgRow.Cells["Marka_Id"].Value = cbMarkalar.SelectedValue;
        }

        private string tekUrunAdiGetir(string barkodKodu) => _dbOperations.ScalarTextCommand($"Select Adi FROM Urunler WHERE Bakod_kodu = '{barkodKodu}'");


        private void txtAdi_TextChanged(object sender, EventArgs e)
        {
            lblMaxKarakter.Text = txtAdi.TextLength.ToString();
            if (txtAdi.TextLength < 25) lblMaxKarakter.ForeColor = Color.Lime;
            else if (txtAdi.TextLength < 50) lblMaxKarakter.ForeColor = Color.ForestGreen;
            else if (txtAdi.TextLength < 75) lblMaxKarakter.ForeColor = Color.Goldenrod;
            else if (txtAdi.TextLength < 100) lblMaxKarakter.ForeColor = Color.DeepPink;
            else lblMaxKarakter.ForeColor = Color.Red;
        }
    }
}

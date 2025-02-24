using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmTopluFiyatGuncelleme : Form
    {
        public static frmTopluFiyatGuncelleme _frmTopluFiyatGuncelleme;
        private readonly DbOperations _dbOperations = new DbOperations();
        private DataTable _dtUrunler;
        public frmTopluFiyatGuncelleme()
        {
            InitializeComponent();
            tabloDuzenle();
        }

        private void tabloDuzenle()
        {
            if(dgUrunler.Rows.Count == 0)
                return;
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

            dgUrunler.Columns["Satis_fiyati"].HeaderText = "Fiyatı (₺)";
            dgUrunler.Columns["Stok"].HeaderText = "Miktarı"; 
            dgUrunler.Columns["Birim_Adi"].HeaderText = "Birimi";
            dgUrunler.Columns["Stok"].Width = dgUrunler.Columns["Birim_Adi"].Width = 65;

            dgUrunler.Columns["Maliyet"].Visible = false;
            dgUrunler.Columns["Stok_birimi"].Visible = false;
            dgUrunler.Columns["Kritik_miktar"].Visible = false;
            dgUrunler.Columns["Hizli_urun"].Visible = false;
            dgUrunler.Columns["Marka_Id"].Visible = false;
        }

        public static frmTopluFiyatGuncelleme SingletonFrmGetir()
        {
            if (_frmTopluFiyatGuncelleme == null)
                _frmTopluFiyatGuncelleme = new frmTopluFiyatGuncelleme();
            return _frmTopluFiyatGuncelleme;
        }

        public DialogResult ShowDialog(DataTable dtUrunler)
        {
            if(dtUrunler.Rows.Count == 0)
                Close();
            _dtUrunler = dtUrunler;
            _dbOperations.LoadComboBox(cbMarkalar, "MARKASORGULAMA");
            var dr = ((DataTable)cbMarkalar.DataSource).NewRow();
            dr.ItemArray = new[] { "-1", "Tüm Ürünler" };
            ((DataTable)cbMarkalar.DataSource).Rows.InsertAt(dr, 0);
            cbMarkalar.SelectedIndex = 0;
            numYuzde.Value = 0;
            return ShowDialog();
        }

        void markayaGoreFiltrele()
        {
            var markaUrunleri = _dtUrunler.DefaultView;
            markaUrunleri.RowFilter = (int)cbMarkalar.SelectedValue < 0 ? "" : $"Marka_Id = {cbMarkalar.SelectedValue}";
            dgUrunler.DataSource = markaUrunleri;
            tabloDuzenle();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            btnIndirim.Enabled = btnZam.Enabled = numYuzde.Value > 0;
        }

        private void cbMarkalar_SelectedIndexChanged(object sender, EventArgs e)
        {
            markayaGoreFiltrele();
        }

        private void btnIndirim_Click(object sender, EventArgs e)
        {
            fiyatGuncellemeBaslat(-numYuzde.Value);
        }

        private void btnZam_Click(object sender, EventArgs e)
        {
            fiyatGuncellemeBaslat(numYuzde.Value);
        }

        void fiyatGuncellemeBaslat(decimal yuzde)
        {
            lblBekleyin.Visible = true;
            var preMessage = (int)cbMarkalar.SelectedValue > 0 ? $"{cbMarkalar.SelectedText} ürünlerinin fiyatını" : "Tüm ürünlerin fiyatını";
            var islemMessage = yuzde > 0 ? "arttırılacaktır" : "indirilecektir";
            var updateDialogResult = MessageBox.Show($"{preMessage} %{Math.Abs(yuzde)} oranında {islemMessage}. \nDevam etmek istiyor musunuz?", "Onay", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (updateDialogResult != DialogResult.Yes)
            {
                lblBekleyin.Visible = false;
                return;
            }    

            fiyatGuncelle(yuzde);
        }

        void fiyatGuncelle(decimal yuzde)
        {
            var katsayi = (1 + Math.Round(yuzde / 100, 2)).ToString("0.##", CultureInfo.InvariantCulture);
            var whereClause = (int)cbMarkalar.SelectedValue > 0 ? $" Where Marka_Id = {katsayi}" : "";
            var sonuc = _dbOperations.ScalarTextCommand($"UPDATE Urunler Set Satis_fiyati = ROUND(Satis_fiyati * {katsayi}, 2), Maliyet = ROUND(Maliyet * {katsayi}, 2) from Urunler{whereClause}");
            if(sonuc == "")
                return;
            sonuc = _dbOperations.ScalarTextCommand($"Insert Into FiyatGuncellemeGecmisi Values({cbMarkalar.SelectedValue}, GETDATE(), {katsayi})");
            if(sonuc == "")
                return;
            dtUrunFiyatGuncelle(yuzde);
            lblBekleyin.Visible = false;
            numYuzde.Value = 0;
        }
        void dtUrunFiyatGuncelle(decimal yuzde)
        {
            foreach (DataGridViewRow dtRow in dgUrunler.Rows)
            {
                dtRow.Cells["Satis_fiyati"].Value = ((decimal)dtRow.Cells["Satis_fiyati"].Value * (1 + Math.Round(yuzde / 100, 2))).ToString("0.##");
                dtRow.Cells["Maliyet"].Value = ((decimal)dtRow.Cells["Maliyet"].Value * (1 + Math.Round(yuzde / 100, 2))).ToString("0.##");
            }
        }
    }
}

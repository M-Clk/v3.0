using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmMarkaYonetim : Form
    {
        public static frmMarkaYonetim _frmMarkaYonetim;
        private readonly DbOperations _dbOperations = new DbOperations();
        public frmMarkaYonetim()
        {
            InitializeComponent();
            _dbOperations.LoadComboBox(cbMarkalar, "MARKASORGULAMA");
        }
        public static frmMarkaYonetim SingletonFrmGetir()
        {
            if (_frmMarkaYonetim == null)
                _frmMarkaYonetim = new frmMarkaYonetim();
            return _frmMarkaYonetim;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) => btnEkle.Enabled = txtMarkaAdi.Text.Length >= 3 && !markaVarMi(txtMarkaAdi.Text);
        
        private bool markaVarMi(string ad)
        {
            foreach (DataRowView cbMarkalarItem in cbMarkalar.Items)
                if (cbMarkalarItem.Row["Adi"].ToString().Replace(" ", "").ToLowerInvariant().Equals(ad.Replace(" ", "").ToLowerInvariant()))
                    return true;

            return false;
        }

        private void cbMarkalar_SelectedIndexChanged(object sender, EventArgs e) => btnSil.Enabled = (int)((DataRowView)cbMarkalar.SelectedItem).Row["Id"] > 0;

        private DataRowView sonEklenenItemAl()
        {
            DataRowView enBuyukItem = (DataRowView)cbMarkalar.Items[0];
            foreach (DataRowView cbMarkalarItem in cbMarkalar.Items)
                if (int.Parse(cbMarkalarItem.Row["Id"].ToString()) > int.Parse(enBuyukItem.Row["Id"].ToString()))
                    enBuyukItem = cbMarkalarItem;
            
            return enBuyukItem;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            _dbOperations.ScalarTextCommand("Insert Into Marka(Adi) Values('" + txtMarkaAdi.Text.ToUpper() + "')");
            _dbOperations.LoadComboBox(cbMarkalar, "MARKASORGULAMA");
            txtMarkaAdi.Text = "";
            cbMarkalar.SelectedItem = sonEklenenItemAl();
            nfBasarili.BalloonTipText = "Marka başarılı bir şekilde eklendi.";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(2000);
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            _dbOperations.ScalarTextCommand("Delete from Marka Where Id = " + ((DataRowView)cbMarkalar.SelectedItem).Row["Id"]);
            _dbOperations.LoadComboBox(cbMarkalar, "MARKASORGULAMA");
            nfBasarili.BalloonTipText = "Marka başarılı bir şekilde silindi.";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(2000);
        }
    }
}

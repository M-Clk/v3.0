using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmUrunSatisVerileri : Form
    {
        public frmUrunSatisVerileri()
        {
            InitializeComponent();
        }
        bool sayiGiriliyor = true;
        private void txtTop_KeyPress(object sender, KeyPressEventArgs e)
        { 
               if(sayiGiriliyor) e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txtTop_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                Doldur();
                if (!sayiGiriliyor) txtTop.Text = "";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedIndex == 2)
            {
                txtTop.Visible = false;
                textBox1.Visible = false;
            }
            else
            {
                
                if (comboBox1.SelectedIndex == 3)
                {
                   
                    sayiGiriliyor = false;
                    txtTop.Text = "";
                    textBox1.Text = "  Kodlu Ürün";
                    txtTop.Width = 100;
                }
                else
                {
                    sayiGiriliyor = true;
                    txtTop.Text = "10";
                    textBox1.Text = "  Ürün Göster";
                    txtTop.Width = 40;
                }
                textBox1.Visible = true;
                txtTop.Visible = true;
                txtTop.Select();
            }
            Doldur();
        }
        int top=10;
        DbOperations DbOp = new DbOperations();
        void Doldur()
        {
            try
            {
                top = Convert.ToInt32(txtTop.Text);
            }
            catch (Exception)
            {
                top = 10;
                if(sayiGiriliyor)
                txtTop.Text = "10";
            }
            try
            {
                dgUrunVerileri.Rows.Clear();
                SqlParameter[] sqlPrm = new SqlParameter[3];
                sqlPrm[0] = new SqlParameter();
                sqlPrm[0].ParameterName = "@GoruntelenecekAdet";
                sqlPrm[0].SqlDbType = SqlDbType.Int;
                sqlPrm[0].SqlValue = top;

                sqlPrm[1] = new SqlParameter();
                sqlPrm[1].ParameterName = "@Tur";
                sqlPrm[1].SqlDbType = SqlDbType.TinyInt;
                sqlPrm[1].Value = comboBox1.SelectedIndex;

                sqlPrm[2] = new SqlParameter();
                sqlPrm[2].ParameterName = "@BarkodKodu";
                sqlPrm[2].SqlDbType = SqlDbType.NVarChar;
                sqlPrm[2].Value = txtTop.Text;
                SqlDataReader tabOku = DbOp.OkuProcedure("ENCOKSATANLAR", sqlPrm);
                string[] sutunlar = new string[5];
                int i = 1;
                while (tabOku.Read())
                {
                    sutunlar[0] = i.ToString();
                    sutunlar[1] = tabOku[1].ToString();
                    sutunlar[2] = tabOku[2].ToString();

                    int ondalik = Convert.ToInt32(tabOku[3]);
                    decimal kesirli = Convert.ToDecimal(tabOku[3]);

                    if (ondalik - kesirli == 0)
                        sutunlar[3] = Convert.ToInt32(tabOku[3]).ToString();
                    else
                        sutunlar[3] = tabOku[3].ToString();

                    sutunlar[4] = tabOku[4].ToString();
                    dgUrunVerileri.Rows.Add(sutunlar);
                    i++;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void frmUrunSatisVerileri_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}

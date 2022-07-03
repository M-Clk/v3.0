using System;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class frmDestek : Form
    {
        public frmDestek()
        {
            InitializeComponent();
        }

        private void frmDestek_Load(object sender, EventArgs e)
        {
            textBox1.SelectionStart = 0;
        }
    }
}
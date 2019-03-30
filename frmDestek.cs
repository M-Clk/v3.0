using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            textBox1.SelectionStart =0;
        }
    }
}

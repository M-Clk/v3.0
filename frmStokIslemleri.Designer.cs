namespace Otomasyon
{
    partial class frmStokIslemleri
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStokIslemleri));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gbDuzenle = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label18 = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            this.txtBarkodSorgula = new System.Windows.Forms.TextBox();
            this.txtAdaGoreAra = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgUrunler = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.btnEkle = new System.Windows.Forms.Button();
            this.btnMarka = new System.Windows.Forms.Button();
            this.btnTopluFiyatGuncelle = new System.Windows.Forms.Button();
            this.tmYanipSonme = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.nfBasarili = new System.Windows.Forms.NotifyIcon(this.components);
            this.saveExceleKaydet = new System.Windows.Forms.SaveFileDialog();
            this.gbDuzenle.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgUrunler)).BeginInit();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDuzenle
            // 
            this.gbDuzenle.Controls.Add(this.tableLayoutPanel2);
            this.gbDuzenle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbDuzenle.Location = new System.Drawing.Point(3, 3);
            this.gbDuzenle.Name = "gbDuzenle";
            this.gbDuzenle.Padding = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.gbDuzenle.Size = new System.Drawing.Size(1306, 616);
            this.gbDuzenle.TabIndex = 1;
            this.gbDuzenle.TabStop = false;
            this.gbDuzenle.Text = "Ürün Görüntüle - Sil";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dgUrunler, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.5F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1300, 594);
            this.tableLayoutPanel2.TabIndex = 11;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 6;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanel3.Controls.Add(this.label18, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnExcel, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtBarkodSorgula, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.txtAdaGoreAra, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label11, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1300, 68);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(120, 27);
            this.label18.Margin = new System.Windows.Forms.Padding(3);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(118, 13);
            this.label18.TabIndex = 10;
            this.label18.Text = "Adına Göre Ürün Arayın";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExcel.Enabled = false;
            this.btnExcel.FlatAppearance.BorderSize = 0;
            this.btnExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcel.Image = ((System.Drawing.Image)(resources.GetObject("btnExcel.Image")));
            this.btnExcel.Location = new System.Drawing.Point(1217, 3);
            this.btnExcel.Margin = new System.Windows.Forms.Padding(4, 3, 3, 3);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(80, 62);
            this.btnExcel.TabIndex = 23;
            this.btnExcel.Text = "Excel\'e Aktar";
            this.btnExcel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Visible = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            this.btnExcel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAnaForm_AllControls);
            // 
            // txtBarkodSorgula
            // 
            this.txtBarkodSorgula.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtBarkodSorgula.Location = new System.Drawing.Point(967, 24);
            this.txtBarkodSorgula.Name = "txtBarkodSorgula";
            this.txtBarkodSorgula.Size = new System.Drawing.Size(127, 20);
            this.txtBarkodSorgula.TabIndex = 2;
            this.txtBarkodSorgula.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarkodSorgula_KeyDown);
            this.txtBarkodSorgula.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAdi_KeyPress);
            // 
            // txtAdaGoreAra
            // 
            this.txtAdaGoreAra.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtAdaGoreAra.Location = new System.Drawing.Point(244, 24);
            this.txtAdaGoreAra.Name = "txtAdaGoreAra";
            this.txtAdaGoreAra.Size = new System.Drawing.Size(100, 20);
            this.txtAdaGoreAra.TabIndex = 2;
            this.txtAdaGoreAra.TextChanged += new System.EventHandler(this.txtAdaGoreAra_TextChanged);
            this.txtAdaGoreAra.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAdaGoreAra_KeyDown);
            this.txtAdaGoreAra.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAdi_KeyPress);
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(556, 1);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(92, 66);
            this.label11.TabIndex = 9;
            this.label11.Text = "Kritik Ürünler";
            this.label11.Visible = false;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(824, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Barkod kodunu okutun (F1)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgUrunler
            // 
            this.dgUrunler.AllowUserToAddRows = false;
            this.dgUrunler.AllowUserToDeleteRows = false;
            this.dgUrunler.AllowUserToResizeRows = false;
            this.dgUrunler.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgUrunler.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgUrunler.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgUrunler.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgUrunler.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgUrunler.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgUrunler.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgUrunler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgUrunler.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgUrunler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgUrunler.EnableHeadersVisualStyles = false;
            this.dgUrunler.GridColor = System.Drawing.SystemColors.Control;
            this.dgUrunler.Location = new System.Drawing.Point(3, 71);
            this.dgUrunler.Name = "dgUrunler";
            this.dgUrunler.ReadOnly = true;
            this.dgUrunler.RowHeadersVisible = false;
            this.dgUrunler.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgUrunler.Size = new System.Drawing.Size(1294, 519);
            this.dgUrunler.TabIndex = 7;
            this.dgUrunler.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgUrunler_CellClick);
            this.dgUrunler.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgUrunler_CellDoubleClick);
            this.dgUrunler.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgUrunler_RowsRemoved);
            this.dgUrunler.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAnaForm_AllControls);
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 4;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel10.Controls.Add(this.btnEkle, 3, 0);
            this.tableLayoutPanel10.Controls.Add(this.btnMarka, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.btnTopluFiyatGuncelle, 1, 0);
            this.tableLayoutPanel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel10.Location = new System.Drawing.Point(2, 624);
            this.tableLayoutPanel10.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 1;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(1308, 79);
            this.tableLayoutPanel10.TabIndex = 34;
            // 
            // btnEkle
            // 
            this.btnEkle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnEkle.FlatAppearance.BorderSize = 0;
            this.btnEkle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEkle.Image = ((System.Drawing.Image)(resources.GetObject("btnEkle.Image")));
            this.btnEkle.Location = new System.Drawing.Point(1225, 3);
            this.btnEkle.Name = "btnEkle";
            this.btnEkle.Size = new System.Drawing.Size(80, 72);
            this.btnEkle.TabIndex = 22;
            this.btnEkle.Text = "Ürün Ekle";
            this.btnEkle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEkle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEkle.UseVisualStyleBackColor = true;
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
            this.btnEkle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAnaForm_AllControls);
            // 
            // btnMarka
            // 
            this.btnMarka.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnMarka.FlatAppearance.BorderSize = 0;
            this.btnMarka.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarka.Image = global::Otomasyon.Properties.Resources.icons8_brand_24_cup;
            this.btnMarka.Location = new System.Drawing.Point(1135, 3);
            this.btnMarka.Name = "btnMarka";
            this.btnMarka.Size = new System.Drawing.Size(80, 72);
            this.btnMarka.TabIndex = 24;
            this.btnMarka.Text = "Marka İşlemleri";
            this.btnMarka.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnMarka.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnMarka.UseVisualStyleBackColor = true;
            this.btnMarka.Click += new System.EventHandler(this.btnMarka_Click);
            // 
            // btnTopluFiyatGuncelle
            // 
            this.btnTopluFiyatGuncelle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTopluFiyatGuncelle.FlatAppearance.BorderSize = 0;
            this.btnTopluFiyatGuncelle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTopluFiyatGuncelle.Image = global::Otomasyon.Properties.Resources.icons8_sale_price_tag_24;
            this.btnTopluFiyatGuncelle.Location = new System.Drawing.Point(1045, 3);
            this.btnTopluFiyatGuncelle.Name = "btnTopluFiyatGuncelle";
            this.btnTopluFiyatGuncelle.Size = new System.Drawing.Size(80, 72);
            this.btnTopluFiyatGuncelle.TabIndex = 25;
            this.btnTopluFiyatGuncelle.Text = "Toplu Fiyat Güncelle";
            this.btnTopluFiyatGuncelle.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTopluFiyatGuncelle.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnTopluFiyatGuncelle.UseVisualStyleBackColor = true;
            this.btnTopluFiyatGuncelle.Click += new System.EventHandler(this.btnTopluFiyatGuncelle_Click);
            // 
            // tmYanipSonme
            // 
            this.tmYanipSonme.Interval = 10;
            this.tmYanipSonme.Tick += new System.EventHandler(this.tmYanipSonme_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel10, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gbDuzenle, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(19, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(11, 12, 11, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.34197F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.65803F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1312, 705);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // nfBasarili
            // 
            this.nfBasarili.BalloonTipText = "İşlem başarıyla gerçekleşti.";
            this.nfBasarili.BalloonTipTitle = "Başarılı İşlem";
            this.nfBasarili.Icon = ((System.Drawing.Icon)(resources.GetObject("nfBasarili.Icon")));
            this.nfBasarili.Text = "Başarılı";
            this.nfBasarili.BalloonTipClicked += new System.EventHandler(this.nfBasariliKapat);
            this.nfBasarili.BalloonTipClosed += new System.EventHandler(this.nfBasariliKapat);
            this.nfBasarili.Click += new System.EventHandler(this.nfBasariliKapat);
            this.nfBasarili.DoubleClick += new System.EventHandler(this.nfBasariliKapat);
            // 
            // frmStokIslemleri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmStokIslemleri";
            this.Padding = new System.Windows.Forms.Padding(19, 12, 19, 12);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stok Islemleri";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmStokIslemleri_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAnaForm_AllControls);
            this.gbDuzenle.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgUrunler)).EndInit();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDuzenle;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtAdaGoreAra;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.TextBox txtBarkodSorgula;
        private System.Windows.Forms.DataGridView dgUrunler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEkle;
        public System.Windows.Forms.Timer tmYanipSonme;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.NotifyIcon nfBasarili;
        private System.Windows.Forms.SaveFileDialog saveExceleKaydet;
        private System.Windows.Forms.Button btnMarka;
        private System.Windows.Forms.Button btnTopluFiyatGuncelle;
    }
}
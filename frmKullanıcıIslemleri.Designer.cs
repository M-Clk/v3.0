namespace Otomasyon
{
    partial class frmKullanıcıIslemleri
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKullanıcıIslemleri));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSifreGoster1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtYeni = new System.Windows.Forms.TextBox();
            this.txtEski = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblK_Adi = new System.Windows.Forms.Label();
            this.btnGuncelle = new System.Windows.Forms.Button();
            this.cbSifreIste = new System.Windows.Forms.CheckBox();
            this.gbGuncelleSil = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbSifreGoster2 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbYetki = new System.Windows.Forms.ComboBox();
            this.txtK_Ad = new System.Windows.Forms.TextBox();
            this.txt_Sifre = new System.Windows.Forms.TextBox();
            this.btnKGuncelle = new System.Windows.Forms.Button();
            this.btnEkle = new System.Windows.Forms.Button();
            this.gbKullanicilar = new System.Windows.Forms.GroupBox();
            this.dgKullanicilar = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Adi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Yetki = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sifre = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.nfBasarili = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbGuncelleSil.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.gbKullanicilar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgKullanicilar)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.groupBox1.Size = new System.Drawing.Size(367, 202);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Şifre Değiştir";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.21365F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.78635F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbSifreGoster1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtYeni, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtEski, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblK_Adi, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGuncelle, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.cbSifreIste, 1, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 29);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(337, 157);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Eski Şifreniz :";
            // 
            // cbSifreGoster1
            // 
            this.cbSifreGoster1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSifreGoster1.AutoSize = true;
            this.cbSifreGoster1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbSifreGoster1.Location = new System.Drawing.Point(151, 72);
            this.cbSifreGoster1.Name = "cbSifreGoster1";
            this.cbSifreGoster1.Size = new System.Drawing.Size(183, 17);
            this.cbSifreGoster1.TabIndex = 10;
            this.cbSifreGoster1.Text = "Şifreleri Göster";
            this.cbSifreGoster1.UseVisualStyleBackColor = true;
            this.cbSifreGoster1.CheckedChanged += new System.EventHandler(this.cbSifreGoster1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Yeni Şifreniz :";
            // 
            // txtYeni
            // 
            this.txtYeni.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtYeni.Location = new System.Drawing.Point(151, 49);
            this.txtYeni.Name = "txtYeni";
            this.txtYeni.PasswordChar = '•';
            this.txtYeni.Size = new System.Drawing.Size(127, 20);
            this.txtYeni.TabIndex = 5;
            // 
            // txtEski
            // 
            this.txtEski.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtEski.Location = new System.Drawing.Point(151, 26);
            this.txtEski.Name = "txtEski";
            this.txtEski.PasswordChar = '•';
            this.txtEski.Size = new System.Drawing.Size(127, 20);
            this.txtEski.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(3, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Kullanıcı Adınız :";
            // 
            // lblK_Adi
            // 
            this.lblK_Adi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblK_Adi.AutoSize = true;
            this.lblK_Adi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblK_Adi.Location = new System.Drawing.Point(151, 3);
            this.lblK_Adi.Name = "lblK_Adi";
            this.lblK_Adi.Size = new System.Drawing.Size(183, 17);
            this.lblK_Adi.TabIndex = 3;
            // 
            // btnGuncelle
            // 
            this.btnGuncelle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGuncelle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnGuncelle.FlatAppearance.BorderSize = 0;
            this.btnGuncelle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuncelle.Location = new System.Drawing.Point(151, 95);
            this.btnGuncelle.Name = "btnGuncelle";
            this.btnGuncelle.Size = new System.Drawing.Size(75, 25);
            this.btnGuncelle.TabIndex = 6;
            this.btnGuncelle.Text = "Güncelle";
            this.btnGuncelle.UseVisualStyleBackColor = false;
            this.btnGuncelle.Click += new System.EventHandler(this.btnGuncelle_Click);
            // 
            // cbSifreIste
            // 
            this.cbSifreIste.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSifreIste.AutoSize = true;
            this.cbSifreIste.ForeColor = System.Drawing.Color.Red;
            this.cbSifreIste.Location = new System.Drawing.Point(151, 126);
            this.cbSifreIste.Name = "cbSifreIste";
            this.cbSifreIste.Size = new System.Drawing.Size(183, 28);
            this.cbSifreIste.TabIndex = 12;
            this.cbSifreIste.Text = "Giriş Yaparken Şifre İstesin";
            this.cbSifreIste.UseVisualStyleBackColor = true;
            this.cbSifreIste.Visible = false;
            this.cbSifreIste.CheckedChanged += new System.EventHandler(this.chbSifreIste_CheckedChanged);
            // 
            // gbGuncelleSil
            // 
            this.gbGuncelleSil.Controls.Add(this.tableLayoutPanel2);
            this.gbGuncelleSil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbGuncelleSil.Location = new System.Drawing.Point(3, 211);
            this.gbGuncelleSil.Name = "gbGuncelleSil";
            this.gbGuncelleSil.Padding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.gbGuncelleSil.Size = new System.Drawing.Size(367, 202);
            this.gbGuncelleSil.TabIndex = 2;
            this.gbGuncelleSil.TabStop = false;
            this.gbGuncelleSil.Text = "Kullanıcı Ekle - Güncelle";
            this.gbGuncelleSil.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.14285F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.14285F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.71429F));
            this.tableLayoutPanel2.Controls.Add(this.cbSifreGoster2, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbYetki, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.txtK_Ad, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.txt_Sifre, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnKGuncelle, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnEkle, 1, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(15, 29);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(337, 157);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // cbSifreGoster2
            // 
            this.cbSifreGoster2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSifreGoster2.AutoSize = true;
            this.cbSifreGoster2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbSifreGoster2.Location = new System.Drawing.Point(219, 47);
            this.cbSifreGoster2.Name = "cbSifreGoster2";
            this.cbSifreGoster2.Size = new System.Drawing.Size(115, 17);
            this.cbSifreGoster2.TabIndex = 11;
            this.cbSifreGoster2.Text = "Şifreyi Göster";
            this.cbSifreGoster2.UseVisualStyleBackColor = true;
            this.cbSifreGoster2.CheckedChanged += new System.EventHandler(this.cbSifreGoster2_CheckedChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Kullanıcı Adı";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 49);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Şifresi";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Yetkisi";
            // 
            // cbYetki
            // 
            this.cbYetki.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbYetki.DisplayMember = "0";
            this.cbYetki.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbYetki.FormattingEnabled = true;
            this.cbYetki.Items.AddRange(new object[] {
            "Diğer",
            "Yönetici"});
            this.cbYetki.Location = new System.Drawing.Point(111, 82);
            this.cbYetki.Name = "cbYetki";
            this.cbYetki.Size = new System.Drawing.Size(102, 21);
            this.cbYetki.TabIndex = 11;
            // 
            // txtK_Ad
            // 
            this.txtK_Ad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtK_Ad.Location = new System.Drawing.Point(111, 8);
            this.txtK_Ad.Name = "txtK_Ad";
            this.txtK_Ad.Size = new System.Drawing.Size(102, 20);
            this.txtK_Ad.TabIndex = 5;
            // 
            // txt_Sifre
            // 
            this.txt_Sifre.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Sifre.Location = new System.Drawing.Point(111, 45);
            this.txt_Sifre.Name = "txt_Sifre";
            this.txt_Sifre.PasswordChar = '•';
            this.txt_Sifre.Size = new System.Drawing.Size(102, 20);
            this.txt_Sifre.TabIndex = 6;
            // 
            // btnKGuncelle
            // 
            this.btnKGuncelle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnKGuncelle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnKGuncelle.FlatAppearance.BorderSize = 0;
            this.btnKGuncelle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKGuncelle.Location = new System.Drawing.Point(239, 119);
            this.btnKGuncelle.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnKGuncelle.Name = "btnKGuncelle";
            this.btnKGuncelle.Size = new System.Drawing.Size(75, 35);
            this.btnKGuncelle.TabIndex = 13;
            this.btnKGuncelle.Text = "Güncelle";
            this.btnKGuncelle.UseVisualStyleBackColor = false;
            this.btnKGuncelle.Click += new System.EventHandler(this.btnKGuncelle_Click);
            // 
            // btnEkle
            // 
            this.btnEkle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.btnEkle.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnEkle.FlatAppearance.BorderSize = 0;
            this.btnEkle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEkle.Location = new System.Drawing.Point(124, 119);
            this.btnEkle.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.btnEkle.Name = "btnEkle";
            this.btnEkle.Size = new System.Drawing.Size(75, 35);
            this.btnEkle.TabIndex = 14;
            this.btnEkle.Text = "Ekle";
            this.btnEkle.UseVisualStyleBackColor = false;
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
            // 
            // gbKullanicilar
            // 
            this.gbKullanicilar.Controls.Add(this.dgKullanicilar);
            this.gbKullanicilar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbKullanicilar.Location = new System.Drawing.Point(380, 3);
            this.gbKullanicilar.Name = "gbKullanicilar";
            this.gbKullanicilar.Padding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.gbKullanicilar.Size = new System.Drawing.Size(371, 414);
            this.gbKullanicilar.TabIndex = 4;
            this.gbKullanicilar.TabStop = false;
            this.gbKullanicilar.Text = "Bütün Kullanıcılar";
            this.gbKullanicilar.Visible = false;
            // 
            // dgKullanicilar
            // 
            this.dgKullanicilar.AllowUserToAddRows = false;
            this.dgKullanicilar.AllowUserToDeleteRows = false;
            this.dgKullanicilar.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgKullanicilar.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgKullanicilar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgKullanicilar.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgKullanicilar.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.DeepSkyBlue;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.InactiveBorder;
            dataGridViewCellStyle3.NullValue = null;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Desktop;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ScrollBar;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgKullanicilar.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgKullanicilar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgKullanicilar.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Adi,
            this.Yetki,
            this.Sifre});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.AliceBlue;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HotTrack;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgKullanicilar.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgKullanicilar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgKullanicilar.EnableHeadersVisualStyles = false;
            this.dgKullanicilar.GridColor = System.Drawing.SystemColors.Control;
            this.dgKullanicilar.Location = new System.Drawing.Point(15, 29);
            this.dgKullanicilar.Name = "dgKullanicilar";
            this.dgKullanicilar.ReadOnly = true;
            this.dgKullanicilar.RowHeadersVisible = false;
            this.dgKullanicilar.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgKullanicilar.Size = new System.Drawing.Size(341, 369);
            this.dgKullanicilar.TabIndex = 11;
            this.dgKullanicilar.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgKullanicilar_CellClick);
            this.dgKullanicilar.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgKullanicilar_CellDoubleClick);
            this.dgKullanicilar.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgKullanicilar_CellFormatting);
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.No.FillWeight = 25F;
            this.No.HeaderText = "No";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.No.Width = 40;
            // 
            // Adi
            // 
            this.Adi.HeaderText = "Kullanıcı Adı";
            this.Adi.Name = "Adi";
            this.Adi.ReadOnly = true;
            this.Adi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Yetki
            // 
            this.Yetki.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Yetki.FillWeight = 45F;
            this.Yetki.HeaderText = "Yetkisi";
            this.Yetki.Name = "Yetki";
            this.Yetki.ReadOnly = true;
            this.Yetki.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Sifre
            // 
            this.Sifre.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Sifre.FillWeight = 50F;
            this.Sifre.HeaderText = "Şifresi";
            this.Sifre.Name = "Sifre";
            this.Sifre.ReadOnly = true;
            this.Sifre.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.gbKullanicilar, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(15, 16);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(754, 420);
            this.tableLayoutPanel3.TabIndex = 5;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.gbGuncelleSil, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(373, 416);
            this.tableLayoutPanel4.TabIndex = 0;
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
            // frmKullanıcıIslemleri
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(784, 452);
            this.Controls.Add(this.tableLayoutPanel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmKullanıcıIslemleri";
            this.Padding = new System.Windows.Forms.Padding(15, 16, 15, 16);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kullanıcı İşlemleri";
            this.Load += new System.EventHandler(this.frmKullanıcıIslemleri_Load);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gbGuncelleSil.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.gbKullanicilar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgKullanicilar)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbSifreGoster1;
        private System.Windows.Forms.Button btnGuncelle;
        private System.Windows.Forms.TextBox txtYeni;
        private System.Windows.Forms.TextBox txtEski;
        private System.Windows.Forms.Label lblK_Adi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox gbGuncelleSil;
        private System.Windows.Forms.CheckBox cbSifreGoster2;
        private System.Windows.Forms.Button btnEkle;
        private System.Windows.Forms.Button btnKGuncelle;
        private System.Windows.Forms.ComboBox cbYetki;
        private System.Windows.Forms.TextBox txt_Sifre;
        private System.Windows.Forms.TextBox txtK_Ad;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox gbKullanicilar;
        private System.Windows.Forms.DataGridView dgKullanicilar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Adi;
        private System.Windows.Forms.DataGridViewTextBoxColumn Yetki;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sifre;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        public System.Windows.Forms.NotifyIcon nfBasarili;
        private System.Windows.Forms.CheckBox cbSifreIste;
    }
}
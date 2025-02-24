namespace Otomasyon
{
    partial class frmTopluFiyatGuncelleme
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTopluFiyatGuncelleme));
            this.cbMarkalar = new System.Windows.Forms.ComboBox();
            this.dgUrunler = new System.Windows.Forms.DataGridView();
            this.numYuzde = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnZam = new System.Windows.Forms.Button();
            this.btnIndirim = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblBekleyin = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgUrunler)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYuzde)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbMarkalar
            // 
            this.cbMarkalar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cbMarkalar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMarkalar.FormattingEnabled = true;
            this.cbMarkalar.Location = new System.Drawing.Point(3, 17);
            this.cbMarkalar.Name = "cbMarkalar";
            this.cbMarkalar.Size = new System.Drawing.Size(244, 21);
            this.cbMarkalar.TabIndex = 39;
            this.cbMarkalar.SelectedIndexChanged += new System.EventHandler(this.cbMarkalar_SelectedIndexChanged);
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
            this.dgUrunler.Location = new System.Drawing.Point(3, 64);
            this.dgUrunler.MultiSelect = false;
            this.dgUrunler.Name = "dgUrunler";
            this.dgUrunler.ReadOnly = true;
            this.dgUrunler.RowHeadersVisible = false;
            this.dgUrunler.RowTemplate.ReadOnly = true;
            this.dgUrunler.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgUrunler.Size = new System.Drawing.Size(1118, 551);
            this.dgUrunler.TabIndex = 40;
            // 
            // numYuzde
            // 
            this.numYuzde.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numYuzde.DecimalPlaces = 2;
            this.numYuzde.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.numYuzde.Location = new System.Drawing.Point(462, 13);
            this.numYuzde.Name = "numYuzde";
            this.numYuzde.Size = new System.Drawing.Size(75, 30);
            this.numYuzde.TabIndex = 41;
            this.numYuzde.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(425, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 25);
            this.label1.TabIndex = 42;
            this.label1.Text = "%";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numYuzde, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnZam, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnIndirim, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 621);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1118, 57);
            this.tableLayoutPanel1.TabIndex = 43;
            // 
            // btnZam
            // 
            this.btnZam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZam.Enabled = false;
            this.btnZam.FlatAppearance.BorderSize = 0;
            this.btnZam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZam.Image = global::Otomasyon.Properties.Resources.icons8_slide_up_32;
            this.btnZam.Location = new System.Drawing.Point(662, 3);
            this.btnZam.Name = "btnZam";
            this.btnZam.Size = new System.Drawing.Size(94, 51);
            this.btnZam.TabIndex = 44;
            this.btnZam.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnZam.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnZam.UseVisualStyleBackColor = true;
            this.btnZam.Click += new System.EventHandler(this.btnZam_Click);
            // 
            // btnIndirim
            // 
            this.btnIndirim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIndirim.Enabled = false;
            this.btnIndirim.FlatAppearance.BorderSize = 0;
            this.btnIndirim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIndirim.Image = global::Otomasyon.Properties.Resources.icons8_down_button_32;
            this.btnIndirim.Location = new System.Drawing.Point(562, 3);
            this.btnIndirim.Name = "btnIndirim";
            this.btnIndirim.Size = new System.Drawing.Size(94, 51);
            this.btnIndirim.TabIndex = 43;
            this.btnIndirim.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnIndirim.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnIndirim.UseVisualStyleBackColor = true;
            this.btnIndirim.Click += new System.EventHandler(this.btnIndirim_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.dgUrunler, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 81.81818F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.090909F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1124, 681);
            this.tableLayoutPanel2.TabIndex = 44;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel3.Controls.Add(this.cbMarkalar, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblBekleyin, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1118, 55);
            this.tableLayoutPanel3.TabIndex = 46;
            // 
            // lblBekleyin
            // 
            this.lblBekleyin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBekleyin.AutoSize = true;
            this.lblBekleyin.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblBekleyin.ForeColor = System.Drawing.Color.Red;
            this.lblBekleyin.Location = new System.Drawing.Point(338, 19);
            this.lblBekleyin.Name = "lblBekleyin";
            this.lblBekleyin.Size = new System.Drawing.Size(441, 17);
            this.lblBekleyin.TabIndex = 40;
            this.lblBekleyin.Text = "Fiyatlar Güncelleniyor... Lütfen Bekleyin.";
            this.lblBekleyin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBekleyin.Visible = false;
            // 
            // frmTopluFiyatGuncelleme
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1124, 681);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTopluFiyatGuncelleme";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Toplu Fiyat Güncelle";
            ((System.ComponentModel.ISupportInitialize)(this.dgUrunler)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYuzde)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbMarkalar;
        private System.Windows.Forms.DataGridView dgUrunler;
        private System.Windows.Forms.NumericUpDown numYuzde;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnZam;
        private System.Windows.Forms.Button btnIndirim;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label lblBekleyin;
    }
}
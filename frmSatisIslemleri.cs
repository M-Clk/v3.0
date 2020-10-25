using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing.Printing;

namespace Otomasyon
{
    public partial class frmSatisIslemleri : Form
    {
        private ExcelPackage _package;
        public frmSatisIslemleri()
        {
            InitializeComponent();
         
        }
        public frmSatisIslemleri(int satisId, decimal tutar, DateTime tarih, string musteriAd, string kasiyerAdi,decimal nakit, decimal kredi, int taksit)
        {
            InitializeComponent();
            seciliSatis = satisId;
            seciliTutar = tutar;
            seciliTarih = tarih;
            seciliMusteriAdi = musteriAd;
            seciliKasiyerAdi = kasiyerAdi;
            seciliNakit = nakit;
            seciliKredi = kredi;
            seciliTaksit = taksit;
            SatisDetayiGoster(satisId, tutar, tarih, musteriAd, kasiyerAdi,nakit,kredi,taksit);

        }
        DbOperations SqlCnnctn = new DbOperations();
        decimal KasadakiSonPara=0,EkGelir=0,EkGider=0;
        private void frmSatisIslemleri_Load(object sender, EventArgs e)
        {
            if (Program.yetki == Program.Yetki.eleman)
                chrtSatislar.Visible = false;
                chrtSatislar.Series[0].Name = "Satış Tutarı";
            chrtSatislar.Series[1].Name = "Satış Kârı";
            cbSecenek.SelectedIndex = 0;
            cbSecenek.Width = 160;
        }
        decimal netKazanc=0,topNakit=0,topKredi=0;
        void SatislariGoster()
        {
            
            dgSatislar.Rows.Clear();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); //Sql sorgusunda datetime formatı ile aynı olması için kültür formatını ingiliz formatına çevir.
            DateTime parameterBaslangicTarihi = DateTime.Now, parameterBitisTarihi = DateTime.Now;
            if (cbSecenek.SelectedIndex == 1)
            {
                //dtBaslangicTarihi değeri geldiğinde geldiği zamanın saati ile gelecektir. Bu da tüm kayıtların gelmemesine sebep olabilir. Bunu engellemek sadece tarihi(.Date) almak gerek. Saat de otomatik olarak 00:00:00 olacaktır.
                parameterBaslangicTarihi = dtBaslangicTarihi.Value.Date;

            }
            else if (cbSecenek.SelectedIndex == 2)
            {
                //Yukarıdaki işlem tekrarlandı.
                parameterBaslangicTarihi = dtBaslangicTarihi.Value.Date;

                parameterBitisTarihi = dtBitisTarihi.Value.Date;
            }

            EkGelir = 0; 
            EkGider = 0;
            KasadakiSonPara = 0;
            topKredi = 0;
            topNakit = 0;
            SqlParameter[] FilterParameter = new SqlParameter[5];
            FilterParameter[0] = new SqlParameter();
            FilterParameter[0].ParameterName = "@SorguTipi";
            FilterParameter[0].SqlDbType = SqlDbType.TinyInt;
            FilterParameter[0].SqlValue = cbSecenek.SelectedIndex;
            FilterParameter[1] = new SqlParameter();
            FilterParameter[1].ParameterName = "@BaslangicTarihi";
            FilterParameter[1].SqlDbType = SqlDbType.DateTime;
            FilterParameter[1].SqlValue = parameterBaslangicTarihi;
            FilterParameter[2] = new SqlParameter();
            FilterParameter[2].ParameterName = "@BitisTarihi";
            FilterParameter[2].SqlDbType = SqlDbType.DateTime;
            FilterParameter[2].SqlValue = parameterBitisTarihi;
            FilterParameter[3] = new SqlParameter();
            FilterParameter[3].ParameterName = "@SatisId";
            FilterParameter[3].SqlDbType = SqlDbType.Int;
            int satisId;
            try
            {
                satisId = Convert.ToInt32(txtAra.Text);
                FilterParameter[3].SqlValue = satisId;
            }
            catch
            {
                FilterParameter[3].SqlValue = 0;
            }
            FilterParameter[4] = new SqlParameter();
            FilterParameter[4].ParameterName = "@MusteriAdi";
            FilterParameter[4].SqlDbType = SqlDbType.NVarChar;
            FilterParameter[4].SqlValue = txtAra.Text;
            
            SqlDataReader FiltreOkuyucu = SqlCnnctn.OkuProcedure("SATISFILTRELE", FilterParameter); Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR"); //Sorgu bittiğinde sonucu tekrar aynı formatta almak için türk formatına geri çevir.
            decimal topKar = 0, topTutar = 0,sonSatirKasadaki=0;
            string[] satir = new string[dgSatislar.ColumnCount];
            if (FiltreOkuyucu != null)
                while (FiltreOkuyucu.Read())
                {
                    if (satir[0] == null) KasadakiSonPara = Convert.ToDecimal(FiltreOkuyucu[7]);
                    if (cbSecenek.SelectedIndex != 0)
                        sonSatirKasadaki = Convert.ToDecimal(FiltreOkuyucu[10]);
                    satir[0] = Convert.ToInt32(FiltreOkuyucu[0]).ToString("D10"); //Satış No
                    satir[1] = FiltreOkuyucu[1].ToString(); //Müşteri Adı 
                    satir[2] = FiltreOkuyucu[2].ToString(); //Telefon
                    satir[3] = FiltreOkuyucu[3].ToString(); //Tarih
                    satir[4] = FiltreOkuyucu[4].ToString(); //Tutar
                    satir[5] = FiltreOkuyucu[5].ToString(); //Kâr
                    satir[6] = FiltreOkuyucu[6].ToString(); //Kasiyer Adı
                    satir[7] = FiltreOkuyucu[8].ToString(); //Ödenen Nakit
                    satir[8] = FiltreOkuyucu[9].ToString(); //Ödenen Kredi
                    satir[9] = FiltreOkuyucu[10].ToString();//Taksit
                    
                    decimal Tutar = Convert.ToDecimal(FiltreOkuyucu[4]);
                    topKar += Convert.ToDecimal(FiltreOkuyucu[5]);
                    topTutar += Tutar;
                    topKredi += Convert.ToDecimal(FiltreOkuyucu[9]);
                   
                    dgSatislar.Rows.Add(satir);
                    if (Convert.ToDecimal(FiltreOkuyucu[4]) == 0) dgSatislar.Rows[dgSatislar.RowCount - 1].Visible = false;
                }
            topNakit = topTutar - topKredi;
            KasadakiSonPara -= sonSatirKasadaki;
            decimal gecici;
            if (FiltreOkuyucu.NextResult())
                while (FiltreOkuyucu.Read())
                {
                        gecici = Convert.ToDecimal(FiltreOkuyucu[3]);
                        if (gecici < 0)
                            EkGider += gecici;
                        else EkGelir += gecici;
                }

            decimal iadeEdilenTop = Convert.ToDecimal(SqlCnnctn.bilgiMessage);
            satir[0] = null;
            decimal topMaliyet = topTutar - topKar;
            if (cbSecenek.SelectedIndex ==3)
                gecici = 0;
            else
            gecici = KasadakiSonPara - topMaliyet+(iadeEdilenTop/100) ;
            netKazanc = gecici;
            lblTopTutar.Text = "Toplam Satış Tutarı : " + topTutar.ToString() + " ₺";
            lblKar.Text = "Toplam Satış Kârı : " + topKar.ToString() + " ₺";
            lblNet.Text = "Kasadaki Toplam : " + (KasadakiSonPara).ToString() + " ₺";
            lblEkGelir.Text = "Toplam Ek Gelir : " + EkGelir.ToString() + " ₺";
            lblEkGider.Text = "Toplam Gider : " + (-EkGider).ToString() + " ₺";
            lblBorc.Text = "Net Kazanç : " + gecici+ " ₺";
            lblNakitToplam.Text = "Nakit : " +topNakit.ToString("F2")+ " ₺";
            lblKrediToplam.Text = "Kredi : " + topKredi.ToString("F2") + " ₺";
            
            GrafikteGoster(cbSecenek.SelectedIndex);
            if (dgSatislar.RowCount > 0) btnExcel.Enabled = true;
            else btnExcel.Enabled = false;
        }
        void GrafikteGoster(int Secenek)
        {
            foreach (Series seriler in chrtSatislar.Series)
            {
                seriler.Points.Clear();
            }
           // DateTime tarh;
            for (int i = 0; i < dgSatislar.Rows.Count; i++)
            {
                //if (Secenek == 0)
                //{
                //    tarh = Convert.ToDateTime(dgSatislar.Rows[i].Cells[4].Value);
                if(dgSatislar.Rows[i].Visible)
                {
chrtSatislar.Series[0].Points.AddXY(Convert.ToInt32(dgSatislar.Rows[i].Cells[0].Value),Convert.ToDecimal(dgSatislar.Rows[i].Cells[4].Value));
                chrtSatislar.Series[1].Points.AddXY(Convert.ToInt32(dgSatislar.Rows[i].Cells[0].Value),Convert.ToDecimal(dgSatislar.Rows[i].Cells[5].Value));
                }
                
                //}
            }
            //chrtSatislar.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
        }

        private void cbSecenek_SelectedIndexChanged(object sender, EventArgs e)
        {

            dtBaslangicTarihi.Visible = false;
            dtBitisTarihi.Visible = false;
            lblTire.Visible = false;
            txtAra.Visible = false;
            lblId.Visible = false;
            txtAra.Text = "";

            if (cbSecenek.SelectedIndex == 1)
            {
                dtBaslangicTarihi.Visible = true;
            }
            else if (cbSecenek.SelectedIndex == 2)
            {
                dtBaslangicTarihi.Visible = true;
                dtBitisTarihi.Visible = true;
                lblTire.Visible = true;
                if(dtBaslangicTarihi.Value.Day==dtBitisTarihi.Value.Day)
                dtBaslangicTarihi.Value=dtBaslangicTarihi.Value.AddDays(-1);
            }
            else if(cbSecenek.SelectedIndex==3)
            {
                txtAra.Visible = true;
                lblId.Visible = true;
                lblId.Text = "Satış Numarası";
            }
            else if(cbSecenek.SelectedIndex==4)
            {
                txtAra.Visible = true;
                lblId.Visible = true;
                lblId.Text = "Müşteri Adı";
            }
            SatislariGoster();
        }
        int seciliSatis = 0,seciliTaksit=0;
        decimal seciliTutar = 0,seciliNakit=0, seciliKredi=0;
        string seciliMusteriAdi = "", seciliKasiyerAdi = "";
        DateTime seciliTarih;
        void SatisDetayiGoster(int Id, decimal tutar, DateTime tarih, string musteriAd, string kasiyerAdi,decimal nakit, decimal kredi, int taksit)
        {
            dgSatisDetayi.Rows.Clear();

            SqlParameter[] DetayAl = new SqlParameter[1];
            DetayAl[0] = new SqlParameter();
            DetayAl[0].ParameterName = "@SatisId";
            DetayAl[0].SqlDbType = SqlDbType.Int;
            DetayAl[0].SqlValue = Id;

            using (SqlDataReader DetayOku = SqlCnnctn.OkuProcedure("SATISDETAYIGETIR", DetayAl))
            {
                string[] satir = new string[dgSatisDetayi.ColumnCount];
                while (DetayOku.Read())
                {
                    satir[0] = DetayOku[0].ToString();

                    satir[1] = DetayOku[1].ToString();

                    satir[2] = DetayOku[2].ToString();

                    satir[3] = DetayOku[3].ToString();

                    int ondalik = Convert.ToInt32(DetayOku[4]);
                    decimal kesirli = Convert.ToDecimal(DetayOku[4]);

                    if (ondalik - kesirli == 0)
                        satir[4] = Convert.ToInt32(DetayOku[4]).ToString();
                    else
                        satir[4] = DetayOku[4].ToString();
                    satir[5] = DetayOku[5].ToString();

                    satir[6] = DetayOku[6].ToString();

                    satir[7] = (Convert.ToDecimal(DetayOku[3]) * Convert.ToDecimal(DetayOku[4])).ToString("F2");
                    dgSatisDetayi.Rows.Add(satir);
                    if (satir[6] == "True" && Convert.ToDecimal(satir[4]) <= 0)
                    {
                        dgSatisDetayi.Rows[dgSatisDetayi.Rows.Count - 1].DefaultCellStyle.BackColor = Color.LightPink;
                        dgSatisDetayi.Rows[dgSatisDetayi.Rows.Count - 1].DefaultCellStyle.SelectionBackColor = Color.LightPink;
                    }

                }
                
                lblTutar.Text = "Tutar : " + tutar + " ₺";
                lblMusteriAdi.Text = "Müşteri Adı : " + musteriAd;
                lblTarih.Text = "Satış Tarihi : " + tarih.ToString();
                lblKasiyer.Text = "Kasiyer Adı : " +kasiyerAdi;
                lblSatisId.Text = "Satış Numarası : " + Id.ToString("D10");
                if (nakit == 0)
                {
                    lblNakit.Text = "Kredi Kartı : " + kredi.ToString("F2") + " ₺";
                    if (taksit == 0) lblParaUstu.Text = "Peşin Ödeme";
                    else lblParaUstu.Text = "Taksit : " + taksit.ToString() + " Ay";
                    lblKredi.Text = "";
                    lblTaksit.Text = "";
                }
                else
                {
                    lblNakit.Text = "Nakit : " + nakit.ToString("F2") + " ₺";
                    decimal paraUstu = nakit + kredi - tutar ;
                    if (paraUstu > 0) lblParaUstu.Text = "Para Üstü : " + paraUstu.ToString("F2") + " ₺";
                    else lblParaUstu.Text = "";
                    if (kredi > 0)
                    {
                        lblKredi.Text = "Kredi Kartı : " + kredi.ToString("F2") + " ₺";
                        if (taksit == 0) lblTaksit.Text = "Peşin Ödeme";
                        else lblTaksit.Text = "Taksit : " + taksit.ToString() + " Ay";
                    }
                }

                btnYazdir.Enabled = true;
                btnExcelDetay.Enabled = true;
            }
        }
        private void dgSatislar_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex !=-1)
            {
                seciliTutar = Convert.ToDecimal(dgSatislar.Rows[e.RowIndex].Cells["Tutar"].Value);
                seciliMusteriAdi = dgSatislar.Rows[e.RowIndex].Cells["Adi"].Value.ToString();
                seciliTarih = Convert.ToDateTime(dgSatislar.Rows[e.RowIndex].Cells["Tarih"].Value.ToString());
                seciliKasiyerAdi = dgSatislar.Rows[e.RowIndex].Cells["KasiyerID"].Value.ToString();
                seciliSatis = Convert.ToInt32(dgSatislar.Rows[e.RowIndex].Cells["Numra"].Value);
                seciliNakit = Convert.ToDecimal(dgSatislar.Rows[e.RowIndex].Cells["Nakit"].Value);
                seciliKredi= Convert.ToDecimal(dgSatislar.Rows[e.RowIndex].Cells["Kredi"].Value);
                seciliTaksit = Convert.ToInt32(dgSatislar.Rows[e.RowIndex].Cells["Taksit"].Value);
                 
                SatisDetayiGoster(seciliSatis,seciliTutar,seciliTarih,seciliMusteriAdi,seciliKasiyerAdi,seciliNakit,seciliKredi,seciliTaksit);
            }
        }

        private void dtBaslangicTarihi_ValueChanged(object sender, EventArgs e)
        {
            if(dtBaslangicTarihi.Visible) SatislariGoster();
        }

        private void dtBitisTarihi_ValueChanged(object sender, EventArgs e)
        {
            if (dtBitisTarihi.Visible) SatislariGoster();
        }

        private void btnExcelDetay_Click(object sender, EventArgs e)
        {
            if (dgSatisDetayi.RowCount > 0) DetayiExceleAktar();
        }
        void DetayiYazdir()
        {
            try
            {
                if (Program.kagiTuru == CrystalDecisions.Shared.PaperSize.PaperEnvelopeB6)
                {

                    CrystalReport2 rapor = new CrystalReport2();
                    rapor.Load(Application.StartupPath + "\\CrystalReport2.rpt");
                    dsFatura ftrTable = new dsFatura();

                    for (int i = 0; i < dgSatisDetayi.Rows.Count; i++)
                    {
                        ftrTable.Tables["tblFatura"].Rows.Add();
                        ftrTable.Tables["tblFatura"].Rows[i][0] = dgSatisDetayi.Rows[i].Cells[2].Value.ToString() + " (" + dgSatisDetayi.Rows[i].Cells[4].Value.ToString() + " " + dgSatisDetayi.Rows[i].Cells[5].Value.ToString() + " X " + dgSatisDetayi.Rows[i].Cells[3].Value.ToString() + " TL)";
                        ftrTable.Tables["tblFatura"].Rows[i][1] = Convert.ToDecimal(dgSatisDetayi.Rows[i].Cells[7].Value);
                    }

                    rapor.SetDataSource(ftrTable);
                    rapor.ParameterFields["TopTutar"].CurrentValues.AddValue(seciliTutar);
                    rapor.ParameterFields["ReportName"].CurrentValues.AddValue(Program.isletmeAdi.ToUpper() + "\n" + Program.adres);
                    rapor.ParameterFields["Tarih"].CurrentValues.AddValue("TARİH : " + seciliTarih.ToShortDateString());
                    rapor.ParameterFields["Saat"].CurrentValues.AddValue("SAAT  : " + seciliTarih.ToShortTimeString());
                    rapor.ParameterFields["SatisNo"].CurrentValues.AddValue(seciliSatis.ToString("D10"));
                    rapor.ParameterFields["KasiyerAdi"].CurrentValues.AddValue(seciliKasiyerAdi.ToUpper());

                    if (seciliNakit == 0)
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("KREDİ KARTI");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                        rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                        rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                    }
                    else
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("NAKİT");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue(" ₺ " + seciliNakit.ToString("F2"));
                        decimal seciliParaUstu = seciliNakit + seciliKredi - seciliTutar;
                        if (seciliParaUstu > 0)
                        {
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliParaUstu.ToString("F2"));
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("PARA ÜSTÜ");
                        }
                        else if (seciliKredi <= 0)
                        {
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                        }
                        if (seciliKredi > 0)
                        {
                            if (seciliParaUstu <= 0)
                            {
                                rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                                rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                            }
                            else
                            {
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                            }

                        }
                        else
                        {
                            rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                            rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        }
                    }


                    rapor.PrintOptions.PrinterName = Program.yaziciAdi;
                    rapor.PrintOptions.PaperSize = Program.kagiTuru;


                    rapor.PrintToPrinter(1, false, 0, 0);

                    rapor.Dispose();
                    ftrTable.Dispose();
                }
                else if (Program.kagiTuru == CrystalDecisions.Shared.PaperSize.PaperEnvelope11)
                {
                    CrystalReport3 rapor = new CrystalReport3();
                    rapor.Load(Application.StartupPath + "\\CrystalReport3.rpt");

                    dsFatura ftrTable = new dsFatura();

                    for (int i = 0; i < dgSatisDetayi.Rows.Count; i++)
                    {
                        ftrTable.Tables["tblFatura"].Rows.Add();
                        ftrTable.Tables["tblFatura"].Rows[i][0] = dgSatisDetayi.Rows[i].Cells[2].Value.ToString() + " (" + dgSatisDetayi.Rows[i].Cells[4].Value.ToString() + " " + dgSatisDetayi.Rows[i].Cells[5].Value.ToString() + " X " + dgSatisDetayi.Rows[i].Cells[3].Value.ToString() + " TL)";
                        ftrTable.Tables["tblFatura"].Rows[i][1] = Convert.ToDecimal(dgSatisDetayi.Rows[i].Cells[7].Value);
                    }

                    rapor.SetDataSource(ftrTable);
                    rapor.ParameterFields["TopTutar"].CurrentValues.AddValue(seciliTutar);
                    rapor.ParameterFields["ReportName"].CurrentValues.AddValue(Program.isletmeAdi.ToUpper() + "\n" + Program.adres);
                    rapor.ParameterFields["Tarih"].CurrentValues.AddValue("TARİH : " + seciliTarih.ToShortDateString());
                    rapor.ParameterFields["Saat"].CurrentValues.AddValue("SAAT  : " + seciliTarih.ToShortTimeString());
                    rapor.ParameterFields["SatisNo"].CurrentValues.AddValue(seciliSatis.ToString("D10"));
                    rapor.ParameterFields["KasiyerAdi"].CurrentValues.AddValue(seciliKasiyerAdi.ToUpper());

                    if (seciliNakit == 0)
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("KREDİ KARTI");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                        rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                        rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                    }
                    else
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("NAKİT");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue(" ₺ " + seciliNakit.ToString("F2"));
                        decimal seciliParaUstu = seciliNakit + seciliKredi - seciliTutar;
                        if (seciliParaUstu > 0)
                        {
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliParaUstu.ToString("F2"));
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("PARA ÜSTÜ");
                        }
                        else if (seciliKredi <= 0)
                        {
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                        }
                        if (seciliKredi > 0)
                        {
                            if (seciliParaUstu <= 0)
                            {
                                rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                                rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                            }
                            else
                            {
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                            }

                        }
                        else
                        {
                            rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                            rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        }
                    }


                    rapor.PrintOptions.PrinterName = Program.yaziciAdi;
                    rapor.PrintOptions.PaperSize = Program.kagiTuru;

                    rapor.PrintToPrinter(1, false, 0, 0);

                    rapor.Dispose();
                    ftrTable.Dispose();
                }

                else
                {


                    CrystalReport1 rapor = new CrystalReport1();
                    MessageBox.Show(Application.StartupPath);
                    dsFatura ftrTable = new dsFatura();

                    for (int i = 0; i < dgSatisDetayi.Rows.Count; i++)
                    {
                        ftrTable.Tables["tblFatura"].Rows.Add();
                        ftrTable.Tables["tblFatura"].Rows[i][0] = dgSatisDetayi.Rows[i].Cells[2].Value.ToString() + " (" + dgSatisDetayi.Rows[i].Cells[4].Value.ToString() + " " + dgSatisDetayi.Rows[i].Cells[5].Value.ToString() + " X " + dgSatisDetayi.Rows[i].Cells[3].Value.ToString() + " TL)";
                        ftrTable.Tables["tblFatura"].Rows[i][1] = Convert.ToDecimal(dgSatisDetayi.Rows[i].Cells[7].Value);
                    }

                    rapor.SetDataSource(ftrTable);
                    rapor.ParameterFields["TopTutar"].CurrentValues.AddValue(seciliTutar);
                    rapor.ParameterFields["ReportName"].CurrentValues.AddValue(Program.isletmeAdi.ToUpper() + "\n" + Program.adres);
                    rapor.ParameterFields["Tarih"].CurrentValues.AddValue("TARİH : " + seciliTarih.ToShortDateString());
                    rapor.ParameterFields["Saat"].CurrentValues.AddValue("SAAT  : " + seciliTarih.ToShortTimeString());
                    rapor.ParameterFields["SatisNo"].CurrentValues.AddValue(seciliSatis.ToString("D10"));
                    rapor.ParameterFields["KasiyerAdi"].CurrentValues.AddValue(seciliKasiyerAdi.ToUpper());

                    if (seciliNakit == 0)
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("KREDİ KARTI");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                        rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                        rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                    }
                    else
                    {
                        rapor.ParameterFields["Nakit"].CurrentValues.AddValue("NAKİT");
                        rapor.ParameterFields["NakitDegeri"].CurrentValues.AddValue(" ₺ " + seciliNakit.ToString("F2"));
                        decimal seciliParaUstu = seciliNakit + seciliKredi - seciliTutar;
                        if (seciliParaUstu > 0)
                        {
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliParaUstu.ToString("F2"));
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("PARA ÜSTÜ");
                        }
                        else if (seciliKredi <= 0)
                        {
                            rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("");
                            rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("");
                        }
                        if (seciliKredi > 0)
                        {
                            if (seciliParaUstu <= 0)
                            {
                                rapor.ParameterFields["ParaUstuDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                                rapor.ParameterFields["ParaUstu"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                            }
                            else
                            {
                                rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("KREDİ KARTI");
                                rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("₺ " + seciliKredi.ToString("F2"));
                            }

                        }
                        else
                        {
                            rapor.ParameterFields["KrediKarti"].CurrentValues.AddValue("");
                            rapor.ParameterFields["KrediDegeri"].CurrentValues.AddValue("");
                        }
                    }


                    rapor.PrintOptions.PrinterName = Program.yaziciAdi;
                    rapor.PrintOptions.PaperSize = Program.kagiTuru;

                    // rapor.PrintToPrinter(1, false, 0, 0);

                    rapor.Dispose();
                    ftrTable.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata Mesajı : " + ex.Message, "Fatura Yazdırılamıyor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void ExceleAktar(string baslik)
        {
            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Satis_Listesi("+DateTime.Now.ToShortDateString()+")";
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
                        MessageBox.Show("Değiştirmek istediğiniz dosya şu anda başka bir uygulama tarafından kullanılıyor. Lütfen başka bir uygulama tarafından kullanılmadığından emin olun. Ya da farklı bir isimde kaydetmeyi deneyin.","Dosya Meşgul",MessageBoxButtons.OK,MessageBoxIcon.Stop);
                        return;
                    }
                }
                    _package = new ExcelPackage(new MemoryStream());
                ExcelWorksheet ws1 = _package.Workbook.Worksheets.Add("Satış Listesi");
                ws1.Cells[1, 1].Value = Program.isletmeAdi + " " + baslik + " Satış Listesi";
                ws1.Cells["A1:F1"].Merge = true;
                ws1.Cells["A1:F1"].Style.Font.Bold = true;
                ws1.Cells["A3:F3"].Style.Font.Bold = true;
                for (int i = 0; i < dgSatislar.ColumnCount - 4; i++)
                {
                    ws1.Cells[3, i + 1].Value = dgSatislar.Columns[i].HeaderText;
                }
                int sonSatir = 0;
                for (var kolon = 0; kolon < dgSatislar.ColumnCount - 4; kolon++)
                {
                    int yukari=0;
                    for (var satir = 0; satir < dgSatislar.RowCount; satir++)
                    {
                        if (dgSatislar.Rows[satir].Visible)
                        {
                            if (kolon == 0)
                            {
                                ws1.Cells[satir - yukari + 4, kolon + 1].Value = Convert.ToInt32(dgSatislar.Rows[satir-yukari].Cells[kolon].Value);
                                if ((satir - yukari) % 2 == 0)
                                {
                                    ws1.Cells["A" + (satir - yukari + 4) + ":F" + (satir - yukari + 4)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws1.Cells["A" + (satir - yukari + 4) + ":F" + (satir - yukari + 4)].Style.Fill.BackgroundColor.SetColor(acikGri);
                                }
                            }
                            else if (kolon > 3) ws1.Cells[satir - yukari + 4, kolon + 1].Value = Convert.ToDecimal(dgSatislar.Rows[satir - yukari].Cells[kolon].Value);
                            else
                                ws1.Cells[satir - yukari + 4, kolon + 1].Value = dgSatislar.Rows[satir - yukari].Cells[kolon].Value;
                            sonSatir = satir - yukari + 4;
                        }
                        else yukari++;
                    }
                    
                    if (kolon == 4)
                    {
                        ws1.Cells["E3:E" + sonSatir].Style.Numberformat.Format = "₺#,0.00";
                        ws1.Cells["E" + (sonSatir + 2)].Style.Numberformat.Format = "₺#,0.00";
                        ws1.Cells["E" + (sonSatir + 2)].Formula = "SUM(E3:E" + sonSatir+")";
                        _package.Workbook.Calculate();
                    }
                    if (kolon == 5)
                    {
                        ws1.Cells["F3:F" + sonSatir].Style.Numberformat.Format = "₺#,0.00";
                        ws1.Cells["F" + (sonSatir + 2)].Style.Numberformat.Format = "₺#,0.00";
                        ws1.Cells["F" + (sonSatir + 2)].Formula = "SUM(F3:F" + sonSatir + ")";
                        _package.Workbook.Calculate();
                    }
                    ws1.Column(kolon + 1).AutoFit();
                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;
                    ws1.Column(kolon + 1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                ws1.Cells["D" + (sonSatir + 2)].Value = "Toplam ";
                ws1.Cells["D" + (sonSatir + 3)].Value = "Tarih   ";
                ws1.Cells["D" + (sonSatir + 4)].Value = "Net Kazanç   ";
                ws1.Cells["E"+ (sonSatir + 3) + ":F"+ (sonSatir + 3) + ""].Merge = true;
                ws1.Cells["E" + (sonSatir + 3)].Value = DateTime.Now.ToString();
                ws1.Cells["E" + (sonSatir + 4)].Value = netKazanc;
                ws1.Cells["D" + (sonSatir + 2) + ":D" + (sonSatir + 4)].Style.Font.Bold = true;
                ws1.Cells["E" + (sonSatir + 4)].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells["A1:F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells["A3:F3"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells["D" + (sonSatir+1) + ":F" + (sonSatir+1)].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;

                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                DialogResult ac = MessageBox.Show("Satışlar başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) System.Diagnostics.Process.Start(saveExceleKaydet.FileName);

                    _package.Dispose();
                ws1.Dispose();
            }
        }
        Color acikGri = ColorTranslator.FromHtml("#f2f2f2");

        private void btnYazdir_Click(object sender, EventArgs e)
        {
            DetayiYazdir();
        }

        void DetayiExceleAktar()
        {
            saveExceleKaydet.Filter = "Excel Dosyaları (*.xlsx)|*.xlsx";
            saveExceleKaydet.FileName = "Satis_Detayi(Satis_No=" + seciliSatis.ToString() + ")";
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
                _package = new ExcelPackage(new MemoryStream());
                ExcelWorksheet ws1 = _package.Workbook.Worksheets.Add(seciliSatis.ToString() + " Numaralı Satış");
                ws1.Cells[1, 1].Value = Program.isletmeAdi + " Satış Detayı";
                ws1.Cells["A1:G1"].Merge = true;
                ws1.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                ws1.Cells["A1:G1"].Style.Font.Bold = true;
                ws1.Cells["A2:G2"].Style.Font.Bold = true;
                ws1.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                ws1.Cells["A2:G2"].Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                for (int i = 0; i < dgSatisDetayi.ColumnCount; i++)
                    if (i < 6)
                    ws1.Cells[2, i + 1].Value = dgSatisDetayi.Columns[i].HeaderText;
                else if (i > 6)
                        ws1.Cells[2, i].Value = dgSatisDetayi.Columns[i].HeaderText;
                int sonSatir = 0;

                for (var kolon = 0; kolon < dgSatisDetayi.ColumnCount; kolon++)
                {

                    for (var satir = 0; satir < dgSatisDetayi.RowCount; satir++)
                    {

                            if (kolon == 0)
                            {
                                ws1.Cells[satir + 3, kolon + 1].Value = Convert.ToInt32(dgSatisDetayi.Rows[satir].Cells[kolon].Value);
                                if (satir % 2 == 0)
                                {
                                    ws1.Cells["A" + (satir + 3) + ":G" + (satir + 3)].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    ws1.Cells["A" + (satir + 3) + ":G" + (satir + 3)].Style.Fill.BackgroundColor.SetColor(acikGri);
                                }
                            }
                            else if (kolon == 3 || kolon == 4) ws1.Cells[satir + 3, kolon + 1].Value = Convert.ToDecimal(dgSatisDetayi.Rows[satir].Cells[kolon].Value);
                            else if (kolon<6)
                                ws1.Cells[satir + 3, kolon + 1].Value = dgSatisDetayi.Rows[satir].Cells[kolon].Value;
                            else if(kolon >6)
                                ws1.Cells[satir + 3, kolon].Value = Convert.ToDecimal(dgSatisDetayi.Rows[satir].Cells[kolon].Value);
                            sonSatir = satir + 3;
                    }
                    ws1.Column(kolon + 1).Style.Font.VerticalAlign = ExcelVerticalAlignmentFont.Superscript;                    
                }
                string aciklamaSutunu = "E", icerikSutunu="F";

                ws1.Cells[aciklamaSutunu.ToString() + (sonSatir + 2)].Value = "Toplam";
                ws1.Cells[icerikSutunu + (sonSatir + 2) + ":G" + (sonSatir + 2) ].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 2)].Value = seciliTutar;
                ws1.Cells[icerikSutunu + (sonSatir + 2)].Style.Numberformat.Format = "₺#,0.00";

                ws1.Cells[aciklamaSutunu + (sonSatir + 3)].Value = "Tarih";
                ws1.Cells[icerikSutunu + (sonSatir + 3) + ":G" + (sonSatir + 3) ].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 3)].Value = seciliTarih.ToShortDateString();

                ws1.Cells[aciklamaSutunu + (sonSatir + 4)].Value = "Saat";
                ws1.Cells[icerikSutunu + (sonSatir + 4) + ":G" + (sonSatir + 4)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 4)].Value = seciliTarih.ToLongTimeString();

                ws1.Cells[aciklamaSutunu + (sonSatir + 5)].Value = "Satış No";
                ws1.Cells[icerikSutunu + (sonSatir + 5) + ":G" + (sonSatir + 5) ].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 5)].Value = seciliSatis;

                ws1.Cells[aciklamaSutunu + (sonSatir + 6)].Value = "Müşteri";
                ws1.Cells[icerikSutunu + (sonSatir + 6) + ":G" + (sonSatir + 6)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 6)].Value = seciliMusteriAdi;

                ws1.Cells[aciklamaSutunu + (sonSatir + 7)].Value = "Kasiyer";
                ws1.Cells[icerikSutunu + (sonSatir + 7) + ":G" + (sonSatir + 7)].Merge = true;
                ws1.Cells[icerikSutunu + (sonSatir + 7)].Value = seciliKasiyerAdi;

                ws1.Cells["D3:D" + (sonSatir)].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["G3:G" + (sonSatir)].Style.Numberformat.Format = "₺#,0.00";
                ws1.Cells["E3:E" + sonSatir].Style.Numberformat.Format = "0.0";

                ws1.Cells[aciklamaSutunu + (sonSatir + 2) + ":G" + (sonSatir + 2)].Style.Border.Top.Style = ExcelBorderStyle.Medium;
                ws1.Cells[aciklamaSutunu + (sonSatir + 2) + ":"+aciklamaSutunu + (sonSatir + 7)].Style.Font.Bold = true;

                ws1.Cells["$A$2:$G$"+(sonSatir+7)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws1.Cells.Style.Font.Name = "Courier New";
                for (int i = 1; i <= 7; i++)
                {
                    ws1.Column(i).AutoFit();
                }
                _package.SaveAs(new FileInfo(saveExceleKaydet.FileName));
                DialogResult ac = MessageBox.Show("Satış detayı başarılı bir şekilde kaydedildi. Kaydettiğiniz dosya açılsın mı?", "Açılış Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ac == DialogResult.Yes) System.Diagnostics.Process.Start(saveExceleKaydet.FileName);
                _package.Dispose();

            }
        }
        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            if (cbSecenek.SelectedIndex == 4) SatislariGoster();
        }

        private void dgSatislar_KeyDown(object sender, KeyEventArgs e) 
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }

        private void txtAra_KeyDown(object sender, KeyEventArgs e)
        {
            if (cbSecenek.SelectedIndex == 3 && e.KeyCode == Keys.Enter)
            { SatislariGoster(); txtAra.Text = ""; }
        }

        private void btnCokSatanlar_Click(object sender, EventArgs e)
        {
            frmUrunSatisVerileri UrunSatisVeri = new frmUrunSatisVerileri();
            UrunSatisVeri.ShowDialog();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
                if(dgSatislar.RowCount>0)
            { 
            if (cbSecenek.SelectedIndex == 1)
                ExceleAktar(dtBaslangicTarihi.Value.ToShortDateString() + " Tarihli");
            else if (cbSecenek.SelectedIndex == 2)
                ExceleAktar(dtBaslangicTarihi.Value.ToShortDateString() + " - " + dtBitisTarihi.Value.ToShortDateString() + " Tarihli");

            else if (cbSecenek.SelectedIndex == 3)
                ExceleAktar(dgSatislar.Rows[0].Cells[0].Value + " Numaralı");
            else if(cbSecenek.SelectedIndex==4 && txtAra.Text!="")
                 ExceleAktar("Müşteri Adı Filtreli");
            else ExceleAktar("Tüm");
            }

        }
    }
}

using System;
using System.Windows.Forms;

namespace Otomasyon
{
    public class OrtakIslemler
    {
        NotifyIcon nfBasarili;
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnaForm));
        public OrtakIslemler()
        {
            nfBasarili = new NotifyIcon();
            this.nfBasarili.BalloonTipText = "İşlem başarıyla gerçekleşti.";
            this.nfBasarili.BalloonTipTitle = "Başarılı İşlem";
            this.nfBasarili.Text = "Başarılı";
            this.nfBasarili.BalloonTipClicked += new System.EventHandler(nfBasariliKapat);
            this.nfBasarili.BalloonTipClosed += new System.EventHandler(this.nfBasariliKapat);
            this.nfBasarili.Click += new System.EventHandler(this.nfBasariliKapat);
            this.nfBasarili.DoubleClick += new System.EventHandler(this.nfBasariliKapat);
        }
        ~OrtakIslemler() { nfBasarili.Dispose(); }
        public void BilgiVer(string yazi,int sure,string baslik,string ikon)
        {
            this.nfBasarili.BalloonTipClicked -= LisansEtkinlestirme;
            this.nfBasarili.BalloonTipClicked += new System.EventHandler(nfBasariliKapat);
            this.nfBasarili.Icon = ((System.Drawing.Icon)(resources.GetObject(ikon)));
            nfBasarili.BalloonTipText = yazi;
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(sure);
        }
        public void LisansUyarisi(string mesaj)
        {
            if (mesaj == "")
                mesaj = "Lisanssız yazılım kullanıyorsunuz. Ürünü etkinleştirmediğiniz sürece kısıtlı kullanmak durumundasınız. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.";
            this.nfBasarili.BalloonTipClicked -= nfBasariliKapat;
            this.nfBasarili.BalloonTipClicked += new System.EventHandler(LisansEtkinlestirme);
            this.nfBasarili.Icon = ((System.Drawing.Icon)(resources.GetObject("nfHata.Icon")));
            nfBasarili.BalloonTipText = mesaj;
            this.nfBasarili.BalloonTipTitle = "Lisansınız Yok!";
            this.nfBasarili.Text = "Teşekkür ederiz...";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(3000);
        }
        EventArgs evA=EventArgs.Empty;
        public void LisansUyarisiMesaj (string mesaj)
        {
            DialogResult sor = MessageBox.Show(mesaj,"Lisans Uyarısı",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
            if(sor==DialogResult.OK)
            {
                LisansEtkinlestirme(0, evA);
            }
        }
        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }
        frmConfiguration lisansYolu;
        private void LisansEtkinlestirme(object sender, EventArgs e)
        {
            lisansYolu = new frmConfiguration();
            lisansYolu.Show();
        }
    }
}

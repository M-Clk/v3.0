using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Otomasyon
{
    public class OrtakIslemler
    {
        private readonly EventArgs evA = EventArgs.Empty;
        private frmConfiguration lisansYolu;
        private readonly NotifyIcon nfBasarili;
        private readonly ComponentResourceManager resources = new ComponentResourceManager(typeof(frmAnaForm));

        public OrtakIslemler()
        {
            nfBasarili = new NotifyIcon();
            nfBasarili.BalloonTipText = "İşlem başarıyla gerçekleşti.";
            nfBasarili.BalloonTipTitle = "Başarılı İşlem";
            nfBasarili.Text = "Başarılı";
            nfBasarili.BalloonTipClicked += nfBasariliKapat;
            nfBasarili.BalloonTipClosed += nfBasariliKapat;
            nfBasarili.Click += nfBasariliKapat;
            nfBasarili.DoubleClick += nfBasariliKapat;
        }

        ~OrtakIslemler()
        {
            nfBasarili.Dispose();
        }

        public void BilgiVer(string yazi, int sure, string baslik, string ikon)
        {
            nfBasarili.BalloonTipClicked -= LisansEtkinlestirme;
            nfBasarili.BalloonTipClicked += nfBasariliKapat;
            nfBasarili.Icon = (Icon)resources.GetObject(ikon);
            nfBasarili.BalloonTipText = yazi;
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(sure);
        }

        public void LisansUyarisi(string mesaj)
        {
            if (mesaj == "")
                mesaj =
                    "Lisanssız yazılım kullanıyorsunuz. Ürünü etkinleştirmediğiniz sürece kısıtlı kullanmak durumundasınız. Eğer ürün anahtarınız varsa etkinleştirmek için tıklayın.";
            nfBasarili.BalloonTipClicked -= nfBasariliKapat;
            nfBasarili.BalloonTipClicked += LisansEtkinlestirme;
            nfBasarili.Icon = (Icon)resources.GetObject("nfHata.Icon");
            nfBasarili.BalloonTipText = mesaj;
            nfBasarili.BalloonTipTitle = "Lisansınız Yok!";
            nfBasarili.Text = "Teşekkür ederiz...";
            nfBasarili.Visible = true;
            nfBasarili.ShowBalloonTip(3000);
        }

        public void LisansUyarisiMesaj(string mesaj)
        {
            var sor = MessageBox.Show(mesaj, "Lisans Uyarısı", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (sor == DialogResult.OK) LisansEtkinlestirme(0, evA);
        }

        private void nfBasariliKapat(object sender, EventArgs e)
        {
            nfBasarili.Visible = false;
        }

        private void LisansEtkinlestirme(object sender, EventArgs e)
        {
            lisansYolu = new frmConfiguration();
            lisansYolu.Show();
        }
    }
}
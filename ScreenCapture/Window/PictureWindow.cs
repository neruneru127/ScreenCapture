using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture.Window
{
    public partial class PictureWindow : Form
    {
        // キャプチャしたデータ
        private Bitmap basePicture;
        // 最前面に固定するフラグ
        private bool mostTopFlg = false;

        /// <summary>
        /// キャプチャした画像を表示するウィンドウ.
        /// 
        /// </summary>
        /// <param name="picture">表示する画像</param>
        /// <param name="showPoint">表示する位置</param>
        public PictureWindow(Bitmap picture, Point showPoint)
        {
            InitializeComponent();

            // ウィンドウサイズをキャプチャサイズに合わせる(コンポーネントの関係で微調整)
            this.Size = new Size(picture.Size.Width + 16, picture.Size.Height + 66);
            pictureBox.Image = picture;
            basePicture = new Bitmap(picture);

            // ウィンドウの設定
            this.StartPosition = FormStartPosition.Manual;
            this.Location = showPoint;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Save_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 保存ダイアログを開く
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = "無題.jpg";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            dialog.Filter = "JPGファイル(*.jpg)|*.jpg|" +
                "PNGファイル(*.png)|*.png|" +
                "すべてのファイル(*.*)|*.*";
            dialog.FilterIndex = 0;
            dialog.Title = "保存先を選択してください";
            dialog.RestoreDirectory = true;


            // ダイアログを表示し、OKボタン押下で保存を実行
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                basePicture.Save(dialog.FileName);
            }
        }

        private void Clipbord_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // コピーのリアクションとして、フェードを行う
            var fadeTime = 50;

            var fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 1;

            var fadeCnt = 0;

            var alphaPercent = 0.0;
            var percentAmount = (double)255 / fadeTime;

            fadeTimer.Tick += (sender, e) =>
            {
                fadeCnt++;

                if (fadeCnt > fadeTime)
                {
                    // フェードの終了
                    fadeTimer.Stop();
                }

                if (this.IsDisposed)
                {
                    // Timerが終わる前に閉じられた場合はTimerを止める
                    fadeTimer.Stop();
                    return;
                }


                if (fadeCnt > fadeTime / 2)
                {
                    alphaPercent -= percentAmount;

                    // 小数点のズレで0未満にならないように調整
                    alphaPercent = alphaPercent < 0 ? 0 : alphaPercent;
                }
                else
                {
                    alphaPercent += percentAmount;
                }

                var picture = new Bitmap(basePicture);

                var graphics = Graphics.FromImage(picture);
                graphics.DrawImage(picture, 0, 0, picture.Width, picture.Height);

                var brush = new SolidBrush(Color.FromArgb((int)alphaPercent, 150, 150, 150));
                graphics.FillRectangle(brush, new Rectangle(0, 0, picture.Width, picture.Height));

                pictureBox.Image = picture;
                pictureBox.Refresh();

            };

            fadeTimer.Start();

            // クリップボードにイメージを設定
            Clipboard.SetImage(pictureBox.Image);
        }

        private void MostTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // フラグの反転
            mostTopFlg = !mostTopFlg;

            // フラグに応じて、最前面に固定するか設定する
            if (mostTopFlg)
            {
                this.MostTopToolStripMenuItem.Image = Properties.Resources.PinIcon;
                this.TopMost = true;
            }
            else
            {
                this.MostTopToolStripMenuItem.Image = Properties.Resources.NotPinIcon;
                this.TopMost = false;
            }
        }
    }
}

using ScreenCapture.Model;
using ScreenCapture.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScreenCapture
{
    /// <summary>
    /// キャプチャの設定や開始を行うホームウィンドウ.
    /// 
    /// </summary>
    public partial class HomeWindow : Form
    {
        // 表示中のキャプチャ後ウィンドウ一覧
        List<PictureWindow> pictureWindows = new List<PictureWindow>();
        // キャプチャ時にウィンドウを非表示にするフラグ
        bool hideWindowFlg = true;
        // キャプチャの際に遅延して実行するフラグ
        bool delayFlg = false;


        public HomeWindow()
        {
            InitializeComponent();

            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }


        private void windowCaptureButton_Click(object sender, EventArgs e)
        {
            CaptureExecute(DummyScreen.CaptureType.WINDOW);
        }

        private void squareCaptureButton_Click(object sender, EventArgs e)
        {
            CaptureExecute(DummyScreen.CaptureType.SQUARE);
        }

        private void fullScreenCaptureButton_Click(object sender, EventArgs e)
        {
            CaptureExecute(DummyScreen.CaptureType.FULL_SCREEN);
        }


        private void hideWindowButton_Click(object sender, EventArgs e)
        {
            hideWindowFlg = !hideWindowFlg;
            
            if (hideWindowFlg)
            {
                this.hideWindowButton.Image = Properties.Resources.HideWindowIcon;
            }
            else
            {
                this.hideWindowButton.Image = Properties.Resources.ShowWindowIcon;
            }
            
        }

        private void delayButton_Click(object sender, EventArgs e)
        {
            delayFlg = !delayFlg;

            if (delayFlg)
            {
                this.delayButton.Image = Properties.Resources.DelayIcon;
            }
            else
            {
                this.delayButton.Image = Properties.Resources.NotDelayIcon;
            }
        }


        private void CaptureExecute(DummyScreen.CaptureType captureType)
        {
            // ウィンドウ一覧で既に閉じたウィンドウを除外
            pictureWindows = pictureWindows.Where(window => !window.IsDisposed).ToList();

            // 待機時間用のタイマーをセット(待ち時間なしでも設定を行う)
            var waitTimer = new System.Windows.Forms.Timer();

            if (delayFlg)
            {
                waitTimer.Interval = 3000;
            }
            else
            {
                waitTimer.Interval = 1;
            }

            waitTimer.Tick += new EventHandler((sender, e) =>
            {
                // 一度のみ実行するようにStopさせる
                waitTimer.Stop();

                SetWindowVisible(false);

                if (Screen.AllScreens.Length == 1 && captureType == DummyScreen.CaptureType.FULL_SCREEN)
                {
                    // 1画面かつフルスクリーンの場合は対象の画面をそのままキャプチャ
                    var picture = CaptureController.Execute(Screen.PrimaryScreen);
                    var pictureWindow = new PictureWindow(picture, DummyScreen.ClickPoint);

                    SetWindowVisible(true);

                    pictureWindows.Add(pictureWindow);
                    pictureWindow.Show();

                    return;
                }

                var dummyScreenList = new List<DummyScreen>();

                // 全てのスクリーンに対してダミー画面を作成
                foreach (var screen in Screen.AllScreens)
                {
                    var picture = CaptureController.Execute(screen);

                    var dummyScreen = new DummyScreen(picture, screen, captureType);

                    dummyScreenList.Add(dummyScreen);

                    // 矩形選択後の処理を実装
                    dummyScreen.FormClosed += new FormClosedEventHandler((sender, e) =>
                    {
                        // 1つでもダミー画面を閉じた後、全てのダミー画面を閉じる
                        dummyScreenList.ForEach(screen => screen.Dispose());

                        SetWindowVisible(true);

                        if (DummyScreen.CutPicture != null)
                        {
                            // キャプチャした画像をウィンドウに設定
                            var pictureWindow = new PictureWindow(DummyScreen.CutPicture, DummyScreen.ClickPoint);
                            pictureWindows.Add(pictureWindow);

                            pictureWindow.Show();
                        }

                    });

                    dummyScreen.Show();
                }
            });

            waitTimer.Start();

        }

        private void SetWindowVisible(bool isShow)
        {
            // 非表示にしない場合は処理を行わない
            if (!hideWindowFlg)
            {
                return;
            }

            if (isShow)
            {
                // ウィンドウを表示
                this.Opacity = 1;
                this.Show();

                pictureWindows.ForEach(window => window.Opacity = 1);
                pictureWindows.ForEach(window => window.Show());
            }
            else
            {
                // ウィンドウを隠す
                this.Opacity = 0;
                this.Hide();

                pictureWindows.ForEach(window => window.Opacity = 0);
                pictureWindows.ForEach(window => window.Hide());
            }
        }

    }
}
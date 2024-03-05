using ScreenCapture.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScreenCapture.Window
{
    /// <summary>
    /// 画面キャプチャする際にキャプチャ範囲を可視化するウィンドウ.
    /// 
    /// </summary>
    public partial class DummyScreen : Form
    {
        // ドラッグ中かどうか
        private bool isDrag = false;
        // クリックの基点
        private Point clickOrigin;
        // キャプチャ範囲の座標
        private Point destinationPoint;
        // ダミーのベースになる画像
        private Bitmap basePicture;
        // 表示対象のスクリーン
        private Screen targetScreen;
        // キャプチャした画像を表示する座標
        private static Point clickPoint;
        // キャプチャした画像を設定
        private static Bitmap cutPicture;
        // スクリーンの表示倍率を設定
        private float scale;
        // キャプチャ方法
        private CaptureType captureType;
        // ウィンドウハンドルを設定
        private static List<int> handleList = new List<int>();

        /// <summary>
        /// キャプチャ方法.
        /// 
        /// </summary>
        public enum CaptureType
        {
            FULL_SCREEN,
            SQUARE,
            WINDOW
        };

        /// <summary>
        /// キャプチャ前に選択範囲を選択する画面.
        /// 
        /// </summary>
        /// <param name="picture">画面全体の画像</param>
        /// <param name="targetScreen">対象にする画面</param>
        /// <param name="captureType">キャプチャ方式</param>
        public DummyScreen(Bitmap picture, Screen targetScreen, CaptureType captureType)
        {
            // キャプチャ開始時にのみ起動(全ダミーウィンドウ間でハンドルを共有)
            if (handleList.Count == 0)
            {
                // デスクトップを除くウィンドウハンドルを取得
                handleList = Process.GetProcesses().Where(proc => proc.MainWindowHandle != IntPtr.Zero).Select(proc => proc.Id).ToList();
            }

            clickPoint = new Point(0, 0);

            basePicture = new Bitmap(picture);
            this.targetScreen = targetScreen;
            this.captureType = captureType;

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = targetScreen.Bounds.Location;
            this.Opacity = 0;
            this.Size = new Size(targetScreen.Bounds.Width, targetScreen.Bounds.Height);

            scale = (float)picture.Width / targetScreen.Bounds.Width;

            cutPicture = null;

            if (captureType == CaptureType.SQUARE)
            {
                Cursor = Cursors.Cross;
            }

            // 画面全体にシェードをかける
            ScreenShade();
        }

        private void DummyScreen_Load(object sender, EventArgs e)
        {
            // ウィンドウをフェードインで表示する
            var fadeTime = 25;

            var fadeTimer = new System.Windows.Forms.Timer();
            fadeTimer.Interval = 1;

            var fadeCnt = 0;
            fadeTimer.Tick += (sender, e) =>
            {
                fadeCnt++;

                if (fadeCnt > fadeTime)
                {
                    // フェード終了
                    fadeTimer.Stop();
                }

                if (this.IsDisposed)
                {
                    // Timerが終わる前に閉じられた場合はTimerを止める
                    fadeTimer.Stop();
                    return;
                }

                this.Opacity = (double)1 / fadeTime * fadeCnt;

            };

            fadeTimer.Start();
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                // クリック以外は終了
                this.Close();
                return;
            }

            switch(captureType)
            {
                case CaptureType.SQUARE:
                    // ドラッグの基点を設定
                    isDrag = true;
                    clickOrigin = e.Location;
                    break;

                case CaptureType.WINDOW:
                case CaptureType.FULL_SCREEN:
                default:
                    break;
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            switch (captureType)
            {
                case CaptureType.FULL_SCREEN:
                    cutPicture = basePicture;
                    clickPoint = targetScreen.Bounds.Location;
                    break;

                case CaptureType.SQUARE:
                    isDrag = false;

                    // 左上の座標と右下の座標を設定
                    var originX = (int)((clickOrigin.X < e.X ? clickOrigin.X : e.X) * scale);
                    var originY = (int)((clickOrigin.Y < e.Y ? clickOrigin.Y : e.Y) * scale);
                    var destinationX = (int)((clickOrigin.X > e.X ? clickOrigin.X : e.X) * scale);
                    var destinationY = (int)((clickOrigin.Y > e.Y ? clickOrigin.Y : e.Y) * scale);

                    if (originX == destinationX || originY == destinationY)
                    {
                        // 矩形になっていない場合終了
                        this.Close();
                        return;
                    }

                    // 画像を取得
                    // 対象スクリーンの座標を加算して位置を調整
                    cutPicture = CaptureController.Execute(new Size(destinationX - originX, destinationY - originY),
                        new Point(originX + targetScreen.Bounds.Location.X, originY + targetScreen.Bounds.Location.Y));

                    // スクリーン座標へ直す
                    clickPoint = this.PointToScreen(new Point(originX, originY));
                    break;

                case CaptureType.WINDOW:

                    if (destinationPoint.X == clickOrigin.X || destinationPoint.Y == clickOrigin.Y)
                    {
                        // ウィンドウを選択していない場合終了
                        this.Close();
                        return;
                    }

                    // ウィンドウの画像を取得
                    cutPicture = CaptureController.Execute(
                        new Size(destinationPoint.X - clickOrigin.X, destinationPoint.Y - clickOrigin.Y),
                        new Point(clickOrigin.X, clickOrigin.Y));

                    clickPoint = clickOrigin;

                    break;

                default:
                    break;
            }

            this.Close();
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Opacity != 1)
            {
                // フェード表示中だと重たいためスキップ
                return;
            }

            switch (captureType)
            {
                case CaptureType.FULL_SCREEN:
                    {
                        // 対象スクリーンをハイライト
                        ScreenShade(10);
                    }
                    break;

                case CaptureType.SQUARE:
                    {
                        if (isDrag)
                        {
                            // ドラッグ中の範囲をハイライト
                            var originX = clickOrigin.X < e.X ? clickOrigin.X : e.X;
                            var originY = clickOrigin.Y < e.Y ? clickOrigin.Y : e.Y;
                            var destinationX = clickOrigin.X > e.X ? clickOrigin.X : e.X;
                            var destinationY = clickOrigin.Y > e.Y ? clickOrigin.Y : e.Y;

                            SquareShade(new Point(originX, originY), new Point(destinationX, destinationY));
                        }
                    }

                    break;

                case CaptureType.WINDOW:
                    {
                        // マウスオーバーしているウィンドウをハイライト
                        IntPtr oldHandle = IntPtr.Zero;
                        var windowHandle = IntPtr.Zero;
                        clickOrigin = new Point(0, 0);
                        destinationPoint = new Point(0, 0);

                        do
                        {
                            windowHandle = Const.FindWindowEx(IntPtr.Zero, oldHandle, null, null);
                            oldHandle = windowHandle;

                            if (windowHandle == this.Handle)
                            {
                                // 自身のハンドルは無視する
                                continue;
                            }

                            int processId = 0;
                            Const.GetWindowThreadProcessId(windowHandle, out processId);

                            if (!handleList.Contains(processId))
                            {
                                // 起動時に存在しなかった場合は無視
                                continue;
                            }

                            if (!Const.GetVirtualDesktopManager().IsWindowOnCurrentVirtualDesktop(windowHandle))
                            {
                                // 現在の仮想デスクトップにウィンドウが無い場合は無視
                                continue;
                            }

                            if (!Const.IsWindowVisible(windowHandle))
                            {
                                // 不可視ウィンドウは無視
                                continue;
                            }


                            var retVal = (int)Const.GetWindowLongPtr(windowHandle, Const.GWL_EXSTYLE);
                            if ((retVal & Const.WS_EX_NOACTIVE) != 0)
                            {
                                // 反応しないウィンドウは無視
                                continue;
                            }

                            if ((retVal & Const.WS_EX_NOREDIRECTIONBITMAP) != 0 && (retVal & Const.WS_EX_NOACTIVE) != 0)
                            {
                                // レンダリングされていないウィンドウは無視(不透明度が0のウィンドウ)
                                continue;
                            }


                            // ウィンドウサイズ取得
                            Const.Rect bounds, rect;
                            Const.DwmGetWindowAttribute(windowHandle, Const.DWMWA_EXTENDED_FRAME_BOUNDS,
                                out bounds, Marshal.SizeOf(typeof(Const.Rect)));
                            if (Const.GetWindowRect(windowHandle, out rect))
                            {
                                var screenCursorPos = this.PointToScreen(e.Location);

                                var windowOriginPos = new Point((int)(bounds.left), (int)(bounds.top));
                                var windowDestinationPos = new Point((int)(bounds.right), (int)(bounds.bottom));
                                
                                if (windowOriginPos.X < screenCursorPos.X && windowDestinationPos.X > screenCursorPos.X
                                    && windowOriginPos.Y < screenCursorPos.Y && windowDestinationPos.Y > screenCursorPos.Y)
                                {
                                    // マウスがウィンドウ範囲内にあると判定
                                    clickOrigin = windowOriginPos;
                                    destinationPoint = windowDestinationPos;

                                    //// ウィンドウのタイトルを取得する(デバッグ用)
                                    //StringBuilder tsb = new StringBuilder(1000);
                                    //Const.GetWindowText(windowHandle, tsb, tsb.Capacity);

                                    //Console.WriteLine(tsb.ToString() + ":" + windowHandle.ToString("x") + "(" + processId.ToString("x") + ")");

                                    break;
                                }
                            }

                            // ウィンドウが見つからない/breakまでループ
                        } while (windowHandle != IntPtr.Zero);

                        if (destinationPoint.X == clickOrigin.X || destinationPoint.Y == clickOrigin.Y)
                        {
                            // ウィンドウが潰れている場合ハイライトしない
                            ScreenShade();
                        }
                        else
                        {
                            // 対象ウィンドウをハイライト
                            var originX = (int)((clickOrigin.X - targetScreen.Bounds.Location.X) / scale);
                            var originY = (int)((clickOrigin.Y - targetScreen.Bounds.Location.Y) / scale);
                            var destinationX = (int)((destinationPoint.X - targetScreen.Bounds.Location.X) / scale);
                            var destinationY = (int)((destinationPoint.Y - targetScreen.Bounds.Location.Y) / scale);

                            SquareShade(new Point(originX, originY), new Point(destinationX, destinationY));
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            // ダミー画面からマウスが離れた場合にシェードを再設定する
            switch (captureType)
            {
                case CaptureType.WINDOW:
                case CaptureType.FULL_SCREEN:
                    ScreenShade();
                    break;

                case CaptureType.SQUARE:
                default:
                    break;
            }

        }

        private void DummyScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 次回キャプチャのためにハンドルリストを初期化しておく
            handleList = new List<int>();
        }


        private void SquareShade(Point originPoint, Point destinationPoint)
        {
            // 矩形の選択範囲を描画
            var picture = new Bitmap(basePicture);

            var graphics = Graphics.FromImage(picture);
            graphics.DrawImage(picture, 0, 0, picture.Width / scale, picture.Height / scale);

            // ドラッグした範囲外の部分を描画する形で、矩形選択の表示を行う
            var pen = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
            graphics.FillRectangle(pen, new Rectangle(0, 0, picture.Width - (picture.Width - originPoint.X), picture.Height));
            graphics.FillRectangle(pen, new Rectangle(originPoint.X, 0, destinationPoint.X - originPoint.X, originPoint.Y));
            graphics.FillRectangle(pen, new Rectangle(destinationPoint.X, 0, picture.Width, picture.Height));
            graphics.FillRectangle(pen, new Rectangle(originPoint.X, destinationPoint.Y, destinationPoint.X - originPoint.X, picture.Height));

            pictureBox.Image = picture;
            pictureBox.Refresh();
        }

        private void ScreenShade()
        {
            // シェードを設定
            ScreenShade(170);
        }

        private void ScreenShade(int alpha)
        {
            // スクリーン全体にシェードを描画
            var picture = new Bitmap(basePicture);

            var graphics = Graphics.FromImage(picture);
            graphics.DrawImage(picture, 0, 0, picture.Width / scale, picture.Height / scale);

            var brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));
            graphics.FillRectangle(brush, new Rectangle(0, 0, picture.Width, picture.Height));

            pictureBox.Image = picture;
            pictureBox.Refresh();
        }

        // キャプチャした画像
        public static Bitmap CutPicture
        {
            get
            {
                return cutPicture;
            }
        }

        // キャプチャ時の基点
        public static Point ClickPoint
        {
            get
            {
                return clickPoint;
            }
        }

        private void DummyScreen_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ESCを押下で処理を終了する
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }
    }
}

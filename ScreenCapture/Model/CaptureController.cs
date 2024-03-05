using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture.Model
{
    /// <summary>
    /// キャプチャ用コントローラ.
    /// 
    /// </summary>
    internal class CaptureController
    {
        /// <summary>
        /// サイズ、座標を指定してキャプチャを実行.
        /// 
        /// </summary>
        /// <param name="size">キャプチャサイズ</param>
        /// <param name="startPoint">基準となる座標</param>
        /// <returns>キャプチャデータ</returns>
        public static Bitmap Execute(Size size, Point startPoint)
        {
            var bitmap = new Bitmap(size.Width, size.Height);

            var graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(startPoint, new Point(0, 0), bitmap.Size);

            graphics.Dispose();

            return bitmap;
        }


        /// <summary>
        /// フルスクリーンを指定してキャプチャを実行.
        /// 
        /// </summary>
        /// <param name="screen">対象のスクリーン</param>
        /// <returns>キャプチャデータ</returns>
        public static Bitmap Execute(Screen screen)
        {
            // スクリーン毎に拡大率が異なる場合があり、それらを考慮してキャプチャを実行
            Const.DEVMODE dm = new Const.DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(Const.DEVMODE));

            Const.EnumDisplaySettingsA(screen.DeviceName, Const.ENUM_CURRENT_SETTINGS, ref dm);

            var bitmap = new Bitmap(dm.dmPelsWidth, dm.dmPelsHeight);

            var graphics = Graphics.FromImage(bitmap);

            graphics.CopyFromScreen(screen.Bounds.Location, new Point(0, 0), bitmap.Size);

            graphics.Dispose();

            return bitmap;
        }



    }
}

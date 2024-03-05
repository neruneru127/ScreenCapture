using ScreenCapture.Model;
using ScreenCapture.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ScreenCapture
{
    /// <summary>
    /// �L���v�`���̐ݒ��J�n���s���z�[���E�B���h�E.
    /// 
    /// </summary>
    public partial class HomeWindow : Form
    {
        // �\�����̃L���v�`����E�B���h�E�ꗗ
        List<PictureWindow> pictureWindows = new List<PictureWindow>();
        // �L���v�`�����ɃE�B���h�E���\���ɂ���t���O
        bool hideWindowFlg = true;
        // �L���v�`���̍ۂɒx�����Ď��s����t���O
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
            // �E�B���h�E�ꗗ�Ŋ��ɕ����E�B���h�E�����O
            pictureWindows = pictureWindows.Where(window => !window.IsDisposed).ToList();

            // �ҋ@���ԗp�̃^�C�}�[���Z�b�g(�҂����ԂȂ��ł��ݒ���s��)
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
                // ��x�̂ݎ��s����悤��Stop������
                waitTimer.Stop();

                SetWindowVisible(false);

                if (Screen.AllScreens.Length == 1 && captureType == DummyScreen.CaptureType.FULL_SCREEN)
                {
                    // 1��ʂ��t���X�N���[���̏ꍇ�͑Ώۂ̉�ʂ����̂܂܃L���v�`��
                    var picture = CaptureController.Execute(Screen.PrimaryScreen);
                    var pictureWindow = new PictureWindow(picture, DummyScreen.ClickPoint);

                    SetWindowVisible(true);

                    pictureWindows.Add(pictureWindow);
                    pictureWindow.Show();

                    return;
                }

                var dummyScreenList = new List<DummyScreen>();

                // �S�ẴX�N���[���ɑ΂��ă_�~�[��ʂ��쐬
                foreach (var screen in Screen.AllScreens)
                {
                    var picture = CaptureController.Execute(screen);

                    var dummyScreen = new DummyScreen(picture, screen, captureType);

                    dummyScreenList.Add(dummyScreen);

                    // ��`�I����̏���������
                    dummyScreen.FormClosed += new FormClosedEventHandler((sender, e) =>
                    {
                        // 1�ł��_�~�[��ʂ������A�S�Ẵ_�~�[��ʂ����
                        dummyScreenList.ForEach(screen => screen.Dispose());

                        SetWindowVisible(true);

                        if (DummyScreen.CutPicture != null)
                        {
                            // �L���v�`�������摜���E�B���h�E�ɐݒ�
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
            // ��\���ɂ��Ȃ��ꍇ�͏������s��Ȃ�
            if (!hideWindowFlg)
            {
                return;
            }

            if (isShow)
            {
                // �E�B���h�E��\��
                this.Opacity = 1;
                this.Show();

                pictureWindows.ForEach(window => window.Opacity = 1);
                pictureWindows.ForEach(window => window.Show());
            }
            else
            {
                // �E�B���h�E���B��
                this.Opacity = 0;
                this.Hide();

                pictureWindows.ForEach(window => window.Opacity = 0);
                pictureWindows.ForEach(window => window.Hide());
            }
        }

    }
}
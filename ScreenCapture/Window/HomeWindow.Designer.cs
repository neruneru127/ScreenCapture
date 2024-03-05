using System.Windows.Forms;

namespace ScreenCapture
{
    partial class HomeWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomeWindow));
            this.windowCaptureButton = new System.Windows.Forms.Button();
            this.squareCaptureButton = new System.Windows.Forms.Button();
            this.fullScreenCaptureButton = new System.Windows.Forms.Button();
            this.screenToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.hideWindowButton = new System.Windows.Forms.Button();
            this.delayButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // windowCaptureButton
            // 
            this.windowCaptureButton.Image = global::ScreenCapture.Properties.Resources.WindowIcon;
            this.windowCaptureButton.Location = new System.Drawing.Point(254, 12);
            this.windowCaptureButton.Name = "windowCaptureButton";
            this.windowCaptureButton.Size = new System.Drawing.Size(48, 32);
            this.windowCaptureButton.TabIndex = 0;
            this.screenToolTip.SetToolTip(this.windowCaptureButton, "ウィンドウを指定してキャプチャ");
            this.windowCaptureButton.UseVisualStyleBackColor = true;
            this.windowCaptureButton.Click += new System.EventHandler(this.windowCaptureButton_Click);
            // 
            // squareCaptureButton
            // 
            this.squareCaptureButton.Image = global::ScreenCapture.Properties.Resources.SquareIcon;
            this.squareCaptureButton.Location = new System.Drawing.Point(201, 12);
            this.squareCaptureButton.Name = "squareCaptureButton";
            this.squareCaptureButton.Size = new System.Drawing.Size(47, 32);
            this.squareCaptureButton.TabIndex = 1;
            this.screenToolTip.SetToolTip(this.squareCaptureButton, "範囲を指定してキャプチャ");
            this.squareCaptureButton.UseVisualStyleBackColor = true;
            this.squareCaptureButton.Click += new System.EventHandler(this.squareCaptureButton_Click);
            // 
            // fullScreenCaptureButton
            // 
            this.fullScreenCaptureButton.Image = global::ScreenCapture.Properties.Resources.ScreenIcon;
            this.fullScreenCaptureButton.Location = new System.Drawing.Point(153, 12);
            this.fullScreenCaptureButton.Name = "fullScreenCaptureButton";
            this.fullScreenCaptureButton.Size = new System.Drawing.Size(42, 32);
            this.fullScreenCaptureButton.TabIndex = 2;
            this.screenToolTip.SetToolTip(this.fullScreenCaptureButton, "画面全体をキャプチャ\r\n");
            this.fullScreenCaptureButton.UseVisualStyleBackColor = true;
            this.fullScreenCaptureButton.Click += new System.EventHandler(this.fullScreenCaptureButton_Click);
            // 
            // hideWindowButton
            // 
            this.hideWindowButton.Image = global::ScreenCapture.Properties.Resources.HideWindowIcon;
            this.hideWindowButton.Location = new System.Drawing.Point(12, 12);
            this.hideWindowButton.Name = "hideWindowButton";
            this.hideWindowButton.Size = new System.Drawing.Size(42, 32);
            this.hideWindowButton.TabIndex = 6;
            this.screenToolTip.SetToolTip(this.hideWindowButton, "キャプチャ時ウィンドウを隠す");
            this.hideWindowButton.UseVisualStyleBackColor = true;
            this.hideWindowButton.Click += new System.EventHandler(this.hideWindowButton_Click);
            // 
            // delayButton
            // 
            this.delayButton.Image = global::ScreenCapture.Properties.Resources.NotDelayIcon;
            this.delayButton.Location = new System.Drawing.Point(60, 12);
            this.delayButton.Name = "delayButton";
            this.delayButton.Size = new System.Drawing.Size(42, 32);
            this.delayButton.TabIndex = 7;
            this.screenToolTip.SetToolTip(this.delayButton, "3秒後にキャプチャ");
            this.delayButton.UseVisualStyleBackColor = true;
            this.delayButton.Click += new System.EventHandler(this.delayButton_Click);
            // 
            // HomeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 56);
            this.Controls.Add(this.delayButton);
            this.Controls.Add(this.hideWindowButton);
            this.Controls.Add(this.fullScreenCaptureButton);
            this.Controls.Add(this.squareCaptureButton);
            this.Controls.Add(this.windowCaptureButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "HomeWindow";
            this.Text = "ScreenCapture";
            this.ResumeLayout(false);

        }

        #endregion

        private Button windowCaptureButton;
        private Button squareCaptureButton;
        private Button fullScreenCaptureButton;
        private ToolTip screenToolTip;
        private Button hideWindowButton;
        private Button delayButton;
    }
}
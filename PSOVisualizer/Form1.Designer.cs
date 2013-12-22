namespace PSOVisualizer
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.doubleBufferControl1 = new PSOVisualizer.DoubleBufferControl();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // doubleBufferControl1
            // 
            this.doubleBufferControl1.BackColor = System.Drawing.Color.Black;
            this.doubleBufferControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferControl1.GraphicTest = PSOVisualizer.DoubleBufferControl.GraphicTestMethods.DrawTest;
            this.doubleBufferControl1.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferControl1.Name = "doubleBufferControl1";
            this.doubleBufferControl1.PaintMethod = PSOVisualizer.DoubleBufferControl.DoubleBufferMethod.ManualDoubleBuffer20;
            this.doubleBufferControl1.Size = new System.Drawing.Size(1216, 750);
            this.doubleBufferControl1.TabIndex = 0;
            this.doubleBufferControl1.Text = "doubleBufferControl1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 750);
            this.Controls.Add(this.doubleBufferControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private DoubleBufferControl doubleBufferControl1;

    }
}


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PSOVisualizer
{
	public class DoubleBufferControl : Control
	{
        const Graphics NoBufferGraphics = null;
        const Bitmap NoBackBuffer = null;
        const BufferedGraphics NoManagedBackBuffer = null;
	    private List<Tuple<int, int>> points;
        Bitmap backBuffer;
        Graphics bufferGraphics;
        BufferedGraphicsContext graphicManager;
        BufferedGraphics managedBackBuffer;
		
		DoubleBufferMethod paintMethod = DoubleBufferMethod.NoDoubleBuffer;
		GraphicTestMethods graphicTest = GraphicTestMethods.DrawTest;

        public void SetPoints(List<Tuple<int, int>> pointList)
        {
            points = pointList;
        }
        public DoubleBufferControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            Application.ApplicationExit += MemoryCleanup;
        }

        private void InitializeComponent()
		{
            SuspendLayout();
            BackColor = Color.Black;
            Resize += DoubleBufferControl_Resize;
            ResumeLayout(false);
		}

		public DoubleBufferMethod PaintMethod
		{
			get { return paintMethod; }
			set
			{
				paintMethod = value;
				RemovePaintMethods();

                switch (value)
                {
                    case DoubleBufferMethod.BuiltInDoubleBuffer:
                        SetStyle(ControlStyles.UserPaint, true);
                        DoubleBuffered = true;
                        break;
                    case DoubleBufferMethod.BuiltInOptimizedDoubleBuffer:
                        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
                        break;
                    case DoubleBufferMethod.ManualDoubleBuffer11:
                        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                        backBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                        bufferGraphics = Graphics.FromImage(backBuffer);
                        break;

                    case DoubleBufferMethod.ManualDoubleBuffer20:
                        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                        graphicManager = BufferedGraphicsManager.Current;
                        graphicManager.MaximumBuffer = new Size(Width + 1, Height + 1);
                        managedBackBuffer = graphicManager.Allocate(CreateGraphics(), ClientRectangle);
                        break;
                }
			}
		}

		public GraphicTestMethods GraphicTest
		{
			get { return graphicTest; }
			set	
			{
				CreateGraphics().Clear(Color.Wheat);
				graphicTest = value;	
			}
		}

		public enum DoubleBufferMethod
		{
			NoDoubleBuffer,
			BuiltInDoubleBuffer,
			BuiltInOptimizedDoubleBuffer,
			ManualDoubleBuffer11,
			ManualDoubleBuffer20
		};

		public enum GraphicTestMethods
		{
			DrawTest,
			FillTest
		};

		private void MemoryCleanup(object sender, EventArgs e)
		{
            if (bufferGraphics != NoBufferGraphics)
                bufferGraphics.Dispose();
			if (backBuffer != NoBackBuffer)
				backBuffer.Dispose();
            if (managedBackBuffer != NoManagedBackBuffer)
                managedBackBuffer.Dispose();
		}


		protected override void OnPaint(PaintEventArgs e)
		{
            if (DesignMode) { base.OnPaint(e); return; }

			switch (paintMethod)
			{
				case DoubleBufferMethod.NoDoubleBuffer:
					base.OnPaint(e);
                    DrawGraphic(e.Graphics);
					break;

				case DoubleBufferMethod.BuiltInDoubleBuffer:
                    DrawGraphic(e.Graphics);
					break;

				case DoubleBufferMethod.BuiltInOptimizedDoubleBuffer:
                    DrawGraphic(e.Graphics); 
					break;

				case DoubleBufferMethod.ManualDoubleBuffer11:
					PaintDoubleBuffer11(e.Graphics); break;

				case DoubleBufferMethod.ManualDoubleBuffer20:
					PaintDoubleBuffer20(e.Graphics); break;
			}
		}

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

		private void RemovePaintMethods()
		{
			DoubleBuffered = false;

			SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            if (bufferGraphics != NoBufferGraphics)
            {
                bufferGraphics.Dispose();
                bufferGraphics = NoBufferGraphics;
            }
            if (backBuffer != NoBackBuffer)
            {
                backBuffer.Dispose();
                backBuffer = NoBackBuffer;
            }
            if (managedBackBuffer != NoManagedBackBuffer)
                managedBackBuffer.Dispose();
		}

        private void DoubleBufferControl_Resize(object sender, EventArgs e)
        {
            switch (paintMethod)
            {
                case DoubleBufferMethod.ManualDoubleBuffer11:
                    backBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
                    bufferGraphics = Graphics.FromImage(backBuffer);
                    break;
                case DoubleBufferMethod.ManualDoubleBuffer20:
                    graphicManager.MaximumBuffer = new Size(Width + 1, Height + 1);
                    if (managedBackBuffer != NoManagedBackBuffer)
                        managedBackBuffer.Dispose();
                    managedBackBuffer = graphicManager.Allocate(CreateGraphics(), ClientRectangle);
                    break;
            }
            Refresh();
        }

        private void PaintDoubleBuffer11(Graphics controlGraphics)
        {
            DrawGraphic(bufferGraphics);
            controlGraphics.DrawImageUnscaled(backBuffer, 0, 0); 
        }

		private void PaintDoubleBuffer20(Graphics controlGraphics)
		{
			try
			{
                DrawGraphic(managedBackBuffer.Graphics);
                managedBackBuffer.Render(controlGraphics);
			}
			catch (Exception exception) { Console.WriteLine(exception.Message); }
		}

        private void DrawGraphic(Graphics tempGraphics)
        {
            Pen colorPen = null;
            var random = new Random(DateTime.Now.Millisecond);

            tempGraphics.Clear(Color.Wheat);
            
            foreach (var point in points)
            {
                colorPen = new Pen(Color.FromArgb(127, random.Next(0, 256), random.Next(256), random.Next(256)));
                tempGraphics.DrawLine(colorPen, point.Item1, point.Item2, point.Item1+2, point.Item2+2 );
            }
            if (colorPen != null) colorPen.Dispose();
        }
    }
}

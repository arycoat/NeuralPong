using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace NeuralPong
{
    public partial class MainApp : Form
    {
        private readonly PongTrainer _trainer;

        bool loaded = false;

        public MainApp(PongTrainer trainer)
        {
            InitializeComponent();

            this.BackColor = Color.Black;
            this.Size = new System.Drawing.Size(430, 450);
            this.Text = "Nerual Pong Trainer";
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            this.KeyDown += new KeyEventHandler(AppKeyPress);

            Timer timer = new Timer();
            timer.Interval = 30; // 0.03 sec
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();

            this._trainer = trainer;
        }

        private void timer_tick(object sender, EventArgs e)
        {
            _trainer.timer_tick(null);
            Invalidate(); // 컨트롤의 전체 화면을 무효화하고 컨트롤을 다시 그립니다.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //trainer.display(e);
            Vector ball = _trainer.Ball;
            float paddleLeft = _trainer.PaddleLeft;
            float paddleRight = _trainer.PaddleRight;

            Graphics g = e.Graphics;
            g.TranslateTransform(200f, 200f);
            g.ScaleTransform((float)200, (float)200);

            border(g, PongTrainer.MaxX, PongTrainer.MaxY);

            g.FillRectangle(Brushes.White, (float)ball.X - 0.01f, (float)ball.Y - 0.01f, 0.02f, 0.02f);
            g.FillRectangle(Brushes.Yellow, PongTrainer.MinX - 0.02f, paddleLeft - 0.15f, 0.03f, 0.30f);
            g.FillRectangle(Brushes.Yellow, PongTrainer.MaxX - 0.01f, paddleRight - 0.15f, 0.03f, 0.30f);

            base.OnPaint(e);
        }

        void border(Graphics g, float w, float h)
        {
            float ww = w + 0.05f;
            float hh = h + 0.05f;
            g.FillRectangle(Brushes.Purple, -ww,  hh, 2 * ww, 0.01f); // bottom
            g.FillRectangle(Brushes.Purple, -ww, -hh + 0.01f, 2 * ww, 0.01f); // top
            g.FillRectangle(Brushes.Purple, -ww, -hh + 0.01f, 0.01f, 2 * hh); // left
            g.FillRectangle(Brushes.Purple, ww - 0.01f, -hh + 0.01f, 0.01f, 2 * hh); // right
        }

        void AppKeyPress(object sender, KeyEventArgs e)
        {
            _trainer.KeyPress(e.KeyCode);
            e.Handled = true;
        }
    }
}

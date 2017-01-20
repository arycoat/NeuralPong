using System;
using System.Windows;
using System.Windows.Forms;

namespace NeuralPong
{
    public class PongTrainer
    {
        private readonly AI _aiLeft;
        private readonly AI _aiRight;
        private readonly Ball _ball;

        public const float MinX = -0.8f;
        public const float MinY = -0.5f;
        public const float MaxX = 0.8f;
        public const float MaxY = 0.5f;

        public int left_hits_per_round = 0;

        int tim = 20;

        private int lastTick;
        private int lastFrameRate;
        private int frameRate;

        bool p = false;

        public Vector Ball { get { return _ball.Position; } }
        public float PaddleLeft { get { return _aiLeft.Position; } }
        public float PaddleRight { get { return _aiRight.Position; } }

        public PongTrainer()
        {
            _aiLeft = new AI(new NeuralNetwork(5, 1, 3, 5, 0.2f, 0.0f));
            _aiRight = new AI(new NeuralNetwork(5, 1, 3, 10, 0.2f, 0.0f));

            _ball = new Ball(PongTrainer.MinY, PongTrainer.MaxY);
            _ball.Reset();
        }

        public void timer_tick(object sender)
        {
            for (int i = 0; i < tim; ++i)
            {
                frame();
            }
        }

        void update()
        {
            float[] inputs = new float[5] { 0, 0, 0, 0, 0 };
            inputs[0] = (float)_ball.Position.X;
            inputs[1] = (float)_ball.Position.Y;
            inputs[2] = (float)_ball.Velocity.X;
            inputs[3] = (float)_ball.Velocity.Y;

            if (_ball.Velocity.X > 0)
            {
                inputs[4] = _aiRight.Position;
                _aiRight.LastPosition = _aiRight.Position;
                float[] outputs = _aiRight.FeedForward(inputs);
                _aiRight.Position += (outputs[0] - 0.5f);
                if (_aiRight.Position > MaxY - 0.10f) _aiRight.Position = MaxY - 0.10f;
                if (_aiRight.Position < MinY + 0.12f) _aiRight.Position = MinY + 0.12f;
            }
            else
            {
                inputs[0] *= -1;
                inputs[2] *= -1;
                inputs[4] = _aiLeft.Position;
                _aiLeft.LastPosition = _aiLeft.Position;
                float[] outputs = _aiLeft.FeedForward(inputs);
                _aiLeft.Position += (outputs[0] - 0.5f);
                if (_aiLeft.Position > MaxY - 0.10) _aiLeft.Position = MaxY - 0.10f;
                if (_aiLeft.Position < MinY + 0.12) _aiLeft.Position = MinY + 0.12f;
            }
        }
        
        public void frame() 
        {
	        if(!p)
            {
		        update();
                _ball.Move();

                float ballPositionX = (float) _ball.Position.X;
                float ballPositionY = (float) _ball.Position.Y;
                if (ballPositionX <= MinX)
                {
                    if ((_aiLeft.Position + 0.15 > ballPositionY) && (_aiLeft.Position - 0.15 < ballPositionY))
                    {
                        _ball.TurnX();
                        _aiLeft.Hits++;
                        left_hits_per_round++;
                    }
                    else
                    {
                        _aiLeft.Miss++;
                        left_hits_per_round = 0;
                        float[] inputs = new float[1];
                        inputs[0] = (ballPositionY - _aiLeft.LastPosition) + 0.5f;
                        _aiLeft.BackPropagation(inputs);
                        _ball.Reset();
                    }
                    print_s();
                }
                else if (ballPositionX >= MaxX)
                {
                    if ((_aiRight.Position + 0.15 > ballPositionY) && (_aiRight.Position - 0.15 < ballPositionY))
                    {
                        _ball.TurnX();
                        _aiRight.Hits++;
                    }
                    else
                    {
                        _aiRight.Miss++;
                        float[] inputs = new float[1];
                        inputs[0] = (ballPositionY - _aiRight.LastPosition) + 0.5f;
                        _aiRight.BackPropagation(inputs);
                        _ball.Reset();
                    }
                    print_s();
                }
	        }
		
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }

            frameRate++;
        }

        void print_s()
        {
            Console.Clear();
            Console.WriteLine(string.Format("AI 1 : miss={0}, hits={1} AI 2 : miss={2}, hits={3}",
                _aiLeft.Miss, _aiLeft.Hits, _aiRight.Miss, _aiRight.Hits));
            //Console.WriteLine(string.Format("LeftHitPerRound : {0}", left_hits_per_round));
            Console.WriteLine(string.Format("Speed : x{0}", tim));
            Console.WriteLine(string.Format("FrameRate : {0}", lastFrameRate));
            Console.WriteLine("");
            Console.WriteLine("(z) speed up, (x) speed down, (r) reset ball position");
        }

        public void KeyPress(Keys keyCode)
        {
            if (keyCode == Keys.Z)
                tim++;
            else if (keyCode == Keys.X && tim > 1)
                tim--;
            else if (keyCode == Keys.R)
                _ball.Reset();
        }
    }
}

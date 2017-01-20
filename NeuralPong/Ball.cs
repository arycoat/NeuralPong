using System;
using System.Windows;

namespace NeuralPong
{
    public class Ball
    {
        private static readonly Random random = new Random();

        public Vector Position;
        public Vector Velocity;

        private readonly double MinY;
        private readonly double MaxY;

        public Ball(double minY, double maxY)
        {
            Position = new Vector(0, 0);
            Velocity = new Vector(0, 0);

            this.MinY = minY;
            this.MaxY = maxY;
        }

        public void Reset()
        {
            Position *= 0;

            do
            {
                Velocity.X = (random.Next(0, 9) / 100f) - 0.05;
            } while (Velocity.X == 0.0f);
            if (Velocity.X >= 0) Velocity.X += 0.01f;
            else Velocity.X -= 0.01f;
            do
            {
                Velocity.Y = (random.Next(0, 9) / 100f) - 0.05;
            } while (Velocity.Y == 0.0);
            if (Velocity.Y >= 0) Velocity.Y += 0.01f;
            else Velocity.Y -= 0.01f;
        }

        public void Move()
        {
            Position += Velocity;

            if (Position.Y < this.MinY)
            {
                Velocity.Y *= -1.0f;
                Position.Y = this.MinY - (Position.Y - this.MinY);
            }
            else if (Position.Y > this.MaxY)
            {
                Velocity.Y *= -1.0f;
                Position.Y = this.MaxY - (Position.Y - this.MaxY);
            }
        }

        public void TurnX()
        {
            Velocity.X *= -1.0f;
        }
    }
}

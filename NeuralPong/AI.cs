using System;

namespace NeuralPong
{
    public class AI
    {
        private readonly NeuralNetwork _neuralNetwork;

        public float Position { get; set; }
        public float LastPosition { get; set; }
        public int Hits { get; set; }
        public int Miss { get; set; }

        public AI(NeuralNetwork neuralNetwork)
        {
            _neuralNetwork = neuralNetwork;

            Position = 0;
            LastPosition = 0;
            Miss = 0;
            Hits = 0;
        }

        public float[] FeedForward(float[] inputs)
        {
            return _neuralNetwork.FeedForward(inputs);
        }

        public void BackPropagation(float[] desiredOutputs)
        {
            _neuralNetwork.BackPropagation(desiredOutputs);
        }
    }
}

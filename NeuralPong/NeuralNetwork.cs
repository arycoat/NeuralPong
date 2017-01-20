using System;
using System.Collections.Generic;
using System.Linq;

namespace NeuralPong
{
    using LayerType = Layer.LayerType;

    public class Neuron
    {
        private static Random random = new Random();
        private static int serial = 1;

        private int index; // for debug

        public readonly float[] weights;
        public readonly float[] inputs;
        public readonly float[] errors;
        public float output;

        public Func<float, float> Activation { get; protected set; }
        public Func<float, float> ActivationGradiant { get; protected set; }

        public Neuron(int inputCount)
        {
            index = serial++;

            weights = new float[inputCount];
            inputs = new float[inputCount];
            errors = new float[inputCount];
        }

        public virtual void Init()
        {
            // RandomWeights
            for (int i = 0; i < weights.Length; ++i)
            {
                weights[i] = (float) random.NextDouble() - 0.5f;
            }

            Activation = (z) => 1.0f / (1.0f + (float) Math.Exp(-z)); // sigmoid
            ActivationGradiant = (x) => x * (1.0f - x); // gradient of sigmoid
        }
    }

    public class InputNeuron : Neuron
    {
        public InputNeuron()
            : base(1)
        {
            
        }

        public override void Init()
        {
            for (int i = 0; i < weights.Length; ++i)
            {
                weights[i] = 1.0f;
            }

            Activation = (z) => z; // linear function
            ActivationGradiant = (x) => 1; // linear function
        }
    }

    public class Layer
    {
        public enum LayerType
        {
            InputLayer,
            HiddenLayer,
            OutputLayer,
        }

        public List<Neuron> neurons;
        public int NodeCount { get { return neurons.Count; } }
        public LayerType Type { get; private set; }

        public Layer(LayerType type)
        {
            this.Type = type;
        }
    }

    public class NeuralNetwork
    {
        private readonly float learningRate;
        private readonly float momentum;
        private List<Layer> layers;

        public NeuralNetwork(int inputNodeCount, int outputNodeCount, int layerCount, int hiddenLayerNodeCount,
            float learningRate, float momentum)
        {
            this.momentum = momentum;
            this.learningRate = learningRate;
            layers = new List<Layer>();

            // input layer.
            {
                AddLayer(LayerType.InputLayer, inputNodeCount, () => new InputNeuron());
            }

            // hidden layers.
            for (int l = 1; l < layerCount - 1; l++)
            {
                int nd = layers.Last().NodeCount;
                AddLayer(LayerType.HiddenLayer, hiddenLayerNodeCount, () => new Neuron(nd + 1));
            }

            // output layer.
            {
                int nd = layers.Last().NodeCount;
                AddLayer(LayerType.OutputLayer, outputNodeCount, () => new Neuron(nd + 1));
            }
        }

        private void AddLayer(Layer.LayerType layerType, int nodeCount, Func<Neuron> func)
        {
            Layer layer = new Layer(layerType);
            layer.neurons = new List<Neuron>();
            for (int i = 0; i < nodeCount; ++i)
            {
                Neuron neuron = func();
                neuron.Init();
                layer.neurons.Add(neuron);
            }
            layers.Add(layer);
        }

        public float[] FeedForward(float[] inputs)
        {
            float[] outputs = null;
            foreach (Layer layer in layers)
            {
                outputs = new float[layer.NodeCount + 1];
                for (int j = 0; j < layer.NodeCount; j++)
                {
                    Neuron neuron = layer.neurons[j];
                    float sum = 0.0f;
                    if (layer.Type == LayerType.InputLayer)
                    {
                        neuron.inputs[0] = inputs[j];
                        sum = neuron.weights[0] * inputs[j];
                    }
                    else
                    {
                        for (int k = 0; k < neuron.inputs.Length; k++)
                        {
                            neuron.inputs[k] = inputs[k];
                            sum += neuron.weights[k] * inputs[k];
                        }
                    }
                    outputs[j] = neuron.Activation(sum);
                    neuron.output = outputs[j];
                }
                outputs[layer.NodeCount] = -1.0f;
                inputs = outputs;
            }

            return outputs;
        }

        public void BackPropagation(float[] desiredOutputs)
        {
            // output layer
            int outputLayer = layers.Count - 1;
            {
                for (int j = 0; j < layers[outputLayer].NodeCount; j++)
                {
                    Neuron neuron = layers[outputLayer].neurons[j];

                    float sum = (desiredOutputs[j] - neuron.output);
                    float dalta = sum * neuron.ActivationGradiant(neuron.output);

                    for (int k = 0; k < neuron.inputs.Length; k++)
                    {
                        neuron.errors[k] = dalta + momentum * neuron.errors[k];
                        neuron.weights[k] += learningRate * neuron.inputs[k] * neuron.errors[k];
                    }
                }
            }

            // hidden layers
            for (int hiddenLayer = layers.Count - 2; hiddenLayer > 0; hiddenLayer--)
            {
                for (int j = 0; j < layers[hiddenLayer].NodeCount; j++)
                {
                    Neuron neuron = layers[hiddenLayer].neurons[j];

                    float sum = layers[hiddenLayer + 1].neurons
                        .Aggregate(0.0f, (a, b) => a += b.errors[j] * b.weights[j]);
                    float dalta = sum * neuron.ActivationGradiant(neuron.output);

                    for (int k = 0; k < neuron.inputs.Length; k++)
                    {
                        neuron.errors[k] = dalta + momentum * neuron.errors[k];
                        neuron.weights[k] += learningRate * neuron.inputs[k] * neuron.errors[k];
                    }
                }
            }

            // input layer
            //
        }
    }
}

using System;

namespace AI_Life
{
    public enum TransferFunctions
    {
        LogSigmoid,
        HardLimit,
        SaturatingLinear,
        PositiveLinear
    }
    // represents one complete feed forward neural network
    //brain of each ant
    public class Network
    {

        //=========================================================//

        public static TransferFunctions tF = TransferFunctions.LogSigmoid;

        int numInputs;
        int numHidden;
        int perHidden;
        int numOutputs;

        int numNeurons;
        int numWeights;

        int numLayers;
        Layer[] layers;
        //=========================================================//

        public Network(int initNumInputs, int initNumHidden, int initPerHidden, int initNumOutputs)
        {
            numInputs = initNumInputs;
            numHidden = initNumHidden;
            perHidden = initPerHidden;
            numOutputs = initNumOutputs;
            numNeurons = numInputs + (numHidden * perHidden) + numOutputs;
            numWeights = ((numInputs + 1) * perHidden) + ((perHidden + 1) * perHidden) * (numHidden - 1) + ((perHidden + 1) * numOutputs);
            numLayers = numHidden + 1; // layers = output + all hidden layers
            layers = new Layer[numLayers];

            int iCurLayer = 0;
            int i;

            if (numHidden > 0)
            {
                layers[iCurLayer] = new Layer(perHidden, numInputs);
                iCurLayer++;
                for (i = 1; i < numHidden; i++)
                {
                    layers[iCurLayer] = new Layer(perHidden, perHidden);
                    iCurLayer++;
                }
                layers[iCurLayer] = new Layer(numOutputs, perHidden);
            }
            else
                layers[iCurLayer] = new Layer(numOutputs, numInputs);
        }

        public Array Weights
        {
            get
            {
                int i, j, k, l;
                Array ret = Array.CreateInstance(typeof(double), numWeights);

                l = 0;
                for (i = 0; i < numLayers; i++)
                {
                    for (j = 0; j < layers[i].NumNeurons; j++)
                    {
                        for (k = 0; k < layers[i].Neuron(j).NumInputs; k++)
                        {
                            ret.SetValue(layers[i].Neuron(j).GetWeight(k), l);
                            l++;
                        }
                    }
                }

                return ret;
            }
            set
            {
                int i, j, k, l;

                l = 0;
                for (i = 0; i < numLayers; i++)
                {
                    for (j = 0; j < layers[i].NumNeurons; j++)
                    {
                        for (k = 0; k < layers[i].Neuron(j).NumInputs; k++)
                        {
                            layers[i].Neuron(j).SetWeight(k, (double)value.GetValue(l));
                            l++;
                        }
                    }
                }
            }
        }

        public double[] FeedData(double[] input)
        {
            double[] curInput;
            double[] curOutput = { };
            int i, j, k;
            double netInput;

            curInput = input;

            for (i = 0; i < numLayers; i++)
            {
                curOutput = new double[layers[i].NumNeurons];
                for (j = 0; j < layers[i].NumNeurons; j++)
                {
                    netInput = 0;
                    for (k = 0; k < layers[i].Neuron(j).NumInputs - 1; k++)
                        netInput += (curInput[k] * layers[i].Neuron(j).GetWeight(k));
                    netInput -= layers[i].Neuron(j).GetWeight(k);
                    curOutput[j] = TransferFunction(netInput);
                }

                if (i < (numLayers - 1))
                    curInput = curOutput;
            }

            return curOutput;

        }

        public static double TransferFunction(double netinput)
        {
            switch (tF)
            {
                case TransferFunctions.LogSigmoid:
                    return (1 / (1 + Math.Exp(-netinput)));
                case TransferFunctions.HardLimit:
                    if (netinput < 0)
                        return 0;
                    else
                        return 1;
                case TransferFunctions.SaturatingLinear:
                    if (netinput < 0)
                        return 0;
                    else if (netinput > 1)
                        return 1;
                    else
                        return netinput;
                case TransferFunctions.PositiveLinear:
                    return (netinput > 0) ? netinput : 0;
            }
            return 0;
        }
    }
}

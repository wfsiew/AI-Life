using System;

namespace AI_Life
{

    public class Neuron
    {

        //=========================================================//
        int numInputs;		// number of inputs
        double[] weights;	// array of weights

        public Neuron(int initNumInputs)
        {
            numInputs = initNumInputs + 1; // + 1 for bias
            weights = new double[numInputs];

            for (int i = 0; i < numInputs; i++)
                weights[i] = ((float)(Cosmos.randomR.NextDouble()) * 2) - 1;

        }

        public double NumInputs
        {
            get
            {
                return numInputs;
            }
        }

        public void SetWeight(int index, double newVal)
        {
            weights[index] = newVal;
        }

        public double GetWeight(int index)
        {
            return weights[index];
        }

    }
}

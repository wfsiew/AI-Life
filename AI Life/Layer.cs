using System;

namespace AI_Life
{

    public class Layer
    {
        //=========================================================//
        int numNeurons;			// number of neurons
        int inputsPerNeuron;	// number of inputs per neuron

        Neuron[] neurons;		// neuron array

        public Layer(int initNumNeurons, int initInputsPerNeuron)
        {
            numNeurons = initNumNeurons;
            inputsPerNeuron = initInputsPerNeuron;

            neurons = new Neuron[numNeurons];
            for (int i = 0; i < numNeurons; i++)
                neurons[i] = new Neuron(inputsPerNeuron);

        }

        public Neuron Neuron(int index)
        {
            return neurons[index];
        }

        public int NumNeurons
        {
            get
            {
                return numNeurons;
            }
        }
    }
}

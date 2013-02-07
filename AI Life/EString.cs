using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace AI_Life
{
    //for sorting
    class OneString : IComparable<OneString>
    {
        public string name;
        public int fitness;
        public OneString(string Name, int Fitness)
        {
            name = Name;
            fitness = Fitness;
        }

        public int CompareTo(OneString other)
        {
            return this.fitness - other.fitness;
        }
    }

    //evolving strings - please read a tutorial on genetic algorithms if it seems confusing
    //that will make meaning of all variables and functions clear
    class EString
    {
        public static float mutationRate = .15f;
        public static float elitism = .351f;
        public static int popSize = 80000;
        public static string target = "I Can Mutate";
        public static uint maxIteration = 16000;
        public static int errorCount = 0;
        public static Random rand = new Random();
        public static int sLen = target.Length;
        public static string bestS = "";
        public static int generationCount = 0;


        public static void Execute(Graphics g, ListView listView)
        {
            sLen = target.Length;
            List<OneString> ones = new List<OneString>(popSize);
            List<OneString> two = new List<OneString>(popSize);
            List<OneString> temp = new List<OneString>(popSize);
            ListViewItem lvI;
            Initialize(ref ones, ref  two, ref  temp);
            for (int i = 0; i < maxIteration && mainForm.eStringRunning; i++)
            {
                generationCount = i;
                CalculateFitness(ref ones);
                ones.Sort();
                lvI = listView.Items.Add((1 + i).ToString() + ". ");
                PrintBest(ones, two, lvI);
                if (ones[0].fitness == 0)
                {
                    break;
                }
                Mate(ref ones, ref  two);
                Swap(ref ones, ref  two, ref  temp);
            }
        }

        private static void Swap(ref List<OneString> ones, ref  List<OneString> two, ref  List<OneString> temp)
        {
            temp = ones;
            ones = two;
            two = temp;
        }

        private static void Mate(ref List<OneString> ones, ref List<OneString> two)
        {
            int esize = (int)(popSize * elitism);
            int spos, i1, i2;
            ElitismF(ref ones, ref  two, esize);
            for (int i = esize; i < popSize; i++)
            {
                i1 = rand.Next(popSize);
                i2 = rand.Next(popSize);
                spos = rand.Next(sLen - 1);
                two[i].name = ones[i1].name.Substring(0, spos) + ones[i2].name.Substring(spos, sLen - spos);
                if (rand.NextDouble() < mutationRate)
                    Mutate(two[i]);
            }
        }

        private static void Mutate(OneString one)
        {
            string a = one.name;
            int random = rand.Next(sLen);
            char[] nameS = new char[sLen];
            nameS = a.ToCharArray();
            nameS[random] = (char)rand.Next(32, 142);
            one.name = nameS.ToString(); ;
        }

        private static void ElitismF(ref List<OneString> ones, ref List<OneString> two, int esize)
        {
            for (int i = 0; i < esize; i++)
            {
                two[i].name = ones[i].name;
                two[i].fitness = ones[i].fitness;
            }
        }

        private static void PrintBest(List<OneString> ones, List<OneString> two, ListViewItem lvI)
        {
            errorCount = ones[0].fitness;
            lvI.SubItems.Add(ones[0].name);
            lvI.SubItems.Add(ones[0].fitness.ToString());
            lvI.EnsureVisible();
            System.Windows.Forms.Application.DoEvents();
        }

        private static void CalculateFitness(ref List<OneString> ones)
        {
            int fitness;
            for (int i = 0; i < popSize; i++)
            {
                fitness = 0;
                for (int j = 0; j < sLen; j++)
                {
                    fitness += Math.Abs(ones[i].name[j] - target[j]);
                }
                ones[i].fitness = fitness;
            }
        }

        static void Initialize(ref List<OneString> ones, ref  List<OneString> two, ref  List<OneString> temp)
        {
            string rString = "";
            for (int i = 0; i < popSize; i++)
            {
                rString = "";
                for (int j = 0; j < sLen; j++)
                {
                    rString += (char)rand.Next(32, 142);
                }
                ones.Add(new OneString(rString, 0));
                two.Add(new OneString(rString, 0));
                temp.Add(new OneString(rString, 0));
            }
        }
    }
}

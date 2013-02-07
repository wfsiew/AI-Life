using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AI_Life
{


    public class Cosmos
    {
        public static Random randomR = new Random();


        //=========================================================//
        // 
        public static double maxForce = 30; //truning force
        public static double maxSpeed = 8;
        public static double foodTolerance = 10;		//how close the food should be?

        public static double mutationRate = 0.2;    //refer some genetic algo. tutorial
        public static double maxPermutation = 0.3;  //same tut. slash value
        public static int elitismPopulation;    //finest one's we should keep them alive
        public static float elitismRate = 0.2f; //it defines them
        public static int maxIteration = 10000;
        public static int fastModeDiP = 1000;			//update disp. after what interval?


        //=========================================================//

        public PointF[] food;

        private int numFood;
        private Ants[] ants;
        private int numAnts;
        private Size clientSize;

        //=========================================================//
        // GUI objects
        private Graphics g;
        private ListView listAnalysis;
        private Panel panel;
        //this is used for back-buffering; so that no flickering occurs
        private BufferedGraphics grafx;

        //=========================================================//
        //static members
        //=========================================================//
        // graphics objects
        public static Brush whiteBrush = Brushes.White;
        public static Pen whitePen = Pens.White;
        public static Brush foodBrush = Brushes.LightGreen;
        public static Brush redFoodBrush = Brushes.Red;
        public static Brush foodCollectedB = Brushes.LightGreen;
        public static Brush antNumberB = Brushes.Yellow;
        public static Pen foodPen = Pens.LightGreen;
        public static Brush backBrush = Brushes.Black;
        public static Font foodFont = new Font("Microsoft Sans Serif", 8.25f);
        public static Font drawFont = new Font(FontFamily.GenericSansSerif, 10);

        public static bool isRunning;
        public static bool showDetails = true;
        public static bool fastMode = false;
        public static bool useElitism = false;
        public static bool landMines = false;

        //=========================================================//
        // variables for analysis
        private int lastTotalFit = 1;
        private int inCG;
        private int generationCount = 0;

        public Cosmos(int NumFood, int NumAnts, Size ClientSize, Graphics g, ListView initListAnalysis, Panel p, BufferedGraphics gr)
        {
            clientSize = ClientSize;
            this.g = g;
            listAnalysis = initListAnalysis;
            panel = p;
            grafx = gr;
            numFood = NumFood;
            food = new PointF[numFood];
            RandomFood();
            numAnts = NumAnts;
            ants = new Ants[numAnts];
            Cosmos.elitismPopulation = (int)(numAnts * Cosmos.elitismRate);
            //create random ants
            for (int i = 0; i < numAnts; i++)
                ants[i] = new Ants(clientSize, this, null, i + 1);

        }
        // main simulation method, function, procedure??

        //basically we let the ants move uptil a number then we see who did best,
        //mutate as necessary, create new population and start the process again.
        public void Step()
        {
            isRunning = true;
            int count = 0;
            generationCount = 1;
            inCG = 0;
            while (true == isRunning)
            {
                for (int i = 0; i < numAnts; i++)
                    ants[i].UpdatePosition();
                inCG++;
                count++;
                if (inCG >= maxIteration)
                    Update();
                if ((count % Cosmos.fastModeDiP == 0) || (fastMode == false))
                {
                    if (!mainForm.leaveTrail)
                    {
                        g.FillRectangle(backBrush, 0, 0, clientSize.Width, clientSize.Height);
                    }
                    DrawToBuffer();
                    grafx.Render(Graphics.FromHwnd(panel.Handle));
                }
                try
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                catch { }
            }
        }

        private void Update()
        {
            int totalFit = CalculateFitness();
            PrintBest(totalFit);
            lastTotalFit = totalFit;
            NewGeneration();
            inCG = 0;
            generationCount++;
        }
        //the team effort
        private int CalculateFitness()
        {
            int totalFit = 0;
            for (int i = 0; i < numAnts; i++)
            {
                totalFit += ants[i].FoodCollected;
            }
            return totalFit;

        }

        private void PrintBest(int totalFit)
        {
            ListViewItem lI = listAnalysis.Items.Add(generationCount.ToString());
            lI.SubItems.Add(totalFit.ToString());
            float temp = ((float)totalFit / (float)lastTotalFit * 100) - 100;
            if (generationCount != 1)
                lI.SubItems.Add((temp > 0 ? "+" : "") + temp.ToString("0.00") + "%");
            lI.Selected = true;
            lI.EnsureVisible();
        }

        public PointF FindClosestFood(double x, double y, ref double foodDist, ref int indMyFood)
        {
            double minDist = 1000000, curDist;
            int indexMinDist = -1;

            for (int i = 0; i < numFood; i++)
            {
                curDist = Math.Sqrt(((x - food[i].X) * (x - food[i].X)) + ((y - food[i].Y) * (y - food[i].Y)));
                if (curDist < minDist)
                {
                    minDist = curDist;
                    indexMinDist = i;
                }
            }

            foodDist = minDist;
            indMyFood = indexMinDist;
            return food[indexMinDist];
        }
        //replace "eaten-food" with new food
        public void NewFood(int oldIndex)
        {
            food[oldIndex].X = (float)(randomR.NextDouble()) * (float)clientSize.Width;
            food[oldIndex].Y = (float)(randomR.NextDouble()) * (float)clientSize.Height;
        }
        //draw random food
        private void RandomFood()
        {
            for (int i = 0; i < numFood; i++)
            {
                food[i].X = (float)(randomR.NextDouble()) * (float)clientSize.Width;
                food[i].Y = (float)(randomR.NextDouble()) * (float)clientSize.Height;
            }
        }
        //draw details and other suff
        private void DrawToBuffer()
        {
            float percentGen = (100 + ((float)(inCG - maxIteration) / (float)Cosmos.maxIteration) * 100);
            if (landMines)
            {
                for (int i = 0; i < numFood; i++)
                    g.FillEllipse(redFoodBrush, (int)food[i].X - 2, (int)food[i].Y - 2, 4, 4);
            }
            else
            {
                for (int i = 0; i < numFood; i++)
                    g.FillEllipse(foodBrush, (int)food[i].X - 2, (int)food[i].Y - 2, 4, 4);
            }
            for (int i = 0; i < numAnts; i++)
                ants[i].DrawToBuffer(g);
            if (showDetails)
            {
                g.DrawString("Elitism Rate(Ins/Del): " + elitismRate.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, 10));
                g.DrawString("Mutation Rate(Home/End): " + mutationRate.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, 25));
                g.DrawString("Food Tolerance(PUp/PDn): " + foodTolerance.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, 40));
                g.DrawString("Steer Force(Q/A): " + maxForce.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, 55));
                g.DrawString("Max Speed(W/S): " + maxSpeed.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, 70));

            }
            g.DrawString("Transfer Function: " + Network.tF.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, clientSize.Height - 35));
            g.DrawString("Current generation: " + generationCount.ToString(), Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, clientSize.Height - 50));
            g.DrawString("Percentage of current generation: " + percentGen.ToString("0.00") + "%", Cosmos.drawFont, Brushes.GreenYellow, new PointF(10, clientSize.Height - 20));
        }
        //choose to mutate and create new gen.
        private void NewGeneration()
        {
            int totalFit = CalculateFitness();

            if (useElitism == true || landMines == true)
            {
                Sort();
                if (landMines)
                {
                    for (int j = 3, i = 0; j < Cosmos.elitismPopulation + 1; j++, i++)
                        ants[j].net.Weights = ants[0].net.Weights;
                    Array temp1 = ants[0].net.Weights;
                    Array temp2 = ants[numAnts - Cosmos.elitismPopulation].net.Weights;
                    Mate(ref temp1, ref temp2);
                    ants[0].net.Weights = temp1;
                    ants[numAnts - Cosmos.elitismPopulation].net.Weights = temp2;
                }
            }

            Array parent1;
            Array parent2;

            Ants[] newAnts = new Ants[numAnts];
            //create new gen
            for (int i = 0; i < (numAnts / 2); i++)
            {
                parent1 = ants[RouletteWheel(totalFit)].net.Weights;
                parent2 = ants[RouletteWheel(totalFit)].net.Weights;
                Mutate(parent1);
                Mutate(parent2);
                newAnts[i * 2] = new Ants(clientSize, this, parent1, i * 2);
                newAnts[i * 2 + 1] = new Ants(clientSize, this, parent2, i * 2 + 1);
            }
            //replace and leave the other to GC
            ants = newAnts;
        }

        //if ants were aligned in a line, this method will take the best ones,
        //place them on one end the the worst ones on the other, and leave other
        //untouched
        //which is best and worst depends on landmines
        private void Sort()
        {
            int maxMinVal, i, j;
            Ants swap;

            for (i = 0; i < Cosmos.elitismPopulation; i++)
            {
                maxMinVal = i;
                for (j = i + 1; j < numAnts; j++)
                {
                    if (ants[j].FoodCollected < ants[maxMinVal].FoodCollected)
                        maxMinVal = j;
                }
                swap = ants[i];
                ants[i] = ants[maxMinVal];
                ants[maxMinVal] = swap;
                swap = null;
            }

            maxMinVal = 0;
            for (i = numAnts - 1; i < Cosmos.elitismPopulation; i--)
            {
                maxMinVal = i;
                for (j = i + 1; j < numAnts; j++)
                {
                    if (ants[j].FoodCollected > ants[maxMinVal].FoodCollected)
                        maxMinVal = j;
                }
                swap = ants[i];
                ants[i] = ants[maxMinVal];
                ants[maxMinVal] = swap;
                swap = null;
            }

            for (i = 0; i < Cosmos.elitismPopulation; i++)
            {
                maxMinVal = i;
                for (j = i + 1; j < Cosmos.elitismPopulation; j++)
                {
                    if (ants[j].DistanceCovered > ants[maxMinVal].DistanceCovered)
                        maxMinVal = j;
                }
                swap = ants[i];
                ants[i] = ants[maxMinVal];
                ants[maxMinVal] = swap;
                swap = null;
            }

            for (i = numAnts - Cosmos.elitismPopulation; i < numAnts; i++)
            {
                maxMinVal = i;
                for (j = i + 1; j < numAnts; j++)
                {
                    if (ants[j].DistanceCovered > ants[maxMinVal].DistanceCovered)
                        maxMinVal = j;
                }
                swap = ants[i];
                ants[i] = ants[maxMinVal];
                ants[maxMinVal] = swap;
                swap = null;
            }


        }
        //a genetic algo. selection process; you'll need to read an article to
        //understand it
        private int RouletteWheel(int totalFit)
        {
            int stopPoint = randomR.Next(totalFit);
            int fitnessSoFar = 0;

            if (landMines)
                switch (randomR.Next(2))
                {
                    case 0:
                    //return randomR.Next(ELITISMRATE);
                    //break;
                    case 1:
                        return randomR.Next(numAnts);
                    //break;
                    default:
                        break;
                }


            for (int i = 0; i < numAnts; i++)
            {
                fitnessSoFar += ants[i].FoodCollected;
                if (fitnessSoFar >= stopPoint)
                    return i;
            }
            return 0;

        }
        //make random changes to random locations
        private void Mutate(Array chromo)
        {
            for (int i = 0; i < chromo.GetUpperBound(0); i++)
            {
                if ((float)(randomR.NextDouble()) < Cosmos.mutationRate)
                {
                    chromo.SetValue((double)chromo.GetValue(i) + (((float)(randomR.NextDouble()) * 2 - 1) * Cosmos.maxPermutation), i);
                }
            }
        }
        //overlapping of DNA to create new
        private void Mate(ref Array first, ref Array second)
        {
            int cL = first.GetLength(0);
            int partL = randomR.Next(cL);
            Array newC1 = Array.CreateInstance(typeof(double), cL);
            Array newC2 = Array.CreateInstance(typeof(double), cL);
            for (int i = 0; i < partL; i++)
            {
                newC1.SetValue(first.GetValue(i), i);
            }
            for (int i = partL; i < cL; i++)
            {
                newC1.SetValue(second.GetValue(i), i);
            }
            for (int i = 0; i < partL; i++)
            {
                newC2.SetValue(second.GetValue(i), i);
            }
            for (int i = partL; i < cL; i++)
            {
                newC2.SetValue(first.GetValue(i), i);
            }
            first = newC1;
            second = newC2;
        }
    }
}

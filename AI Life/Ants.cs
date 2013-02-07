using System;
using System.Drawing;

namespace AI_Life
{
    //class for individual ant
    public class Ants
    {
        //---------------------------------------------------------//
        //              Special Thanks to Jan Krumsiek             //
        //---------------------------------------------------------//


        // a lot part of this and other code is self-explanatory;//
        // just interpret them as they are in simple english     //
        //=========================================================//
        double x, y;				//ant location
        double dir;			//its direction
        PointF myFood;

        // constants for org drawing
        public static int AntTopX = 0;
        public static int AntTopY = -14;
        public static int AntLeftX = -6;
        public static int AntLeftY = 4;
        public static int AntRightX = 6;
        public static int AntRightY = 4;

        //=========================================================//
        private Size clientSize;
        int indMyFood;			//its food
        int foodCollected = 0;
        int antNumber = 0;
        //=========================================================//
        public Network net;		//private network
        Cosmos myWorld;		//the world
        private PointF startPosition, finishPosition;   //used to find the distance traveled. so that we can make a choice
        //used in land mines case

        public Ants(Size ClientSize, Cosmos MyWorld, Array weights, int AntNumber)
        {
            clientSize = ClientSize;
            myWorld = MyWorld;
            x = Cosmos.randomR.Next(clientSize.Width);
            y = Cosmos.randomR.Next(clientSize.Height);
            dir = (float)(Cosmos.randomR.NextDouble());
            net = new Network(4, 1, 6, 2);  //initialize network and weights
            if (weights != null)
                this.net.Weights = weights;

            antNumber = AntNumber;
            startPosition = new PointF((float)x, (float)y);
            finishPosition = startPosition;
        }

        public void UpdatePosition()
        {
            double foodDist = 0;
            double leftTrack;
            double rightTrack;
            double curSpeed;

            myFood = myWorld.FindClosestFood(x, y, ref foodDist, ref indMyFood);
            double[] input = new double[4];
            input[0] = Math.Cos(dir);   //could be sin also; doesn't matter aslong as its between 0 and 1
            input[1] = Math.Cos(dir);
            input[2] = (myFood.X - x) / foodDist;   //let the ant know about nearest food
            input[3] = (myFood.Y - y) / foodDist;   //same
            double[] output = net.FeedData(input);  //feed input into network and get output
            leftTrack = output[0];
            rightTrack = output[1];
            dir += (rightTrack - leftTrack) * (Cosmos.maxForce / 100);   //find the direction
            curSpeed = (rightTrack + leftTrack) / 2;
            x += Math.Sin(dir) * Cosmos.maxSpeed * curSpeed / 10;       //max speed is just a twaking parameter; don't get confused by it
            y -= Math.Cos(dir) * Cosmos.maxSpeed * curSpeed / 10;       //try varying it in simulation
            finishPosition = new PointF((float)x + finishPosition.X, (float)y + finishPosition.Y);
            if (x < 0)
                x = clientSize.Width;
            if (x > clientSize.Width)
                x = 0;
            if (y < 0)
                y = clientSize.Height;
            if (y > clientSize.Height)
                y = 0;

            if ((myFood.X >= (x - Cosmos.foodTolerance)) &&
                (myFood.X <= (x + Cosmos.foodTolerance)) &&
                (myFood.Y >= (y - Cosmos.foodTolerance)) &&
                (myFood.Y <= (y + Cosmos.foodTolerance)))
            {
                foodCollected++;
                myWorld.NewFood(indMyFood);
            }
        }

        public void DrawToBuffer(Graphics g)
        {
            int tx, ty, lx, ly, rx, ry;
            tx = (int)(Ants.AntTopX * Math.Cos(dir) - Ants.AntTopY * Math.Sin(dir) + x);
            ty = (int)(Ants.AntTopY * Math.Cos(dir) + Ants.AntTopX * Math.Sin(dir) + y);
            lx = (int)(Ants.AntLeftX * Math.Cos(dir) - Ants.AntLeftY * Math.Sin(dir) + x);
            ly = (int)(Ants.AntLeftY * Math.Cos(dir) + Ants.AntLeftX * Math.Sin(dir) + y);
            rx = (int)(Ants.AntRightX * Math.Cos(dir) - Ants.AntRightY * Math.Sin(dir) + x);
            ry = (int)(Ants.AntRightY * Math.Cos(dir) + Ants.AntRightX * Math.Sin(dir) + y);
            g.FillPolygon(Cosmos.whiteBrush, new Point[] { new Point(tx, ty), new Point(lx, ly), new Point(rx, ry) });
            g.DrawString(foodCollected.ToString(), Cosmos.foodFont, Cosmos.foodCollectedB, (int)x + 5, (int)y + 8);
            g.DrawString(antNumber.ToString(), Cosmos.foodFont, Cosmos.antNumberB, (int)x + 5, (int)y - 20);
        }

        public int FoodCollected
        {
            get
            {
                return foodCollected;
            }
        }

        public PointF StartPosition
        {
            get
            {
                return startPosition;
            }
        }
        public PointF FinishPosition
        {
            get
            {
                return finishPosition;
            }
        }

        public double DistanceCovered
        {
            get
            {
                return Vector2.Length(Vector2.Subtract(new Vector2(startPosition), new Vector2((float)x, (float)y)));
            }
        }
    }
}

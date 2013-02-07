using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace AI_Life
{
    //SBC stands for steering behaviours controller; this class manages all the vehicles
    class SBC
    {
        public static Vector2 targetPosition;
        public static bool mirrored = false;
        private static Font drawFont10 = new Font(FontFamily.GenericSansSerif, 10);
        private static Font drawFont12 = new Font(FontFamily.GenericSansSerif, 12);
        private Vehicle[] cars;
        private Vehicle otherCar;
        private Graphics g;
        private SB SBs;
        private Size clientSize;
        public SBC(Graphics g, Size ClientSize, SB steeringBehaviour, int NumberOfVehicles)
        {
            cars = new Vehicle[NumberOfVehicles];
            SBs = steeringBehaviour;
            clientSize = ClientSize;
            this.g = g;
            InitializeCars();
        }

        private void DrawTitle()
        {
            g.DrawString("Current Position: " + cars[0].CurrentPosition.ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, clientSize.Height - 35));
            g.DrawString("Current Speed: " + Vector2.Length(cars[0].Velocity).ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, clientSize.Height - 20));
            g.DrawString("Total Vehicles: " + cars.Length.ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, clientSize.Height - 50));
            g.DrawString("Mass(Ins/Del): " + cars[0].Mass.ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, 10));
            g.DrawString("MaxSpeed(Home/End): " + cars[0].MaxSpeed.ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, 25));
            g.DrawString("MaxForce(PUp/PDn): " + cars[0].MaxForce.ToString(), SBC.drawFont10, Brushes.GreenYellow, new PointF(10, 40));
            switch (cars[0].SteeringBehaviour)
            {
                case SB.None:
                    g.DrawString("None", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Seek:
                    g.DrawString("Seek", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Flee:
                    g.DrawString("FOV(Y/H): " + Vehicle.FOV.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 55));
                    g.DrawString("Flee", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Arrive:
                    g.DrawString("Arrive Radius(Q/A): " + Vehicle.ArriveRadius.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 55));
                    g.DrawString("Arrive", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Pursuit:
                    g.DrawString("Pursuit", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Evade:
                    g.DrawString("Evade", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Wander:
                    g.DrawString("Wander Distance(W/S): " + Vehicle.WanderDistance.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 55));
                    g.DrawString("Wander Radius(E/D): " + Vehicle.WanderRadius.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 70));
                    g.DrawString("Wander Jitter(R/F): " + Vehicle.WanderJitter.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 85));
                    g.DrawString("Wander", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.PathFollowing:
                    g.DrawString("Path Following", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Cohesion:
                    g.DrawString("Cohesion Radius(T/G): " + Vehicle.CohesionRadius.ToString(), SBC.drawFont10, Brushes.Yellow, new PointF(10, 55));
                    g.DrawString("Cohesion", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Alignment:
                    g.DrawString("Alignment", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.Separation:
                    g.DrawString("Separation", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.CF:
                    g.DrawString("CF", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.CA:
                    g.DrawString("CA", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.CAS:
                    g.DrawString("CAS", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.CS:
                    g.DrawString("CS", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.FCAS:
                    g.DrawString("FCAS", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                case SB.FCS:
                    g.DrawString("FCS", SBC.drawFont12, Brushes.Orange, new PointF(clientSize.Width - 140, 23));
                    break;
                default:
                    break;
            }

        }
        //initialize all cars including "otherCar"
        private void InitializeCars()
        {
            otherCar = new Vehicle(g, clientSize, SB.Wander, -2);
            otherCar.Mass = 2;
            otherCar.MaxSpeed = 4;
            otherCar.CreateObstacles();
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i] = new Vehicle(g, clientSize, SBs, i + 1);
                if (!mainForm.identicalVehicles)
                    Thread.Sleep(50);
            }
            Vehicle.SetCarsData(ref cars);
        }

        public Vehicle FirstVehicle
        {
            get
            {
                return cars[0];
            }
        }

        public void Step()
        {
            DrawTitle();
            SetSameBehaviour();
            for (int i = 0; i < cars.Length; i++)
            {
                cars[i].Step();
            }
            if (mainForm.steeringBehaviour == SB.CF || mainForm.steeringBehaviour == SB.FCS || mainForm.steeringBehaviour == SB.FCAS)
            {
                otherCar.Step();
                SBC.targetPosition = otherCar.CurrentPosition;
            }
        }
        //when behaviour changes, should it affect all vehicles?
        private void SetSameBehaviour()
        {
            if (mainForm.identicalBehaviour)
            {
                if (mainForm.behaviourChanged)
                {
                    for (int i = 0; i < cars.Length; i++)
                    {
                        cars[i].SteeringBehaviour = mainForm.steeringBehaviour;
                    }
                    mainForm.behaviourChanged = false;
                }
            }
        }
    }
}

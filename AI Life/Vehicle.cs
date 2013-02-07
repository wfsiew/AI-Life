using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AI_Life
{

    public struct Obstacles
    {
        public PointF Location;
        public int Size;
        public Obstacles(PointF location, int size)
        {
            Location = location;
            Size = size;
        }
    }


    enum SpecialPaths
    {
        AI
    }

    enum SB
    {
        None,
        Seek,
        Flee,
        Arrive,
        Pursuit,
        Evade,
        Wander,
        PathFollowing,
        Cohesion,
        Alignment,
        Separation,
        CF,
        FCAS,
        FCS,
        CS,
        CA,
        CAS
    }


    //represents a single vehicle
    class Vehicle
    {
        private static Vehicle[] allCars;

        //=========================================================//
        //general variables
        public static bool enableSpecialPath = false;
        private bool targetChanged;
        private float mass;
        private int max_speed, max_force, vehicleNo;
        private Vector2 currentPosition, velocity, acceleration, heading, steerForce, targetPosition;
        private Brush brushBack, brushWhite, brushCar;
        private Pen whitePen;
        private Graphics g;
        private static Random random;
        private Size clientSize;
        private SB SB;
        public static Obstacles[] obstacles = new Obstacles[10];    //not used
        //=========================================================//

        //=========================================================//
        //wander specific variables
        private Vector2 wanderTarget;
        public static float WanderRadius = 40, WanderDistance = 80;
        public static int WanderJitter = 10;
        //=========================================================//

        //=========================================================//
        //arrive specific variables
        public static int ArriveRadius = 100;
        //=========================================================//

        //=========================================================//
        //pathfollowing specific varibles
        Point[] specialPathPoints, pathPoints, sP1, sP2;
        int currentPathPoint, maxSpecialPathPoints, maxPathPoints;

        //=========================================================//
        //cohesion specific variables
        public static int CohesionRadius = 100;
        //=========================================================//

        //=========================================================//
        //flee specific variables
        public static int FOV = 100;    //field of view
        //=========================================================//

        public static bool weightedSum = true;
        //the constructor
        public Vehicle(Graphics GraphicsObject, Size ClientSize, SB SteeringBehavior, int VehicleNumber)
        {
            //=========================================================//
            //general initialization
            g = GraphicsObject;
            clientSize = ClientSize;
            random = new Random();
            mass = random.Next(20, 80);
            max_force = 20;
            vehicleNo = VehicleNumber;
            max_speed = 10;
            if (mainForm.identicalVehicles)
            {
                velocity = new Vector2(5, 5);
            }
            else
            {
                velocity = new Vector2(random.Next(0, 10), random.Next(0, 10));
            }
            acceleration = new Vector2(0, 0);
            heading = Vector2.Normalize(velocity);
            currentPosition = new Vector2(random.Next(clientSize.Width), random.Next(clientSize.Height));
            steerForce = new Vector2(0);
            targetPosition = SBC.targetPosition;
            brushBack = Brushes.Black;
            brushWhite = Brushes.White;
            brushCar = Brushes.Blue;
            whitePen = Pens.White;
            targetChanged = false;
            SB = SteeringBehavior;
            //=========================================================//

            //=========================================================//
            //wander specific variables
            wanderTarget = new Vector2(0);
            //=========================================================//

            //=========================================================//

            //=========================================================//
            //pathfollowing s
            maxSpecialPathPoints = 40;
            maxPathPoints = 10;
            currentPathPoint = 0;
            specialPathPoints = new Point[maxSpecialPathPoints];
            pathPoints = new Point[maxPathPoints];
            sP1 = new Point[22];
            sP2 = new Point[18];
            NewPath();

        }

        //=========================================================//




        #region Properties
        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public Vector2 CurrentPosition
        {
            get
            {
                return currentPosition;
            }
        }

        public SB SteeringBehaviour
        {
            get { return SB; }
            set { SB = value; }
        }

        public bool TargetChanged
        {
            get { return targetChanged; }
            set { targetChanged = value; }
        }

        public int MaxPathPoints
        {
            get { return maxSpecialPathPoints; }
            set { maxSpecialPathPoints = value; }
        }

        public Vector2 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }

        public int MaxForce
        {
            get { return max_force; }
            set { max_force = value; }
        }

        public int MaxSpeed
        {
            get { return max_speed; }
            set { max_speed = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        } 
        #endregion

        //=========================================================//


        private void DrawToBuffer()
        {
            PointF top = new PointF(heading.X * 14 + currentPosition.X, heading.Y * 14 + currentPosition.Y);
            PointF left = new PointF(heading.Y * (-5) + currentPosition.X, heading.X * 5 + currentPosition.Y);
            PointF right = new PointF(heading.Y * 5 + currentPosition.X, heading.X * (-5) + currentPosition.Y);
            switch (random.Next(1, 4))
            {
                case 0:
                    brushCar = Brushes.Black;
                    break;
                case 1:
                    brushCar = Brushes.Red;
                    break;
                case 2:
                    brushCar = Brushes.White;
                    break;
                case 3:
                    brushCar = Brushes.Blue;
                    break;
                case 4:
                    brushCar = Brushes.LightGreen;
                    break;
                default:
                    break;

            }
            switch (SB)
            {
                case SB.None:
                    break;
                case SB.Seek:
                    break;
                case SB.Flee:
                    break;
                case SB.Arrive:
                    break;
                case SB.Pursuit:
                    break;
                case SB.Evade:
                    break;
                case SB.Wander:
                    break;
                case SB.PathFollowing:
                    if (!enableSpecialPath)
                    {
                        g.DrawLines(Pens.White, pathPoints);
                        g.DrawLine(Pens.White, pathPoints[0], pathPoints[maxPathPoints - 1]);
                        g.FillRectangle(Brushes.Red, new RectangleF(new PointF(pathPoints[0].X - 2, pathPoints[0].Y - 2), new SizeF(4f, 4f)));
                    }
                    else
                    {
                        g.DrawLines(Pens.White, sP1);
                        g.DrawLines(Pens.White, sP2);
                        //g.DrawLine(Pens.White, pathPoints[0], pathPoints[maxPathPoints - 1]);
                        g.FillRectangle(Brushes.Red, new RectangleF(new PointF(sP1[0].X - 2, sP1[0].Y - 2), new SizeF(4f, 4f)));
                    }


                    break;
                default:
                    break;
            }
            g.DrawLine(whitePen, top, left);
            g.DrawLine(whitePen, top, right);
            g.DrawLine(whitePen, right, left);
            g.FillPolygon(brushCar, new PointF[] { top, left, right });
            g.FillRectangle(Brushes.White, new RectangleF(new PointF(SBC.targetPosition.X - 2, SBC.targetPosition.Y - 2), new SizeF(4f, 4f)));
        }
        //calculate force and update position
        private void UpdatePosition()
        {
            heading = Vector2.Normalize(velocity);
            switch (SB)
            {
                case SB.None:
                    steerForce = SteeringBehaviours.None();
                    break;
                case SB.Seek:
                    steerForce = SteeringBehaviours.Seek(ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.Flee:
                    steerForce = SteeringBehaviours.Flee(g, ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo);
                    break;
                case SB.Arrive:
                    steerForce = SteeringBehaviours.Arrive(g, ref SBC.targetPosition, ref currentPosition, ref velocity, ArriveRadius, max_speed, vehicleNo);
                    break;
                case SB.Pursuit:
                    break;
                case SB.Evade:
                    break;
                case SB.Wander:
                    steerForce = SteeringBehaviours.Wander(g, ref wanderTarget, ref currentPosition, ref velocity, ref heading, Vehicle.WanderRadius, Vehicle.WanderDistance, Vehicle.WanderJitter);
                    break;
                case SB.PathFollowing:
                    if (!enableSpecialPath)
                    {
                        steerForce = SteeringBehaviours.PathFollowing(ref SBC.targetPosition, ref currentPosition, ref velocity, ref pathPoints, ref currentPathPoint, maxPathPoints, max_speed);
                    }
                    else
                        steerForce = SteeringBehaviours.PathFollowing(ref SBC.targetPosition, ref currentPosition, ref velocity, ref specialPathPoints, ref currentPathPoint, maxSpecialPathPoints, max_speed);
                    break;
                case SB.Cohesion:
                    steerForce = SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius);
                    break;
                case SB.Alignment:
                    steerForce = SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.Separation:
                    steerForce = SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed);
                    break;
                case SB.CF:
                    steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(g, ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo));
                    break;
                case SB.CA:
                    if (weightedSum)
                    {
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .1f);
                    }
                    else
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed));
                    }
                    break;
                case SB.CAS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed)));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .3f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .5f);
                    }

                    break;
                case SB.CS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .8f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                    }
                    break;
                case SB.FCAS:
                    if (weightedSum)
                    {
                        steerForce = Vector2.Add(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(g, ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo))));
                    }
                    else
                    {
                        steerForce = Vector2.Multiply(SteeringBehaviours.Flee(g, ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo), .4f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), .3f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), .2f);
                        steerForce += Vector2.Multiply(SteeringBehaviours.Alignment(ref allCars, this, ref currentPosition, ref velocity, max_speed), .5f);
                    }
                    break;
                case SB.FCS:
                    steerForce = Vector2.Add(SteeringBehaviours.Separation(ref allCars, this, ref currentPosition, ref velocity, max_speed), Vector2.Add(SteeringBehaviours.Cohesion(ref allCars, this, currentPosition, velocity, max_speed, CohesionRadius), SteeringBehaviours.Flee(g, ref SBC.targetPosition, ref currentPosition, ref velocity, max_speed, FOV, vehicleNo)));
                    break;
                default:
                    break;
            }
            steerForce = Vector2.Truncate(steerForce, max_force);
            acceleration = steerForce / mass;
            velocity = Vector2.Truncate(velocity + acceleration, max_speed);
            currentPosition = Vector2.Add(velocity, currentPosition);

            if (!SBC.mirrored)
            {
                if (currentPosition.X > clientSize.Width)
                    currentPosition.X = 0;
                if (currentPosition.Y > clientSize.Height)
                    currentPosition.Y = 0;
                if (currentPosition.X < 0)
                    currentPosition.X = clientSize.Width;
                if (currentPosition.Y < 0)
                    currentPosition.Y = clientSize.Height;
            }
            else
            {
                if (currentPosition.X > clientSize.Width)
                    velocity.X *= -1;
                if (currentPosition.Y > clientSize.Height)
                    velocity.Y *= -1;
                if (currentPosition.X < 0)
                    velocity.X *= -1; ;
                if (currentPosition.Y < 0)
                    velocity.Y *= -1; ;
            }
            targetChanged = false;
        }

        //=========================================================//

        public void NewPath()
        {
            for (int i = 0, j = 0, k = 0; i < 10; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Point(100 + j, 400 - k);
            }
            for (int i = 10, j = 0, k = 0; i < 20; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Point(specialPathPoints[9].X + j, specialPathPoints[9].Y + k);
            }
            specialPathPoints[20] = new Point(specialPathPoints[16].X, specialPathPoints[16].Y);
            specialPathPoints[21] = new Point(specialPathPoints[3].X, specialPathPoints[3].Y);
            for (int i = 22, j = 0, k = 0; i < 26; i++, j += 40, k += 30)
            {
                specialPathPoints[i] = new Point(450 + j, specialPathPoints[9].Y);
            }
            for (int i = 26, j = 0, k = 0; i < 36; i++, j += 10, k += 30)
            {
                specialPathPoints[i] = new Point((specialPathPoints[25].X + specialPathPoints[22].X) / 2, specialPathPoints[9].Y + k);
            }
            for (int i = 36, j = 0, k = 0; i < maxSpecialPathPoints; i++, j += 40, k += 30)
            {
                specialPathPoints[i] = new Point(450 + j, specialPathPoints[0].Y);
            }

            for (int i = 0; i < 22; i++)
            {
                sP1[i] = specialPathPoints[i];
            }
            for (int i = 0, j = 22; i < 18; i++, j++)
            {
                sP2[i] = specialPathPoints[j];
            }
            for (int i = 0; i < maxPathPoints; i++)
            {
                pathPoints[i] = new Point(random.Next(100, clientSize.Width - 100), random.Next(100, clientSize.Height - 100));
            }
        }

        public void Step()
        {
            UpdatePosition();
            DrawToBuffer();
        }

        public static void SetCarsData(ref Vehicle[] cars)
        {
            allCars = cars;
        }

        public void CreateObstacles()
        {
            for (int i = 0; i < Vehicle.obstacles.Length; i++)
            {
                obstacles[i].Location = new PointF((float)random.Next(clientSize.Width - 100), (float)random.Next(clientSize.Height - 100));
                obstacles[i].Size = random.Next(20, 100);
            }
        }
    }
}

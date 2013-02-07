using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AI_Life
{
    enum CurrentSimulation
    {
        SteeringBehaviours,
        EString,
        Ants,
    }


    //this class deals with the main windows form and manages top-level objects
    //of other classes
    //some of the class components are to help EString work
    public partial class mainForm : Form
    {
        //=========================================================//
        public static bool behaviourChanged = false, identicalBehaviour = true, eStringRunning = true;
        public static bool leaveTrail = false;
        public static bool identicalVehicles = false;
        internal static SB steeringBehaviour = SB.None;

        //=========================================================//

        private Point[] axisPoints = new Point[3];
        private Point newPoint, oldPoint = new Point(80, 450 - 200);
        private SBC sBC;
        private Cosmos world;
        private AboutBox aboutBox;
        private Pen arrowPen = new Pen(Brushes.Red, 3), boldPen = new Pen(Brushes.PowderBlue, 2);
        private CurrentSimulation CS;
        private BufferedGraphicsContext context;        //buffered display; check msdn for this class
        private BufferedGraphics grafx;
        private Random random = new Random();
        private bool drawingSurfaceInitialized = false;
        Graphics g;
        //=========================================================//

        private PointF eStringGraph = new PointF(100 + EString.generationCount, 500 - EString.errorCount);
        public mainForm()
        {
            InitializeComponent();
            arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            axisPoints[1] = new Point(80, 450);
            axisPoints[0] = new Point(80, 150);
            axisPoints[2] = new Point(380, 450);
            CS = CurrentSimulation.SteeringBehaviours;
            context = new BufferedGraphicsContext();
        }

        
        //main title
        private void DrawTitle()
        {
            switch (CS)
            {
                case CurrentSimulation.SteeringBehaviours:
                    g.DrawString("Steering Behaviours", this.Font, Brushes.White, new PointF(mainPanel.Width - 140, 10));
                    break;
                case CurrentSimulation.EString:
                    //g.DrawString("E Strings", this.Font, Brushes.White, new PointF(mainPanel.Width / 2, 10));
                    break;
                case CurrentSimulation.Ants:
                    //g.DrawString("Sheeps", this.Font, Brushes.White, new PointF(mainPanel.Width / 2, 10));
                    break;
                default:
                    break;
            }
        }

        private void ClearScreen()
        {
            g.FillRectangle(Brushes.Black, mainPanel.ClientRectangle);
        }

        private void Start()
        {
            if (g != null)
            {
                timer1.Enabled = true;
                if (CS == CurrentSimulation.EString)
                {
                    listView1.Items.Clear();
                    if (textBoxTargetString.Text != "")
                    {
                        {
                            EString.target = textBoxTargetString.Text;
                            EString.Execute(g, listView1);
                        }
                    }
                }
                timer1.Enabled = true;
            }
        }
        //to accommodate different resolutions
        private void InitializeDrawingSurface()
        {
            listView1.Visible = false;
            context.MaximumBuffer = mainPanel.ClientSize;
            grafx = context.Allocate(mainPanel.CreateGraphics(), new Rectangle(0, 0, mainPanel.ClientSize.Width, mainPanel.ClientSize.Height));
            g = grafx.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            drawingSurfaceInitialized = true;
        }

        private void DestroyAll()
        {
            leaveTrailToolStripMenuItem.Checked = false;
            leaveTrail = false;
            timer1.Enabled = false;
            Cosmos.isRunning = false;
            eStringRunning = false;
            listView1.Visible = false;
            sBC = null;
            world = null;
        }

        #region Events
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!leaveTrail)
            {
                ClearScreen();
            }
            switch (CS)
            {

                case CurrentSimulation.SteeringBehaviours:
                    if (sBC != null)
                        sBC.Step();
                    break;
                case CurrentSimulation.EString:
                    newPoint = new Point(80 + (EString.generationCount * 2), 450 - (EString.errorCount * 2));
                    g.DrawLine(boldPen, oldPoint, newPoint);
                    oldPoint = newPoint;
                    g.DrawString("E Strings", new Font(FontFamily.GenericSansSerif, 14), Brushes.White, new PointF(20, 20));
                    g.DrawString("Target String: " + EString.target, new Font(FontFamily.GenericSansSerif, 20), Brushes.PaleGoldenrod, new PointF(20, 50));
                    g.DrawString("Error Graph", new Font(FontFamily.GenericSansSerif, 20), Brushes.Yellow, new PointF(180, 180));
                    g.DrawLine(arrowPen, axisPoints[1], axisPoints[0]);
                    g.DrawLine(arrowPen, axisPoints[1], axisPoints[2]);
                    g.DrawString("Y", this.Font, Brushes.White, new PointF(axisPoints[0].X - 30, axisPoints[0].Y));
                    g.DrawString("X", this.Font, Brushes.White, new PointF(axisPoints[2].X - 10, axisPoints[2].Y + 25));
                    g.DrawString("0", this.Font, Brushes.White, new PointF(axisPoints[1].X - 10, axisPoints[1].Y + 10));
                    break;
                case CurrentSimulation.Ants:
                    if (world != null)
                    {
                        world.Step();
                    }
                    break;
                default:
                    break;
            }
            DrawTitle();
            grafx.Render(Graphics.FromHwnd(mainPanel.Handle));
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eStringRunning = false;
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cosmos.isRunning = false;
            eStringRunning = false;
            timer1.Enabled = !timer1.Enabled;
        }

        private void initializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!drawingSurfaceInitialized)
                InitializeDrawingSurface();
            DestroyAll();
            listView1.Visible = false;
            CS = CurrentSimulation.SteeringBehaviours;
            sBC = new SBC(g, mainPanel.ClientSize, steeringBehaviour, int.Parse(numberOfCars.Text));
            Start();
        }

        private void mainPanel_MouseClick(object sender, MouseEventArgs e)
        {
            SBC.targetPosition = new Vector2(e.X, e.Y);
        }

        private void initializeMainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.Visible = false;
            context.MaximumBuffer = mainPanel.ClientSize;
            grafx = context.Allocate(mainPanel.CreateGraphics(), new Rectangle(0, 0, mainPanel.ClientSize.Width, mainPanel.ClientSize.Height));
            g = grafx.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Seek;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Seek;
        }

        private void fleeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Flee;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Flee;
        }

        private void arriveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Arrive;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Arrive;
        }

        private void pursuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Pursuit;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Pursuit;
        }

        private void evadeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Evade;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Evade;
        }

        private void wanderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Wander;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Wander;
        }

        private void pathFollowingToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.PathFollowing;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.PathFollowing;
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.None;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.None;
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (CS)
            {
                case CurrentSimulation.SteeringBehaviours:
                    if (sBC != null)
                    {
                        if (e.KeyCode == Keys.Insert)
                            sBC.FirstVehicle.Mass++;
                        if (e.KeyCode == Keys.Delete)
                            sBC.FirstVehicle.Mass--;
                        if (e.KeyCode == Keys.Home)
                            sBC.FirstVehicle.MaxSpeed++;
                        if (e.KeyCode == Keys.End)
                            sBC.FirstVehicle.MaxSpeed--;
                        if (e.KeyCode == Keys.PageUp)
                            sBC.FirstVehicle.MaxForce++;
                        if (e.KeyCode == Keys.PageDown)
                            sBC.FirstVehicle.MaxForce--;
                        if (e.KeyCode == Keys.Q)
                            Vehicle.ArriveRadius++;
                        if (e.KeyCode == Keys.A)
                            Vehicle.ArriveRadius--;
                        if (e.KeyCode == Keys.W)
                            Vehicle.WanderDistance++;
                        if (e.KeyCode == Keys.S)
                            Vehicle.WanderDistance--;
                        if (e.KeyCode == Keys.E)
                            Vehicle.WanderRadius++;
                        if (e.KeyCode == Keys.D)
                            Vehicle.WanderRadius--;
                        if (e.KeyCode == Keys.R)
                            Vehicle.WanderJitter++;
                        if (e.KeyCode == Keys.F)
                            Vehicle.WanderJitter--;
                        if (e.KeyCode == Keys.T)
                            Vehicle.CohesionRadius++;
                        if (e.KeyCode == Keys.G)
                            Vehicle.CohesionRadius--;
                        if (e.KeyCode == Keys.Y)
                            Vehicle.FOV++;
                        if (e.KeyCode == Keys.H)
                            Vehicle.FOV--;
                    }
                    break;
                case CurrentSimulation.EString:
                    break;
                case CurrentSimulation.Ants:
                    if (world != null)
                    {
                        if (e.KeyCode == Keys.Insert)
                            Cosmos.elitismRate += .1f;
                        if (e.KeyCode == Keys.Delete)
                            Cosmos.elitismRate -= .1f;
                        if (e.KeyCode == Keys.Home)
                            Cosmos.mutationRate += .1;
                        if (e.KeyCode == Keys.End)
                            Cosmos.mutationRate -= .1;
                        if (e.KeyCode == Keys.PageUp)
                            Cosmos.foodTolerance += 1;
                        if (e.KeyCode == Keys.PageDown)
                            Cosmos.foodTolerance--;
                        if (e.KeyCode == Keys.Q)
                            Cosmos.maxForce++;
                        if (e.KeyCode == Keys.A)
                            Cosmos.maxForce--;
                        if (e.KeyCode == Keys.W)
                            Cosmos.maxSpeed++;
                        if (e.KeyCode == Keys.S)
                            Cosmos.maxSpeed--;
                    }
                    break;
                default:
                    break;
            }

        }

        private void leaveTrailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leaveTrail = !leaveTrail;
        }

        private void mirroredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SBC.mirrored = !SBC.mirrored;
        }

        private void identicalBehaviourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            identicalBehaviour = !identicalBehaviour;
        }

        private void cohesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Cohesion;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Cohesion;
        }

        private void alignmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Alignment;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Alignment;
        }

        private void cFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.CF;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.CF;
        }

        private void initializeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!drawingSurfaceInitialized)
                InitializeDrawingSurface();
            DestroyAll();
            timer1.Enabled = false;
            oldPoint = new Point(80, 450 - 200);
            newPoint = new Point(80, 450 - 200);
            ClearScreen();
            grafx.Render(Graphics.FromHwnd(mainPanel.Handle));
            leaveTrail = true;
            EString.elitism = float.Parse(textBoxElitismRate.Text);
            EString.maxIteration = uint.Parse(textBoxMaxIteration.Text);
            EString.mutationRate = float.Parse(textBoxMutationRate.Text);
            EString.popSize = int.Parse(textBoxPopulationSize.Text);
            CS = CurrentSimulation.EString;
            listView1.Visible = true;
            eStringRunning = true;
            listView1.Items.Clear();
            Start();

        }

        private void initializeToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!drawingSurfaceInitialized)
                InitializeDrawingSurface();
            DestroyAll();
            listView1.Items.Clear();
            DestroyAll();
            listView1.Visible = true;
            CS = CurrentSimulation.Ants;
            world = new Cosmos(int.Parse(textBoxFood.Text), int.Parse(textBoxNo.Text), mainPanel.ClientSize, g, listView1, mainPanel, grafx);
            world.Step();
        }

        private void fastModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cosmos.fastMode = !Cosmos.fastMode;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cosmos.isRunning = false;
            eStringRunning = false;
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cosmos.showDetails = !Cosmos.showDetails;
        }

        private void destroyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DestroyAll();
        }

        private void separationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.Separation;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.Separation;
        }

        private void elitismToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cosmos.useElitism = !Cosmos.useElitism;
        }

        private void fCASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.FCAS;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.FCAS;
        }

        private void cASToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.CAS;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.CAS;
        }

        private void fCSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.FCS;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.FCS;
        }

        private void cSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.CS;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.CS;
        }

        private void cAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            behaviourChanged = true;
            steeringBehaviour = SB.CA;
            if (sBC != null)
                sBC.FirstVehicle.SteeringBehaviour = SB.CA;
        }

        private void weightedSumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vehicle.weightedSum = !Vehicle.weightedSum;
        }

        private void landMinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cosmos.landMines = !Cosmos.landMines;
        }

        private void identicalVehiclesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.identicalVehicles = !mainForm.identicalVehicles;
        }

        private void sigmoidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Network.tF = TransferFunctions.LogSigmoid;
        }

        private void hardLimitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Network.tF = TransferFunctions.HardLimit;
        }

        private void saturatingLinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Network.tF = TransferFunctions.SaturatingLinear;
        }

        private void positiveLinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Network.tF = TransferFunctions.PositiveLinear;
        }

        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sBC != null)
                sBC.FirstVehicle.NewPath();
        }

        private void specialPathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vehicle.enableSpecialPath = !Vehicle.enableSpecialPath;
        } 
        #endregion
    }
}
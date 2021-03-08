using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DotFuzzy;

namespace fuzzylogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int count = 0;
        Car car;
        List<double> coneListx = new List<double>();
        List<double> coneListy = new List<double>();
        Boolean animationStart = false;
        MembershipFunctionCollection angle,distance,adjangle ;
        LinguisticVariable myAngle,myDistance, myAdjAngle;
        FuzzyRuleCollection myRules;
        FuzzyEngine fe;
        public MainWindow()
        {
            InitializeComponent();
            setMembers();
            setRules();
            car = new Car();
        }
        public delegate void CheckPosition();

        private void btnAnimateClick(object sender, EventArgs e)
        {

            
            car.Initialize(Canvas.GetRight(sprCar), Canvas.GetTop(sprCar), 10);

            AnimateCar();

            if (!animationStart)
            {
               // DoubleAnimation sprite1 = new DoubleAnimation(Canvas.GetTop(sprPerson5) + 150, TimeSpan.FromSeconds(5));
               // sprite1.AutoReverse = true;
                animationStart = true;
              //  sprite1.Completed += new EventHandler(Animation_Completed);

               // sprPerson5.BeginAnimation(Canvas.TopProperty, sprite1);
            }
        }

        void Animation_Completed(object sender, EventArgs e)
        {
           // DoubleAnimation sprite1 = new DoubleAnimation(Canvas.GetTop(sprPerson5) + 150, TimeSpan.FromSeconds(5));
           // sprite1.AutoReverse = true;
            animationStart = true;
         //   sprite1.Completed += Animation_Completed;

           // sprPerson5.BeginAnimation(Canvas.TopProperty, sprite1);

           // DoubleAnimation sprite2 = new DoubleAnimation(Canvas.GetRight(sprPerson4) - 150, TimeSpan.FromSeconds(5));
          //  sprite2.AutoReverse = true;
          ///  sprPerson4.BeginAnimation(Canvas.RightProperty, sprite2);
        }

        private void AnimateCar()
        {
            UpdateStatistics();
            Double temp = car.angle;
            car.PointTowardsDestination();

            CheckForCollision();

            SmoothCarHeading(temp);

            sprCar.RenderTransform = new RotateTransform(car.angle, sprCar.ActualWidth / 2, sprCar.ActualHeight / 2);

            car.MoveInDirection(car.angle);

            //Final Direction should be determined before this point
           
            DoubleAnimation sprCarX = new DoubleAnimation(car.xPos, TimeSpan.FromMilliseconds(car.xDuration));
            DoubleAnimation sprCarY = new DoubleAnimation(car.yPos, TimeSpan.FromMilliseconds(car.yDuration));
           // Console.WriteLine("DSADA");

            if (!DestinationReached())
            {
                sprCarY.Completed += btnAnimateClick;
            }

            sprCar.BeginAnimation(Canvas.RightProperty, sprCarX);
            sprCar.BeginAnimation(Canvas.TopProperty, sprCarY);
        }

        private void SmoothCarHeading(Double prevAngle)
        {
            Double newHeading = car.angle;

            if (newHeading - prevAngle > 4)
            {
                if (newHeading - prevAngle > 180) { car.angle = prevAngle - 5; } else { car.angle = prevAngle + 4; }
            }
            else if (car.angle - prevAngle < -4)
            {
                if (newHeading - prevAngle < -180) { car.angle = prevAngle + 5; } else { car.angle = prevAngle - 4; }
            }

            if (car.angle < 0) { car.angle += 360; }
            else if (car.angle > 360) { car.angle -= 360; }
        }
        public Double newDeffuzify(double obsAngle, double obstacleDist)
        {
        
            obsAngle = Math.Abs(obsAngle);
            myAngle.InputValue = obsAngle;
           
            myDistance.InputValue = obstacleDist;
            
            double w1 = myAngle.Fuzzify("SMALL") * myDistance.Fuzzify("NEAR");
            //Console.WriteLine(w1+"w1");
            double w2 = myAngle.Fuzzify("SMALL") * myDistance.Fuzzify("FAR");
           // Console.WriteLine(w2 + "w2");
            double w3 = myAngle.Fuzzify("SMALL") * myDistance.Fuzzify("VERYFAR");
           // Console.WriteLine(w3 + "w3");
            double w4 = myAngle.Fuzzify("MEDIUM") * myDistance.Fuzzify("NEAR");
            //Console.WriteLine(w4 + "w4");
            double w5 = myAngle.Fuzzify("MEDIUM") * myDistance.Fuzzify("FAR");
           // Console.WriteLine(w5 + "w5");
            double w6 = myAngle.Fuzzify("MEDIUM") * myDistance.Fuzzify("VERYFAR");
           // Console.WriteLine(w6 + "w6");
            double w7 = myAngle.Fuzzify("LARGE") * myDistance.Fuzzify("NEAR");
           // Console.WriteLine(w7 + "w7");
            double w8 = myAngle.Fuzzify("LARGE") * myDistance.Fuzzify("FAR");
           // Console.WriteLine(w8 + "w8");
            double w9 = myAngle.Fuzzify("LARGE") * myDistance.Fuzzify("VERYFAR");
           // Console.WriteLine(w9 + "w9");

            Double temp;
            temp = (w1 * 90) + (w2 * 55) + (w3 * 35) +
                    (w4 * 55) + (w5 * 35) + (w6 * 20) +
                    (w7 * 35) + (w8 * 20) + (w9 * 5);

            if ((w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8 + w9) == 0)
            {
                return 0;
            }
            else
            {
                return temp / (w1 + w2 + w3 + w4 + w5 + w6 + w7 + w8 + w9);
            }
        }
        public void setMembers()
        {
            angle = new MembershipFunctionCollection();
            angle.Add(new MembershipFunction("SMALL", -1.0, 0.0, 45.0, 60.0));
            angle.Add(new MembershipFunction("MEDIUM", 55, 85, 95, 105));
            angle.Add(new MembershipFunction("LARGE", 75, 110, 115, 180));
            myAngle = new LinguisticVariable("ANGLE", angle);

            distance = new MembershipFunctionCollection();
            distance.Add(new MembershipFunction("NEAR", -1, 12, 25, 40));
            distance.Add(new MembershipFunction("FAR", 30, 55, 65, 75));
            distance.Add(new MembershipFunction("VERYFAR", 70, 90, 100, 150));
            myDistance = new LinguisticVariable("DISTANCE", distance);

            adjangle = new MembershipFunctionCollection();
            adjangle.Add(new MembershipFunction("SS", 0, 0, 2.5, 5));
            adjangle.Add(new MembershipFunction("LS", 5, 10, 10, 20));
            adjangle.Add(new MembershipFunction("S", 10, 20, 25, 35));
            adjangle.Add(new MembershipFunction("VS", 25, 35, 45, 55));
            adjangle.Add(new MembershipFunction("MS", 45, 55, 75, 90));
            
            
            
            
            myAdjAngle = new LinguisticVariable("ADJANGLE", adjangle);


        }

        public void setRules()
        {
            myRules = new FuzzyRuleCollection();
            myRules.Add(new FuzzyRule("IF (ANGLE IS SMALL) AND (DISTANCE IS NEAR) THEN ADJANGLE IS MS"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS SMALL) AND (DISTANCE IS FAR) THEN ADJANGLE IS VS"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS SMALL) AND (DISTANCE IS VERYFAR) THEN ADJANGLE IS S"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS MEDIUM) AND (DISTANCE IS NEAR) THEN ADJANGLE IS VS"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS MEDIUM) AND (DISTANCE IS FAR) THEN ADJANGLE IS S"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS MEDIUM) AND (DISTANCE IS VERYFAR) THEN ADJANGLE IS LS"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS LARGE) AND (DISTANCE IS NEAR) THEN ADJANGLE IS S"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS LARGE) AND (DISTANCE IS FAR) THEN ADJANGLE IS LS"));
            myRules.Add(new FuzzyRule("IF (ANGLE IS LARGE) AND (DISTANCE IS VERYFAR) THEN ADJANGLE IS SS"));
        }

        public void setFuzzyEngine()
        {
            fe = new FuzzyEngine();
            fe.LinguisticVariableCollection.Add(myAngle);
            fe.LinguisticVariableCollection.Add(myDistance);
            fe.LinguisticVariableCollection.Add(myAdjAngle);
            fe.FuzzyRuleCollection = myRules;
        }
        private void adjustAngle(Double pAngle, Double obstacleDist)
        {
            
            lblStatus.Content = "Obstacle angle: " + pAngle.ToString() + "Dist: " + obstacleDist.ToString();

            Double adjAngle = newDeffuzify(pAngle,obstacleDist);

            if (pAngle > 0) { adjAngle *= -1; }

            if (car.angle + adjAngle > 360) 
            {
               Console.WriteLine("Greater than 360");
                car.angle = (car.angle + adjAngle) - 360; 
            }
            else if (car.angle + adjAngle < 0) 
            {
                Console.WriteLine("Lesser than 0");
                car.angle = car.angle + adjAngle + 360; 
            }
            else 
            {
                Console.WriteLine("Else");
                car.angle += adjAngle; 
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Rectangle r = new Rectangle();
            r.Height = 50;
            r.Width = 50;
            r.Name = "Cone" + count;
            Random rand = new Random();
            ImageBrush imgBrush = new ImageBrush();

            imgBrush.ImageSource = new BitmapImage(new Uri( "C:/Users/Peterson/source/repos/fuzzylogic/fuzzylogic/Images/tcone.ico", UriKind.Relative));
            imgBrush.Stretch = Stretch.Fill;
            imgBrush.AlignmentX = AlignmentX.Center;
            imgBrush.AlignmentY = AlignmentY.Center;

            r.Fill = imgBrush;
            mainCanvas.Children.Add(r);
            coneListx.Add(rand.Next(100, 550));
            coneListy.Add(rand.Next(75, 250));
          
            Canvas.SetRight(r, coneListx[count]);
            Canvas.SetTop(r, coneListy[count]);
            count++;

        }

        private void CheckForCollision()
        {
            Point carPosition = new Point();
            Double pAngle = 0, obstacleDist = 0;

            Double adjAngle = 360, adjDistance = 10000;

            for (int i = 0; i < count; i++)
            {
                string name = "Cone" + i;

                carPosition.X = coneListx[i];// + 25;
                carPosition.Y = coneListy[i] + 10;
                if (car.CheckCollsions(ref obstacleDist, carPosition, ref pAngle))
                {
                    if (obstacleDist < adjDistance)
                    {
                        adjAngle = pAngle;
                        adjDistance = obstacleDist;
                       
                    }
                    //adjustAngle(pAngle, obstacleDist);
                }
              
            }
      
            if (adjDistance < 10000) {
        
                adjustAngle(adjAngle, adjDistance);
                
            }
        }

        private bool DestinationReached()
        {
            if (Math.Abs((car.xDestination - car.xPos)) < 5)
            {
                if (Math.Abs((car.yDestination - car.yPos)) < 5) { return true; }
            }
            return false;
        }

        protected void NewDestination(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(mainCanvas);
            p.X = this.ActualWidth - p.X;
            car.SetDestination(p.X - 50, p.Y - 10);

            sprTarget.Visibility = Visibility.Visible;
            Canvas.SetRight(sprTarget, p.X - (sprTarget.ActualWidth) + 5);
            Canvas.SetTop(sprTarget, p.Y - (sprTarget.ActualHeight / 2));

            AnimateCar();
        }

        private void UpdateStatistics()
        {
            lblAngle.Content = car.angle.ToString();
         ;
        }
    }
}

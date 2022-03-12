using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.IO.Ports;
using System.Windows.Media.Animation;

namespace MapPainter.View
{
    public partial class PaintPage : Page
    {
        private SerialPort serialPort;
        private List<double> angles = new();
        private List<double> lengths = new();
        private List<Point> points = new();
        private StylusPoint firstPoint;
        private bool isFirstPointCreated;
        private double firstAngle;

        public PaintPage(string portName)
        {
            InitializeComponent();
            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(InkCanvasMouseDown), true);

            serialPort = new(portName);
            serialPort.BaudRate = 9600;
            if (!serialPort.IsOpen)
                serialPort.Open();
            else
                throw new Exception("port already open");
        }

        private void InkCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isFirstPointCreated)
            {
                firstPoint = new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);
                
                // Set position for image where we clicked
                Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);    
                Canvas.SetTop(robot, e.GetPosition(this).Y - robot.Height / 2);
                
                isFirstPointCreated = true;
                robot.Visibility = Visibility.Visible;

                points.Add(new Point(firstPoint.X, firstPoint.Y));
            }
            else
            {
                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                StylusPoint secondPoint = new(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);
                Stroke stroke = new (new StylusPointCollection()
                {
                    firstPoint,
                    secondPoint,
                });

                points.Add(new Point(secondPoint.X, secondPoint.Y));

                stroke.DrawingAttributes.Color = Colors.AliceBlue;

                if((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)) != 0)
                {
                    if (angles.Count == 0)
                    {                       
                        lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));

                        if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                            firstAngle = (int)(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));
                        else
                            firstAngle = (int)(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y)));
                        angles.Add(0);
                        
                        inkCanvas.Strokes.Add(stroke);
                    }
                    else
                    {
                        lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));

                        if (e.GetPosition(inkCanvas).Y <= firstPoint.Y)
                            angles.Add((int)(90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y))) - firstAngle);
                        else
                            angles.Add((int)(-90 + 180 / Math.PI * Math.Atan((e.GetPosition(inkCanvas).X - firstPoint.X) / (e.GetPosition(inkCanvas).Y - firstPoint.Y))) - firstAngle);
                        inkCanvas.Strokes.Add(stroke);
                    }
                }                    
            }
        }

        private void ButtonClose(object sender, RoutedEventArgs e)
        { 
            serialPort.Close();
            Application.Current.Shutdown();
        }

        private void BorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            serialPort.Close();
            Application.Current.MainWindow.Content = new ConnectPage();
        }

        private void LaunchButtonClick(object sender, RoutedEventArgs e)
        {
            // If we dont have at least 1 line yet
            if (points.Count > 1)
            {
                serialPort.WriteLine(GetRoute());
                MessageBox.Show(GetRoute());
                StartRobotAnimation();
            }
        }

        private bool isMoving;
        
        private void RobotMouseDown(object sender, MouseButtonEventArgs e)
        {
            isMoving = true;

            inkCanvas.Strokes.Clear();
            angles.Clear();
            lengths.Clear();
            points.Clear();
            robot.CaptureMouse();
        }

        private void RobotMouseUp(object sender, MouseButtonEventArgs e)
        {
            isMoving = false;
            
            robot.ReleaseMouseCapture();
            
            double sX, sY;
            sX = e.GetPosition(inkCanvas).X;
            sY = e.GetPosition(inkCanvas).Y;

            if (e.GetPosition(this).X - robot.Width / 2 < 20)
                sX = 0;
            else if (e.GetPosition(this).X - robot.Width / 2 > 1200)
                sX = 1186;

            if (e.GetPosition(this).Y - robot.Height / 2 < 40)
                sY = 0;
            else if (e.GetPosition(this).Y - robot.Height / 2 > 575)
                sY = 555;

            firstPoint = new StylusPoint(sX, sY);
        }

        private void RobotMouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                if (e.GetPosition(this).X - robot.Width / 2 > 20 && e.GetPosition(this).X - robot.Width / 2 < 1200)
                    Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);
                if (e.GetPosition(this).Y - robot.Height / 2 > 40 && e.GetPosition(this).Y - robot.Height / 2 < 585)
                    Canvas.SetTop(robot, e.GetPosition(this).Y - robot.Height / 2);
            }
        }

        private string GetRoute()
        {
            string route = string.Empty;
            for (int i = 0; i < lengths.Count; ++i)
                route += $"{angles[i]} {Math.Round(lengths[i] * (double.Parse(scaleTextBox.Text) / 100))};";
            return route;
        }

        private void ScaleTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0)) e.Handled = true;
        }

        private void ScaleTextBoxGotFocus(object sender, RoutedEventArgs e) 
            => scaleTextBox.Text = string.Empty;

        public void StartRobotAnimation()
        {
            // Create a NameScope for the page so that
            // we can use Storyboards.
            NameScope.SetNameScope(this, new NameScope());

            // Create a transform. This transform
            // will be used to move the robot image.
            TranslateTransform animatedTranslateTransform = new();

            // Register the transform's name with the page
            // so that they it be targeted by a Storyboard.
            RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

            robot.RenderTransform = animatedTranslateTransform;

            // Create the animation path.
            PathGeometry animationPath = new();
            PathFigure pathFigure = new();
            PolyLineSegment polyLineSegment = new();

            for (int i = 1; i < points.Count; ++i)
                polyLineSegment.Points.Add(new Point(points[i].X - points.First().X, points[i].Y - points.First().Y));

            pathFigure.Segments.Add(polyLineSegment);
            animationPath.Figures.Add(pathFigure);

            // Freeze the PathGeometry for performance benefits.
            animationPath.Freeze();

            DoubleAnimationUsingPath translateXAnimation = new();
            translateXAnimation.PathGeometry = animationPath;
            translateXAnimation.Duration = 
                TimeSpan.FromMilliseconds((lengths.Sum() * (double.Parse(scaleTextBox.Text) / 10)) + angles.Count * 500);

            translateXAnimation.Source = PathAnimationSource.X;

            Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateXAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            DoubleAnimationUsingPath translateYAnimation = new();
            translateYAnimation.PathGeometry = animationPath;
            translateYAnimation.Duration = 
                TimeSpan.FromMilliseconds((lengths.Sum() * (double.Parse(scaleTextBox.Text) / 10)) + angles.Count * 500);

            translateYAnimation.Source = PathAnimationSource.Y;

            // Set the animation to target the Y property
            // of the TranslateTransform named "AnimatedTranslateTransform".
            Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateYAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            // Create a Storyboard to contain and apply the animations.
            Storyboard pathAnimationStoryboard = new();
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);

            pathAnimationStoryboard.Completed += (s, e) =>
            {
                menuButton.IsEnabled = true;
                launchButton.IsEnabled = true;
                scaleTextBox.IsEnabled = true;
                inkCanvas.IsEnabled = true;
            };

            menuButton.IsEnabled = false;
            launchButton.IsEnabled = false;
            scaleTextBox.IsEnabled = false;
            inkCanvas.IsEnabled = false;

            pathAnimationStoryboard.Begin(this);
        }
    }
}
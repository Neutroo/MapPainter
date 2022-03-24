using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MapPainter.View
{
    public partial class PaintPage : Page
    {
        private SerialPort serialPort;
        private List<int> angles = new();
        private List<int> lengths = new();
        private List<Point> points = new();
        private StylusPoint firstPoint;
        private int firstAngle;
        private int lastAngle;
        private bool isRestarted;

        public PaintPage(string portName)
        {
            InitializeComponent();

            inkCanvas.AddHandler(MouseDownEvent, new MouseButtonEventHandler(InkCanvasMouseDown), true);

            serialPort = new(portName) { BaudRate = 9600 };

            if (!serialPort.IsOpen)
                serialPort.Open();
            else
                throw new Exception("port already open");
        }

        private void InkCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (robot.Visibility == Visibility.Hidden)
            {
                // Initializing the first point of robot's way
                firstPoint = new StylusPoint(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);

                // Set position for image where we clicked
                Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);
                Canvas.SetTop(robot, e.GetPosition(this).Y - robot.Height / 2);

                // Unhiding the robot's image
                robot.Visibility = Visibility.Visible;

                points.Add(new Point(firstPoint.X, firstPoint.Y));
            }
            else
            {
                // Finding the last point of the current way
                foreach (Stroke s in inkCanvas.Strokes)
                    firstPoint = s.StylusPoints[^1];

                // Initializing new point at the click position
                StylusPoint secondPoint = new(e.GetPosition(inkCanvas).X, e.GetPosition(inkCanvas).Y);

                Stroke stroke = new(new StylusPointCollection()
                {
                    firstPoint,
                    secondPoint,
                });

                points.Add(new Point(secondPoint.X, secondPoint.Y));

                stroke.DrawingAttributes.Color = Colors.AliceBlue;

                // Calculating the length of a new line
                // by the Pythagorean theorem
                lengths.Add((int)Math.Sqrt(Math.Pow(e.GetPosition(inkCanvas).X - firstPoint.X, 2) + Math.Pow(e.GetPosition(inkCanvas).Y - firstPoint.Y, 2)));

                if (inkCanvas.Strokes.Count == 0)
                {
                    // Finding the angle in 1 and 4 quarters
                    // using the ArcTan, assuming that the Y axis is directed downward, and the X is to the right
                    firstAngle = (int)(180 / Math.PI * Math.Atan((-firstPoint.Y + e.GetPosition(inkCanvas).Y) / (e.GetPosition(inkCanvas).X - firstPoint.X)));

                    // Checking for finding the angle in 2 and 3 quarters
                    if (e.GetPosition(inkCanvas).X < firstPoint.X)
                        firstAngle = (firstAngle > 0) ? -180 + firstAngle : 180 + firstAngle;

                    // Adding first angle as 0
                    if (!isRestarted)
                        angles.Add(0);
                }
                else
                {
                    // Finding the angle in 1 and 4 quarters
                    // using the ArcTan, assuming that the Y axis is directed downward, and the X is to the right
                    int angle = (int)(180 / Math.PI * Math.Atan((-firstPoint.Y + e.GetPosition(inkCanvas).Y) / (e.GetPosition(inkCanvas).X - firstPoint.X)));

                    // Checking for finding the angle in 2 and 3 quarters
                    if (e.GetPosition(inkCanvas).X < firstPoint.X)
                        angle = (angle > 0) ? -180 + angle : 180 + angle;

                    // Compensating for the rotation of the coordinate system by the first angle
                    angle -= firstAngle;

                    // Compensating angle in restart
                    angle += lastAngle;

                    // Checking and compensation of angle deviation from the interval [-180;180]
                    if (angle > 180)
                        angle -= 360;

                    if (angle < -180)
                        angle += 360;

                    angles.Add(angle);
                }

                // Adding a new line
                inkCanvas.Strokes.Add(stroke);
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
            // If we have at least 1 line yet and scaleBox not empty
            if (points.Count > 1 && scaleTextBox.Text != string.Empty)
            {
                serialPort.WriteLine(GetRoute());
                StartRobotAnimation();
                isRestarted = true;
            }
        }

        private void RobotMouseDown(object sender, MouseButtonEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            angles.Clear();
            lengths.Clear();
            points.Clear();
            robot.CaptureMouse();
        }

        private void RobotMouseUp(object sender, MouseButtonEventArgs e)
        {
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
            points.Add(new Point(firstPoint.X, firstPoint.Y));

            if (isRestarted)
                angles.Add(lastAngle);
        }

        private void RobotMouseMove(object sender, MouseEventArgs e)
        {
            if (robot.IsMouseCaptured)
            {
                if (e.GetPosition(this).X - robot.Width / 2 > 20 && e.GetPosition(this).X - robot.Width / 2 < 1200)
                    Canvas.SetLeft(robot, e.GetPosition(this).X - robot.Width / 2);
                if (e.GetPosition(this).Y - robot.Height / 2 > 40 && e.GetPosition(this).Y - robot.Height / 2 < 595)
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
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }

        private void ScaleTextBoxGotFocus(object sender, RoutedEventArgs e)
            => scaleTextBox.Text = string.Empty;

        public void StartRobotAnimation()
        {
            // Create a NameScope for the page so that
            // we can use Storyboards
            NameScope.SetNameScope(this, new NameScope());

            // Create a transform. This transform
            // will be used to move the robot image
            TranslateTransform animatedTranslateTransform = new();

            // Register the transform's name with the page
            // so that they it be targeted by a Storyboard
            RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

            robot.RenderTransform = animatedTranslateTransform;

            // Create the animation path
            PathGeometry animationPath = new();
            PathFigure pathFigure = new();
            PolyLineSegment polyLineSegment = new();

            for (int i = 1; i < points.Count; ++i)
                polyLineSegment.Points.Add(new Point(points[i].X - points.First().X, points[i].Y - points.First().Y));

            pathFigure.Segments.Add(polyLineSegment);
            animationPath.Figures.Add(pathFigure);

            // Freeze the PathGeometry for performance benefits
            animationPath.Freeze();

            DoubleAnimationUsingPath translateXAnimation = new();
            translateXAnimation.PathGeometry = animationPath;
            translateXAnimation.Duration =
                TimeSpan.FromMilliseconds((lengths.Sum() * (double.Parse(scaleTextBox.Text) / 10)) + angles.Count * 700);

            translateXAnimation.Source = PathAnimationSource.X;

            Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateXAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            DoubleAnimationUsingPath translateYAnimation = new();
            translateYAnimation.PathGeometry = animationPath;
            translateYAnimation.Duration =
                TimeSpan.FromMilliseconds((lengths.Sum() * (double.Parse(scaleTextBox.Text) / 10)) + angles.Count * 700);

            translateYAnimation.Source = PathAnimationSource.Y;

            Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
            Storyboard.SetTargetProperty(translateYAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            // Create a Storyboard to contain and apply the animations
            Storyboard pathAnimationStoryboard = new();
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);

            pathAnimationStoryboard.Completed += (s, e) =>
            {
                menuButton.IsEnabled = true;
                launchButton.IsEnabled = true;
                scaleTextBox.IsEnabled = true;
                inkCanvas.IsEnabled = true;
                robot.IsEnabled = true;

                menuButton.Opacity = 1;
                launchButton.Opacity = 1;
                scaleGrid.Opacity = 1;

                inkCanvas.Strokes.Clear();
                lastAngle = angles.Last();
                angles.Clear();
                angles.Add(lastAngle);
                lengths.Clear();

                Canvas.SetLeft(robot, points.Last().X + 20.9);
                Canvas.SetTop(robot, points.Last().Y + 47);

                robot.RenderTransform = new TranslateTransform(0, 0);   // Reset RenderTransform

                firstPoint = new StylusPoint(points.Last().X, points.Last().Y);
                points.Clear();
                points.Add(new Point(firstPoint.X, firstPoint.Y));
            };

            menuButton.IsEnabled = false;
            launchButton.IsEnabled = false;
            scaleTextBox.IsEnabled = false;
            inkCanvas.IsEnabled = false;
            robot.IsEnabled = false;

            menuButton.Opacity = 0.7;
            launchButton.Opacity = 0.7;
            scaleGrid.Opacity = 0.7;

            pathAnimationStoryboard.Begin(this);
        }
    }
}
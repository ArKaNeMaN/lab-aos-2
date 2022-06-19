using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class MainWindow
    {
        private static Thread? _triangleThread;
        private static Thread? _rectangleThread;

        private const int TriangleTurn = 0;
        private const int RectangleTurn = 1;

        private const string TriangleShape = "triangle";
        private const string RectangleShape = "rectangle";

        private static int _turn = TriangleTurn;
        private static readonly bool[] Flag = { false, false };
        private static readonly int[] Speed = { 500, 500 };

        private static bool _stop;
        
        private static readonly int[] CanvasBounds = { 800, 700 };

        private const int ShapesSize = 50;

        private static MainWindow? _window;

        private static bool SyncState = true;

        public MainWindow()
        {
            InitializeComponent();

            _window = this;

            UpdateState();
        }

        private void SwitchSync(object? sender = null, RoutedEventArgs? e = null)
        {
            SyncState = !SyncState;

            SwitchSyncBtn.Content = SyncState ? "+" : "-";
        }

        private void UpdateState(object? sender = null, RoutedEventArgs? e = null)
        {
            if (_isCanvasOverflowed)
            {
                StateTextBlock.Text = "Область переполнена";
            }
            else if (_triangleThread == null)
            {
                StateTextBlock.Text = "Остановлен";
            }
            else
            {
                StateTextBlock.Text = "Запущен";
            }
        }
        
        private void DrawShape(string shape, double x, double y)
        {
            Shape? sh = null;

            switch (shape)
            {
                case RectangleShape:
                    var rect = new Rectangle
                    {
                        Stroke = Brushes.Chartreuse,
                        Width = ShapesSize,
                        Height = ShapesSize,
                    };

                    sh = rect;
                    break;
                case TriangleShape:
                    var polygon = new Polygon
                    {
                        Points = new PointCollection
                        {
                            new(ShapesSize / 2, 0),
                            new(0, ShapesSize),
                            new(ShapesSize, ShapesSize),
                        },
                        Stroke = Brushes.Aqua,
                    };

                    sh = polygon;
                    break;
            }

            if (sh == null)
            {
                return;
            }
            
            sh.SetValue(Canvas.LeftProperty, x);
            sh.SetValue(Canvas.TopProperty, y);
                
            ShapesCanvas.Children.Add(sh);
        }

        private void OnStart(object sender, RoutedEventArgs e)
        {
            if (_triangleThread == null)
            {
                Console.WriteLine("Drawers started.");
                StartDrawers();
            }

            UpdateState();
        }

        private void OnApplySpeed(object sender, RoutedEventArgs e)
        {
            Speed[0] = int.Parse(Speed1Field.Text);
            Speed[1] = int.Parse(Speed2Field.Text);
            
            Console.WriteLine("Applied speed [{0}, {1}].", Speed[0], Speed[1]);
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            if (_triangleThread != null)
            {
                StopDrawers();
                Console.WriteLine("Drawers stopped.");
            }

            UpdateState();
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            OnStop(sender, e);
            
            ShapesCanvas.Children.Clear();
            _latestPoint = new Point(-1, -1);
            _isCanvasOverflowed = false;

            UpdateState();
        }
        
        // Лень делать нормально, пусть будет статикой))

        private static void StartDrawers()
        {
            _stop = false;
            
            _turn = TriangleTurn;
            Flag[0] = false;
            Flag[1] = false;
            
            _triangleThread = new Thread(TriangleDrawer);
            _rectangleThread = new Thread(RectangleDrawer);

            _triangleThread.Start();
            _rectangleThread.Start();
            
            _window?.UpdateState();
        }

        private static void StopDrawers()
        {
            _stop = true;

            _triangleThread?.Join();
            _rectangleThread?.Join();

            _triangleThread = null;
            _rectangleThread = null;
            
            _stop = false;
            
            _window?.UpdateState();
        }

        private static void RectangleDrawer()
        {
            while (true)
            {
                Flag[RectangleTurn] = true;
                _turn = TriangleTurn;

                if (SyncState)
                {
                    while (Flag[TriangleTurn] && _turn == TriangleTurn)
                    {
                    }
                }

                // КС
                Draw(RectangleShape, GetRandomPoint());
                
                Flag[RectangleTurn] = false;
                Thread.Sleep(Speed[RectangleTurn]);

                if (_stop)
                {
                    break;
                }
            }
        }

        private static void TriangleDrawer()
        {
            while (true)
            {
                Flag[TriangleTurn] = true;
                _turn = RectangleTurn;

                if (SyncState)
                {
                    while (Flag[RectangleTurn] && _turn == RectangleTurn)
                    {
                    }
                }

                // КС
                Draw(TriangleShape, GetRandomPoint());
                
                Flag[TriangleTurn] = false;
                Thread.Sleep(Speed[TriangleTurn]);
                
                if (_stop)
                {
                    break;
                }
            }
        }

        private static void Draw(string shape, Point point)
        {
            Draw(shape, point.X, point.Y);
        }
        
        private static void Draw(string shape, double x, double y)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                _window?.DrawShape(shape, x, y);
            });
        }

        // private static readonly Random Rnd = new();
        private static Point _latestPoint = new(-1, -1);
        private const double PointsGap = 10;
        private static bool _isCanvasOverflowed = false;

        private static Point GetRandomPoint()
        {
            // return new Point(Rnd.Next(0, CanvasBounds[0] - ShapesSize), Rnd.Next(0, CanvasBounds[1] - ShapesSize));

            if (_latestPoint.X < 0)
            {
                _latestPoint = new Point(PointsGap, PointsGap);
                return _latestPoint;
            }

            _latestPoint.X += ShapesSize + PointsGap;
            if (_latestPoint.X >= CanvasBounds[0] - ShapesSize)
            {
                _latestPoint.X = PointsGap;
                _latestPoint.Y += ShapesSize + PointsGap;
            }

            if (_latestPoint.Y >= CanvasBounds[1])
            {
                _isCanvasOverflowed = true;
                _latestPoint = new Point(PointsGap, PointsGap);
                
                // Асинхронно, чтобы текущий поток не ждал себя же)
                Application.Current.Dispatcher.InvokeAsync(StopDrawers);
            }

            return _latestPoint;
        }
    }
}
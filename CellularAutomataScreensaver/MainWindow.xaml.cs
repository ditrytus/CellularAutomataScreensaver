using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CellularAutomataScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int cellSize = 16;

        bool[] currencGeneration;

        CellularAutomaton automaton;

        double rowsPerSecond = 3.0;

        int subrows = 32;

        Timer timer;

        WindowMode mode;

        Path currentCellRows;

        public MainWindow(WindowMode mode)
        {
            InitializeComponent();
            this.mode = mode;
            automaton = new CellularAutomaton(30, true);
            switch(mode)
            {
                case WindowMode.Preview:
                    Cursor = Cursors.Arrow;
                    break;
                case WindowMode.Window:
                    Cursor = Cursors.Arrow;
                    ResizeMode = ResizeMode.CanResize;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    break;
            }
        }

        void timer_Elapsed(object state)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                ProgressGeneration();
                AddNewCellsRow();
            }));
        }

        private void ProgressGeneration()
        {
            this.currencGeneration = automaton.Transform(this.currencGeneration);
        }

        private void Canvas_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.UpdateLayout();

            this.currencGeneration = GenerateRandomGeneration();

            AddNewCellsRow();
        }

        private void AddNewCellsRow()
        {
            var cellsRow = CreateCellsRow();
            Storyboard ascentBoard = CreateAscendingAnimation(cellsRow);
            this.LayoutCanvas.Children.Add(cellsRow);
            var timerInterval = TimeSpan.FromSeconds((((1 * subrows) - 0.25) / rowsPerSecond));
            currentCellRows = cellsRow;
            if (timer != null)
            {
                timer.Dispose();
            }
            timer = new Timer(new TimerCallback(timer_Elapsed), null, timerInterval, TimeSpan.FromMilliseconds(-1));
        }

        private Path CreateCellsRow()
        {
            Path cellsRow = new Path()
            {
                Fill = this.ForegroundColor
            };
            StreamGeometry rowsGeometry = new StreamGeometry();
            using (var geometryContext = rowsGeometry.Open())
            {
                for (int j = 0; j < subrows; j++)
                {
                    ProgressGeneration();
                    for (int i = 0; i < this.currencGeneration.Length; i++)
                    {
                        var cell = this.currencGeneration[i];
                        if (cell)
                        {
                            var rectStartPoint = new Point(i * cellSize, j * cellSize);
                            geometryContext.BeginFigure(rectStartPoint, true, true);
                            geometryContext.LineTo(new Point(rectStartPoint.X + cellSize, rectStartPoint.Y), false, false);
                            geometryContext.LineTo(new Point(rectStartPoint.X + cellSize, rectStartPoint.Y + cellSize), false, false);
                            geometryContext.LineTo(new Point(rectStartPoint.X, rectStartPoint.Y + cellSize), false, false);
                            geometryContext.LineTo(new Point(rectStartPoint.X, rectStartPoint.Y), false, false);
                            var cellShape = new Rectangle()
                            {
                                Width = cellSize,
                                Height = cellSize,
                                Fill = new SolidColorBrush(Colors.Black)
                            };
                        }
                    }
                }
            }

            cellsRow.Data = rowsGeometry;

            Canvas.SetLeft(cellsRow, 0);
            double topLength = LayoutCanvas.ActualHeight - cellSize;
            if (currentCellRows != null)
            {
                topLength = Canvas.GetTop(currentCellRows) + currentCellRows.ActualHeight - 3;
            }
            Canvas.SetTop(cellsRow, topLength);
            return cellsRow;
        }

        private Storyboard CreateAscendingAnimation(UIElement cellsRow)
        {
            DoubleAnimation ascentAnimation = new DoubleAnimation(Canvas.GetTop(cellsRow) - (this.MaxRows + subrows) * cellSize, new Duration(TimeSpan.FromSeconds((this.MaxRows + subrows) / rowsPerSecond)));
            Storyboard.SetTarget(ascentAnimation, cellsRow);
            Storyboard.SetTargetProperty(ascentAnimation, new PropertyPath("(Canvas.Top)"));

            Storyboard ascentBoard = new Storyboard();
            ascentBoard.Children.Add(ascentAnimation);
            ascentBoard.Completed += ascentBoard_Completed;
            ascentBoard.Begin();

            return ascentBoard;
        }

        private bool[] GenerateRandomGeneration()
        {
            var rand = new Random();
            var randomGeneration = Enumerable.Range(0, this.CellsInRow).Select(x => rand.Next() % 2 == 0 ? true : false).ToArray();
            return randomGeneration;
        }

        private int MaxRows
        {
            get
            {
                return (int)Math.Ceiling(this.LayoutCanvas.ActualHeight / cellSize);
            }
        }

        private int CellsInRow
        {
            get
            {
                return (int)Math.Ceiling(this.LayoutCanvas.ActualWidth / cellSize);
            }
        }

        void ascentBoard_Completed(object sender, EventArgs e)
        {
            this.LayoutCanvas.Children.Remove((UIElement)Storyboard.GetTarget(((sender as ClockGroup).Timeline as Storyboard).Children[0]));
        }

        private bool IsInScreensaver => mode == WindowMode.Screensaver;

        private void ShutdownIfInPreview()
        {
            if (IsInScreensaver)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ShutdownIfInPreview();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShutdownIfInPreview();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ShutdownIfInPreview();
        }
    }
}

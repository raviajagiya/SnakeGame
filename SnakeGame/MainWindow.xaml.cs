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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.empty },
            {GridValue.Snake,Images.body },
            {GridValue.Food,Images.food}
        };

            // for rotation of head
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0},
            {Direction.Down, 180},
            {Direction.Left, 270},
            {Direction.Right, 90}
        };


        private readonly int rows=25, cols=25;
        private readonly Image[,] gridimages;
        private GameState gameState;
        private bool gameRunning;


        public MainWindow()
        {
            InitializeComponent();
            gridimages = SetupGrid();
            gameState=new GameState(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        // Press any key to start window event 
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }
            if(!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left); break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right); break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up); break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);break;
            }
        }

        private async Task GameLoop()
        {
            while(!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }
        private Image[,] SetupGrid()
        {
            Image[,] images= new Image[rows, cols];
            GameGrid.Rows= rows;
            GameGrid.Columns= cols;
            GameGrid.Width = GameGrid.Height * (cols/(double)rows); 

            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image()
                    {
                        Source = Images.empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);

                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakHead();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridimages[r, c].Source = gridValToImage[gridVal];
                    gridimages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }


        // for draw head and its rotation according to dictory
        private void DrawSnakHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridimages[headPos.Row, headPos.Col];
            image.Source = Images.head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnack()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for(int i=0;i< positions.Count;i++)
            {
                Position pos = positions[i];
                ImageSource sourse = (i == 0) ? Images.deadHead : Images.deadBody;
                gridimages[pos.Row, pos.Col].Source = sourse;
                await Task.Delay(50);
            }
        }

        // count down
        private async Task ShowCountDown()
        {
            for(int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        // game over
        private async Task ShowGameOver()
        {
            await DrawDeadSnack();
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}

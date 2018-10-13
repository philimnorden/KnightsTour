using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Knights_Tour
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitGrid(Int32.Parse(sliderWidth.Value.ToString()), Int32.Parse(sliderHeight.Value.ToString()));
 

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            StartKnightsTour(Int32.Parse(sliderWidth.Value.ToString()), Int32.Parse(sliderHeight.Value.ToString()),
                Int32.Parse(tbStartX.Text), Int32.Parse(tbStartY.Text));
        }

        public static int step = 0;

        public async void StartKnightsTour(int width, int height, int startx, int starty)
        {
            if (width < startx || height < starty)
            {
                MessageBoxResult r = MessageBox.Show("Please use a valid start position","", MessageBoxButton.OK);
                return;
            }
            
            int fields = width * height;

            Position startposition = new Position(startx, starty);
            

            
            long triedmoves = 0;

            var board = new int[width, height];

            board[startposition.x, startposition.y] = 1;
            step = 1;

            await Task.Run(() =>
            {
                while (step != fields)
                {
                   
                        Dispatcher.Invoke(new Action(() => DrawGrid(board, false)));
                    try
                    {
                        board = Move(board, -1);
                        triedmoves++;

                        Dispatcher.Invoke(new Action(() => labelMoves.Content = "Moves: " + triedmoves));
                        
                    }
                    catch (NotSolvableException)
                    {
                        MessageBoxResult r = MessageBox.Show("There is no solution possible.", "Sorry!", MessageBoxButton.OK);
                        return;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            });

            Dispatcher.Invoke(new Action(() => DrawGrid(board, true)));
            Console.WriteLine("Done");

        }

        public void DrawGrid(int[,] board, bool done)
        {
            DisplayGrid.Children.Clear();

            int max = board.Cast<int>().Max();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    TextBlock txtBlock1 = new TextBlock();
                    txtBlock1.Text = board[i,j].ToString();
                    txtBlock1.FontSize = 14;
                    txtBlock1.TextAlignment = TextAlignment.Center;
                    Grid.SetRow(txtBlock1, i);
                    Grid.SetColumn(txtBlock1, j);

                    if (board[i, j] == max)
                    {
                        txtBlock1.Background = new SolidColorBrush(Colors.Red);
                    }
                    else if(done){
                        txtBlock1.Background = new SolidColorBrush(Colors.Green);
                    }
                    if (board[i, j] == 0)
                    {
                        txtBlock1.Background = new SolidColorBrush(Colors.LightGray);
                    }
                    DisplayGrid.Children.Add(txtBlock1);

                }
            }


        }

        public void InitGrid(int width, int height)
        {
            for (int i = 0; i < width; i++)
            {

                DisplayGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < height; j++)
            {
                DisplayGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            DisplayGrid.Background = new SolidColorBrush(Colors.LightSteelBlue);
   
        }

        public int[,] Move(int[, ] board, int wrongRoute)
        {
            if (wrongRoute == 7)
            {
                return Backtrack(board);
            }
            Position currentPosition = GetCurrentPosition(board, 0);
            List<Move> moves = GetMoves();

            for (int i = 0; i < 8; i++)
            {
                if (i <= wrongRoute)
                {
                    i = wrongRoute + 1;
                }

                int x = currentPosition.x + moves[i].movex;
                int y = currentPosition.y + moves[i].movey;

                int width = board.GetLength(0);
                int height = board.GetLength(1);

                if (x >= 0 && x < width
                    && y >= 0 && y < height
                    && board[x,y] == 0)
                {
                    step++;
                    board[x, y] = step;
                    return board;
                }

            }
            return Backtrack(board);

        }

        public int[,] Backtrack(int[,] board)
        {
            Position currentPosition = GetCurrentPosition(board, 0);
            Position lastPosition = GetCurrentPosition(board, -1);

            Move failedMove = GetPositionRelation(lastPosition, currentPosition);
            board = RemoveLastStep(board);

            return Move(board,failedMove.id);


        }

        public int[,] RemoveLastStep(int[,] board)
        {
            int max = board.Cast<int>().Max();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == max)
                    {
                        board[i,j] = 0;
                        step--;
                    }
                }
            }
            return board;
        }

        public Move GetPositionRelation(Position lastPosition, Position currentPosition)
        {
            List<Move> moves = GetMoves();
            int newPositionx;
            int newPositiony;

            foreach (var move in moves)
            {
                newPositionx = lastPosition.x + move.movex;
                newPositiony = lastPosition.y + move.movey;

                if (newPositionx == currentPosition.x && newPositiony == currentPosition.y)
                {
                    return move;
                }
            }
            
            throw new NotSolvableException();

        }

        public Position GetCurrentPosition(int[,] board, int laststep)
        {
            int max = board.Cast<int>().Max();
            int desired = max + laststep;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i,j] == desired)
                    {
                        return new Position(i, j);
                    }
                }
            }

            throw new Exception();
            
        }

        public List<Move> GetMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            possibleMoves.Add(new Move(0, -2, -1));
            possibleMoves.Add(new Move(1, -2, 1));
            possibleMoves.Add(new Move(2, -1, -2));
            possibleMoves.Add(new Move(3, -1, 2));
            possibleMoves.Add(new Move(4, 1, -2));
            possibleMoves.Add(new Move(5, 1, 2));
            possibleMoves.Add(new Move(6, 2, -1));
            possibleMoves.Add(new Move(7, 2, 1));

            return possibleMoves;
        }

        private void sliderHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            ForceGridRebuild();
        }

        private void sliderWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            ForceGridRebuild();
        }
        public void ForceGridRebuild()
        {
            try
            {
                DisplayGrid.RowDefinitions.Clear();
                DisplayGrid.ColumnDefinitions.Clear();
                InitGrid(Int32.Parse(sliderWidth.Value.ToString()), Int32.Parse(sliderHeight.Value.ToString()));
            }
            catch (Exception)
            {

                
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }



    public class Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Move
    {
        public int id;
        public int movex;
        public int movey;

        public Move(int id, int movex, int movey)
        {
            this.id = id;
            this.movex = movex;
            this.movey = movey;
        }
    }

    public class NotSolvableException : Exception
    {
        public NotSolvableException()
        {
        }

        public NotSolvableException(string message)
            : base(message)
        {
        }

        public NotSolvableException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

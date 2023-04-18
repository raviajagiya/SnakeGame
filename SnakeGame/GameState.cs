using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public class GameState
    {
        public int Rows { get;}
        public int Cols { get;}

        public GridValue[,] Grid { get;}
        public Direction Dir { get; private set;}
        public int Score { get; private set;}
        public bool GameOver { get; private set;}

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>();

        private readonly LinkedList<Position>snakePosiotions= new LinkedList<Position>();
        private readonly Random random= new Random();

        public GameState (int rows,int cols)
        {
            Rows=rows;
            Cols=cols;
            Grid = new GridValue[rows, cols];
            Dir=Direction.Right;
            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;
            for(int c=1; c<= 3; c++)
            {
                Grid[r, c] = GridValue.Snake;
                snakePosiotions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0; r < Rows; r++)
            {
                for(int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridValue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> Empty= new List<Position>(EmptyPositions());
            if(Empty.Count==0) { return; }
            Position pos = Empty[random.Next(Empty.Count)];
            Grid[pos.Row,pos.Col]=GridValue.Food;
        }

        public Position HeadPosition()
        {
            return snakePosiotions.First.Value;
        }

        public Position TailPosiotion()
        {
            return snakePosiotions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePosiotions;
        }

        private void AddHead(Position pos)
        {
            snakePosiotions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridValue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePosiotions.Last.Value;
            Grid[tail.Row, tail.Col] = GridValue.Empty;
            snakePosiotions.RemoveLast();
        }

        private Direction GetLastDirection()
        {
            if(dirChanges.Count==0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if (dirChanges.Count == 2)
            {
                return false;
            }
            Direction LastDir = GetLastDirection();
            return newDir!= LastDir && newDir!=LastDir.Opposite();
        }

        public void ChangeDirection(Direction dir)
        {   if(CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
            //like a buffer add to last
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row<0 || pos.Col<0 || pos.Row >=Rows || pos.Col >=Cols;
        }

        private GridValue WillHit(Position newHeadPos)
        {
            if (OutsideGrid(newHeadPos))
            {
                return GridValue.Outside;
            }
            if (newHeadPos == TailPosiotion())
            {
                return GridValue.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {   
            if(dirChanges.Count>0)
            {
                Dir=dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit= WillHit(newHeadPos);

            if(hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            }
            else if(hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit== GridValue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}

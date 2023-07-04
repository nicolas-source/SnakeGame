using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public GridValue[,] Grid { get; set; }
        public Direction direction { get; set; }
        public int Score { get; set; }
        public bool GameOver { get; set; }

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            Grid = new GridValue[Rows, Columns];
            direction = Direction.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridValue.snake;
                snakePositions.AddFirst(new Position(c, r));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (Grid[r, c] == GridValue.empty)
                    {
                        yield return new Position(r, c);
                    }
                }

            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0)
            {
                return;
            }

            Position position = empty[random.Next(empty.Count)];
            Grid[position.Row, position.Column] = GridValue.food;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public Position TailPosition()
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position position)
        {
            snakePositions.AddFirst(position);
            Grid[position.Row, position.Column] = GridValue.snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.empty;
            snakePositions.RemoveLast();
        }

        public void ChangeDirection(Direction dir)
        {
            direction = dir;
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }

        private GridValue WillHit(Position newHeadPosition)
        {
            if (OutsideGrid(newHeadPosition))
            {
                return GridValue.outside;
            }

            if (newHeadPosition == TailPosition())
            {
                return GridValue.empty;
            }

            return Grid[newHeadPosition.Row, newHeadPosition.Column];
        }

        public void Move()
        {
            Position newHeadPosition = HeadPosition().Translate(direction);
            GridValue hit = WillHit(newHeadPosition);

            if (hit == GridValue.outside || hit == GridValue.snake)
            {
                GameOver = true;
            }
            else if (hit == GridValue.empty)
            {
                RemoveTail();
                AddHead(newHeadPosition);
            }
            else if (hit == GridValue.food)
            {
                AddHead(newHeadPosition);
                Score++;
                AddFood();
            }
        }
    }

}
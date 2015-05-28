using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe
{
    public class Game
    {
        private readonly Dictionary<Tuple<int, int>, Player> _moves =
            new Dictionary<Tuple<int, int>, Player>();

        public enum Player
        {
            X,
            O
        }

      


        private static char PlayerToString(Player player)
        {
            return player == Player.O ? 'O' : 'X';
        }

        public List<Tuple<int, int>> GetPlayerMoves(Player player)
        {
            return _moves.Where(e => e.Value.Equals(player)).Select(p => p.Key).ToList();
        }

        public GameState? Move(Player player, int row, int col)
        {
            CheckOutOfBounds(row, col);
            CheckAlreadyUsed(row, col);
            _moves.Add(new Tuple<int, int>(row, col), player);

            return InternalGameState;
        }

        private void CheckAlreadyUsed(int row, int col)
        {
            if (_moves.ContainsKey(new Tuple<int, int>(row, col)))
            {
                throw new InvalidMoveException();
            }
        }

        private static void CheckOutOfBounds(int row, int col)
        {
            if ((row < 1 || row > 3) || (col < 1 || col > 3))
            {
                throw new InvalidMoveException();
            }
        }

        private GameState? InternalGameState
        {
            get
            {
                if (_moves.Count == 0)
                {
                    return GameState.NotStarted;
                }

                var horizontalWinner = CheckForHorizontalWinner();
                if (horizontalWinner != null)
                {
                    return horizontalWinner;
                }
                var verticalWinner = CheckForVerticalWinner();
                if (verticalWinner != null)
                {
                    return verticalWinner;
                }
                var diagonalWinner = CheckForDiagonalWinner();
                if (diagonalWinner != null)
                {
                    return diagonalWinner;
                }

                if (_moves.Count == 9)
                {
                    return GameState.Draw;
                }
                return GameState.InProgress;
            }
        }

        private GameState? CheckForHorizontalWinner()
        {
            if (State.Contains("XXX"))
            {
                return GameState.PlayerXWon;
            }
            if (State.Contains("OOO"))
            {
                return GameState.PlayerOWon;
            }

            return null;
        }

        private GameState? CheckForDiagonalWinner()
        {
            var regex1 = new Regex("X.." + Environment.NewLine + ".X." + Environment.NewLine + "..X");
            var regex2 = new Regex("..X" + Environment.NewLine + ".X." + Environment.NewLine + "X..");


            if (regex1.IsMatch(this.State) || regex2.IsMatch(this.State) )
            {
                return GameState.PlayerXWon;
            }
            var regex11 = new Regex("O.." + Environment.NewLine + ".O." + Environment.NewLine + "..O");
            var regex22 = new Regex("..O" + Environment.NewLine + ".O." + Environment.NewLine + "O..");


            if (regex11.IsMatch(this.State) || regex22.IsMatch(this.State) )
            {
                return GameState.PlayerOWon;
            }

            return null;
        }
        private GameState? CheckForVerticalWinner()
        {
            var regex1 = new Regex("X.." + Environment.NewLine + "X.." + Environment.NewLine + "X..");
            var regex2 = new Regex(".X." + Environment.NewLine + ".X." + Environment.NewLine + ".X.");
            var regex3 = new Regex("..X" + Environment.NewLine + "..X" + Environment.NewLine + "..X");

            
            if (regex1.IsMatch(this.State) || regex2.IsMatch(this.State) || regex3.IsMatch(this.State))
            {
                return GameState.PlayerXWon;
            }
            var regex11 = new Regex("O.." + Environment.NewLine + "O.." + Environment.NewLine + "O..");
            var regex22 = new Regex(".O." + Environment.NewLine + ".O." + Environment.NewLine + ".O.");
            var regex33 = new Regex("..O" + Environment.NewLine + "..O" + Environment.NewLine + "..O");


            if (regex11.IsMatch(this.State) || regex22.IsMatch(this.State) || regex33.IsMatch(this.State))
            {
                return GameState.PlayerOWon;
            }

            return null;
        }

        public string State
        {
            get
            {
                var sb = new StringBuilder();

                for (int i = 1; i <= 3; i++)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        var position = new Tuple<int, int>(i, j);
                        var cellPlayed = _moves.ContainsKey(position);
                        if (!cellPlayed)
                        {
                            sb.Append("-");
                        }
                        else
                        {
                            sb.Append(PlayerToString(_moves[position]));
                        }
                       //if (j!=3)
                       // sb.Append("|");
                    }

                   if (i != 3)
                   {
                       sb.Append(Environment.NewLine);
                       //sb.AppendLine("_____");
                       
                   }
                }

                return sb.ToString();
            }
        }
    }
}
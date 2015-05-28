using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TicTacToe.Test
{
    [TestFixture]
    public class GameFixture
    {
        [Test]
        public void Ctor_ShouldInitializeGameState()
        {
            var game = new Game();
            const string expectedState = @"---
---
---";
            Console.WriteLine(game.State);
            game.State.Should().Be(expectedState);
        }

        [Test]
        public void Move_ShouldThrow_WhenPositionAlreadyUsed()
        {
            var game = new Game();
            game.Move(Game.Player.O, 1, 1);
            Action action = () => game.Move(Game.Player.O, 1, 1);

            action.ShouldThrowExactly<InvalidMoveException>();
        }

        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(43, 0)]
        [TestCase(4, 1)]
        [TestCase(33, 1)]
        public void Move_ShouldThrow_WhenPositionOutOfBounds(int row, int col)
        {
            var game = new Game();
            Action action = () => game.Move(Game.Player.O, row, col);
            action.ShouldThrowExactly<InvalidMoveException>();
        }

        [Test]
        public void Move_ShouldUpdateMoveHistory_WhenSinglePlayer()
        {
            var game = new Game();
            game.Move(Game.Player.O, 1, 1);
            game.Move(Game.Player.O, 1, 3);

            game.GetPlayerMoves(Game.Player.O).Should().BeEquivalentTo(new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 1),
                new Tuple<int, int>(1, 3)
            });

            game.Move(Game.Player.O, 3, 3);

            game.GetPlayerMoves(Game.Player.O).Should().BeEquivalentTo(new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 1),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(3, 3)
            });
        }

        [Test]
        public void Move_ShouldUpdateMoveHistory_WhenMultiplePlayers()
        {
            var game = new Game();
            game.Move(Game.Player.O, 1, 1);
            game.Move(Game.Player.O, 1, 3);
            game.Move(Game.Player.X, 2, 3);

            game.GetPlayerMoves(Game.Player.O).Should().BeEquivalentTo(new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 1),
                new Tuple<int, int>(1, 3)
            });

            game.Move(Game.Player.X, 3, 3);

            game.GetPlayerMoves(Game.Player.O).Should().BeEquivalentTo(new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 1),
                new Tuple<int, int>(1, 3)
            });
        }

        [Test]
        public void Move_ShouldUpdateGameState()
        {
            var game = new Game();
            const string expectedState =
                @"OXO
XXX
XOO";

            game.Move(Game.Player.O, 1, 1);
            game.Move(Game.Player.X, 1, 2);
            game.Move(Game.Player.O, 1, 3);

            game.Move(Game.Player.X, 2, 1);
            game.Move(Game.Player.X, 2, 2);
            game.Move(Game.Player.X, 2, 3);

            game.Move(Game.Player.X, 3, 1);
            game.Move(Game.Player.O, 3, 2);
            game.Move(Game.Player.O, 3, 3);

            game.State.Should().Be(expectedState);
        }

        [TestCase(Game.Player.X)]
        [TestCase(Game.Player.O)]
        public void Move_ShouldReturnPlayerWon_WhenThreeInARowHorizontally(Game.Player player)
        {
            var winningMoves =
                new List<List<Tuple<int, int>>>
                {
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(1, 1),
                        new Tuple<int, int>(1, 2),
                        new Tuple<int, int>(1, 3),
                    },
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(2, 1),
                        new Tuple<int, int>(2, 2),
                        new Tuple<int, int>(2, 3)
                    },
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(3, 1),
                        new Tuple<int, int>(3, 2),
                        new Tuple<int, int>(3, 3)
                    }
                };

            foreach (var winningMoveSet in winningMoves)
            {
                var game = new Game();
                var state = Move(game, player, winningMoveSet);
                var expectedState = player.Equals(Game.Player.X) ? GameState.PlayerXWon : GameState.PlayerOWon;
                state.Should().Be(expectedState);
            }
        }

        [Test]
        public void Move_ShouldReturnDraw()
        {
            var game = new Game();
 
            game.Move(Game.Player.X, 1, 1);
            game.Move(Game.Player.O, 1, 2);
            game.Move(Game.Player.X, 1, 3);
            game.Move(Game.Player.O, 2, 1);
            game.Move(Game.Player.X, 2, 2);
            game.Move(Game.Player.X, 2, 3);
            game.Move(Game.Player.O, 3, 1);
            game.Move(Game.Player.X, 3, 2);
            game.Move(Game.Player.O, 3, 3).Should().Be(GameState.Draw);
        }

        [TestCase(Game.Player.X)]
        [TestCase(Game.Player.O)]
        public void Move_ShouldReturnPlayerWon_WhenThreeInARowDiagonally(Game.Player player)
        {
            var game = new Game();

            var winningMoves =
                new List<Tuple<int, int>>
                {
                    new Tuple<int, int>(1, 1),
                    new Tuple<int, int>(2, 2),
                    new Tuple<int, int>(3, 3)
                };

            var state = Move(game, player, winningMoves);
            var expectedState = player.Equals(Game.Player.X) ? GameState.PlayerXWon : GameState.PlayerOWon;
            state.Should().Be(expectedState);
        }

        [TestCase(Game.Player.X)]
        [TestCase(Game.Player.O)]
        public void Move_ShouldReturnPlayerWon_WhenThreeInARowVertically(Game.Player player)
        {
            var winningMoves =
                new List<List<Tuple<int, int>>>
                {
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(1, 1),
                        new Tuple<int, int>(2, 1),
                        new Tuple<int, int>(3, 1),
                    },
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(1, 2),
                        new Tuple<int, int>(2, 2),
                        new Tuple<int, int>(3, 2)
                    },
                    new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(1, 3),
                        new Tuple<int, int>(2, 3),
                        new Tuple<int, int>(3, 3)
                    }
                };

            foreach (var winningMoveSet in winningMoves)
            {
                var game = new Game();
                var state = Move(game, player, winningMoveSet);
                var expectedState = player.Equals(Game.Player.X) ? GameState.PlayerXWon : GameState.PlayerOWon;
                state.Should().Be(expectedState);
            }
        }

        private GameState Move(Game game, Game.Player player, IList<Tuple<int, int>> moves)
        {
            GameState? returnState = null;
            foreach (var move in moves)
            {
                var isLast = moves.IndexOf(move) == moves.Count - 1;
                if (!isLast)
                {
                    game.Move(player, move.Item1, move.Item2).Should().Be(GameState.InProgress);
                }
                else
                {
                    returnState = game.Move(player, move.Item1, move.Item2);
                }
            }
            return returnState.Value;
        }
    }
}
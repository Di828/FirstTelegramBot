using FirstTelegramBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTelegramBot.Controllers
{
    public class TicTacToeController
    {
        private TicTakToe _TicTakToe { get; set; }
        public bool GameOver = false;
        private string[] _roles = { "X", "O" };
        private string _gameResult = "unknown";
        Random random = new Random();
        private string _emptyCell = "   ";
        private int[,] _corners =
        {
            {0, 0},
            {0, 2},
            {2, 2},
            {2, 0}
        };
        private int[,] _notCorners =
        {
            {0, 1},
            {1, 0},
            {1, 2},
            {2, 1}
        };
        private int[][] _directions = new int[4][]
        {
            new int[2] { 1, 0 },
            new int[2] {-1, 0 },
            new int[2] { 0, 1 },
            new int[2] { 0,-1 }
        };
        private int[] _opponentPreviousTurn = new int[2];
        private int[] _myPreviousTurn = new int[2];
        private int _turn = 1;
        public TicTacToeController()
        {
            _TicTakToe = new TicTakToe();
            var myRole = random.Next(2);
            _TicTakToe.Role = _roles[myRole];
            _TicTakToe.OpponentRole = _roles[1 - myRole];
        }
        public string StartGameMessage()
        {
            string result = $"Я буду играть {_TicTakToe.Role}-ом\n\r";
            if (_TicTakToe.Role == "X")
                MyTurn(1);
            result += PlayingFieldInterpretation();
            result += "  Твой ход :";
            return result;
        }

        private void MyTurn(int turn)
        {
            int x = -1;
            int y = -1;
            if (turn == 1)
                _TicTakToe.PlayingField[1, 1] = _TicTakToe.Role!;
            if (turn == 2)
            {
                if (_TicTakToe.PlayingField[1, 1] == _emptyCell)
                    _TicTakToe.PlayingField[1, 1] = _TicTakToe.Role!;
                else
                {
                    var corner = random.Next(4);
                    x = _corners[corner, 0];
                    y = _corners[corner, 1];
                    _TicTakToe.PlayingField[_corners[corner, 0], _corners[corner, 1]] = _TicTakToe.Role!;
                }
            }
            if (turn == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (_TicTakToe.PlayingField[_corners[i, 0], _corners[i, 1]] == _TicTakToe.OpponentRole)
                    {
                        _TicTakToe.PlayingField[2 - _corners[i, 0], 2 - _corners[i, 1]] = _TicTakToe.Role!;
                        _turn++;
                        _myPreviousTurn[0] = 2 - _corners[i, 0];
                        _myPreviousTurn[1] = 2 - _corners[i, 1];
                        return;
                    }
                }
                var corner = random.Next(4);
                _TicTakToe.PlayingField[_corners[corner, 0], _corners[corner, 1]] = _TicTakToe.Role!;
            }
            if (turn == 4)
            {
                if (CanWinNextTurn(_TicTakToe.OpponentRole!, out x, out y))
                {
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
                else if (OpponentTakeCorners(2))
                {
                    FindFreeCell(_notCorners, out x, out y);
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
                else if (OpponentTakeCorners(1) && OpponentTakeCenter())
                {
                    FindFreeCell(_corners, out x, out y);
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
                else
                {
                    FindAnyFreeCellNearPreviousTurn(_opponentPreviousTurn, out x, out y);
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
            }
            if (turn > 4)
            {
                if (CanWinNextTurn(_TicTakToe.Role!, out x, out y))
                {
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                    _gameResult = "Win";
                }
                else if (CanWinNextTurn(_TicTakToe.OpponentRole!, out x, out y))
                {
                    _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
                else
                {
                    FindAnyFreeCellNearPreviousTurn(_myPreviousTurn, out x, out y);
                    if (x == -1)
                        _gameResult = "Draw";
                    else
                        _TicTakToe.PlayingField[x, y] = _TicTakToe.Role!;
                }
            }
            _myPreviousTurn[0] = x;
            _myPreviousTurn[1] = y;
            _turn++;
            if (_turn > 9)
                _gameResult = "Draw";
            return;
        }
        private bool OpponentTakeCenter()
        {
            return _TicTakToe.PlayingField[1, 1] == _TicTakToe.OpponentRole;
        }
        private void FindAnyFreeCellNearPreviousTurn(int[] previousTurn, out int x, out int y)
        {
            foreach (var cell in _directions)
                if (CellIsEmptyAndInGame(previousTurn[0] - cell[0], previousTurn[1] - cell[1]))
                {
                    x = previousTurn[0] - cell[0];
                    y = previousTurn[1] - cell[1];
                    return;
                }
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (_TicTakToe.PlayingField[i, j] == _emptyCell)
                    {
                        x = i;
                        y = j;
                        return;
                    }
            x = -1;
            y = -1;
            return;
        }
        private bool CellIsEmptyAndInGame(int x, int y)
        {
            if (x < 0 || y < 0 || x > 2 || y > 2 || _TicTakToe.PlayingField[x, y] != _emptyCell)
                return false;
            return true;
        }
        private void FindFreeCell(int[,] cells, out int x, out int y)
        {
            for (int i = 0; i < 4; i++)
            {
                if (_TicTakToe.PlayingField[cells[i, 0], cells[i, 1]] == _emptyCell)
                {
                    x = cells[i, 0];
                    y = cells[i, 1];
                    return;
                }
            }
            x = -1;
            y = -1;
            return;
        }
        private bool OpponentTakeCorners(int opponentCorners)
        {
            int corners = 0;
            for (int i = 0; i < 4; i++)
                if (_TicTakToe.PlayingField[_corners[i, 0], _corners[i, 1]] == _TicTakToe.OpponentRole)
                    corners++;
            return corners >= opponentCorners ? true : false;
        }
        private bool CanWinNextTurn(string role, out int x, out int y)
        {
            x = -1;
            y = -1;

            int leftDiagonal = 0;
            int rightDiagonal = 0;
            for (int i = 0; i < 3; i++)
            {
                int rowSymbols = 0;
                int columnSymbols = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (_TicTakToe.PlayingField[i, j] == role)
                        columnSymbols++;
                    else if (_TicTakToe.PlayingField[i, j] != _emptyCell)
                        columnSymbols--;
                    if (_TicTakToe.PlayingField[j, i] == role)
                        rowSymbols++;
                    else if (_TicTakToe.PlayingField[j, i] != _emptyCell)
                        rowSymbols--;
                }
                if (rowSymbols == 2)
                {
                    for (int j = 0; j < 3; j++)
                        if (_TicTakToe.PlayingField[j, i] == _emptyCell)
                        {
                            x = j; y = i;
                            return true;
                        }
                }
                if (columnSymbols == 2)
                {
                    for (int j = 0; j < 3; j++)
                        if (_TicTakToe.PlayingField[i, j] == _emptyCell)
                        {
                            x = i; y = j;
                            return true;
                        }
                }
                if (_TicTakToe.PlayingField[i, i] == role)
                    leftDiagonal++;
                else if (_TicTakToe.PlayingField[i, i] != _emptyCell)
                    leftDiagonal--;
                if (_TicTakToe.PlayingField[i, 2 - i] == role)
                    rightDiagonal++;
                else if (_TicTakToe.PlayingField[i, 2 - i] != _emptyCell)
                    rightDiagonal--;
            }
            if (leftDiagonal == 2)
            {
                for (int i = 0; i < 3; i++)
                    if (_TicTakToe.PlayingField[i, i] == _emptyCell)
                    {
                        x = i; y = i;
                        return true;
                    }
            }
            if (rightDiagonal == 2)
            {
                for (int i = 0; i < 3; i++)
                    if (_TicTakToe.PlayingField[i, 2 - i] == _emptyCell)
                    {
                        x = i; y = 2 - i;
                        return true;
                    }
            }
            return false;
        }
        private string PlayingFieldInterpretation()
        {
            string result = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    _TicTakToe.PlayingFieldPresentation[((i + 1) * 2), ((j + 1) * 2)] = _TicTakToe.PlayingField[i, j];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                    result += _TicTakToe.PlayingFieldPresentation[i, j];
                result += "\n\r";
            }
            return result;
        }
        public string OpponentsTurn(string turn)
        {
            char[] turnArray = turn.ToUpper().ToCharArray();
            string result = "";
            if (turnArray.Length > 2)
            {
                result = "Ход должен состоять из 2 символов, например B2 соответствует ходу в центр";
                return "Ход должен состоять из 2 символов, например B2 соответствует ходу в центр";
            }
            int row = -1;
            int column = -1;
            InterpritateTurn(turnArray, out row, out column);            
            if (row < 3 && row >= 0 && column < 3 && column >= 0)
            {
                if (_TicTakToe.PlayingField[row, column] == _emptyCell)
                {
                    _TicTakToe.PlayingField[row, column] = _TicTakToe.OpponentRole!;
                    _opponentPreviousTurn[0] = row; _opponentPreviousTurn[1] = column;
                    _turn++;
                    result = "А я так : \n\r";
                    MyTurn(_turn);
                    result += PlayingFieldInterpretation();
                    if (_gameResult == "Win")
                    {
                        result += "\n\r Я Победил!";
                        GameOver = true;
                    }
                    else if (_gameResult == "Draw")
                    {
                        result += "\n\r Ничья!";
                        GameOver = true;
                    }
                    else
                        result += "\n\r Ходи :";
                }
                else
                    result = "Клетка уже занята";
            }
            else
                result = "Такой ход не возможен";
            return result;
        }
        private void InterpritateTurn(char[] turnArray, out int row, out int column)
        {
            row = -1;
            column = -1;
            foreach (var x in turnArray)
                if (char.IsDigit(x))
                    row = x - '1';
                else
                {
                    if (x - 'Z' < 0)
                        column = x - 'A';
                    else if (x == 'С')
                        column = 2;
                    else
                        column = x - 'А';
                }
            return;
        }
    }
}

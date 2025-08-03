using ChessLogicNS;
using ChessMoveNS;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace Chess
{
    public class ChessViewModel : INotifyPropertyChanged
    {
        private ChessLogic _chessLogic;
        private int[,] _board;
        public List<int> Board { get => FlattenBoard(); }
        public ChessViewModel()
        {
            _chessLogic = new ChessLogic();
            _board = _chessLogic.board.boardState;

            OnPropertyChanged(nameof(Board));
        }

        

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MakeMove(int fromX, int fromY, int toX, int toY)
        {
            _chessLogic.makeMove(new ChessMove(fromX, fromY, toX, toY));
            OnPropertyChanged(nameof(Board));
        }

        private List<int> FlattenBoard()
        {
            List<int> flattenedBoard = new List<int>();
            for (int row = 0; row < _board.GetLength(0); row++)
            {
                for (int col = 0; col < _board.GetLength(1); col++)
                {
                    flattenedBoard.Add(_board[row, col]);
                }
            }
            return flattenedBoard;
        }
    }
}

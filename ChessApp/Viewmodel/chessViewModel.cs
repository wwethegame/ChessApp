using ChessApp.Logic;
using ChessApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace ChessApp.Viewmodel
{
    public class ChessViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cell> Cells { get; } = new();
        private ChessLogic _logic = new ChessLogic();
        public Cell SelectedCell { get; private set; } = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public ChessViewModel()
        {
            InitializeBoard();

        }
        private void InitializeBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var cell = new Cell
                    {
                        Row = y,
                        Column = x,
                        PieceCode = _logic.board[x, y]
                    };
                    Cells.Add(cell);
                }
            }
        }
        private void UpdateBoard()
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                var cell = Cells[i];
                int newCode = _logic.board[cell.Column, cell.Row];
                if (cell.PieceCode != newCode)
                {
                    cell.PieceCode = newCode;
                    // Notify change manually if needed
                    //OnPropertyChanged(nameof(cell.ImageSource));
                }
            }
            //_logic.board.print();
        }
        public void PrintSelections()
        {
            for (int y = 0; y < 8; y++)
            {
                string row = "";
                for (int x = 0; x < 8; x++)
                {
                    var cell = Cells[y * 8 + x];
                    row += cell.IsSelected ? "1 " : "0 ";
                }
                Debug.WriteLine(row.Trim());
            }
        }



        public void handleClickRelease(Cell clickedCell)
        {
            if (this.SelectedCell is null)
            {
                return;
            }
            if (ReferenceEquals(this.SelectedCell, clickedCell))
            {
                return;
            }
            handleClick(clickedCell);
        }
        public void handleClick(Cell clickedCell)
        {
            // 1) First click: nothing selected yet
            if (SelectedCell == null)
            {

                // Don't allow selecting empty cells
                if (clickedCell.PieceCode == 0)
                    return;

                // Don't allow selecting opponent's pieces (assuming white = positive, black = negative)
                bool isWhiteTurn = _logic.board.isItWhitesTurn; // Add this property to your logic if not there yet
                bool isWhitePiece = clickedCell.PieceCode > 0;

                if (isWhitePiece != isWhiteTurn)
                    return;

                setSelectedCellPointer(clickedCell);


                return;

            }

            // 2) Second click received
            // If user clicked the same cell, deselect and reset
            if (SelectedCell == clickedCell)
            {
                removeSelectedCellPointer();
                return;
            }

            // 3) Real move: from _selectedCell → clickedCell
            bool moved = _logic.makeMove(
                SelectedCell.Column, SelectedCell.Row,
                clickedCell.Column, clickedCell.Row
            );

            // 4) Cleanup selection highlight
            removeSelectedCellPointer();
            // 5) If move succeeded, update the board
            if (moved)
            {
                UpdateBoard();
            }

        }

        private void OnPropertyChanged(string name)
        {
            Debug.WriteLine($"▶️ VM: PropertyChanged: {name}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void removeSelectedCellPointer()
        {
            var previouslySelectedCell = SelectedCell;
            SelectedCell.IsSelected = false;

            SelectedCell = null;
            OnPropertyChanged(nameof(previouslySelectedCell));


        }

        private void setSelectedCellPointer(Cell clickedCell)
        {

            SelectedCell = clickedCell;
            SelectedCell.IsSelected = true;
            OnPropertyChanged(nameof(SelectedCell));

        }
    }
}

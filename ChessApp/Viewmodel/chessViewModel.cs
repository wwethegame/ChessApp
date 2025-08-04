using ChessApp.Logic;
using ChessApp.Models;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace ChessApp.Viewmodel
{
    public class ChessViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cell> Cells { get; } = new();
        private ChessLogic _logic = new ChessLogic();
        private Cell _selectedCell = null;
        public event PropertyChangedEventHandler PropertyChanged;

        public ChessViewModel()
        {
            UpdateBoard();
            
        }

        private void UpdateBoard()
        {
            Cells.Clear();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Cells.Add(new Cell
                    {
                        Row = y,
                        Column = x,
                       

                        PieceCode = _logic.board[x, y]
                    });
                }
            }
            OnPropertyChanged(nameof(Cells));
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

        
        public void handleClick(Cell clickedCell)
        {
            // 1) First click: nothing selected yet
            if (_selectedCell == null)
            {
                _selectedCell = clickedCell;
                _selectedCell.IsSelected = true;
                // highlight in UI
               
                

                return;

            }

            // 2) Second click received
            // If user clicked the same cell, deselect and reset
            if (_selectedCell == clickedCell)
            {
                _selectedCell.IsSelected = false;
                _selectedCell = null;
                
                return;
            }

            // 3) Real move: from _selectedCell → clickedCell
            bool moved = _logic.makeMove(
                _selectedCell.Column, _selectedCell.Row,
                clickedCell.Column, clickedCell.Row
            );

            // 4) Cleanup selection highlight
            _selectedCell.IsSelected = false;
            _selectedCell = null;

            // 5) If move succeeded, update the board
            if (moved)
            {
                UpdateBoard();
            }
            
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

using ChessApp.Logic;
using ChessApp.Models;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ChessApp.Viewmodel
{
    public class ChessViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cell> Cells { get; } = new();
        private ChessLogic _logic = new ChessLogic();

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
                        Value = _logic.board[x, y].ToString()
                    });
                }
            }
            OnPropertyChanged(nameof(Cells));
        }

        

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

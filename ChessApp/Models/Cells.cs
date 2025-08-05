using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChessApp.Models
{
    public class Cell : INotifyPropertyChanged
    {
        public int Row { get; init; }
        public int Column { get; init; }


        private int _pieceCode;
        public int PieceCode
        {
            get => _pieceCode;
            set
            {
                if (_pieceCode != value)
                {
                    _pieceCode = value;
                    OnPropertyChanged(nameof(PieceCode));
                    OnPropertyChanged(nameof(ImageSource));
                }
            }
        }

        public string ImageSource => GetImagePath(PieceCode);

        //Makes it possible to set Image and the dragged image that appears at the cursors location while moving a piece independently.
        //also needed to fix a bug, that caused a deselection without movement of a piece to not hide the dragged image.
        public string DraggedImageSource => GetDraggedImagePath(IsSelected);
        private string GetImagePath(int code)
        {
            if (code == 0)
                return "ms-appx:///Assets/Chess/empty.svg"; // fallback if you have a blank tile

            var isWhite = code > 0;
            var colorSuffix = isWhite ? "w" : "b";

            var piecePrefix = Math.Abs(code) switch
            {
                1 => "pawn",
                2 => "knight",
                3 => "bishop",
                4 => "rook",
                5 => "queen",
                6 => "king",
                _ => "unknown"
            };
            return $"ms-appx:///Assets/Chess/{piecePrefix}-{colorSuffix}.svg";
        }
        private string GetDraggedImagePath(Boolean isSelected)
        {
            
            if (isSelected)
            {
                return GetImagePath(PieceCode);
            }
            else
            {
                return "ms-appx:///Assets/Chess/empty.svg";
            }
            
           
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(DraggedImageSource));
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        // Reusable static brushes to avoid repeated allocation
        private static readonly SolidColorBrush LightBrush = new(Colors.LightGray);
        private static readonly SolidColorBrush DarkBrush = new(Colors.DarkGray);
        private static readonly SolidColorBrush LightBrushText = new(Colors.White);
        private static readonly SolidColorBrush DarkBrushText = new(Colors.Black);
        private static SolidColorBrush CreateBrush(byte r, byte g, byte b) =>
        new SolidColorBrush(new Windows.UI.Color { A = 255, R = r, G = g, B = b });

        private static readonly SolidColorBrush LightBrushSelected = CreateBrush(190, 190, 190);
        private static readonly SolidColorBrush DarkBrushSelected = CreateBrush(105, 105, 105);

        public Brush Background =>
            IsSelected
                ? (Row + Column) % 2 == 0 ? LightBrushSelected : DarkBrushSelected
                : (Row + Column) % 2 == 0 ? LightBrush : DarkBrush;

        public Brush Foreground =>
            (Row + Column) % 2 == 0
                ? DarkBrushText
                : LightBrushText;


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            Debug.WriteLine($"▶️ Cell: PropertyChanged: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Models
{
    public class Cell
    {
        public int Row { get; init; }
        public int Column { get; init; }
      

        public int PieceCode { get; set; }

        public string ImageSource => GetImagePath(PieceCode);
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


        public bool IsSelected { get; set; }
        // Reusable static brushes to avoid repeated allocation
        private static readonly SolidColorBrush LightBrush = new(Colors.LightGray);
        private static readonly SolidColorBrush DarkBrush = new(Colors.DarkGray);
        private static readonly SolidColorBrush LightBrushText = new(Colors.White);
        private static readonly SolidColorBrush DarkBrushText = new(Colors.Black);
        private static SolidColorBrush CreateBrush(byte r, byte g, byte b) =>
        new SolidColorBrush(new Windows.UI.Color { A = 255, R = r, G = g, B = b });

        private static readonly SolidColorBrush LightBrushSelected = CreateBrush(169, 169, 169);
        private static readonly SolidColorBrush DarkBrushSelected = CreateBrush(105, 105, 105);

        public Brush Background =>
            IsSelected
                ? (Row + Column) % 2 == 0 ? LightBrushSelected : DarkBrushSelected
                : (Row + Column) % 2 == 0 ? LightBrush : DarkBrush;

        public Brush Foreground =>
            (Row + Column) % 2 == 0
                ? DarkBrushText
                : LightBrushText;
    }
}

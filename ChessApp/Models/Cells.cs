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
        public string Value { get; init; }

        // Reusable static brushes to avoid repeated allocation
        private static readonly SolidColorBrush LightBrush = new(Colors.White);
        private static readonly SolidColorBrush DarkBrush = new(Colors.Black);

        public Brush Background =>
            (Row + Column) % 2 == 0
                ? LightBrush
                : DarkBrush;

        public Brush Foreground =>
            (Row + Column) % 2 == 0
                ? DarkBrush
                : LightBrush;
    }
}

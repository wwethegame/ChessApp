using System;
using System.Collections.Generic;

namespace ChessBoardNS

{
    enum ChessColor
    {
        White = 1,
        Black = -1
    }
     class ChessBoard
    {
        public (int x, int y, ChessColor color)? enPassant { get; set; }
        public bool isItWhitesTurn { get; set; }

        public bool hasWhiteKingMoved;
        public bool hasBlackKingMoved;
        private static int[,] startingBoard = new int[8, 8] {
            { -4, -2, -3, -5, -6, -3, -2, -4 },  // Black's back row
            { -1, -1, -1, -1, -1, -1, -1, -1 },  // Black pawns
            {  0,  0,  0,  0,  0,  0,  0,  0 },  // Empty row 1
            {  0,  0,  0,  0,  0,  0,  0,  0 },  // Empty row 2
            {  0,  0,  0,  0,  0,  0,  0,  0 },  // Empty row 3
            {  0,  0,  0,  0,  0,  0,  0,  0 },  // Empty row 4
            {  1,  1,  1,  1,  1,  1,  1,  1 },  // White pawns
            {  4,  2,  3,  5,  6,  3,  2,  4 }   // White's back row
        };
        public int[,] boardState { get; private set; }
        // Piece Mapping:
        //  0  = Empty Field
        //  1  = Pawn
        //  2  = Knight
        //  3  = Bishop
        //  4  = Rook
        //  5  = Queen
        //  6  = King
        // Positive numbers represent White pieces, Negative numbers represent Black pieces.
        public int this[int x, int y]
        {
            get => boardState[y, x];
            set => boardState[y, x] = value;
        }
        public ChessBoard()
        {
            resetBoard();
        }
        public void resetBoard()
        {
            this.isItWhitesTurn = true;
            this.boardState = (int[,])startingBoard.Clone();
            this.hasWhiteKingMoved = false;
            this.hasBlackKingMoved = false;
        }
        public void print()
        {
            if (this.isItWhitesTurn)
            {
                Console.WriteLine("It's White's turn:");
            }
            else
            {
                Console.WriteLine("It's Black's turn:");
            }
            int rows = this.boardState.GetLength(0);  // Number of rows
            int columns = this.boardState.GetLength(1);  // Number of columns
            Dictionary<int, char> pieceMapping = new Dictionary<int, char>
                {
                    { 1, 'P' }, { 2, 'N' }, { 3, 'B' }, { 4, 'R' }, { 5, 'Q' }, { 6, 'K' },
                    { -1, 'p' }, { -2, 'n' }, { -3, 'b' }, { -4, 'r' }, { -5, 'q' }, { -6, 'k' },{ 0, '.' }
                };

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Console.Write(pieceMapping[this.boardState[y, x]] + " ");  // Print each element followed by a tab for spacing
                }
                Console.WriteLine();  // Move to the next line after printing each row
            }
            Console.WriteLine();
        }

        private string GetSymbol(int value)
        {
            return value switch
            {
                -1 => "♙",   // White Pawn
                -2 => "♘",
                -3 => "♗",
                -4 => "♖",
                -5 => "♕",
                -6 => "♔",
                1 => "♟",  // Black Pawn
                2 => "♞",
                3 => "♝",
                4 => "♜",
                5 => "♛",
                6 => "♚",
                0 => ""
            };
        }
    }
}
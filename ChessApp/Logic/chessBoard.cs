using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace ChessApp.Logic

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



        public Dictionary<(int x, int y), Boolean> castlePiecesMoved;

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
            isItWhitesTurn = true;
            boardState = (int[,])startingBoard.Clone();
            castlePiecesMoved = new Dictionary<(int x, int y), Boolean>
        {
            { (0, 0), false }, // left black rook
            { (7, 0), false }, //right black rook
            { (0, 7), false }, //left white rook
            { (7, 7), false }, //right white rook
            { (4, 0), false }, // black king
            { (4, 7), false }  //white king
        };
        }


        public (int x, int y) GetKingPosition(ChessColor color)
        {
            int targetKing = (int)color * 6; // 6 for White, -6 for Black

            for (int y = 0; y < boardState.GetLength(0); y++)
            {
                for (int x = 0; x < boardState.GetLength(1); x++)
                {
                    if (boardState[y, x] == targetKing)
                    {
                        return (x, y); // Return as soon as we find the King
                    }
                }
            }
            throw new InvalidOperationException("King not found on the board.");

        }



        public ChessBoard Clone()
        {
            ChessBoard copy = new ChessBoard();

            // Deep copy the board state
            copy.boardState = (int[,])this.boardState.Clone();

            // Copy other fields
            copy.isItWhitesTurn = this.isItWhitesTurn;
            copy.castlePiecesMoved = new Dictionary<(int x, int y), bool>(this.castlePiecesMoved);
            copy.enPassant = this.enPassant;

            return copy;
        }
        public void print()
        {
            if (isItWhitesTurn)
            {
                Debug.WriteLine("It's White's turn:");
            }
            else
            {
                Debug.WriteLine("It's Black's turn:");
            }
            int rows = boardState.GetLength(0);  // Number of rows
            int columns = boardState.GetLength(1);  // Number of columns
            Dictionary<int, char> pieceMapping = new Dictionary<int, char>
                {
                    { 1, 'P' }, { 2, 'N' }, { 3, 'B' }, { 4, 'R' }, { 5, 'Q' }, { 6, 'K' },
                    { -1, 'p' }, { -2, 'n' }, { -3, 'b' }, { -4, 'r' }, { -5, 'q' }, { -6, 'k' },{ 0, '.' }
                };

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    Debug.Write(pieceMapping[boardState[y, x]] + " ");  // Print each element followed by a tab for spacing
                }
                Debug.WriteLine("");  // Move to the next line after printing each row
            }
            Debug.WriteLine("");
        }


    }
}
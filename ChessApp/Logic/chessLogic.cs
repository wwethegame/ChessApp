using System;
using System.Collections.Generic;

namespace ChessApp.Logic
{
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() : base("Invalid Move!") { }

        public InvalidMoveException(string message) : base(message) { }

        public InvalidMoveException(string message, Exception innerException)
            : base(message, innerException) { }
    }
    class ChessLogic
    {


        public ChessBoard board { get; set; }
        public ChessLogic()
        {
            board = new ChessBoard();
        }

        public void makeMove(ChessMove move)
        {
            if (isMoveLegal(move))
            {


                board[move.destination.x, move.destination.y] = board[move.origin.x, move.origin.y];
                board[move.origin.x, move.origin.y] = 0;
                board.isItWhitesTurn = !board.isItWhitesTurn;

            }
            else
            {
                throw new InvalidMoveException();
            }
        }
        public bool isMoveLegal(ChessMove move)
        {
            int currentPiece = board[move.origin.x, move.origin.y];

            if (currentPiece == 0)//Origin is empty
            {
                return false;
            }
            if (currentPiece > 0 != board.isItWhitesTurn)//Origin piece isn't the same colour as the player whos turn it is.
            {
                return false;
            }
            List<(int, int)> viableDestinationList = getViableDestinations((move.origin.x, move.origin.y));
            if (viableDestinationList.Contains(move.destination))
            {
                return true;
            }

            return false;
        }
        public List<(int, int)> getViableDestinations((int x, int y) coords)
        {
            int chessPiece = board[coords.x, coords.y];
            int pieceColor = Math.Sign(chessPiece);
            int pieceType = Math.Abs(chessPiece);
            List<(int, int)> viableDestinations = new List<(int, int)>();
            (int, int)[] availableDirections;

            switch (pieceType)
            {
                case 0: //Empty Field
                    break;

                case 1: //Pawn
                    int pawnDirection = -pieceColor;
                    //EnPassant Check
                    if (board.enPassant?.color == (ChessColor)(-pieceColor))//Color check should be unnecessary... but better save then sorry i guess
                    {
                        if (
                            board.enPassant?.y == coords.y + pawnDirection &&
                            Math.Abs((int)board.enPassant?.x - coords.x) == 1
                            )
                        {
                            viableDestinations.Add(((int)board.enPassant?.x, (int)board.enPassant?.y));
                        }
                    }
                    //Double Move check
                    if (pieceColor == 1 && coords.y == 6 && board[coords.x, 5] == 0 && board[coords.x, 4] == 0)
                    {
                        viableDestinations.Add((coords.x, 4));
                    }
                    if (pieceColor == -1 && coords.y == 1 && board[coords.x, 2] == 0 && board[coords.x, 3] == 0)
                    {
                        viableDestinations.Add((coords.x, 3));
                    }
                    //Normal Movement
                    if (IsWithinBoardBounds((coords.x, coords.y + pawnDirection)) &&
                        board[coords.x, coords.y + pawnDirection] == 0)
                    {
                        viableDestinations.Add((coords.x, coords.y + pawnDirection));
                    }
                    //Capture Move
                    if (IsWithinBoardBounds((coords.x - 1, coords.y + pawnDirection)) &&
                        Math.Sign(board[coords.x - 1, coords.y + pawnDirection]) == -pieceColor)
                    {
                        viableDestinations.Add((coords.x - 1, coords.y + pawnDirection));
                    }
                    if (IsWithinBoardBounds((coords.x + 1, coords.y + pawnDirection)) &&
                        Math.Sign(board[coords.x + 1, coords.y + pawnDirection]) == -pieceColor)
                    {
                        viableDestinations.Add((coords.x + 1, coords.y + pawnDirection));
                    }
                    break;

                case 2: //Knight
                    (int, int)[] knightMoves =
                        [
                            (2, 1), (2, -1), (-2, 1), (-2, -1),
                            (1, 2), (1, -2), (-1, 2), (-1, -2)
                        ];

                    foreach ((int x, int y) kMove in knightMoves)
                    {
                        int currentExaminatedFieldX = coords.x + kMove.x;
                        int currentExaminatedFieldY = coords.y + kMove.y;
                        if (IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                        {
                            if (Math.Sign(board[currentExaminatedFieldX, currentExaminatedFieldY]) != pieceColor)
                            {
                                viableDestinations.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                            }
                        }
                    }
                    break;

                case 3: //Bishop
                    availableDirections =
                    [
                            (1, 1), (-1, -1), (-1, 1), (1, -1)
                        ];

                    viableDestinations = getViableDestinationsInDirections(coords, availableDirections);
                    break;

                case 4: //Rook
                    availableDirections =
                    [
                            (0, 1), (1,0), (0,-1), (-1,0)
                        ];

                    viableDestinations = getViableDestinationsInDirections(coords, availableDirections);
                    break;

                case 5: //Queen
                    availableDirections =
                    [       (1, 1), (-1, -1), (-1, 1), (1, -1),
                            (0, 1), (1,0), (0,-1), (-1,0)
                        ];

                    viableDestinations = getViableDestinationsInDirections(coords, availableDirections);
                    break;

                case 6: //King
                    availableDirections =
                        [       (1, 1), (-1, -1), (-1, 1), (1, -1),
                            (0, 1), (1,0), (0,-1), (-1,0)
                            ];


                    foreach ((int x, int y) direction in availableDirections)
                    {
                        int currentExaminatedFieldX = coords.x + direction.x;
                        int currentExaminatedFieldY = coords.y + direction.y;

                        if (IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                        {
                            int currentExaminatedFigure = board[currentExaminatedFieldX, currentExaminatedFieldY];
                            if (Math.Sign(currentExaminatedFigure) != pieceColor)
                            {
                                viableDestinations.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                            }

                        }
                    }

                    break;

                default:
                    throw new Exception("Unrecognized Chess Piece Type while calculating viable Moves!");

            }
            return viableDestinations;

        }

        /// <summary>
        /// Returns all empty Fields and the first field with an opposing Figure in all given directions. Useful for Figures that move in straight lines.
        /// </summary>
        private List<(int, int)> getViableDestinationsInDirections((int x, int y) coords, (int, int)[] availableDirections)

        {
            int chessPiece = board[coords.x, coords.y];
            int pieceColor = Math.Sign(chessPiece);
            List<(int, int)> viableDestinations = new List<(int, int)>();

            foreach ((int x, int y) direction in availableDirections)
            {
                int currentExaminatedFieldX = coords.x;
                int currentExaminatedFieldY = coords.y;

                for (int i = 1; i < 8; i++) //Avoiding while(true) in fear of infinite Loop
                {
                    currentExaminatedFieldX += direction.x;
                    currentExaminatedFieldY += direction.y;
                    if (!IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                    {
                        break;
                    }

                    int currentExaminatedFigure = board[currentExaminatedFieldX, currentExaminatedFieldY];

                    if (Math.Sign(currentExaminatedFigure) == pieceColor)
                    {
                        break;
                    }

                    if (Math.Sign(currentExaminatedFigure) == -pieceColor)
                    {
                        viableDestinations.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                        break;
                    }

                    viableDestinations.Add((currentExaminatedFieldX, currentExaminatedFieldY));

                }

            }

            return viableDestinations;
        }


        public List<(int, int)> isThreatenedBy((int x, int y) position)
        {
            int chessPiece = board[position.x, position.y];
            int pieceColor = Math.Sign(chessPiece);
            List<(int, int)> threateningPieces = new List<(int, int)>();
            (int, int)[] availableDirections;

            //Checking for Rooks and Queens
            availableDirections =
                [
                        (0, 1), (1,0), (0,-1), (-1,0)
                    ];
            foreach ((int x, int y) direction in availableDirections)
            {
                int currentExaminatedFieldX = position.x;
                int currentExaminatedFieldY = position.y;

                for (int i = 1; i < 8; i++) //Avoiding while(true) in fear of infinite Loop
                {
                    currentExaminatedFieldX += direction.x;
                    currentExaminatedFieldY += direction.y;
                    if (!IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                    {
                        break;
                    }

                    int currentExaminatedFigure = board[currentExaminatedFieldX, currentExaminatedFieldY];

                    if (Math.Sign(currentExaminatedFigure) == pieceColor)
                    {
                        break;
                    }

                    if (Math.Sign(currentExaminatedFigure) == -pieceColor &&
                        (Math.Abs(currentExaminatedFigure) == 4 || Math.Abs(currentExaminatedFigure) == 5))
                    {
                        threateningPieces.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                        break;
                    }
                }

            }

            //Checking for Bishops and Queens

            availableDirections =
                [
                        (1, 1), (-1, -1), (-1, 1), (1, -1)
                    ];
            foreach ((int x, int y) direction in availableDirections)
            {
                int currentExaminatedFieldX = position.x;
                int currentExaminatedFieldY = position.y;

                for (int i = 1; i < 8; i++) //Avoiding while(true) in fear of infinite Loop
                {
                    currentExaminatedFieldX += direction.x;
                    currentExaminatedFieldY += direction.y;
                    if (!IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                    {
                        break;
                    }

                    int currentExaminatedFigure = board[currentExaminatedFieldX, currentExaminatedFieldY];

                    if (Math.Sign(currentExaminatedFigure) == pieceColor)
                    {
                        break;
                    }

                    if (Math.Sign(currentExaminatedFigure) == -pieceColor &&
                        (Math.Abs(currentExaminatedFigure) == 3 || Math.Abs(currentExaminatedFigure) == 5))
                    {
                        threateningPieces.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                        break;
                    }
                }

            }
            //Checking for Knights
            (int, int)[] knightMoves =
                        [
                            (2, 1), (2, -1), (-2, 1), (-2, -1),
                            (1, 2), (1, -2), (-1, 2), (-1, -2)
                        ];

            foreach ((int x, int y) kMove in knightMoves)
            {
                int currentExaminatedFieldX = position.x + kMove.x;
                int currentExaminatedFieldY = position.y + kMove.y;

                if (IsWithinBoardBounds((currentExaminatedFieldX, currentExaminatedFieldY)))
                {
                    int currentExaminatedFigure = board[currentExaminatedFieldX, currentExaminatedFieldY];
                    if (Math.Sign(currentExaminatedFigure) == -pieceColor &&
                        Math.Abs(currentExaminatedFigure) == 2)
                    {
                        threateningPieces.Add((currentExaminatedFieldX, currentExaminatedFieldY));
                    }
                }
            }
            //Checking for Pawns
            int pawnDirection = -pieceColor;
            if (IsWithinBoardBounds((position.x - 1, position.y + pawnDirection)) &&
                        Math.Sign(board[position.x - 1, position.y + pawnDirection]) == -pieceColor)
            {
                threateningPieces.Add((position.x - 1, position.y + pawnDirection));
            }
            if (IsWithinBoardBounds((position.x + 1, position.y + pawnDirection)) &&
                Math.Sign(board[position.x + 1, position.y + pawnDirection]) == -pieceColor)
            {
                threateningPieces.Add((position.x + 1, position.y + pawnDirection));
            }
            //Checking for EnPassante
            //ToDo


            return threateningPieces;

        }

        public bool IsWithinBoardBounds((int x, int y) position)
        {
            if (position.x >= 0 &&
                position.x <= 7 &&
                position.y >= 0 &&
                position.y <= 7)
            {
                return true;
            }
            return false;
        }
    }
}
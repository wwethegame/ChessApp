using System;
using System.Collections.Generic;

namespace ChessApp.Logic
{

    class ChessLogic
    {


        public ChessBoard board { get; set; }
        public ChessLogic()
        {
            board = new ChessBoard();
        }

        /// <summary>
        /// Returns 1 if white won, -1 if Black won, 0 if its a draw and null if its still not decided.
        /// </summary>
        public int? getWinner() //
        {
            int currentPlayer = (board.isItWhitesTurn) ? 1 : -1;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (Math.Sign(board[x, y]) == currentPlayer)
                    {
                        var availableMoves = getViableDestinations((x, y));
                        if (availableMoves.Count > 0)
                        {
                            return null;
                        }
                    }

                }
            }
            if (getThreatsToKing((ChessColor)currentPlayer).Count > 0)
            {
                return -currentPlayer;
            }
            return 0;
        }
        public Boolean makeMove(ChessMove move)
        {
            if (isMoveLegal(move))



            {
                //En Passant Logic
                if (Math.Abs(board[move.origin.x, move.origin.y]) == 1 && move.destination == board.enPassant?.coords) //Make En Passant capture
                {
                    board[move.destination.x, move.destination.y] = board[move.origin.x, move.origin.y];
                    board[move.origin.x, move.origin.y] = 0;

                    if (board.enPassant is { coords: var c, color: var col })
                    {
                        board[c.x, c.y - Math.Sign((int)col)] = 0;
                    }

                    board.isItWhitesTurn = !board.isItWhitesTurn;
                    board.enPassant = null;
                    return true;
                }
                board.enPassant = null;

                if (Math.Abs(board[move.origin.x, move.origin.y]) == 1 && Math.Abs(move.origin.y - move.destination.y) == 2) // Make double Pawn move and mark field as en passantable
                {
                    board.enPassant = ((move.destination.x,
                        move.destination.y + Math.Sign(board[move.origin.x, move.origin.y])),
                        (ChessColor)Math.Sign(board[move.origin.x, move.origin.y]));
                }

                //King Logic
                if (board.castlePiecesMoved.Keys.Contains(move.origin))
                {
                    board.castlePiecesMoved[move.origin] = true;
                }

                if (board.castlePiecesMoved.Keys.Contains(move.destination))
                {
                    board.castlePiecesMoved[move.destination] = true;
                }



                if (Math.Abs(board[move.origin.x, move.origin.y]) == 6 && Math.Abs(move.origin.x - move.destination.x) == 2)// Check for Castle Move
                {
                    int rookX = move.destination.x - move.origin.x > 0 ? 7 : 0;
                    int rookXdest = move.destination.x - move.origin.x > 0 ? 5 : 3;
                    int rookY = move.origin.y;
                    board[move.destination.x, move.destination.y] = board[move.origin.x, move.origin.y];
                    board[rookXdest, rookY] = board[rookX, rookY];
                    board[move.origin.x, move.origin.y] = 0;
                    board[rookX, rookY] = 0;
                    board.isItWhitesTurn = !board.isItWhitesTurn;
                }
                else
                {
                    board[move.destination.x, move.destination.y] = board[move.origin.x, move.origin.y];
                    board[move.origin.x, move.origin.y] = 0;
                    board.isItWhitesTurn = !board.isItWhitesTurn;
                }






                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean makeMove(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            ChessMove move = new ChessMove(xOrigin, yOrigin, xDestination, yDestination);
            return makeMove(move);
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
            ChessBoard backupBoard = board.Clone();

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
                            board.enPassant?.coords.y == coords.y + pawnDirection &&
                            Math.Abs((int)board.enPassant?.coords.x - coords.x) == 1
                            )
                        {
                            viableDestinations.Add(((int)board.enPassant?.coords.x, (int)board.enPassant?.coords.y));
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


                    //Castle movement 

                    int rank = (pieceColor == -1) ? 0 : (pieceColor == 1 ? 7 : throw new ArgumentException("Error while determening color in castle move check!")); //determining the rank of the castle move

                    if (board.castlePiecesMoved[(4, rank)] == false) //if king hasnt moved
                    {
                        if (getThreatsToKing((ChessColor)pieceColor).Count == 0)//Cant castle out of check
                        {


                            if (board.castlePiecesMoved[(0, rank)] == false) //if left rook hasnt moved
                            {
                                if (board[1, rank] == 0 && board[2, rank] == 0 && board[3, rank] == 0) //check if fields between king and rook are empty
                                {
                                    board[3, rank] = pieceType * pieceColor; //Fields that the king "skipps" cant be threatened
                                    board[4, rank] = 0;
                                    if (getThreatsToKing((ChessColor)pieceColor).Count == 0)
                                    {
                                        viableDestinations.Add((2, rank));
                                    }
                                    board = backupBoard.Clone();

                                }

                            }
                            if (board.castlePiecesMoved[(7, rank)] == false)// if right rook hasnt moved
                            {
                                if (board[5, rank] == 0 && board[6, rank] == 0) //check if fields between king and rook are empty
                                {
                                    board[5, rank] = pieceType * pieceColor; //Fields that the king "skipps" cant be threatened
                                    board[4, rank] = 0;
                                    if (getThreatsToKing((ChessColor)pieceColor).Count == 0)
                                    {
                                        viableDestinations.Add((6, rank));
                                    }
                                    board = backupBoard.Clone();

                                }
                            }

                        }
                    }

                    break;

                default:
                    throw new Exception("Unrecognized Chess Piece Type while calculating viable Moves!");

            }
            // Check if your king is threatened after moving the piece

            List<(int, int)> safeDestinations = new List<(int x, int y)>(); //Final list with destinations the figure can move to without putting your own king in check
            foreach ((int x, int y) destination in viableDestinations)
            {

                board[destination.x, destination.y] = board[coords.x, coords.y];
                board[coords.x, coords.y] = 0;



                if (getThreatsToKing((ChessColor)pieceColor).Count == 0)
                {
                    safeDestinations.Add(destination);
                }
                board = backupBoard.Clone();


            }


            return safeDestinations;

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


        public List<(int, int)> getThreatsToKing(ChessColor color)
        {
            (int x, int y) position = board.GetKingPosition(color);
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
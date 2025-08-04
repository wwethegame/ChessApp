using System;

namespace ChessApp.Logic
{

    class ChessMove
    {
        public (int x, int y) origin { get; }
        public (int x, int y) destination { get; }
        public ChessMove(int originX, int originY, int destinationX, int destinationY)
        {
            if (originX >= 0 && originX <= 7 &&
                originY >= 0 && originY <= 7 &&
                destinationX >= 0 && destinationX <= 7 &&
                destinationY >= 0 && destinationY <= 7)
            {
                origin = (originX, originY);
                destination = (destinationX, destinationY);
            }
            else
            {
                throw new IndexOutOfRangeException("Move coordinates out of bounds!");
            }

        }
        public ChessMove(string move, ChessBoard board)
        {

        }

        public ChessMove((int x, int y) origin, (int x, int y) destination) : this(origin.x, origin.y, destination.x, destination.y)
        {

        }

    }

}
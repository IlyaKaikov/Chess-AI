namespace ChessAI.Core
{
    using ChessAI.Pieces;
    using UnityEngine;

    public static class FlagUpdater
    {
        public static void UpdateCastlingFlags(Board board, Vector2Int from, Vector2Int to, int piece, bool isWhiteTurn)
        {
            // Handle moving the rook
            if (Piece.PieceType(piece) == Piece.Rook)
            {
                if (isWhiteTurn)
                {
                    if (from.x == 7 && from.y == 0 && board.WhiteAllowedToCastleShort())
                    {
                        board.WhiteDisallowToCastleShort();
                    }
                    else if (from.x == 0 && from.y == 0 && board.WhiteAllowedToCastleLong())
                    {
                        board.WhiteDisallowToCastleLong();
                    }
                }
                else
                {
                    if (from.x == 7 && from.y == 7 && board.BlackAllowedToCastleShort())
                    {
                        board.BlackDisallowToCastleShort();
                    }
                    else if (from.x == 0 && from.y == 7 && board.BlackAllowedToCastleLong())
                    {
                        board.BlackDisallowToCastleLong();
                    }
                }
            }
            // Handle moving the king
            else if (Piece.PieceType(piece) == Piece.King)
            {
                if (isWhiteTurn)
                {
                    board.WhiteDisallowToCastleShort();
                    board.WhiteDisallowToCastleLong();
                }
                else
                {
                    board.BlackDisallowToCastleShort();
                    board.BlackDisallowToCastleLong();
                }
            }
            // Handle capturing the opponent's rook
            int targetPiece = board.GetPieceAt(to);
            if (Piece.PieceType(targetPiece) == Piece.Rook)
            {
                if (to.x == 7 && to.y == 0 && board.WhiteAllowedToCastleShort())
                {
                    board.WhiteDisallowToCastleShort();
                }
                else if (to.x == 0 && to.y == 0 && board.WhiteAllowedToCastleLong())
                {
                    board.WhiteDisallowToCastleLong();
                }
                else if (to.x == 7 && to.y == 7 && board.BlackAllowedToCastleShort())
                {
                    board.BlackDisallowToCastleShort();
                }
                else if (to.x == 0 && to.y == 7 && board.BlackAllowedToCastleLong())
                {
                    board.BlackDisallowToCastleLong();
                }
            }
        }

        public static void UpdateEnPassantFlags(Board board, int piece, Vector2Int from, Vector2Int to)
        {
            board.SetEnPassantRow(-1);

            if (Piece.PieceType(piece) == Piece.Pawn)
            {
                // White pawn double step
                if (Piece.IsColor(piece, Piece.White) && from.y == 1 && to.y == 3 && from.x == to.x)
                {
                    board.SetEnPassantRow(to.x);
                }
                // Black pawn double step
                else if (Piece.IsColor(piece, Piece.Black) && from.y == 6 && to.y == 4 && from.x == to.x)
                {
                    board.SetEnPassantRow(to.x);
                }
            }
        }
    }
}

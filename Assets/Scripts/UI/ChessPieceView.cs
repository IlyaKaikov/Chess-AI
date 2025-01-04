namespace ChessAI.UI
{
    using UnityEngine;

    public class ChessPieceView : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public Sprite whiteKing;
        public Sprite blackKing;
        public Sprite whiteQueen;
        public Sprite blackQueen;
        public Sprite whiteRook;
        public Sprite blackRook;
        public Sprite whiteBishop;
        public Sprite blackBishop;
        public Sprite whiteKnight;
        public Sprite blackKnight;
        public Sprite whitePawn;
        public Sprite blackPawn;

        /// <summary>
        /// Sets the sprite for the chess piece based on type and color.
        /// </summary>
        public void SetPieceSprite(int pieceType)
        {
            spriteRenderer.sprite = pieceType switch
            {
                Pieces.Piece.White | Pieces.Piece.King => whiteKing,
                Pieces.Piece.Black | Pieces.Piece.King => blackKing,
                Pieces.Piece.White | Pieces.Piece.Queen => whiteQueen,
                Pieces.Piece.Black | Pieces.Piece.Queen => blackQueen,
                Pieces.Piece.White | Pieces.Piece.Rook => whiteRook,
                Pieces.Piece.Black | Pieces.Piece.Rook => blackRook,
                Pieces.Piece.White | Pieces.Piece.Bishop => whiteBishop,
                Pieces.Piece.Black | Pieces.Piece.Bishop => blackBishop,
                Pieces.Piece.White | Pieces.Piece.Knight => whiteKnight,
                Pieces.Piece.Black | Pieces.Piece.Knight => blackKnight,
                Pieces.Piece.White | Pieces.Piece.Pawn => whitePawn,
                Pieces.Piece.Black | Pieces.Piece.Pawn => blackPawn,
                _ => null,// No piece
            };
        }
    }
}

using System.Linq;
using UnityEngine;

public class Rook : Piece
{
    public Rook(bool whiteOrBlack, bool hasMoved = false) : base(whiteOrBlack, hasMoved) { }
    
    public override Piece Copy() => new Rook(whiteOrBlack, hasMoved);
    public override PieceType GetPieceType() => PieceType.Rook;
    public override int GetPieceValue() => 5;
    public override Sprite GetSprite() => Chess.I.rook[whiteOrBlack ? 0 : 1];
    
    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        ContinuousLines(boardState, currentSquare,
            new [] {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            });
}
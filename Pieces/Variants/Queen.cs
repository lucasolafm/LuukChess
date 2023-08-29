using System.Linq;
using UnityEngine;

public class Queen : Piece
{
    public Queen(bool whiteOrBlack) : base(whiteOrBlack) { }
    
    public override Piece Copy() => new Queen(whiteOrBlack);
   
    public override PieceType GetPieceType() => PieceType.Queen;
   
    public override int GetPieceValue() => 9;

    public override Sprite GetSprite() => Chess.I.queen[whiteOrBlack ? 0 : 1];

    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        ContinuousLines(boardState, currentSquare,
            new[]
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.up + Vector2Int.right,
                Vector2Int.right + Vector2Int.down,
                Vector2Int.down + Vector2Int.left,
                Vector2Int.left + Vector2Int.up
            });
}
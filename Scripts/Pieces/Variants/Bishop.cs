using System.Linq;
using UnityEngine;

public class Bishop : Piece
{
    public Bishop(bool whiteOrBlack) : base(whiteOrBlack) { }

    public override Piece Copy() => new Bishop(whiteOrBlack);

    public override PieceType GetPieceType() => PieceType.Bishop;

    public override int GetPieceValue() => 3;

    public override Sprite GetSprite() => Chess.I.bishop[whiteOrBlack ? 0 : 1];

    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        ContinuousLines(boardState, currentSquare,
            new[]
            {
                Vector2Int.up + Vector2Int.right,
                Vector2Int.right + Vector2Int.down,
                Vector2Int.down + Vector2Int.left,
                Vector2Int.left + Vector2Int.up
            });
}
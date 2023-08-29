using System.Linq;
using UnityEngine;

public class Knight : Piece
{
    public Knight(bool whiteOrBlack) : base(whiteOrBlack) { }
    
    public override Piece Copy() => new Knight(whiteOrBlack);
    public override PieceType GetPieceType() => PieceType.Knight;
    public override int GetPieceValue() => 3;
    public override Sprite GetSprite() => Chess.I.knight[whiteOrBlack ? 0 : 1];

    public override Vector2Int[] GetSquaresAttacking(BoardState boardState, Vector2Int currentSquare) =>
        new[]
        {
            currentSquare + Vector2Int.up * 2 + Vector2Int.right,
            currentSquare + Vector2Int.right * 2 + Vector2Int.up,
            currentSquare + Vector2Int.right * 2 + Vector2Int.down,
            currentSquare + Vector2Int.down * 2 + Vector2Int.right,
            currentSquare + Vector2Int.down * 2 + Vector2Int.left,
            currentSquare + Vector2Int.left * 2 + Vector2Int.down,
            currentSquare + Vector2Int.left * 2 + Vector2Int.up,
            currentSquare + Vector2Int.up * 2 + Vector2Int.left
        }.Where(boardState.IsInsideBoard).ToArray();
}
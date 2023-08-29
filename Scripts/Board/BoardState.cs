using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardState
{
    private Piece[][] board;
    private Piece[][] previousBoard;
    private Vector2Int squareMovedTo;

    public BoardState()
    {
        board = new Piece[8][];
        for (int i = 0; i < board.Length; i++) board[i] = new Piece[8];

        board[0][0] = new Rook(true);
        board[1][0] = new Knight(true);
        board[2][0] = new Bishop(true);
        board[3][0] = new Queen(true);
        board[4][0] = new King(true);
        board[5][0] = new Bishop(true);
        board[6][0] = new Knight(true);
        board[7][0] = new Rook(true);
        for (int i = 0; i < board.Length; i++) board[i][1] = new Pawn(true);
        board[0][7] = new Rook(false);
        board[1][7] = new Knight(false);
        board[2][7] = new Bishop(false);
        board[3][7] = new Queen(false);
        board[4][7] = new King(false);
        board[5][7] = new Bishop(false);
        board[6][7] = new Knight(false);
        board[7][7] = new Rook(false);
        for (int i = 0; i < board.Length; i++) board[i][6] = new Pawn(false);
    }

    public BoardState(BoardState original)
    {
        board = CopyBoard(original);
    }

    public Piece[][] GetBoard() => board;
    public Piece[][] GetPreviousBoard() => previousBoard;
    public Vector2Int GetSquareMovedTo() => squareMovedTo;
    
    public bool IsInsideBoard(Vector2Int position) =>
        position.x >= 0 && position.y >= 0 && position.x < board.Length && position.y < board.Length;
    
    public Piece GetPiece(Vector2Int square, Piece[][] board = null)
    {
        board ??= this.board;
        return IsInsideBoard(square) ? board[square.x][square.y] : null;
    }

    public void SetPiece(Vector2Int square, Piece piece) => board[square.x][square.y] = piece;
    
    public bool HasPiece(Vector2Int square) => GetPiece(square) != null;
    
    public bool HasPiece(Vector2Int square, bool whiteOrBlack, Piece[][] board = null)
    {
        board ??= this.board;
        Piece piece = GetPiece(square, board);
        return piece != null && piece.IsWhiteOrBlack() == whiteOrBlack;
    }

    public Vector2Int[] GetSquares(Vector2Int squarePiece) =>
        GetPiece(squarePiece).GetSquares(this, squarePiece);

    public Vector2Int[] GetSquaresAttacking(Vector2Int squarePiece) =>
        GetPiece(squarePiece).GetSquaresAttacking(this, squarePiece);
    
    public Vector2Int[] GetPiecesSquares(bool whiteOrBlack, Piece[][] board = null)
    {
        board ??= this.board;
        List<Vector2Int> squares = new List<Vector2Int>();
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                Vector2Int square = new Vector2Int(i, j);
                Piece piece = GetPiece(square, board);
                if (piece == null || piece.IsWhiteOrBlack() != whiteOrBlack) continue;
                
                squares.Add(square);
            }
        }

        return squares.ToArray();
    }

    public Vector2Int GetKingSquare(bool whiteOrBlack) =>
        GetPiecesSquares(whiteOrBlack)
            .First(t => GetPiece(t).GetPieceType() == PieceType.King);

    public Vector2Int[] GetAttackers(Vector2Int squareAttacked, bool whiteOrBlack)
    {
        List<Vector2Int> attackers = new();
        foreach (Vector2Int squareAttacker in GetPiecesSquares(whiteOrBlack))
        {
            foreach (Vector2Int attackingSquare in GetSquaresAttacking(squareAttacker))
            {
                if (attackingSquare != squareAttacked) continue;
                
                attackers.Add(squareAttacker);
            }
        }

        return attackers.ToArray();
    }

    public bool IsAttacked(Vector2Int square, bool whiteOrBlack) =>
        GetAttackers(square, !whiteOrBlack).Length > 0;

    public void MovePiece(Vector2Int originalSquare, Vector2Int targetSquare)
    {
        previousBoard = CopyBoard(this);
        squareMovedTo = targetSquare;
        Piece piece = GetPiece(originalSquare);
        SetPiece(targetSquare, piece);
        SetPiece(originalSquare, null);
        piece.OnMove(targetSquare, this);
    }

    private Piece[][] CopyBoard(BoardState original)
    {
        Piece[][] newBoard = new Piece[8][];
        for (int i = 0; i < newBoard.Length; i++) newBoard[i] = new Piece[8];
        
        for (int i = 0; i < original.board.Length; i++)
        {
            for (int j = 0; j < original.board[i].Length; j++)
            {
                newBoard[i][j] = original.GetPiece(new Vector2Int(i, j))?.Copy();
            }
        }

        return newBoard;
    }
}
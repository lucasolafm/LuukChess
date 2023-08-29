using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Chess : MonoBehaviour
{
    public static Chess I;

    [SerializeField] private bool useComputerPlayer;
    public Sprite[] pawn;
    public Sprite[] knight;
    public Sprite[] bishop;
    public Sprite[] rook;
    public Sprite[] queen;
    public Sprite[] king;
    [SerializeField] private Transform inCheckPointer;
    [SerializeField] private float minTimeComputerThink;
    [SerializeField] private float maxTimeComputerThink;

    private PhysicalBoardManager boardManager;
    private PointerManager pointerManager;
    private Camera camera;
    private BoardState currentBoardState;
    private bool turnWhiteOrBlack;
    private Vector2Int selectedSquare;
    private bool isSelected;
    private Vector2Int[] legalSquares = {};
    private bool isGameOver;

    void Awake()
    {
        I = this;
        boardManager = GetComponent<PhysicalBoardManager>();
        pointerManager = new PointerManager();
        camera = Camera.main;
    }
    
    void Start()
    {
        BoardState boardState = new BoardState();
        Piece[][] board = boardState.GetBoard();
        boardManager.SpawnBoard(board);

        currentBoardState = boardState;
        boardManager.UpdatePhysicalBoard(currentBoardState);
        PassTurn();

        camera.transform.position += new Vector3((board.Length - 1) / 2f, (board.Length - 1) / 2f);
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || isGameOver) return;
        
        Vector3 worldMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int currentSquare =
            new Vector2Int(Mathf.RoundToInt(worldMousePos.x), Mathf.RoundToInt(worldMousePos.y));

        if (!currentBoardState.IsInsideBoard(currentSquare))
        {
            pointerManager.HidePointers();
            legalSquares = new Vector2Int[]{};
            return;
        }

        Piece piece = currentBoardState.GetPiece(currentSquare);
        if (isSelected && currentSquare == selectedSquare)
        {
            pointerManager.HidePointers();
            legalSquares = new Vector2Int[]{};
            isSelected = false;
        }
        else if (piece != null && piece.IsWhiteOrBlack() == turnWhiteOrBlack)
        {
            selectedSquare = currentSquare;
            isSelected = true;
            legalSquares = GetLegalMoves(currentBoardState, selectedSquare);
            pointerManager.HidePointers();
            pointerManager.ShowPointers(legalSquares, currentBoardState);
        }
        else if (legalSquares.Contains(currentSquare))
        {
            MakeAMove(selectedSquare, currentSquare);
            pointerManager.HidePointers();
            legalSquares = new Vector2Int[]{};
        }
        else
        {
            pointerManager.HidePointers();
            legalSquares = new Vector2Int[]{};
            isSelected = false;
        }
    }
    
    public Vector2Int[] GetMoves(BoardState boardState, bool whiteOrBlack, Vector2Int squareToMove,
        List<Func<BoardState, bool, bool>> problemsPersists)
    {
        problemsPersists.Insert(0, BoardCondition.IsInCheck);
        List<Vector2Int> squares = new List<Vector2Int>();
        
        foreach (Vector2Int square in boardState.GetSquares(squareToMove))
        {
            BoardState newBoardState = new BoardState(currentBoardState);
            newBoardState.MovePiece(squareToMove, square);

            if (problemsPersists.Any(t => t(newBoardState, whiteOrBlack))) continue;

            squares.Add(square);
        }

        return squares.ToArray();
    }

    public Vector2Int[] GetLegalMoves(BoardState boardState, Vector2Int square)
    {
        return GetMoves(boardState, boardState.GetPiece(square).IsWhiteOrBlack(), square,
            new List<Func<BoardState, bool, bool>>());
    }

    public void MakeAMove(Vector2Int originalSquare, Vector2Int targetSquare)
    {
        currentBoardState.MovePiece(originalSquare, targetSquare);
        AudioManager.I.OnMadeMove(turnWhiteOrBlack);
        
        inCheckPointer.gameObject.SetActive(false);
        if (BoardCondition.IsInCheck(currentBoardState, !turnWhiteOrBlack))
        {
            AudioManager.I.OnGaveCheck();
            inCheckPointer.gameObject.SetActive(true);
            inCheckPointer.position = (Vector3Int)currentBoardState.GetKingSquare(!turnWhiteOrBlack);
            
            if (BoardCondition.IsCheckMate(currentBoardState, !turnWhiteOrBlack))
            {
                AudioManager.I.OnCheckMated();
                isGameOver = true;
            }
        }

        if (currentBoardState.HasPiece(targetSquare, !turnWhiteOrBlack, currentBoardState.GetPreviousBoard()))
        {
            AudioManager.I.OnCapture();
        }
        
        AudioManager.I.OnTurnPlayed();
        boardManager.UpdatePhysicalBoard(currentBoardState);
        PassTurn();
    }

    public void Quit() => Application.Quit();
    
    public void Restart() => SceneManager.LoadScene(0);

    private IEnumerator ComputerThinking(Action completed)
    {
        yield return new WaitForSeconds(Random.Range(minTimeComputerThink, maxTimeComputerThink));
        completed();
    }

    private void PassTurn()
    {
        if (isGameOver) return;
        
        turnWhiteOrBlack = !turnWhiteOrBlack;
        if (useComputerPlayer && !turnWhiteOrBlack)
        {
            StartCoroutine(ComputerThinking(() => ComputerPlayer.Play(currentBoardState, false)));
        }
    }
}

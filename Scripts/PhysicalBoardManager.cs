using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PhysicalBoardManager : MonoBehaviour
{
    [SerializeField] private GameObject squareWhite;
    [SerializeField] private GameObject squareBlack;
    [SerializeField] private float timeAnimation;
    
    private Dictionary<Piece, Transform> transformOfPiece = new ();
    private Dictionary<Vector2Int, SpriteRenderer> rendererOfSquare = new ();
    
    public void UpdatePhysicalBoard(BoardState boardState)
    {
        foreach (Transform pieceTransform in transformOfPiece.Values)
        {
            pieceTransform.gameObject.SetActive(false);
        }

        Piece[][] board = boardState.GetBoard();
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                Vector2Int square = new Vector2Int(i, j);
                Piece piece = boardState.GetPiece(square);
                if (piece == null) continue;

                transformOfPiece.TryGetValue(piece, out Transform pieceTransform);
                if (!pieceTransform)
                {
                    SpawnPiece(piece, square);
                    continue;
                }
                
                StartCoroutine(AnimatePiece(pieceTransform, (Vector3Int)square));
                pieceTransform.gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator AnimatePiece(Transform transformPiece, Vector3Int targetPosition)
    {
        Vector3 origin = transformPiece.position;
        float t = 0;
        while (t < 1)
        {
            t = Mathf.Min(t + Time.deltaTime / timeAnimation, 1);
            transformPiece.position = Vector3.Lerp(origin, targetPosition, -(Mathf.Cos(Mathf.PI * t) - 1) / 2);
            yield return null;
        }
    }

    public void SpawnBoard(Piece[][] board)
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                Vector2Int position = new Vector2Int(i, j);
                
                SpriteRenderer squareRenderer = SpawnSquare((i + (j % 2 == 0 ? 0 : 1)) % 2 != 0, position);
                rendererOfSquare[position] = squareRenderer;
            }
        }
    }
    
    public SpriteRenderer SpawnSquare(bool whiteOrBlack, Vector2Int position)
    {
        return Instantiate(whiteOrBlack ? squareWhite : squareBlack, 
                (Vector3Int)position, quaternion.identity)
            .GetComponent<SpriteRenderer>();
    }
    
    public void SpawnPiece(Piece piece, Vector2Int position)
    {
        GameObject obj = new GameObject();
        Transform objTransform = obj.transform;
        objTransform.position = (Vector3Int)position;
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = piece.GetSprite();
        renderer.sortingOrder = 2;
        transformOfPiece[piece] = objTransform;
    }
}
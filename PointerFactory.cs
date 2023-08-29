using System.Collections.Generic;
using UnityEngine;

public class PointerFactory : MonoBehaviour
{
    public static PointerFactory I;
    
    [SerializeField] private Transform prefabSquarePointer;
    [SerializeField] private Transform prefabPiecePointer;
    
    private Stack<Transform> poolSquarePointer = new ();
    private Stack<Transform> poolPiecePointer = new ();

    void Awake()
    {
        I = this;
    }

    public Transform GetSquarePointer()
    {
        Transform pointer = poolSquarePointer.Count == 0 ? Instantiate(prefabSquarePointer) : poolSquarePointer.Pop();
        pointer.gameObject.SetActive(true);
        return pointer;
    }

    public void ReturnSquarePointers(List<Transform> pointers)
    {
        foreach (var pointer in pointers)
        {
            pointer.gameObject.SetActive(false);
            poolSquarePointer.Push(pointer);
        }
    }
    
    public Transform GetPiecePointer()
    {
        Transform pointer = poolPiecePointer.Count == 0 ? Instantiate(prefabPiecePointer) : poolPiecePointer.Pop();
        pointer.gameObject.SetActive(true);
        return pointer;
    }

    public void ReturnPiecePointers(List<Transform> pointers)
    {
        foreach (var pointer in pointers)
        {
            pointer.gameObject.SetActive(false);
            poolPiecePointer.Push(pointer);
        }
    }
}
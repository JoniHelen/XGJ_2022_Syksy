using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour
{
    public List<PuzzlePiece> PuzzlePieces = new();
    private bool Completed = false;
    private Renderer rend;

    public UnityEvent Complete;

    private void Awake()
    {
        AssignPieces();
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (PuzzlePieces.TrueForAll(piece => piece.IsInPlace) && !Completed)
        {
            Completed = true;
            StartCoroutine(CompletePuzzle());
        }
    }

    private IEnumerator CompletePuzzle()
    {
        PuzzlePieces.ForEach(piece => piece.StartCoroutine(piece.FlashComplete()));
        yield return new WaitForSeconds(0.75f);
        Complete.Invoke();
    }

    private void AssignPieces()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.tag = "PuzzlePiece";
            t.gameObject.AddComponent<SphereCollider>();
            PuzzlePieces.Add(t.gameObject.AddComponent<PuzzlePiece>());
        }
    }

    public void ScatterPuzzlePieces(List<Vector2> LeftBounds, List<Vector2> RightBounds)
    {
        int half = PuzzlePieces.Count / 2;
        for (int i = 0; i < PuzzlePieces.Count; i++)
        {
            if (i < half)
                PuzzlePieces[i].Scatter(LeftBounds[0], LeftBounds[1], (float)(i + 1) / half);
            else
                PuzzlePieces[i].Scatter(RightBounds[0], RightBounds[1], (float)(i - half + 1) / (PuzzlePieces.Count - half));
        }
    }
}
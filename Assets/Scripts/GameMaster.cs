using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public List<SO_Puzzle> Puzzles;
    private GameObject CurrentPuzzle;
    private List<PuzzlePiece> CurrentPuzzlePieces = new();

    [SerializeField] List<Vector2> LeftBounds;
    [SerializeField] List<Vector2> RightBounds;
    // Start is called before the first frame update
    void Start()
    {
        CurrentPuzzle = Instantiate(Puzzles[0].Puzzle);
        AssignPieces();
        ScatterPuzzlePieces();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignPieces()
    {
        foreach (Transform t in CurrentPuzzle.transform)
        {
            t.gameObject.tag = "PuzzlePiece";
            t.gameObject.AddComponent<SphereCollider>();
            CurrentPuzzlePieces.Add(t.gameObject.AddComponent<PuzzlePiece>());
        }
    }

    private void ScatterPuzzlePieces()
    {
        int half = CurrentPuzzlePieces.Count / 2;
        for (int i = 0; i < CurrentPuzzlePieces.Count; i++)
        {
            if (i < half)
            {
                CurrentPuzzlePieces[i].Scatter(LeftBounds[0], LeftBounds[1], (float)(i + 1) / half);
                CurrentPuzzlePieces[i].ReadyToMove = true;
            }
            else
            {
                CurrentPuzzlePieces[i].Scatter(RightBounds[0], RightBounds[1], (float)(i - half + 1) / (CurrentPuzzlePieces.Count - half));
                CurrentPuzzlePieces[i].ReadyToMove = true;
            }
        }
    }
}

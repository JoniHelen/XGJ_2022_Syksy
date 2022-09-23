using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Puzzle Object", menuName = "ScriptableObject/Puzzle")]
public class SO_Puzzle : ScriptableObject
{
    public GameObject Puzzle;

    public List<Vector3> GetPiecePositions()
    {
        List<Vector3> piecePositions = new();

        foreach (Transform t in Puzzle.transform) piecePositions.Add(t.position);

        return piecePositions;
    }
}
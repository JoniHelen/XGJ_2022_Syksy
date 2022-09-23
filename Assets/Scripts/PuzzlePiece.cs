using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public float CorrectDistance = 0.5f;
    public Vector3 CorrectPosition = Vector3.zero;
    public bool IsInPlace = false;

    private Vector3 RotationMultiplier = Vector3.zero;
    private float DistanceToPosition { get => (CorrectPosition - transform.position).magnitude; }

    private void Start()
    {
        RotationMultiplier = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
    }
    private void Update()
    {
        if (!IsInPlace)
        {
            transform.Rotate(new Vector3(
                360 * RotationMultiplier.x * Time.deltaTime / 10,
                360 * RotationMultiplier.y * Time.deltaTime / 10,
                360 * RotationMultiplier.z * Time.deltaTime / 10
            ));
        }

        if (DistanceToPosition <= CorrectDistance && !IsInPlace)
        {
            IsInPlace = true;
            StartCoroutine(MoveToCorrectPosition(0.15f));
        }
    }

    private IEnumerator MoveToCorrectPosition(float moveTime)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;

        while(elapsedTime < moveTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, Quaternion.identity, elapsedTime / moveTime);
            transform.position = Vector3.Lerp(startPos, CorrectPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = Quaternion.identity;
        transform.position = CorrectPosition;
    }
}

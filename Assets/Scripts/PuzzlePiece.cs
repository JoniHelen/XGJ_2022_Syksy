using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public float CorrectDistance = 0.5f;
    public Vector3 CorrectPosition = Vector3.zero;
    private Quaternion CorrectRotaion = Quaternion.identity;
    public bool IsInPlace = false;

    public bool ReadyToMove = false;

    private Vector3 RotationMultiplier = Vector3.zero;
    private float DistanceToPosition { get => (CorrectPosition - transform.position).magnitude; }

    private void Start()
    {
        RotationMultiplier = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
    }
    private void Update()
    {
        if (ReadyToMove)
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
    }

    public void Scatter(Vector2 boundsMin, Vector2 boundsMax, float proportion)
    {
        CorrectPosition = transform.localPosition;
        CorrectRotaion = transform.localRotation;
        float rndX = Random.Range(boundsMin.x, boundsMax.x);
        float rangeY = Mathf.Abs(boundsMax.y - boundsMin.y);

        transform.position = new Vector3(rndX, rangeY * proportion - rangeY / 2, 0.05f);
    }

    private IEnumerator MoveToCorrectPosition(float moveTime)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;

        while(elapsedTime < moveTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, CorrectRotaion, elapsedTime / moveTime);
            transform.position = Vector3.Lerp(startPos, CorrectPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = CorrectRotaion;
        transform.position = CorrectPosition;
    }
}

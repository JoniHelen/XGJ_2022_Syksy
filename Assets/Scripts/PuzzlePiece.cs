using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public float CorrectDistance = 0.5f;
    public Vector3 CorrectPosition = Vector3.zero;
    private Quaternion CorrectRotation = Quaternion.identity;
    private Quaternion RandomRotation = Quaternion.identity;
    public bool IsInPlace = false;

    private Renderer rend;

    public bool ReadyToMove = false;

    private Vector3 RotationMultiplier = Vector3.zero;
    private float DistanceToPosition { get => (CorrectPosition - transform.position).magnitude; }

    private void Start()
    {
        rend = GetComponent<Renderer>();
        RotationMultiplier = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
        RandomRotation = Quaternion.Euler(360 * Random.value, 360 * Random.value, 360 * Random.value);
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
                StartCoroutine(MoveToPosition(0.15f));
            }
        }
    }

    private float Ease(float t)
    {
        //float c1 = 1.70158f;
        //float c3 = c1 + 1f;

        return 1 - Mathf.Pow(1 - t, 3); //1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    public void Scatter(Vector2 boundsMin, Vector2 boundsMax, float proportion)
    {
        CorrectPosition = transform.localPosition;
        CorrectRotation = transform.localRotation;
        float rndX = Random.Range(boundsMin.x, boundsMax.x);
        float rangeY = Mathf.Abs(boundsMax.y - boundsMin.y);

        StartCoroutine(MoveToPosition(0.3f, new Vector3(rndX, rangeY * proportion - rangeY / 2, 0.05f), false));
    }

    public IEnumerator FlashComplete()
    {
        float flashTime = 0.3f;
        float elapsedTime = 0;

        float flashAmount;
        float flashNumber = 1f;

        while (elapsedTime < flashTime)
        {
            if (elapsedTime < flashTime / 2)
                flashAmount = elapsedTime / (flashTime / 2);
            else
                flashAmount = 1 - (elapsedTime - flashTime / 2) / (flashTime / 2);

            rend.material.SetFloat("_FlashIntensity", flashNumber * flashAmount);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        rend.material.SetFloat("_FlashIntensity", 0);
    }

    private IEnumerator MoveToPosition(float moveTime, Vector3 position = default, bool correctPosition = true)
    {
        float elapsedTime = 0;
        Vector3 startPos = transform.position;
        Quaternion startRotation = transform.rotation;

        while(elapsedTime < moveTime)
        {
            if (correctPosition)
                transform.SetPositionAndRotation(
                    Vector3.Lerp(startPos, CorrectPosition, Ease(elapsedTime / moveTime)),
                    Quaternion.Lerp(startRotation, CorrectRotation, Ease(elapsedTime / moveTime))
                );
            else
                transform.SetPositionAndRotation(
                    Vector3.Lerp(startPos, position, Ease(elapsedTime / moveTime)),
                    Quaternion.Lerp(startRotation, RandomRotation, Ease(elapsedTime / moveTime))
                );

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (correctPosition)
        {
            transform.SetPositionAndRotation(CorrectPosition, CorrectRotation);
        }
        else
        {
            transform.SetPositionAndRotation(position, RandomRotation);
            ReadyToMove = true;
        }
    }
}

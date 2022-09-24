using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public List<Puzzle> Puzzles = new();
    private Puzzle CurrentPuzzle;
    private Puzzle LastPuzzle;

    [SerializeField] List<Vector2> LeftBounds;
    [SerializeField] List<Vector2> RightBounds;

    [SerializeField] ParticleSystem leftSystem;
    [SerializeField] ParticleSystem rightSystem;

    // Start is called before the first frame update
    void Start()
    {
        NewPuzzle();
    }

    public void NewPuzzle()
    {
        LastPuzzle = CurrentPuzzle;
        GameObject pObj = Instantiate(Puzzles[Random.Range(0, Puzzles.Count)].gameObject, Vector3.down * 5, Quaternion.identity);
        CurrentPuzzle = pObj.GetComponent<Puzzle>();
        CurrentPuzzle.Complete.AddListener(NewPuzzle);
        CurrentPuzzle.Complete.AddListener(FireParticles);

        if (LastPuzzle != null)
            StartCoroutine(MoveUp(LastPuzzle.gameObject, true));

        StartCoroutine(MoveUp(CurrentPuzzle.gameObject, false));
        StartCoroutine(WaitExecution());
    }

    private void FireParticles()
    {
        rightSystem.gameObject.SetActive(true);
        leftSystem.gameObject.SetActive(true);
    }

    private IEnumerator MoveUp(GameObject obj, bool destroy)
    {
        float moveTime = 2f;
        float elapsedTime = 0;

        Vector3 originalPos = obj.transform.position;
        Vector3 destination = originalPos + Vector3.up * 5;

        while (elapsedTime < moveTime)
        {
            obj.transform.position = Vector3.Lerp(originalPos, destination, Ease(elapsedTime / moveTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        obj.transform.position = destination;

        if (destroy) Destroy(obj);
    }

    

    private float Ease(float t)
    {
        //float c1 = 1.70158f;
        //float c3 = c1 + 1f;

        return t < 0.5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2; //1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    private IEnumerator WaitExecution()
    {
        yield return new WaitForSeconds(2);
        CurrentPuzzle.ScatterPuzzlePieces(LeftBounds, RightBounds);
    }
}

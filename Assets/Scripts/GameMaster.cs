using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public List<Puzzle> Puzzles = new();
    private Puzzle CurrentPuzzle;
    private Puzzle LastPuzzle;

    private bool TimerRunning = false;
    private bool GameActive = false;
    private bool ScoreShown = true;

    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] List<Vector2> LeftBounds;
    [SerializeField] List<Vector2> RightBounds;

    [SerializeField] ParticleSystem leftSystem;
    [SerializeField] ParticleSystem rightSystem;

    [SerializeField] Button retryButton;

    public UnityEvent TimerEnded;

    private int HighScore;
    [SerializeField] TextMeshProUGUI HighScoreText;

    private int Score = 0;
    [SerializeField] TextMeshProUGUI ScoreText;

    [SerializeField] ScoreScreen scoreScreen;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        if (!PlayerPrefs.HasKey("HighScore"))
        {
            HighScore = 0;
        }
        else
        {
            HighScore = PlayerPrefs.GetInt("HighScore");
        }

        UpdateHighScore();
        TimerEnded.AddListener(GameOver);
    }

    public void StartGame()
    {
        if (ScoreShown)
        {
            ScoreShown = false;
            AudioMaster.instance.PlaySound("MenuClick");

            if (scoreScreen.gameObject.activeSelf)
                scoreScreen.gameObject.SetActive(false);

            if (CurrentPuzzle != null)
            {
                CurrentPuzzle.AutoComplete();
                Score = 0;
                UpdateScore();
                StartCoroutine(RetryWait());
            }
            else
            {
                GameActive = true;
                NewPuzzle();
            }
        }
    }

    private void UpdateHighScore()
    {
        HighScoreText.text = HighScore.ToString("High Score: 00");
    }

    private void UpdateScore()
    {
        ScoreText.text = Score.ToString("Score: 00");
    } 

    private IEnumerator RetryWait()
    {
        yield return new WaitForSeconds(1);
        GameActive = true;
        NewPuzzle();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("HighScore", HighScore);
    }

    public void NewPuzzle()
    {
        if (GameActive)
        {
            GetComponent<PlayerControls>().enabled = false;
            LastPuzzle = CurrentPuzzle;
            GameObject pObj = Instantiate(Puzzles[Random.Range(0, Puzzles.Count)].gameObject, Vector3.down * 5, Quaternion.identity);
            CurrentPuzzle = pObj.GetComponent<Puzzle>();
            CurrentPuzzle.Complete.AddListener(NewPuzzle);
            CurrentPuzzle.Complete.AddListener(FireParticles);
            CurrentPuzzle.Complete.AddListener(AddScore);

            AudioMaster.instance.PlaySound("Swoosh", true);
            if (LastPuzzle != null)
                StartCoroutine(MoveUp(LastPuzzle.gameObject, true));

            StartCoroutine(MoveUp(CurrentPuzzle.gameObject, false));
            StartCoroutine(WaitExecution());
        }
    }

    private void AddScore()
    {
        Score++;
        UpdateScore();
    }

    private void FireParticles()
    {
        AudioMaster.instance.PlaySound("PuzzleFinished");
        rightSystem.gameObject.SetActive(true);
        leftSystem.gameObject.SetActive(true);
    }

    private IEnumerator Timer(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            timeText.text = time.ToString("Time left: 00.0s");
            yield return new WaitForEndOfFrame();
        }
        GameActive = false;
        TimerRunning = false;
        AudioMaster.instance.PlaySound("Alarm");
        StartCoroutine(ShowHighScore());
        TimerEnded.Invoke();
    }

    private void GameOver()
    {
        AudioMaster.instance.StopClockSounds();
        CurrentPuzzle.Complete.RemoveAllListeners();
        GetComponent<PlayerControls>().enabled = false;
    }

    private IEnumerator ShowHighScore()
    {
        yield return new WaitForSeconds(2);

        scoreScreen.score = Score;
        scoreScreen.highScore = HighScore;
        scoreScreen.gameObject.SetActive(true);

        if (Score > HighScore)
        {
            HighScore = Score;
            FireParticles();
            UpdateHighScore();
        }

        retryButton.interactable = true;
        ScoreShown = true;
    }

    public void ResetHighScore()
    {
        if (ScoreShown)
        {
            PlayerPrefs.DeleteKey("HighScore");
            HighScore = 0;
            UpdateHighScore();
        }
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
        if (GameActive)
        {
            CurrentPuzzle.ScatterPuzzlePieces(LeftBounds, RightBounds);
            GetComponent<PlayerControls>().enabled = true;
            if (!TimerRunning)
            {
                TimerRunning = true;
                AudioMaster.instance.PlaySound("Clock", false, true);
                StartCoroutine(Timer(60f));
            }
        }
        else
        {
            GameOver();
        }

    }
}

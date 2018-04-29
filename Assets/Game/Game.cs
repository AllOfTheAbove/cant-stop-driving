using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Game : MonoBehaviour {

    private static Game instance;
    public static Game Instance
    {
        get { return instance; }
    }

    public bool debug = false;

    /*
     * State -1 : Architect is waiting for a driver to join (can't play, can pause)
     * State 0  : Both players are connected but they are waiting for the countdown (can't play, can't pause)
     * State 1  : Normal (can play, can pause)
     * State 2  : Game Over
     */
    public int state = -1;
    public int score = 0;

    private float startTime = 0;
    private Coroutine countdownCoroutine;
    private Coroutine fadeOutCoroutine;

    public GameObject driver;
    public GameObject architect;
    public GameObject[] vehicles;
    public int currentVehicleId;
    public GameObject serverAsset;
    public AudioMixer audioMixer;
    public AudioSource titleMusic;
    public AudioSource gameMusic;

    public bool gamePaused = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyObject(gameObject);
        }
    }

    public void Start()
    {
        audioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
        audioMixer.SetFloat("soundVolume", PlayerPrefs.GetFloat("soundVolume"));
    }

    public void RestartServer()
    {
        GameObject o = Instantiate(serverAsset);
        o.name = serverAsset.name;
    }

    public Server GetServer()
    {
        return GameObject.Find(serverAsset.name).GetComponent<Server>();
    }

    public void Reset()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        state = -1;
        score = 0;
    }

    public void Pause()
    {
        if (gamePaused)
        {
            gamePaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            gamePaused = true;
            Time.timeScale = 0f;
        }
        GameScene.Instance.pauseUI.SetActive(gamePaused);
    }

    public void ChangeState(int state)
    {
        if (state == 0)
        {
            this.state = state;
            startTime = Time.time;
            GameScene.Instance.waitingUI.GetComponent<TextMeshProUGUI>().SetText("(client)");
            SwitchAudio("game");
            countdownCoroutine = StartCoroutine(StartCountdown());
        } else if(state == 2)
        {
            this.state = state;
            Time.timeScale = 0f;
            GameScene.Instance.gameoverScoreLabel.GetComponent<TextMeshProUGUI>().SetText("Score: " + score);
            GameScene.Instance.gameoverTimeLabel.GetComponent<TextMeshProUGUI>().SetText("Time: " + Mathf.Floor(Time.time - startTime));
            GameScene.Instance.gameoverUI.SetActive(true);
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(gameMusic));
        }
    }

    public IEnumerator StartCountdown()
    {
        int time = 3;

        while (time >= -1)
        {
            yield return new WaitForSeconds(1);
            if (time == 0)
            {
                GameScene.Instance.countdownEndSound.Play();
                GameScene.Instance.countdownLabel.GetComponent<TextMeshProUGUI>().SetText("GO");
                state = 1;
                GameScene.Instance.driverTutorial.SetActive(false);
                GameScene.Instance.architectTutorial.SetActive(false);
                gameMusic.Play();
            }
            else if (time < 0)
            {
                GameScene.Instance.countdownLabel.SetActive(false);
                StopCoroutine(countdownCoroutine);
            }
            else
            {
                GameScene.Instance.countdownBeepSound.Play();
                GameScene.Instance.countdownLabel.GetComponent<TextMeshProUGUI>().SetText("" + Mathf.Floor(time));
            }
            time--;
        }
    }

    public void SwitchAudio(string scene)
    {
        if (scene == "title")
        {
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(gameMusic));
            titleMusic.Play();
        }
        else if (scene == "game")
        {
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(titleMusic));
        }
    }

    public IEnumerator FadeOutAudio(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / 1f;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        StopCoroutine(fadeOutCoroutine);
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public bool paused = false;

    private float startTime = 0;
    private Coroutine countdownCoroutine;
    private Coroutine fadeUI;
    private Coroutine fadeOutCoroutine;
    public int currentMusicId = 0;
    private System.Random random = new System.Random();

    public GameObject driver;
    public GameObject architect;
    public GameObject[] vehicles;
    public int currentVehicleId;
    public GameObject serverAsset;
    public AudioMixer audioMixer;
    public AudioSource titleMusic;
    public AudioSource[] gameMusics;
    public AudioSource waterBackgroundSound;



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

        audioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
        audioMixer.SetFloat("soundVolume", PlayerPrefs.GetFloat("soundVolume"));
    }

    public void Start()
    {
        StartCoroutine(BackgroundSounds());
        audioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
        audioMixer.SetFloat("soundVolume", PlayerPrefs.GetFloat("soundVolume"));
        titleMusic.Play();
    }

    private void Update()
    {
        if(!gameMusics[currentMusicId].isPlaying && state == 1 && !titleMusic.isPlaying)
        {
            int id = random.Next(0, gameMusics.Length);
            while (id  == currentMusicId)
            {
                id = random.Next(0, gameMusics.Length);
            }
            currentMusicId = id;
            gameMusics[currentMusicId].Play();
        }
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
        paused = false;
        state = -1;
        score = 0;
    }

    public void Pause()
    {
        if (paused)
        {
            paused = false;
            if (GetServer().singleplayer)
            {
                Time.timeScale = 1f;
                GameObject.FindGameObjectsWithTag("Driver")[0].GetComponent<Driver>().checkForDeath = true;
            }
        }
        else
        {
            paused = true;
            if (GetServer().singleplayer)
            {
                Time.timeScale = 0f;
                GameObject.FindGameObjectsWithTag("Driver")[0].GetComponent<Driver>().checkForDeath = false;
            }
        }
        GameScene.Instance.pauseUI.SetActive(paused);
        
    }

    public void ChangeState(int state)
    {
        if (state == -1)
        {
            this.state = state;
            if (GetServer().firstGame)
            {
                fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.architectTutorial.GetComponent<CanvasGroup>(), 0f, 1f, 0.5f));
                fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.driverTutorial.GetComponent<CanvasGroup>(), 0f, 1f, 0.5f));
            }
            SwitchAudio("game");
        } else if (state == 0)
        {
            this.state = state;
            startTime = Time.time;
            GameScene.Instance.waitingUI.GetComponent<TextMeshProUGUI>().SetText("(client)");
            countdownCoroutine = StartCoroutine(StartCountdown());
        } else if (state == 1)
        {
            this.state = state;
            GameScene.Instance.countdownEndSound.Play();
            GameScene.Instance.countdownLabel.GetComponent<TextMeshProUGUI>().SetText("GO");
            fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.score.GetComponent<CanvasGroup>(), 0f, 1f, 0.5f));
            fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.speed.GetComponent<CanvasGroup>(), 0f, 1f, 0.5f));
            fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.nextTile.GetComponent<CanvasGroup>(), 0f, 1f, 0.5f));
            if (GetServer().firstGame)
            {
                fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.architectTutorial.GetComponent<CanvasGroup>(), 1f, 0f, 1f));
                fadeUI = StartCoroutine(FadeCanvas(GameScene.Instance.driverTutorial.GetComponent<CanvasGroup>(), 1f, 0f, 1f));
            }
        } else if (state == 2)
        {
            this.state = state;
            StartCoroutine(FadeOutAudio(gameMusics[currentMusicId]));
            if (paused)
            {
                Pause();
            }
            if (GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<AudioListener>())
            {
                GameObject.FindGameObjectsWithTag("Driver")[0].GetComponentInChildren<AudioListener>().enabled = false;
            }
            if (GameObject.FindGameObjectsWithTag("Architect")[0].GetComponentInChildren<AudioListener>())
            {
                GameObject.FindGameObjectsWithTag("Architect")[0].GetComponentInChildren<AudioListener>().enabled = false;
            }

            GameObject driver = GameObject.FindGameObjectsWithTag("Driver")[0];
            List<Vector3> lastPositions = driver.GetComponent<Driver>().lastPositions;
            List<Quaternion> lastRotations = driver.GetComponent<Driver>().lastRotations;
            Destroy(driver.GetComponent<Driver>().vehicle);
            GameObject vehicle = Instantiate(vehicles[driver.GetComponent<Driver>().vehicleId], lastPositions[0], lastRotations[0]);
            vehicle.GetComponent<Vehicle>().enabled = false;
            vehicle.AddComponent<ReproduceMovements>();
            vehicle.GetComponent<ReproduceMovements>().positions = lastPositions;
            vehicle.GetComponent<ReproduceMovements>().rotations = lastRotations;

            GameScene.Instance.score.SetActive(false);
            GameScene.Instance.speed.SetActive(false);
            GameScene.Instance.nextTile.SetActive(false);
            GameScene.Instance.warningLabel.SetActive(false);
            GameScene.Instance.camera.SetActive(true);
            GameScene.Instance.camera.GetComponent<DeathCamera>().target = vehicle;
            GameScene.Instance.gameoverScoreLabel.GetComponent<TextMeshProUGUI>().SetText("Score: " + score);
            GameScene.Instance.gameoverTimeLabel.GetComponent<TextMeshProUGUI>().SetText("Time: " + Mathf.Floor(Time.time - startTime));
            GameScene.Instance.gameoverUI.SetActive(true);
        }
    }

    public IEnumerator BackgroundSounds()
    {
        int delay = 0;

        while (true)
        {
            delay = random.Next(0, 10);
            yield return new WaitForSecondsRealtime(10 + delay);
            waterBackgroundSound.Play();
        }
    }

    public IEnumerator StartCountdown()
    {
        yield return new WaitForSecondsRealtime(2);
        int time = 3;

        while (time >= -1)
        {
            yield return new WaitForSecondsRealtime(1);
            if (time == 0)
            {
                ChangeState(1);
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
            fadeOutCoroutine = StartCoroutine(FadeOutAudio(gameMusics[currentMusicId]));
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
        //StopCoroutine(fadeOutCoroutine);
    }

    public IEnumerator FadeCanvas(CanvasGroup canvas, float startAlpha, float endAlpha, float duration)
    {
        var startTime = Time.time;
        var endTime = Time.time + duration;
        var elapsedTime = 0f;

        canvas.alpha = startAlpha;
        while (Time.time <= endTime)
        {
            elapsedTime = Time.time - startTime;
            var percentage = 1 / (duration / elapsedTime);
            if (startAlpha > endAlpha)
            {
                canvas.alpha = startAlpha - percentage;
            }
            else
            {
                canvas.alpha = startAlpha + percentage;
            }

            yield return new WaitForEndOfFrame();
        }
        canvas.alpha = endAlpha;
        //StopCoroutine(fadeUI);
    }

}

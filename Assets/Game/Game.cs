﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Game : MonoBehaviour {

    private static Game instance;
    public static Game Instance
    {
        get { return instance; }
    }

    public GameObject server;
    public AudioMixer audioMixer;

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

        audioMixer.SetFloat("musicVolume", PlayerPrefs.GetFloat("musicVolume"));
        audioMixer.SetFloat("soundVolume", PlayerPrefs.GetFloat("soundVolume"));
    }

    public void StartServer()
    {
        GameObject o = Instantiate(server);
        o.name = server.name;
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

}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {

    public GameObject registerUI;
    public InputField username;
    public TextMeshProUGUI localuser;
    public GameObject loading;

    public void Start()
    {
        if (!PlayerPrefs.HasKey("scoreboard_username"))
        {
            registerUI.SetActive(true);
        } else
        {
            StartCoroutine(GetScores());
        }
    }

    public void RefreshScores()
    {
        loading.SetActive(true);
        localuser.SetText("");
        for (int i = 1; i <= 10; i++)
        {
            GameObject.Find("user" + i).GetComponent<TextMeshProUGUI>().SetText("");
        }
        StartCoroutine(GetScores());
    }

    public IEnumerator GetRank()
    {
        WWW hs_get = new WWW("http://northwaterman.com/csd/apiscoreboard/getrank.php?score=" + PlayerPrefs.GetInt("scoreboard_highscore") + "&username=" + PlayerPrefs.GetString("scoreboard_username"));
        yield return hs_get;
        if (hs_get.error == null)
        {
            string num = "th";
            if(hs_get.text == "1")
            {
                num = "st";
            }
            if (hs_get.text == "2")
            {
                num = "nd";
            }
            if (hs_get.text == "3")
            {
                num = "rd";
            }
            localuser.SetText(PlayerPrefs.GetString("scoreboard_username") + ": " + PlayerPrefs.GetInt("scoreboard_highscore") + " (" + hs_get.text + num + " player)");
        }
    }

    public IEnumerator GetScores()
    {
        StartCoroutine(GetRank());

        WWW hs_get = new WWW("http://northwaterman.com/csd/apiscoreboard/get.php");
        yield return hs_get;
        if (hs_get.error == null)
        {
            loading.SetActive(false);
            string[] lines = hs_get.text.Split("\r\n".ToCharArray());
            for(int i = 1; i <= 10 && i < lines.Length; i++)
            {
                string[] data = lines[i - 1].Split(new char[] { ':' });
                GameObject.Find("user" + i).GetComponent<TextMeshProUGUI>().SetText(i + ". " + data[0] + ": " + data[1]);
            }
            
        }
    }

    public static IEnumerator AddScore(int score)
    {
        string add_highscore_url = "http://northwaterman.com/csd/apiscoreboard/add.php?score=" + score + "&username=" + WWW.EscapeURL(PlayerPrefs.GetString("scoreboard_username")) + "&hash=capefreioubliedeleverlancre";
        WWW hs_post = new WWW(add_highscore_url);
        yield return hs_post;
    }

    public void Register()
    {
        PlayerPrefs.SetString("scoreboard_username", username.text);
        StartCoroutine(AddScore(PlayerPrefs.GetInt("scoreboard_highscore")));
        registerUI.SetActive(false);
        RefreshScores();
    }
}

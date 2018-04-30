using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Networking.Match;
using TMPro;
using UnityEngine.UI;

public class Multiplayer : NetworkBehaviour
{
    NetworkManager nm;
    
    void Awake()
    {
        nm = GameObject.Find("Server").GetComponent<Server>();
        nm.StartMatchMaker();

        InvokeRepeating("GetMatchList", 0, 2.5f);
    }

    /**
    * MATCH LISTING
    **/

    private void GetMatchList()
    {
        nm.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (success && GameObject.Find("NetworkMatch"))
        {
            int id = 0;
            foreach(var match in matchList)
            {
                var e = GameObject.Find("match_" + id);
                e.GetComponentInChildren<TextMeshProUGUI>().SetText(match.name);
                e.GetComponent<Button>().onClick.RemoveAllListeners();
                e.GetComponent<Button>().onClick.AddListener(delegate {
                    nm.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
                });

                id++;
            }

            while(id < 10)
            {
                var e = GameObject.Find("match_" + id);
                e.GetComponentInChildren<TextMeshProUGUI>().SetText("");
                e.GetComponent<Button>().onClick.RemoveAllListeners();
                id++;
            }
        }
    }

    /**
     * MATCH CREATION
     **/

    public void OnMatchCreate()
    {
        var name = GameObject.Find("CreateInput").GetComponent<InputField>().text;
        GameObject.Find("CreateInput").GetComponent<InputField>().text = "";
        if(name != null)
        {
            nm.matchMaker.CreateMatch(name, 2, true, "", "", "", 0, 0, OnMatchCreated);
            GetMatchList();
        }
    }

    public void OnMatchCreated(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            nm.StartHost(matchInfo);
        }
    }

    /**
    * MATCH JOIN
    **/

    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            nm.StartClient(matchInfo);
        }
    }

}
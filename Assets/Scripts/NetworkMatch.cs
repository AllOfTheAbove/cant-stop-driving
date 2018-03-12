using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

public class NetworkMatch : NetworkBehaviour
{
    List<MatchInfoSnapshot> matchList = new List<MatchInfoSnapshot>();
    NetworkManager nm;
    public string roomName = "CantStopDriving";

    void Awake()
    {
        nm = GameObject.Find("Server").GetComponent<Server>();
        nm.StartMatchMaker();
    }

    void OnGUI()
    {
        roomName = GUI.TextField(new Rect(100, 0, 200, 20), roomName, 16);

        if (GUILayout.Button("Create Room"))
        {
            nm.matchMaker.CreateMatch(roomName, 2, true, "", "", "", 0, 0, OnMatchCreate);
        }

        if (GUILayout.Button("List rooms"))
        {
            nm.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        }

        if (matchList.Count > 0)
        {
            GUILayout.Label("Current rooms");
        }
        foreach (var match in matchList)
        {
            if (GUILayout.Button(match.name))
            {
                nm.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
            }
        }
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            nm.StartHost(matchInfo);
        }
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success && matches != null && matches.Count > 0)
        {
            matchList = matches;
        }
    }

    public void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            nm.StartClient(matchInfo);
        }
    }

}
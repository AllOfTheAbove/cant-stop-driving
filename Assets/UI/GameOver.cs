using UnityEngine;

public class GameOver : MonoBehaviour
{

    public void Continue()
    {
        GameObject o1 = GameObject.Find("Driver");
        GameObject o2 = GameObject.Find("Driver(Clone)");
        GameObject o3 = GameObject.Find("Architect");
        GameObject o4 = GameObject.Find("Architect(Clone)");

        if(o1)
        {
            o1.GetComponent<Driver>().RestartGame();
        }
        if(o2)
        {
            o2.GetComponent<Driver>().RestartGame();
        }
        if(o3)
        {
            o3.GetComponent<Architect>().RestartGame();
        }
        if(o4)
        {
            o4.GetComponent<Architect>().RestartGame();
        }
    }

    public void Menu()
    {
        Game.Instance.GetServer().Disconnect();
    }

    public void Quit()
    {
        Application.Quit();
    }

}
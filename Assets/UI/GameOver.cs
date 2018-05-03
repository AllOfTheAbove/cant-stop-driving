using UnityEngine;

public class GameOver : MonoBehaviour
{

    public void Continue()
    {
        GameObject.FindGameObjectsWithTag("Driver")[0].GetComponent<Driver>().RestartGame();
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
using UnityEngine;

public class GameOver : MonoBehaviour
{

    public void Continue()
    {
        GameObject.FindGameObjectsWithTag("Architect")[0].GetComponent<Architect>().RestartGame();
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
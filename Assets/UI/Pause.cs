using UnityEngine;

public class Pause : MonoBehaviour {

    public void Resume()
    {
        Game.Instance.Pause();
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

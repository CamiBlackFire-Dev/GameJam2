
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectInteraction : MonoBehaviour
{
    public void CivilianInteraction()
    {
        Destroy(gameObject);
    }
    public void DebrisInteraction(int hp)
    {        
        if (hp >= 3)
        {
            Destroy(gameObject);
        }
    }

    public void FinalScene()
    {
        SceneManager.LoadScene("Final");
    }
    public void FinalTutorial()
    {
        SceneManager.LoadScene("Menu");
    }
}

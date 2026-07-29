using UnityEngine;

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
}

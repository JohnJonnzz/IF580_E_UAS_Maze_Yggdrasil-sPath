using UnityEngine;

public class OnDestroyCallback : MonoBehaviour
{
    public System.Action onDestroy;

    void OnDestroy()
    {
        if (onDestroy != null)
        {
            onDestroy.Invoke();
        }
    }
}

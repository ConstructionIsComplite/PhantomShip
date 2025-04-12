using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitor : MonoBehaviour
{
    public void TransitScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}

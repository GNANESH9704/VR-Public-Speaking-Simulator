using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToNextScene(){
        SceneManager.LoadScene("Inner Scene");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
    private string scene_name;

    /*
    private void OnDestroy() {
          Debug.Log("a");
    }
    */

    public void _LoadScene(string scene_name) {
        SceneManager.LoadScene(scene_name);
    }
}

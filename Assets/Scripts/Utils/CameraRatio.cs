using UnityEngine;

public class CameraRatio : MonoBehaviour {
    void OnPreCull() {
        if (this.gameObject.name == "Main Camera")
            GL.Clear(true, true, Color.black);
    }

    void Awake() {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scale_height;
        float scale_width;

        camera.orthographicSize = 10;
        scale_height = ((float)Screen.width / Screen.height) / ((float)9/16);
        scale_width = 1f / scale_height;

        if(scale_height < 1){
            rect.height = scale_height;
            rect.y = (1f - scale_height) /2f;
        }
        else{
            rect.width = scale_width;
            rect.x = (1f - scale_width) /2f;
        }
        camera.rect = rect;
    }
}

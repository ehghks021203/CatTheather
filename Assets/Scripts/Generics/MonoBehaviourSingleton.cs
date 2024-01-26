using UnityEngine;

// 제너릭으로 MonoBehaviour를 상속하는 모노싱글톤 구현
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance 
    { 
        get 
        {
            if (instance == null) 
            {
                GameObject obj = GameObject.Find(typeof(T).Name);
                if (obj == null) 
                {
                    obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                } 
                else 
                {
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }
}
using System.Collections;
using UnityEngine;

public class Sample : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitAndComplete());
    }

    private IEnumerator WaitAndComplete()
    {
        yield return new WaitForSeconds(5f);
        
        Debug.Log("完了");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneLoad : MonoBehaviour
{
    [SerializeField] private string LoadSceneName;  //로드할 씬 이름 설정

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (FadeManager.Instance != null)
                FadeManager.Instance.LoadScene(LoadSceneName);
        }
    }
}

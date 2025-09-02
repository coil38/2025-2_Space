using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    public int stageToLoad = 1;
    public string battleSceneName = "CharacterTestScene2";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FadeManager.Instance.LoadScene(battleSceneName);
        }
    }
}


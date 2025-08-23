using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        ResetHp();
        if(PlayerUIManager.instance != null )
            PlayerUIManager.instance.ResetHpUI();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    RestartScene();
        //}
    }

    private void RestartScene()
    {
        SceneManager.LoadScene("BattleTestScene");
    }

    private void ResetHp()
    {
        Debug.Log("작동한다!!");
        PlayerStatus.m_hp = 5;
    }
}

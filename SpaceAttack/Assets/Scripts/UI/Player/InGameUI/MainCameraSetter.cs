using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraSetter : MonoBehaviour
{
    private void Awake()
    {
        Canvas mainCanvas = GetComponent<Canvas>();       //캔버스에 초기 메인 카메라 자동설정
        if (mainCanvas != null)
        {
            mainCanvas.worldCamera = Camera.main;
            mainCanvas.planeDistance = 1;
        }
    }
}

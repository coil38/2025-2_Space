using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipsetSelectObject : MonoBehaviour
{
    [SerializeField] private ChipsetSelectUI chipsetSelectUI;

    public void OnChipsetSelectUI()
    {
        if (chipsetSelectUI == null)
        {
            LogUtil.LogError("칩셋선택UI프리팹이 할당되지 않았습니다.");
            return;
        }

        if(!chipsetSelectUI.gameObject.activeSelf)
            chipsetSelectUI.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipsetSoundManager : MonoBehaviour
{
    public static GameObject thisObject;
    private ChipSetType chipSetType;
    private void OnEnable()
    {
        StartCoroutine(WaitInitialize());
    }
    private IEnumerator WaitInitialize()
    {
        yield return new WaitUntil(() => SoundManager.instance != null && SoundManager.instance.endInitialize);
        Initialized();
    }
    public void Initialized()
    {
        thisObject = gameObject;

        chipSetType = GetComponent<ChipSetType>();
        if (chipSetType is WarriorChipset)
        {
            SoundManager.instance.RegisterGameObjectByAttribute(gameObject, "Warrior");
            //LogUtil.Log($"{gameObject.name}가 전사칩셋이다.");
        }
        else if (chipSetType is ArcherChipset)
        {
            SoundManager.instance.RegisterGameObjectByAttribute(gameObject, "Archer");
            //LogUtil.Log($"{gameObject.name}가 궁수칩셋이다.");
        }
    }
    //전사 칩셋 사운드
    public static void PlayPlayerAttackSound()
    {
        SoundManager.instance.PlaySound(thisObject, "SwordBasicAttack");
    }
}

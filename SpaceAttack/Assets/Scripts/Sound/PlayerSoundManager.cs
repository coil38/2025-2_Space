using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public static GameObject thisObject;
    public void Initialized()
    {
        thisObject = gameObject;
        SoundManager.instance.RegisterGameObjectByAttribute(gameObject, "Character");
    }
    public static void PlayPlayerMoveSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterMove");
    }

    public static void StopPlayerMoveSound()
    {
        SoundManager.instance.StopSound(thisObject, "CharacterMove");
    }

    public static void PlayPlayerHitSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterHit");
    }

    public static void PlayPlayerDashSound()
    {
        SoundManager.instance.PlaySound(thisObject, "CharacterDash3");
    }

    public static void PlayPlayerDeadSound()
    {

    }
}

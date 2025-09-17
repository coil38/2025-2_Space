using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    public void PlayPlayerRunSound()     //달리기 사운드 재생 + 이동 애니메이션 클립에 이벤트로 연결
    {
        PlayerSoundManager.StopPlayerMoveSound();
        PlayerSoundManager.PlayPlayerMoveSound();
    }
}

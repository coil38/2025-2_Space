using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroController : MonoBehaviour
{
    [Header("참조 설정")]
    public Transform boss;               // 보스 Transform
    public Transform bossCameraPoint;    // 줌인 시 카메라 위치
    public float zoomDuration = 2f;      // 줌인 시간
    public float holdTime = 2f;          // 보스 보여주는 시간
    public AudioClip bossMusic;          // 보스전 BGM

    private Camera mainCam;
    private bool introPlayed = false;

    private void Start()
    {
        Debug.Log("BossIntroController Start 실행됨!");
        mainCam = Camera.main;
        StartCoroutine(PlayBossIntro());
    }
    private IEnumerator PlayBossIntro()
    {
        if (introPlayed) yield break;
        introPlayed = true;

        CameraFallow camFollow = mainCam.GetComponent<CameraFallow>();
        Vector3 savedCameraDir = Vector3.zero;
        Vector3 savedCameraRot = Vector3.zero;

        if (camFollow != null)
        {
            camFollow.LockCamera(true);
            savedCameraDir = camFollow.cameraDir;  
            savedCameraRot = camFollow.cameraRot;
        }

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = true;

        Boss bossScript = boss.GetComponent<Boss>();
        if (bossScript != null)
            bossScript.enabled = false;

        Vector3 originalPos = mainCam.transform.position;
        Quaternion originalRot = mainCam.transform.rotation;

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerp = t / zoomDuration;
            mainCam.transform.position = Vector3.Lerp(originalPos, bossCameraPoint.position, lerp);
            mainCam.transform.rotation = Quaternion.Lerp(originalRot, bossCameraPoint.rotation, lerp);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        if (AudioManager.instance != null && bossMusic != null)
            AudioManager.instance.PlayBGM(bossMusic, 0.8f);

        Transform player = PlayerStatus.Instance?.transform;
        if (player != null && camFollow != null)
        {
            t = 0f;

            Vector3 targetPos = player.position + camFollow.cameraDir.normalized * camFollow.cameraDis;
            Quaternion targetRot = Quaternion.Euler(camFollow.cameraRot);

            while (t < zoomDuration)
            {
                t += Time.deltaTime;
                float lerp = t / zoomDuration;

                mainCam.transform.position = Vector3.Lerp(bossCameraPoint.position, targetPos, lerp);
                mainCam.transform.rotation = Quaternion.Slerp(bossCameraPoint.rotation, targetRot, lerp);
                yield return null;
            }
        }

        if (camFollow != null)
        {
            camFollow.cameraDir = savedCameraDir;
            camFollow.cameraRot = savedCameraRot;
            camFollow.LockCamera(false);
        }

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;

        if (bossScript != null)
        {
            bossScript.enabled = true;
            StartCoroutine(UseFirstPageSkillWithDelay(bossScript, 4f));
        }
    }

    private IEnumerator UseFirstPageSkillWithDelay(Boss bossScript, float delay)
    {
        yield return new WaitForSeconds(delay);

        List<System.Action> firstPageSkills = new List<System.Action>();
        firstPageSkills.Add(() => bossScript.BossAttack());
        firstPageSkills.Add(() => bossScript.StartCoinRain());
        firstPageSkills.Add(() => bossScript.StartJumpAttack());

        System.Action chosenSkill = firstPageSkills[Random.Range(0, firstPageSkills.Count)];
        chosenSkill.Invoke();
    }

}

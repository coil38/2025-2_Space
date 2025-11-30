using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class SkillEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem skillEffect;

    private bool isActivating = false;
    private void Start()
    {
        skillEffect.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateSkillEffect(transform.position - Vector3.forward * Time.deltaTime * 2f);
    }

    public void OnSkillEffect(Vector3 genPos, Vector3 dir)
    {
        if (skillEffect == null)
        {
            LogUtil.Log("공격범위의 변경될 스프라이트 대상이 할당되지 않음");
            return;
        }

        if (isActivating) return;

        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);              //방향 조정
        transform.position = new Vector3(genPos.x, transform.position.y, genPos.z); //위치 조정

        skillEffect.gameObject.SetActive(true);
        isActivating = true;
    }

    public void UpdateSkillEffect(Vector3 position)
    {
        if (!isActivating) return;

        transform.position = position;
    }

    public void EndSkillEffect()
    {
        if (!isActivating) return;

        skillEffect.gameObject.SetActive(false);
        isActivating = false;
    }
}
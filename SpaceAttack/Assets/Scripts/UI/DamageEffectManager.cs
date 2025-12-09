using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;
    private Canvas effectCanvas;

    public static DamageEffectManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        effectCanvas = textPrefab.gameObject.GetComponent<Canvas>();
    }

    public void ShowDamageText(Vector3 position, string text, Color color, bool isCritical = false)
    {
        if (textPrefab == null || effectCanvas == null)
        {
            LogUtil.LogError("프리팹 혹은 프리팹에 캔버스가 없읍니다.");
            return;
        }

        GameObject damageText = Instantiate(textPrefab);
        
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.position = position;
        }

        TextMeshProUGUI temp = damageText.GetComponent<DamageTextEffect>().textMesh;
        if (temp != null)
        {
            temp.text = text;
            temp.color = color;
            temp.outlineColor = new Color(
                Mathf.Clamp01(color.r - 0.3f),
                Mathf.Clamp01(color.g - 0.3f),
                Mathf.Clamp01(color.b - 0.3f)
                );

            float scale = 1.0f;
            int numbericValue;

            if(int.TryParse(text.Replace("+","").Replace("CRITI", ""), out numbericValue))
            {
                scale = Mathf.Clamp(numbericValue / 10f, 0.8f, 1.2f);
            }

            if (isCritical) scale = 1f;

            damageText.transform.localScale = new Vector3(scale, scale, scale);
        }

        DamageTextEffect effect = damageText.GetComponent<DamageTextEffect>();
        if (effect != null)
        {
            effect.Initialized(isCritical);
        }
    }

    public void ShowDamage(Vector3 position, int amount, bool isPlayer, bool isCritical = false)
    {
        string text = amount.ToString();
        Color color = Color.white;

        if (isPlayer) color = new Color(1.0f, 0.3f, 0.3f);  //맞는 대상이 Player일 경우, 빨간색
        else color = isCritical ? new Color(1f, 0.8f, 0.0f) : Color.white;

        if (isCritical)
        {
            text = "CRITI\n" + text;
        }
        ShowDamageText(position, text, color, isCritical);
    }
    public void ShowDoubleAttack()
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        string text = $"더블공격";
        Color color = Color.cyan;

        ShowDamageText(position, text, color);
    }
    public void ShowWeaknessAnalyzer(bool isOn, int count = 0)
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        string text = $"약점분석 중첩 X {count}";
        Color color = Color.cyan;

        if (!isOn)
        {
            text = $"약점분석 중첩감소";
            color = Color.red;
        }

        ShowDamageText(position, text, color);
    }

    public void ShowAttackSpeedUp(bool isOn)
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        string text = "공격속도 증가";
        Color color = Color.cyan;
        if (!isOn)
        {
            text = "공격속도 감소";
            color = Color.red;
        }
        ShowDamageText(position, text, color);
    }
    public void ShowAttackValueUp(bool isOn)
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        Color color = Color.cyan;
        string text = "공격데미지 증가";
        if (!isOn)
        {
            text = "공격데미지 감소";
            color = Color.red;
        }
        ShowDamageText(position, text, color);
    }

    public void ShowGetShild(bool isOn)
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        Color color = Color.cyan;
        string text = "쉴드 획득";
        if (!isOn)
        {
            text = "쉴드 제거";
            color = Color.red;
        }
        ShowDamageText(position, text, color);
    }

    public void ShowSpeedUp(bool isOn)
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        Color color = Color.cyan;
        string text = "공격속도 증가";
        if (!isOn)
        {
            text = "공격속도 감소";
            color = Color.red;
        }
        ShowDamageText(position, text, color);
    }

    public void ShowResurrection()
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        Color color = Color.yellow;
        string text = "부활";
        ShowDamageText(position, text, color);
    }
    public void ShowHeal(Vector3 position, int amount)
    {
        string text = amount.ToString();
        Color color = new Color(0.3f, 0.9f, 0.3f);
        ShowDamageText(position, text, color);
    }

    public void ShowMiss()
    {
        Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;

        ShowDamageText(position, "회피성공", Color.gray);
    }

    public void ShowExecution(Vector3 position)
    {
        ShowDamageText(position, "처형", Color.red);
    }

    private Queue<string> effects = new Queue<string>();
    private Coroutine currentCor;
    public void ShowLevelUpCorrection(string text)
    {
        effects .Enqueue(text);

        if (currentCor == null)
        {
            currentCor = StartCoroutine(ShowLevelUp());
        }
    }

    public IEnumerator ShowLevelUp()
    {
        float genTime = 0.6f;

        while (effects.Count > 0)
        {
            string text = effects.Dequeue();

            Vector3 position = PlayerStatus.Instance.gameObject.transform.position + PlayerStatus.Instance.gameObject.transform.up * 0.5f;
            Color color = Color.cyan;
            ShowDamageText(position, text, color);

            yield return new WaitForSeconds(genTime);
        }
        currentCor = null;
    }
}

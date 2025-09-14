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

    public void ShowHeal(Vector3 position, int amount)
    {
        string text = amount.ToString();
        Color color = new Color(0.3f, 0.9f, 0.3f);
        ShowDamageText(position, text, color);
    }

    public void ShowMiss(Vector3 position)
    {
        ShowDamageText(position, "Miss", Color.gray);
    }
}

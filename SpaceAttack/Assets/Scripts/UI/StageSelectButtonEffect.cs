using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectButtonEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image targetImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 0.7f);   
    public float flashSpeed = 5f;

    [Header("Scale Effect")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public float scaleSpeed = 10f;

    [Header("Sound")]
    public AudioSource sfx;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public bool isBossButton = false;
    public AudioClip bossClickClip;

    private bool hovering = false;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
        sfx = GetComponent<AudioSource>();
    }

    private void Update()
    { 
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            hovering ? hoverScale : normalScale,
            Time.deltaTime * scaleSpeed
        );

        if (hovering)
        {
            float t = (Mathf.Sin(Time.time * flashSpeed) + 1) * 0.5f;
            targetImage.color = Color.Lerp(normalColor, hoverColor, t);
        }
        else
        {
            targetImage.color = Color.Lerp(targetImage.color, normalColor, Time.deltaTime * 10f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;

        if (sfx != null && hoverClip != null)
            sfx.PlayOneShot(hoverClip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sfx == null) return;

        if (isBossButton && bossClickClip != null)
        {
            sfx.PlayOneShot(bossClickClip);
        }
        else if (clickClip != null)
        {
            sfx.PlayOneShot(clickClip);
        }
    }
}

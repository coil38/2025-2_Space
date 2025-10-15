using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PlayerInventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image image { get; private set; }
    public Sprite itemImage {  get; private set; }
    public RelicSO relic { get; private set; }

    private void OnEnable()
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        image = Array.Find(images, i => i.gameObject != this.gameObject);
    }

    public void AddItemImage(RelicSO relic)
    {
        this.relic = relic;
        Sprite sprite = relic.iconSprite;
        itemImage = sprite;
        image.sprite = sprite;
        ChangeColorA();

        LogUtil.Log($"{relic.relicName}획득");
    }

    public void RemoveItemImage()
    {
        itemImage = null;
        image.sprite = null;
        ChangeColorA();
    }

    private void ChangeColorA()
    {
        int value = 0;

        if (image.color.a == 0) value = 1;
        else value = 0;

        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (relic == null) return;
        PlayerUIManager.instance.OnPlayerInventoryInfo(relic);
    }
}

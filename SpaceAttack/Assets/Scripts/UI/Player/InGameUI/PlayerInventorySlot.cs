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
    public int relicInstanceId { get; private set; }

    private void OnEnable()
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        image = Array.Find(images, i => i.gameObject != this.gameObject);

        if(image != null) ChangeColorA(0);
    }

    public void AddItemImage(RelicSO relic, int relicInstanceId)
    {
        this.relic = relic;
        this.relicInstanceId = relicInstanceId;
        Sprite sprite = relic.iconSprite;
        itemImage = sprite;
        image.sprite = sprite;
        ChangeColorA(1);

        //LogUtil.Log($"{relic.relicName}획득");
    }

    public void RemoveItem()
    {
        itemImage = null;
        relic = null;
        relicInstanceId = -1;
        image.sprite = null;
        ChangeColorA(0);
    }

    private void ChangeColorA(float value)
    {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (relic == null) return;
        PlayerUIManager.instance.OnPlayerInventoryInfo(relic, relicInstanceId);
    }
}

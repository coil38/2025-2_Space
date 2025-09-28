using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    private Image[] slotImages;
    private int currentIndex;
    private void OnEnable()
    {
        if(slotImages == null) 
            slotImages = GetComponentsInChildren<Image>();
    }
    public void GetItem(Sprite relicIcon)
    {
        Image itemImage = slotImages[currentIndex].GetComponentInChildren<Image>();
        itemImage.sprite = relicIcon;

        currentIndex ++;
    }
    
    public void RemoveItem()
    {
        Image itemImage = slotImages[currentIndex].GetComponentInChildren<Image>();
        itemImage.sprite = null;

        currentIndex--;
    }
}

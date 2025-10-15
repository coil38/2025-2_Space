using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemUI : MonoBehaviour
{
    private List<PlayerInventorySlot> slots;
    private List<RelicSO> items = new List<RelicSO>();
    private int currentIndex;

    public GameObject slotPrf;       //슬롯 프리팹
    public int minSlotCount = 7;     //슬롯 최소 개수

    private void OnEnable()
    {
        if (slots == null)
        {
            PlayerInventorySlot[] temps = GetComponentsInChildren<PlayerInventorySlot>();
            slots = new List<PlayerInventorySlot>(temps);
        }
    }
    public void AddItem(RelicSO relicSO)  //아이템 장착
    {
        items.Add(relicSO);

        if (currentIndex >= minSlotCount)
        {
            PlayerInventorySlot temp = Instantiate(slotPrf, this.gameObject.transform).GetComponent<PlayerInventorySlot>();
            temp.AddItemImage(relicSO);
            slots.Add(temp);
        }
        else
        {
            LogUtil.Log($"아이템_{currentIndex} 획득");
            slots[currentIndex].AddItemImage(relicSO);
        }

        currentIndex++;
    }
    
    public void RemoveItem(RelicSO relicSO)              //아이템 장착 해제
    {
        Sprite relicIcon = relicSO.iconSprite;
        int targetIndex = 0;

        foreach (var slot in slots)
        {
            if (slot.itemImage == relicIcon)
            {
                slot.RemoveItemImage();
                targetIndex = slots.IndexOf(slot);
                break;
            }
        }

        for (int i = targetIndex; i < currentIndex - 1; i++)
        {
            RelicSO relic = slots[i + 1].relic;
            slots[i + 1].RemoveItemImage();
            slots[i].AddItemImage(relic);
        }

        if(currentIndex >= minSlotCount)
        {
            slots.RemoveAt(currentIndex);
            Destroy(slots[currentIndex].gameObject);
        }
        currentIndex--;
    }

    public void RemoveAllItems()
    {
        Initialize();
        foreach (var slot in slots)
        {
            slot.RemoveItemImage();
        }
    }

    private void Initialize()
    {
        currentIndex = 0;
    }
}

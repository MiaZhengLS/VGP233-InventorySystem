using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ItemDataRuntime
{
    public ItemData config;
    public int num;
    public bool isValid
    {
        get
        {
            return config != null && num > 0;
        }
    }
}

public class UIInventoryManager : MonoBehaviour
{
    public int maxInventorySlots;
    public List<ItemData> configList;
    public RectTransform rootInventory;
    public GameObject inventoryItem;
    public List<ItemDataRuntime> inventoryList;
    void Start()
    {
        inventoryList = new List<ItemDataRuntime>();
        for(int i=0;i<maxInventorySlots;++i)
        {
            GameObject item = Instantiate(inventoryItem, rootInventory);
            RectTransform rt = item.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            UIInventoryItem scp = item.GetComponent<UIInventoryItem>();
            scp.onItemClicked += OnItemUsed;
            scp.onSwapItems += OnItemSwapped;
            inventoryList.Add(new ItemDataRuntime());
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            int randIdx = Random.Range(0, configList.Count);
            int randNum = Random.Range(1, 3);
            randNum = configList[randIdx].canStack ? randNum : 1;
            OnItemAdded(configList[randIdx], randNum);
        }
    }

    void OnItemAdded(ItemData itemData, int num)
    {
        int idx = -1;
        if (itemData.canStack)
        {
            for (int i = 0; i < inventoryList.Count; i++)
            {
                if (!inventoryList[i].isValid)
                {
                    continue;
                }
                if (inventoryList[i].config.itemId == itemData.itemId)
                {
                    inventoryList[i].num += num;
                    idx = i;
                    break;
                }
            }
        }

        if (idx == -1)
        { 
            for (int i = 0; i < inventoryList.Count; i++)
            {                
                if (!inventoryList[i].isValid)
                {
                    inventoryList[i].config = itemData;
                    inventoryList[i].num = num;
                    idx = i;
                    break;
                }
            }
        }
        if (idx != -1)
        {
            Transform item = rootInventory.GetChild(idx);
            UIInventoryItem scp = item.GetComponent<UIInventoryItem>();
            scp.SetItem(inventoryList[idx]);
        }
    }

    void OnItemUsed(object sender, EventArgs eventArgs)
    {
        UIInventoryItem item = (UIInventoryItem)sender;
        UseItemEventArg args = eventArgs as UseItemEventArg;
        int idx = args.itemIdx;
        if (!inventoryList[idx].isValid)
        {
            return;
        }
        inventoryList[idx].num--;
        item.SetItem(inventoryList[idx]);
    }

    void OnItemSwapped(object sender, EventArgs eventArgs)
    {
        SwapItemsEventArg args = eventArgs as SwapItemsEventArg;
        var tempData = inventoryList[args.swapItemIdx1];
        inventoryList[args.swapItemIdx1] = inventoryList[args.swapItemIdx2];
        inventoryList[args.swapItemIdx2] = tempData;
        
        rootInventory.GetChild(args.swapItemIdx1).GetComponent<UIInventoryItem>().SetItem(inventoryList[args.swapItemIdx1]);
        rootInventory.GetChild(args.swapItemIdx2).GetComponent<UIInventoryItem>().SetItem(inventoryList[args.swapItemIdx2]);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UseItemEventArg : EventArgs
{
    public int itemIdx;
    public ItemDataRuntime data;
}

public class SwapItemsEventArg : EventArgs
{
    public int swapItemIdx1;
    public int swapItemIdx2;
}

public class UIInventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    public Image imgItem;
    public Text txtNum;
    public Color colorDragging;
    public event EventHandler onItemClicked;
    public event EventHandler onSwapItems;
    private Transform parentTransform;
    private RectTransform rtItem;
    private CanvasGroup canvasGroup;
    private float canvasScale;
    public ItemDataRuntime data
    {
        get;
        private set;
    }
    private void Awake()
    {
        var canvas = GetComponentInParent<Canvas>();
        canvasScale = canvas.scaleFactor;
        parentTransform = transform.parent.parent;
        rtItem = imgItem.gameObject.GetComponent<RectTransform>();
        canvasGroup = imgItem.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;
        txtNum.gameObject.SetActive(false);
        imgItem.gameObject.SetActive(false);
    }

    public void SetItem(ItemDataRuntime data)
    {
        if (data == null || data.num == 0)
        {
            RemoveItem();
        }
        else
        {
            this.data = data;
            imgItem.sprite = data.config.itemImg;
            imgItem.gameObject.SetActive(true);
            txtNum.text = data.num.ToString();
            txtNum.gameObject.SetActive(data.num > 1);
        } 
    }

    public void RemoveItem()
    {
        this.data = null;
        imgItem.sprite = null;
        imgItem.gameObject.SetActive(false);
        txtNum.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            return;
        }

        rtItem.anchoredPosition += eventData.delta / canvasScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (data == null)
        {
            return;
        }
        canvasGroup.blocksRaycasts = false;
        rtItem.SetParent(parentTransform);
        imgItem.color = colorDragging;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        imgItem.color = Color.white;
        rtItem.SetParent(transform);
        rtItem.anchoredPosition = Vector2.zero;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject objDragging = eventData.pointerDrag;
        if (objDragging.GetComponent<UIInventoryItem>().data == null)
        {
            return;
        }
        SwapItemsEventArg arg = new SwapItemsEventArg();
        arg.swapItemIdx1 = transform.GetSiblingIndex();
        arg.swapItemIdx2 = objDragging.transform.GetSiblingIndex();
        onSwapItems.Invoke(this,arg);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (data!=null)
        {
            UseItemEventArg arg = new UseItemEventArg();
            arg.data = this.data;
            arg.itemIdx = transform.GetSiblingIndex();
            onItemClicked.Invoke(this, arg);
        }
    }
}

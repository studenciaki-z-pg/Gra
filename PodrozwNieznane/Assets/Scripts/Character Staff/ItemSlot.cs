using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image image;
    public event Action<Item> OnRightClickEvent;
    public Item _item;
    public Item Item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item == null)
                image.enabled = false;
            else
            {
                image.sprite = _item.Icon;
                image.enabled = true;
            }
        }
    }


    protected virtual void OnValidate()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData != null && eventData.button == PointerEventData.InputButton.Right) //przycisk myszy
        {
            if (Item != null && OnRightClickEvent != null)
                OnRightClickEvent(Item);
        }
    }


}

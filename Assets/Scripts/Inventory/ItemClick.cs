using UnityEngine;
using UnityEngine.EventSystems;

public class ItemClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            transform.parent.GetComponent<ItemSlot>().Use();
        }
    } 
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NavigationClickedHandler : MonoBehaviour, IPointerClickHandler
{
    public DirectionDisplay directionDisplay;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Increment curIndex by 1
        directionDisplay.curIndex++;

        // Call SetDirection() to update the display
        directionDisplay.SetDirection();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public static class PointerOverGameObject
{
    // https://answers.unity.com/questions/1115464/ispointerovergameobject-not-working-with-touch-inp.html
    /// <returns>true if mouse or first touch is over any event system object ( usually gui elements )</returns>
    public static bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                return true;
        }

        return false;
    }
}

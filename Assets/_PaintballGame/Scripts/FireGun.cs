using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGun : MonoBehaviour
{
    public GameObject paintballPrefab;
    public Transform barrelTip;

    private void Start()
    {
        if (barrelTip == null)
        {
            barrelTip = transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // for mobile Joystick
            if (PointerOverGameObject.IsPointerOverGameObject())
                return;

            Instantiate(paintballPrefab, barrelTip.position, barrelTip.rotation);
        }
    }
}

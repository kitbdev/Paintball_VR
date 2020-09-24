using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int hitCount = 0;
    public Text countText;

    private void OnCollisionEnter(Collision other)
    {
        hitCount++;
        countText.text = "Hit Count: " + hitCount;
    }
}

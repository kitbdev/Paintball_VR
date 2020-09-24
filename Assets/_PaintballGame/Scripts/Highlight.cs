using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Color hightlightColor = Color.red;

    private Color _defaultColor;
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _defaultColor = GetColor();
    }

    public void ShowHighlight(bool show)
    {
        if (show)
        {
            SetColor(hightlightColor);
        }
        else
        {
            SetColor(_defaultColor);
        }
    }
    
    private Color GetColor()
    {
        return _renderer.material.GetColor("_BaseColor");
    }

    private void SetColor(Color color)
    {
        _renderer.material.SetColor("_BaseColor", color);
    }
}

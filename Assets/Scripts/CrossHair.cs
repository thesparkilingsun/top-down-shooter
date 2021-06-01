using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public LayerMask _targetMask;
    public SpriteRenderer _dot;
    public Color _dotHighlightColor;
    Color _ogColor;

    void Start()
    {
        Cursor.visible = false;
        _ogColor = _dot.color;    
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);    
    }

    public void TargetDetection(Ray ray)
    {
        if(Physics.Raycast(ray,100, _targetMask))
        {
            _dot.color = _dotHighlightColor;
        }
        else
        {
            _dot.color = _ogColor;
        }
    }

}

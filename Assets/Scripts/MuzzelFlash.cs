using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzelFlash : MonoBehaviour
{
    public GameObject _flashHolder;
    public Sprite[] _flashSprite;

    public SpriteRenderer[] _spriteRenderers; 
    public float _flashTime;
    void Start()
    {
        Deactivate();
    }
    public void Activate()
    {
        _flashHolder.SetActive(true);
        int flashSpriteIndex = Random.Range(0, _flashSprite.Length);
        for(int i=0; i< _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].sprite = _flashSprite[flashSpriteIndex];
        }

        Invoke("Deactivate", _flashTime);
    }

    void Deactivate()
    {
        _flashHolder.SetActive(false);
    }
}

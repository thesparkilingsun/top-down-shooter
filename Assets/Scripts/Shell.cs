using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody _shellRigidbody;
    public float _forceMin;
    public float _forceMax;
    float _lifetime = 4;
    float _fadetime = 2;
    // Start is called before the first frame update
    void Start()
    {
        float force = Random.Range(_forceMin, _forceMax);
        _shellRigidbody.AddForce(transform.right * force);
        _shellRigidbody.AddTorque(Random.insideUnitSphere * force);
        StartCoroutine(Fade());
    }

    // Update is called once per frame
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(_lifetime);
        float percent = 0;
        float fadeSpeed = 1 / _fadetime;
        Material shellMaterial = GetComponent<Renderer>().material;
        Color initialColor = shellMaterial.color;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            shellMaterial.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }
        Destroy(gameObject);
    }
}

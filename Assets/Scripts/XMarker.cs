using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMarker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyTimer());
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}

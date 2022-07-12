using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySparkParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThis", 2.0f);
    }

    // Update is called once per frame
    void DestroyThis()
    {
        Destroy(gameObject);
    }
}

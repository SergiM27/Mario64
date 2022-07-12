using UnityEngine;
using System.Collections;

public class PowerUpHealth : MonoBehaviour
{
	public AudioClip clip;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
            Destroy(transform.parent.gameObject);
        }
    }
}

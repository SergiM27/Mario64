using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioPunch : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    GameObject particles;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Goomba"))
        {
            Debug.Log("GoombaPunch"); 
            Vector3 collisionPosition = collision.contacts[0].point;
            GameObject goomba = collision.gameObject;
            goomba.GetComponent<GoombaEnemy>().ReciveHit(playerController.gameObject,3);
            particles = Instantiate(playerController.PunchParticles, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(particles, 0.5f);
        }

        if (collision.collider.CompareTag("Koopa"))
        {
            Debug.Log("KoopaPunch");
            Vector3 collisionPosition = collision.contacts[0].point;
            GameObject goomba = collision.gameObject;
            goomba.GetComponent<KoopaEnemy>().ReciveHit(playerController.gameObject, 3);
            particles = Instantiate(playerController.PunchParticles, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(particles, 0.5f);
        }
    }
}

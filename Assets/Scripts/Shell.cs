using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour, GameManager.IRestartGameElement
{


    private void Start()
    {
        GameManager.instance.AddRestartGameElement(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Koopa")
        {
            if(this.GetComponent<Rigidbody>().velocity.magnitude > 2)
            {
                collision.gameObject.GetComponent<KoopaEnemy>().Kill();
            }
        }
        else if (collision.gameObject.tag == "Goomba")
        {
            if (this.GetComponent<Rigidbody>().velocity.magnitude > 2)
            {
                collision.gameObject.GetComponent<GoombaEnemy>().Kill();
            }
        }
    }

    public void RestartGame()
    {
        gameObject.SetActive(false);
    }
}

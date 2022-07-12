using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpShell : MonoBehaviour
{
    public GameObject myHands;
    bool canpickup; 
    GameObject objectIWantToPickUp; 
    public bool hasItem;
    public float speed;
    public bool shotShell;
    public float shotShellCD;
    public Animator animator;

    void Start()
    {
        shotShellCD = 0.5f;
        shotShell = false;
        canpickup = false;
        hasItem = false;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canpickup == true) 
        {
            if ((InputSystem.instance.Grab) && hasItem==false) 
            {
                if (hasItem == false)
                {
                    if (shotShell == false)
                    {
                        objectIWantToPickUp.GetComponent<Rigidbody>().isKinematic = true;
                        objectIWantToPickUp.transform.position = myHands.transform.position;
                        objectIWantToPickUp.transform.parent = myHands.transform;
                        hasItem = true;
                        animator.SetBool("Grab", true);
                    }
                }

            }
            else if ((InputSystem.instance.Grab) && hasItem==true)
            {
                if (hasItem == true)
                {
                    objectIWantToPickUp.GetComponent<Rigidbody>().isKinematic = false;
                    objectIWantToPickUp.transform.parent = null;
                    objectIWantToPickUp.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
                    hasItem = false;
                    shotShell = true;
                    Invoke("NoDamageToMyself", shotShellCD);
                    animator.SetBool("Grab", false);
                }
            }
        }
    }

    void NoDamageToMyself()
    {
        shotShell = false;
    }
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Shell") 
        {
            canpickup = true;
            objectIWantToPickUp = other.gameObject; 
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canpickup = false;
    }


}

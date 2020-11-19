using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveOnContact : MonoBehaviour
{
    public Animator animator;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") {
            animator.SetTrigger("Evolve");
        }
    }
}

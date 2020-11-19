using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    /*
     * Le but de ce TD est de permettre à Maxime de faire se déplacer une mouche entre plusieurs points
     */

    // Permet de set des points dans l inspector
    public Transform[] points;

    // Index du point cible courant
    private int index;

    void Awake()
    {
        index = 0;
    }

    void Update()
    {
        // Vector3.MoveTowards ou Vector3.Smoothdamp pour lisser, proposer les 2 avec une option peut etre...
    }
}

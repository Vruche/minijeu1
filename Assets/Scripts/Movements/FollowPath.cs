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
    private Vector3 deltaMove;

    void Awake()
    {
        index = 0;
    }

    void Update()
    {
        float ySin;
        // Vector3.MoveTowards ou Vector3.Smoothdamp pour lisser, proposer les 2 avec une option peut etre...
        //Changement de target
        if (transform.position.x.Equals(points[index].position.x))
            index = (index + 1) % points.Length;

        deltaMove = Vector3.MoveTowards(transform.position, points[index].position, 2f * Time.deltaTime);
        ySin = Mathf.Sin(Time.time * 10) * Time.deltaTime;
        deltaMove.y += ySin;

        transform.position = deltaMove;
    }
}

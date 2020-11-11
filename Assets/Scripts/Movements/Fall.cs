using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Fall : MonoBehaviour
{
    public float speed;

    private Controller2D controller;

    private void Awake()
    {
        controller = GetComponent<Controller2D>();
    }

    void FixedUpdate()
    {
        controller.Move(Vector3.down * speed * Time.deltaTime); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goutte : MonoBehaviour
{
    // Parameter
    public float respawnDelay = 2f;

    // Dependencies
    private Animator anim;
    private Fall fallMovement;
    private Vector3 spawnPosition;

    // State
    public enum State { Formation, Falling, Splosh };
    private State state;

    // Initialize dependencies and store spawn position
    private void Awake()
    {
        anim = GetComponent<Animator>();
        fallMovement = GetComponent<Fall>();
        spawnPosition = transform.position;
    }

    private void OnEnable()
    {
        Spawn();
    }

    // Reset attributes when we spanw the GOUTTE
    public void Spawn()
    {
        state = State.Formation;
        transform.position = spawnPosition;
        fallMovement.enabled = false;
        anim.Rebind();
    }

    // Called by the Animator when the "Formation" animation is done
    public void OnFormationDone()
    {
        state = State.Falling;
        anim.SetTrigger("StartsFalling");
        fallMovement.enabled = true;
    }

    // Called by the Animator when the "Splosh" animation is done
    public void OnSploshDone()
    {
        StartCoroutine("WaitAndRespawn");
    }

    // Detect collision (collider is only enabled while falling, see animation)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        state = State.Splosh;
        anim.SetTrigger("HitsSomething");
    }

    // Coroutine to respawn after a delay
    private IEnumerator WaitAndRespawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        Spawn();
    }
}

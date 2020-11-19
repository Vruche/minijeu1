using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    // Dependance externe
    public PlayerInput input;

    // Dependance interne
    private Controller2D controller;
    private PlayerAnimator playerAnimator;

    // Parametres
    public float moveSpeed = 3.75f; // vitesse de deplacement: 60 pixels par seconde (1 unit = 16 pixels)
    private float gravity;
    public float minJumpHeight = 2f;
    public float maxJumpHeight = 2f;
    public float timeToJumpApex = .5f;
    private float minJumpVelocity, maxJumpVelocity;

    public bool isJumpAllowed;

    // Etat
    public Vector2 velocity;
    public bool isGrounded;
    public bool lookRight;

    // Awake est appele a linitialisation de lobjet lorsque la scene se charge
    private void Awake()
    {
        controller = GetComponent<Controller2D>();
        playerAnimator = GetComponent<PlayerAnimator>();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // Start est appele avant la 1ere frame ou lobjet est actif dans la scene
    private void Start()
    {
        velocity = Vector2.zero;
        lookRight = true;
    }

    // Methode appelee a chaque frame
    private void Update()
    {
        input.GetInput();

        isGrounded = controller.collisions.below;
        if (isGrounded) {
            velocity.y = 0f;
        }
        if (isJumpAllowed && isGrounded && input.jumpDown) {
            velocity.y = maxJumpVelocity;
        }
        velocity.x = input.direction.x * moveSpeed;
        velocity.y += gravity * Time.deltaTime;

        // Update look direction
        if (input.direction.x > 0f) {
            lookRight = true;
        }
        else if (input.direction.x < 0f) {
            lookRight = false;
        }

        controller.Move(velocity * Time.deltaTime);

        playerAnimator.UpdateAnimation();
    }
}

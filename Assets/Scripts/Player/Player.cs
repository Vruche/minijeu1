using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Controller2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    // Dependance aux autres composants
    private PlayerInput input;
    private Controller2D controller;
    private Animator animator;

    // Parametres
    public float moveSpeed = 3.75f; // vitesse de deplacement: 60 pixels par seconde (1 unit = 16 pixels)
    private float gravity;
    public float minJumpHeight = 2f;
    public float maxJumpHeight = 2f;
    public float timeToJumpApex = .5f;
    private float minJumpVelocity, maxJumpVelocity;

    public bool isJumpAllowed;

    // Etat
    private Vector2 velocity;
    private bool isGrounded;
    private bool lookRight;

    // Awake est appele a linitialisation de lobjet lorsque la scene se charge
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();

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
    // On y mettra a jour l etat du joueur
    private void Update()
    {
        input.GetInput();

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
        isGrounded = controller.collisions.below;
        if (isGrounded) {
            velocity.y = 0f;
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        // Temporairement degueu
        if (gameObject.name == "Graine") {
            animator.SetFloat("Direction", Mathf.Sign(velocity.x));
        }
        else {
            animator.SetFloat("Direction", Mathf.Sign(velocity.x));
            animator.SetFloat("xVelocity", velocity.x);
            animator.SetFloat("yVelocity", velocity.y);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("LookRight", lookRight);
        }
    }

    // Methode appelee a chaque frame ou la physique va etre mise a jour
    // On l utilisera pour deplacer le joueur
    private void FixedUpdate()
    {
        // todo: bouger le joueur en fonction de la gravite et les inputs du joueur
        // En utilisant la methode Move du controller
        /*
        controller.Move(velocity * Time.deltaTime);

        isGrounded = controller.collisions.below;
        if (isGrounded) {
            velocity.y = 0f;
        }
        */
    }

    // Y a aussi LateUpdate qui sexecute a chaque frame a la fin de la boucle, donc forcement apres tous les Update et FixedUpdate
    // Utile par exemple pour controller les cameras ou les animations
    private void LateUpdate()
    {

    }

}

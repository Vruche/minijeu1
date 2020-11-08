using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Controller2D))]
public class Player : MonoBehaviour
{
    // Dependance aux autres composants
    private PlayerInput input;
    private Controller2D controller;

    // Parametres
    public float moveSpeed = 4f; // vitesse de deplacement tbd
    private float gravity = -10f; // a definir

    // Awake est appele a linitialisation de lobjet lorsque la scene se charge
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        controller = GetComponent<Controller2D>();
    }

    // Start est appele avant la 1ere frame ou lobjet est actif dans la scene
    private void Start()
    {
        
    }

    // Methode appelee a chaque frame
    // On y mettra a jour l etat du joueur
    private void Update()
    {
        
    }

    // Methode appelee a chaque frame ou la physique va etre mise a jour
    // On l utilisera pour deplacer le joueur
    private void FixedUpdate()
    {
        // todo: bouger le joueur en fonction de la gravite et les inputs du joueur
    }

    // Y a aussi LateUpdate qui sexecute a chaque frame a la fin de la boucle, donc forcement apres tous les Update et FixedUpdate
    // Utile par exemple pour controller les cameras ou les animations
}

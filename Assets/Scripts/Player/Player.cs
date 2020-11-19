using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton
    public static Player instance;

    public GameObject[] evolutions;
    int evolutionIndex;

    void Awake()
    {
        evolutionIndex = 0;
        instance = this;
    }

    public void NextEvolution()
    {
        evolutions[evolutionIndex].SetActive(false);
        evolutionIndex++;
        evolutions[evolutionIndex].transform.parent = transform;
        evolutions[evolutionIndex].SetActive(true);
    }
}

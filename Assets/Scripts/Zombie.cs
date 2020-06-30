using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class Zombie : MonoBehaviour
{
    [SerializeField] public float wanderSpeed;
    [SerializeField] public float searchSpeed;
    [SerializeField] public float chaseSpeed;
    [SerializeField] public float maxHealth;
    public float currentHealth;

}

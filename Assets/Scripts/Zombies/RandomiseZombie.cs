using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseZombie : MonoBehaviour
{
    [SerializeField] Transform modelsParent;
    GameObject[] zombieOptions;
    int zombieCount;

    // Start is called before the first frame update
    void Start()
    {
        zombieCount = modelsParent.childCount - 1;

        // Ensure all are disabled        
        for (int i = 1; i <= zombieCount; i++)
        {
            modelsParent.GetChild(i).gameObject.SetActive(false);
        }

        int randoChoice = Random.Range(1, zombieCount);
        
        modelsParent.GetChild(randoChoice).gameObject.SetActive(true);
    }
}

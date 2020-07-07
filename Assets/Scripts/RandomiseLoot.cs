using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseLoot : MonoBehaviour
{
    [SerializeField] Transform modelsParent;
    int boxCount;

    // Start is called before the first frame update
    void Awake()
    {
        boxCount = modelsParent.childCount;
        // Ensure all are disabled        
        for (int i = 0; i < boxCount; i++)
        {
            modelsParent.GetChild(i).gameObject.SetActive(false);
        }

        int randoChoice = Random.Range(1, boxCount);
        
        modelsParent.GetChild(randoChoice).gameObject.SetActive(true);
        Debug.Log("Loot choice made");
    }
}

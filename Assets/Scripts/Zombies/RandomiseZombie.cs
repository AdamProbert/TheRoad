using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseZombie : MonoBehaviour
{
    [SerializeField] Transform modelsParent;
    [SerializeField] List<Material> altMaterial;
    int zombieCount;

    // Start is called before the first frame update
    void Awake()
    {
        int randoChoiceMaterial = Random.Range(0, altMaterial.Count);

        zombieCount = modelsParent.childCount - 1;

        // Ensure all are disabled        
        for (int i = 1; i <= zombieCount; i++)
        {
            modelsParent.GetChild(i).gameObject.SetActive(false);
        }

        int randoChoice = Random.Range(1, zombieCount);
        
        modelsParent.GetChild(randoChoice).gameObject.SetActive(true);
        modelsParent.GetComponentInChildren<Renderer>().material = altMaterial[randoChoiceMaterial];
    }
}

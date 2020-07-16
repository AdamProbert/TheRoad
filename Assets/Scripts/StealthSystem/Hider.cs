using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterEventManager))]
[RequireComponent(typeof(Entity))]
public class Hider : MonoBehaviour
{

    [Header("Visibility")]
    [SerializeField] LayerMask lightBlockingLayers;
    
    [SerializeField] float visibilityUpdateTime;
    
    GameObject sunlight;
    Entity entity;
    float nextUpdateTime = 0;
    Light[] lightSources; // Used to determine if in shadow
    bool isMoving;

    private float _currentVisibility = 0;
    public float currentVisibility{
        get {return _currentVisibility;}
        set {
            if(_currentVisibility != value)
            {
                _currentVisibility = value;
                characterEventManager.OnCharacterVisibilityChange(_currentVisibility);
            }
        }
    }

    bool isStealthed = false;
    CharacterEventManager characterEventManager;

    private void Awake() 
    {
        characterEventManager = GetComponent<CharacterEventManager>();    
        lightSources = GameObject.FindObjectsOfType<Light>();
        sunlight = GameObject.Find("Sunlight");
        entity = GetComponent<Entity>();
    }

    private void Update() 
    {
        if(Time.time > nextUpdateTime)
        {
            float newVisibility = CalculateCurrentVisibility();
            nextUpdateTime = Time.time + visibilityUpdateTime;
            if(newVisibility != currentVisibility)
            {
                characterEventManager.OnCharacterVisibilityChange(newVisibility);
            }
            currentVisibility = newVisibility;
        }
    }

    public Vector3 GetVisiblePoint()
    {
        return entity.GetAimPointPosition();
    }

    public bool IsStealthed()
    {
        return isStealthed;
    }

    private float CalculateCurrentVisibility()
    {

        RaycastHit hit;
        float visibility = 0f;

        if(isStealthed)
        {
            visibility -= 1f;
        }
        
        // Factor in shadow
        // First sun light
        if(Physics.Linecast(transform.position, sunlight.transform.position, out hit, lightBlockingLayers))
        {
            // If in direct sunlight = easier to spot
            if(hit.collider.GetComponent<Light>())
            {
                Debug.DrawLine(transform.position, hit.point, Color.white, .1f);
                visibility += 1f;
            }
            // If in shaows harder to spot
            else
            {
                visibility -= 1f;
                Debug.DrawLine(transform.position, hit.point, Color.red, .1f);
            }
        }
        else
        {
            Debug.DrawLine(transform.position, sunlight.transform.position, Color.green, .1f);
        }

        // Factor in moving / doing things
        if(isMoving)
        {
            visibility += 0.5f;
        }
        return visibility;

        // // Then any other light sources
        // foreach (Light light in lightSources)
        // {
        //     if(Physics.Linecast(transform.position, light.transform.position, out hit, lightBlockingLayers))
        //     {
        //         if(hit.collider.GetComponent<Light>())
        //         {
        //             // Check range to light
        //             if(Vector3.Distance(transform.position, light.transform.position) < light.range)
        //             {
        //                 // Check light pointing in this direction
        //                 if(light.type == LightType.Spot)
        //                 {
        //                     Vector3 dirFromAtoB = (transform.position - light.transform.position).normalized;
        //                     float dotProd = Vector3.Dot(dirFromAtoB, light.transform.forward);
        //                     if(dotProd > 0.9) 
        //                     {
        //                         visibility += 0.2f;
        //                     }
        //                 }
        //                 else
        //                 {
        //                     visibility += 0.2f;
        //                 }
        //             }
        //         }
        //     }
        //}
        // return visibility;
    //}
    
    }
    private void HandleChangeStealthState(CharacterStealthState newState)
    {
        if(newState == CharacterStealthState.SNEAKING)
        {
            isStealthed = true;
        }
        else
        {
            isStealthed = false;
        }
    }
    private void HandleCharacterDeath()
    {
        this.enabled = false;
    }

    private void HandleStateChange(CharacterState newState)
    {
        if(newState == CharacterState.WAITING)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }

    private void OnEnable() 
    {
        characterEventManager.OnCharacterChangeStealthState += HandleChangeStealthState;
        characterEventManager.OnCharacterDied += HandleCharacterDeath;
        characterEventManager.OnCharacterChangeState += HandleStateChange;
    }

    private void OnDisable() 
    {
        characterEventManager.OnCharacterChangeStealthState -= HandleChangeStealthState;
        characterEventManager.OnCharacterDied -= HandleCharacterDeath;
        characterEventManager.OnCharacterChangeState -= HandleStateChange;
    }
}

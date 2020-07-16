using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ZombieData))]
[RequireComponent(typeof(Entity))]
public class Finder : MonoBehaviour
{
    [Header("Vision attributes")]
    [SerializeField] float findUpdateDelay;
    [SerializeField][Tooltip("At what value does a Hider become visible")] float visibileTrigger; // Could configure this per hider.. that would be cool

    [Header("Sight Line attributes")]
    [SerializeField][Tooltip("At what vlaue should the sight line start rendering")] float minSightLineValue;

    public Action<Hider> OnCharacterSpotted = delegate{};
    private Hider _visibleHider;
    public Hider visibleHider{
        get {return _visibleHider;}
        set {}
    }
    float nextUpdateTime;

    // A reference to every "Hider" in the game.
    // The float value will change over time as the character becomes more or less visible.
    Dictionary<Hider, float> possibleTargets = new Dictionary<Hider, float>();
    LineRenderer sightLine;
    Entity entity; 
    ZombieData data;

    private void Awake() 
    {
        nextUpdateTime = Time.time + UnityEngine.Random.Range(0f,1f); // Offset update time randomly to "share the load"
        data = GetComponent<ZombieData>();
        entity = GetComponent<Entity>();
        sightLine = GetComponent<LineRenderer>();
        // sightLine.material = new Material(Shader.Find("Particles/Additive"));
        // Initialize hider dictionary
        foreach (Hider h in FindObjectsOfType<Hider>())
        {
            possibleTargets.Add(h, 0);
        }
    }

    private void Update() 
    {
        if(Time.time > nextUpdateTime)
        {
            CalculateChanceToSeeAllTargets();
            nextUpdateTime = Time.time + findUpdateDelay;
        }

        DrawSightLines();
        
    }

    private void DrawSightLines()
    {
        // Source: https://stackoverflow.com/questions/10290838/how-to-get-max-value-from-dictionary/10290858
        Hider hiderWithMaxValue = possibleTargets.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        float value = possibleTargets[hiderWithMaxValue];

        if(hiderWithMaxValue.IsStealthed() && value > minSightLineValue)
        {
            float lineThickness = (value/visibileTrigger) /2; // Should set the thickness between 0 and 0.5
            // Color newColor = new Color(255, 255, 255, alphaCalc);
            sightLine.enabled = true;
            sightLine.positionCount = 2;
            sightLine.SetPosition(0, entity.GetAimPointPosition());
            sightLine.SetPosition(1, hiderWithMaxValue.GetVisiblePoint());
            sightLine.startWidth = lineThickness;
            sightLine.endWidth = lineThickness;
        }
        else
        {
            sightLine.enabled = false;
        }
        
    }

    private void CalculateChanceToSeeAllTargets()
    {
        float chance;
        // Can't iterate and change a dictionary apparently..
        List<Hider> keys = new List<Hider>(possibleTargets.Keys);
        keys.Shuffle(); // Randomise list so we don't priorities the first character in sight
        foreach (Hider h in keys)
        {
            chance = CalculateChanceToSeeTarget(h);
            float otherChance = possibleTargets[h];
            otherChance = Mathf.Clamp(otherChance + chance, 0, visibileTrigger * 1.2f);
            possibleTargets[h] = otherChance;

            if(possibleTargets[h] >= visibileTrigger)
            {
                if(!_visibleHider)
                {
                    _visibleHider = h;
                    OnCharacterSpotted(_visibleHider);
                }
            }

            if(possibleTargets[h] < visibileTrigger && _visibleHider == h)
            {
                _visibleHider = null;
            }
        }
    }

    private float CalculateChanceToSeeTarget(Hider target)
    {
        // First is the hider visible at all
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if ( distance > data.maxViewDistance) {
            return -3;
        }

        RaycastHit hit;
        if(Physics.Linecast(entity.GetAimPointPosition(), target.GetVisiblePoint(), out hit, data.visibleLayers)){
            if(hit.collider != target.GetComponent<Collider>())
            {
                return -3;
            }
        }

        if(
            Vector3.Angle(target.transform.position - transform.position, transform.forward) > data.viewAngle
            && distance > data.maxAwarenessDistance) 
        {
            return -3;
        }

        // Now we can definitely sense them in some capacity.
        float visibility = target.currentVisibility;

        // Factor in distance
        if(distance < 5f)
        {
            visibility += 3f;
        }
        else if(distance < 10f)
        {
            visibility += 0.5f;
        }
        else
        {
            visibility += 0.2f;
        }

        return visibility;
    }

    private void HandleCharacterDeath(Character deadboi)
    {
        if(deadboi.GetComponent<Hider>())
        {
            Hider h = deadboi.GetComponent<Hider>();
            if(possibleTargets.ContainsKey(h))
            {
                possibleTargets.Remove(h);
            }
        }
    }

    private void OnEnable() 
    {
        PlayerEventManager.Instance.OnCharacterDied += HandleCharacterDeath;    
    }

    private void OnDisable() 
    {
        if(PlayerEventManager.Instance)
        {
            PlayerEventManager.Instance.OnCharacterDied -= HandleCharacterDeath;    
        }
    }
}

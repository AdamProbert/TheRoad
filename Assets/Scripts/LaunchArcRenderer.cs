using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour 
{
	[SerializeField] private float h = 25;
	[SerializeField] public float gravity = -18;
    [SerializeField] int resolution = 12;
    LineRenderer line;

    private void Awake() 
    {
        line = GetComponent<LineRenderer>();    
        line.positionCount = resolution;
    }

    public void RenderArc(Vector3 start, Vector3 end)
    {
        line.enabled = true;
        h = Vector3.Distance(start, end) / 4;
        Debug.Log("h before clamp: " + h);
        h = Mathf.Clamp(h, end.y - transform.position.y, 99f);
        Debug.Log("h after clamp: " + h);
        List<Vector3> positions = GetArcPositions(start, end);
        DrawPath(positions);
    }

    public void StopRenderArc()
    {
        line.enabled = false;
    }

    public List<Vector3> GetArcPositions(Vector3 start, Vector3 end)
    {
        ArcData arcData = CalculateLaunchData(start, end);
        List<Vector3> positions = new List<Vector3>();
		for (int i = 1; i <= resolution; i++) {
			float simulationTime = i / (float)resolution * arcData.timeToTarget;
			Vector3 displacement = arcData.initialVelocity * simulationTime + Vector3.up *gravity * simulationTime * simulationTime / 2f;
            positions.Add(start + displacement);
		}
        return positions;
    }


	ArcData CalculateLaunchData(Vector3 start, Vector3 end) {
		float displacementY = end.y - start.y;
		Vector3 displacementXZ = new Vector3 (end.x - start.x, 0, end.z - start.z);
		float time = Mathf.Sqrt(-2*h/gravity) + Mathf.Sqrt(2*(displacementY - h)/gravity);
		Vector3 velocityY = Vector3.up * Mathf.Sqrt (-2 * gravity * h);
		Vector3 velocityXZ = displacementXZ / time;

		return new ArcData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	}

	void DrawPath(List<Vector3> positions) 
    {
        line.SetPositions(positions.ToArray());
	}

	struct ArcData {
		public readonly Vector3 initialVelocity;
		public readonly float timeToTarget;

		public ArcData (Vector3 initialVelocity, float timeToTarget)
		{
			this.initialVelocity = initialVelocity;
			this.timeToTarget = timeToTarget;
		}
		
	}
}
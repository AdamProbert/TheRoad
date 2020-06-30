using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("GameObject")]
    [Description("A combination of line of sight and view angle check for characters")]
    public class CanSeeAnyCharacter : ConditionTask<Transform>
    {
        [RequiredField]
        public BBParameter<List<Character>> targets;
        public BBParameter<float> maxDistance = 50;
        public BBParameter<float> awarnessDistance = 0f;
        public BBParameter<Vector3> offset;

        [SliderField(1, 180)]
        public BBParameter<float> viewAngle = 70f;
        private RaycastHit hit;

        protected override string info {
            get { return "Can See any of " + targets; }
        }

        protected override bool OnCheck() 
        {
            foreach (Character c in targets.value)
            {
                float distance = Vector3.Distance(agent.position, c.transform.position);
                if ( distance > maxDistance.value ) {
                    return false;
                }

                if ( Physics.Linecast(agent.position + offset.value, c.transform.position + offset.value, out hit) ) {
                    if ( hit.collider != c.GetComponent<Collider>() ) {
                        return false;
                    }
                }

                if (Vector3.Angle(c.transform.position - agent.position, agent.forward) < viewAngle.value ) 
                {
                    return true;
                }

                if ( distance < awarnessDistance.value ) 
                {
                    return true;
                }

            }
            return false;
        }

        public override void OnDrawGizmosSelected() {
            if ( agent != null ) {
                Gizmos.DrawLine(agent.position, agent.position + offset.value);
                Gizmos.DrawLine(agent.position + offset.value, agent.position + offset.value + ( agent.forward * maxDistance.value ));
                Gizmos.DrawWireSphere(agent.position + offset.value + ( agent.forward * maxDistance.value ), 0.1f);
                Gizmos.DrawWireSphere(agent.position, awarnessDistance.value);
                Gizmos.matrix = Matrix4x4.TRS(agent.position + offset.value, agent.rotation, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, viewAngle.value, 5, 0, 1f);
            }
        }
    }
}
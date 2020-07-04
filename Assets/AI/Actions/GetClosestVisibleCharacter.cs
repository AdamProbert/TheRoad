using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions
{

    [Category("GameObject")]
    [Description("A combination of line of sight and view angle check for any characters in list")]
    public class GetClosestVisibleCharacter : ActionTask<Transform>
    {

        [BlackboardOnly]
        public BBParameter<Character> saveAs;

        [RequiredField]
        public BBParameter<List<Character>> targets;
        public BBParameter<float> maxDistance = 50;
        public BBParameter<float> awarnessDistance = 0f;
        public BBParameter<Vector3> offset;
        [SliderField(1, 180)]
        public BBParameter<float> viewAngle = 70f;
        private RaycastHit hit;

        protected override string info {
            get { return "Get closest character from " + targets; }
        }

        protected override void OnExecute() 
        {

            Character closest = null;
            float minDist = 9999f;

            foreach (Character c in targets.value)
            {
                float distance = Vector3.Distance(agent.position, c.transform.position);
                if ( distance > maxDistance.value ) {
                    continue;
                }

                if ( Physics.Linecast(agent.position + offset.value, c.transform.position + offset.value, out hit) ) {
                    if ( hit.collider != c.GetComponent<Collider>() ) {
                        continue;
                    }
                }

                if (Vector3.Angle(c.transform.position - agent.position, agent.forward) < viewAngle.value ) 
                {
                    if(distance < minDist)
                    {
                        minDist = distance;
                        closest = c;
                    }
                    continue;
                }

                if ( distance < awarnessDistance.value ) 
                {
                    if(distance < minDist)
                    {
                        minDist = distance;
                        closest = c;
                    }
                    continue;
                }
            }
            
            if(closest != null)
            {
                saveAs.value = closest;
                EndAction(true);   
            }

            EndAction();   
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
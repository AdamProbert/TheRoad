using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Tasks.Actions
{

    [Category("Audio")]
    public class PlayRandomSoundAtPoint : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<List<AudioClip>> audioClips;
        public BBParameter<Vector3> position;
        [SliderField(0, 1)]
        public float volume = 1;
        public bool waitActionFinish;
        public bool overrideAgentPosition;
        
        AudioClip selectedClip;
        Vector3 selectedPosition;


        protected override string info {
            get { return "Playing rando audio from list " + audioClips.ToString(); }
        }

        protected override void OnExecute() {
            if(overrideAgentPosition)
            {
                selectedPosition = position.value;
            }
            else
            {
                selectedPosition = agent.position;
            }

            selectedClip = audioClips.value[Random.Range(0, audioClips.value.Count)];
            AudioSource.PlayClipAtPoint(selectedClip, selectedPosition, volume);
            if ( !waitActionFinish )
                EndAction();
        }

        protected override void OnUpdate() {
            if ( elapsedTime >= selectedClip.length )
                EndAction();
        }
    }
}
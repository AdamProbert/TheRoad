//The purpose of this script is to setup and update the global shader properties for the capped sections 

// Edited by Adam
using UnityEngine;

namespace WorldSpaceTransitions
{
    public class IndividualCappedSectionFollow : MonoBehaviour
    {

        private enum Mode { box, corner };
        private Mode sectionMode;

        private Vector3 tempPos;
        private Vector3 tempScale;
        private Quaternion tempRot;
        private Color capcolor = new Color(0,0,0,1);

        public bool followPosition = true;
        //public bool followRotation = true;

        [SerializeField] private Renderer r; // Change this to find the renderer

        void Awake()
        {

            if (gameObject.GetComponent<CappedSectionBox>()) sectionMode = Mode.box;
            if (gameObject.GetComponent<CappedSectionCorner>()) sectionMode = Mode.corner;
        }
        void Start()
        {
            r.material.SetVector("_SectionDirX", transform.right);
            r.material.SetVector("_SectionDirY", transform.up);
            r.material.SetVector("_SectionDirZ", transform.forward);
            r.material.SetColor("_SectionColor", capcolor);
            SetSection();
        }

        void LateUpdate()
        {

            if (tempPos != transform.position || tempRot != transform.rotation || tempScale != transform.localScale)
            {

                tempPos = transform.position;
                tempRot = transform.rotation;
                tempScale = transform.localScale;
                SetSection();
            }
            r.material.SetVector("_SectionDirX", transform.right);
            r.material.SetVector("_SectionDirY", transform.up);
            r.material.SetVector("_SectionDirZ", transform.forward);
        }


        void OnDisable()
        {

            r.material.DisableKeyword("CLIP_BOX");
            r.material.SetInt("_CLIP_BOX", 0);
            r.material.DisableKeyword("CLIP_CORNER");
            r.material.SetInt("_CLIP_CORNER", 0);
        }

        void OnEnable()
        {
            if (sectionMode == Mode.box)
            {
                r.material.EnableKeyword("CLIP_BOX");
                r.material.SetInt("_CLIP_BOX", 1);
            }
            if (sectionMode == Mode.corner)
            {
                r.material.EnableKeyword("CLIP_CORNER");
                r.material.SetInt("_CLIP_CORNER", 1);
            }
            SetSection();
        }


        void OnApplicationQuit()
        {
            r.material.DisableKeyword("CLIP_BOX");
            r.material.SetInt("_CLIP_BOX", 0);
            r.material.DisableKeyword("CLIP_CORNER");
            r.material.SetInt("_CLIP_CORNER", 0);
        }

        void SetSection()
        {
            if (followPosition)
            {
                r.material.SetVector("_SectionCentre", transform.position);
                r.material.SetVector("_SectionScale", transform.localScale);
            }
        }

    }
}
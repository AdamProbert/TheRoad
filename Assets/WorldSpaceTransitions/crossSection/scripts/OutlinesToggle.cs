using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldSpaceTransitions
{
    public class OutlinesToggle : MonoBehaviour
    {
        public Material outlineMaterial;
        private Color c = Color.white;
        void Start()
        {
            c = outlineMaterial.GetColor("_OutlineColor");
        }
        void OnEnable()
        {
            c = outlineMaterial.GetColor("_OutlineColor");
        }

        void OnDisable()
        {
            ShowOutline(true);
        }

        // Update is called once per frame
        public void ShowOutline(bool val)
        {
            Debug.Log(val);
            Color _c = c;
            if (!val)
            {
                _c.a = 0;
            }
            outlineMaterial.SetColor("_OutlineColor", _c);
        }
    }
}
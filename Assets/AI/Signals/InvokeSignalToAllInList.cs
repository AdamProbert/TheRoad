using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{
    ///----------------------------------------------------------------------------------------------

    [Category("✫ Utility")]
    [Description("Invoke a defined Signal with agent as the target and optionally global.")]
    public class InvokeSignalToAllInList : ActionTask<Transform>
    {
        public BBParameter<SignalDefinition> signalDefinition;
        public BBParameter<List<GameObject>> targets;

        [SerializeField]
        private Dictionary<string, BBObjectParameter> argumentsMap = new Dictionary<string, BBObjectParameter>();
        private object[] args;

        protected override string info { get { return signalDefinition.ToString(); } }

        protected override string OnInit() {
            if ( signalDefinition.isNoneOrNull ) { return "Missing Definition"; }
            args = new object[argumentsMap.Count];
            return null;
        }

        protected override void OnExecute() {
            if(targets.value == null || targets.value.Count <= 0)
            {
                EndAction();
            }

            var def = signalDefinition.value;
            for ( var i = 0; i < def.parameters.Count; i++ ) {
                args[i] = argumentsMap[def.parameters[i].ID].value;
            }
            foreach (GameObject g in targets.value)
            {
                def.Invoke(agent, g.transform, false, args);    
            }
            EndAction();
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnTaskInspectorGUI() {
            base.OnTaskInspectorGUI();
            if ( signalDefinition.isNoneOrNull ) { return; }
            var parameters = signalDefinition.value.parameters;
            EditorUtils.Separator();
            foreach ( var parameter in parameters ) {
                BBObjectParameter bbParam = null;
                if ( !argumentsMap.TryGetValue(parameter.ID, out bbParam) ) {
                    bbParam = argumentsMap[parameter.ID] = new BBObjectParameter(parameter.type) { bb = ownerSystemBlackboard };
                }
                NodeCanvas.Editor.BBParameterEditor.ParameterField(parameter.name, bbParam);
            }

            foreach ( var key in argumentsMap.Keys.ToArray() ) {
                if ( !parameters.Select(v => v.ID).Contains(key) ) {
                    argumentsMap.Remove(key);
                }
            }
        }

#endif
        ///----------------------------------------------------------------------------------------------


    }
}
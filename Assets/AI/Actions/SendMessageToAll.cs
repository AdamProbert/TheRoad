using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Actions
{

    [Category("✫ Reflected")]
    [Description("SendMessage to everyone in list with optional Transform argument")]
    public class SendMessageToAll : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<string> methodName;
        public BBParameter<GameObject[]> receivers;

        protected override string info {
            get { return string.Format("Message {0}()", methodName); }
        }

        protected override void OnExecute() {
            foreach (GameObject receiver in receivers.value)
            {
                receiver.SendMessage(methodName.value);
            }
            EndAction();
        }
    }


    [Category("✫ Reflected")]
    [Description("SendMessage to the agent, optionaly with an argument")]
    public class SendMessage<T> : ActionTask<Transform>
    {

        [RequiredField]
        public BBParameter<string> methodName;
        public BBParameter<T> argument;
        public BBParameter<GameObject[]> receivers;

        protected override string info {
            get { return string.Format("Message {0}({1})", methodName, argument.ToString()); }
        }

        protected override void OnExecute() {
            
            if ( argument.isNull ) {
                 foreach (GameObject receiver in receivers.value)
                {
                    receiver.SendMessage(methodName.value);
                }
                
            } else {
                 foreach (GameObject receiver in receivers.value)
                {
                    receiver.SendMessage(methodName.value, argument.value);
                }
            }
            EndAction();
        }
    }
}
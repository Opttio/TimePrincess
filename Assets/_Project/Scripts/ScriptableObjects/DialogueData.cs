using UnityEngine;

namespace _Project.Scripts.ScriptableObjects
{
    public enum SpeakerSide { Left, Right }
    
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public SpeakerSide speakerSide;
        [TextArea(2, 5)] public string text;
        public Sprite icon;
    }
    
    [CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}
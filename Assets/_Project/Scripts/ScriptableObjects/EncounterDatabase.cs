using UnityEngine;
using Zenject;

namespace _Project.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "EncounterDatabase", menuName = "Story/EncounterDatabase")]
    public class EncounterDatabase : ScriptableObjectInstaller<BonusDatabase>
    {
        [Header("Chrona Encounter")]
        public DialogueData chronaFirstDialogue;
        // public DialogueData chronaSecondDialogue;
        
        // [Header("Blugar Encounter")]
        // public DialogueData bluugarIntroDialogue;

        public override void InstallBindings() => Container.BindInstance(this).AsSingle();
    }
}
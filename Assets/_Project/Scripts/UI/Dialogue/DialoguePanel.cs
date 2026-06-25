using _Project.Scripts.Managers;
using _Project.Scripts.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.Scripts.UI.Dialogue
{
    public class DialoguePanel : MonoBehaviour
    {
        [Header("Left Speaker")]
        [SerializeField] private GameObject _leftBlock;
        [SerializeField] private Image _leftSpeakerIcon;
        [SerializeField] private TextMeshProUGUI _leftSpeakerName;
        [SerializeField] private TextMeshProUGUI _leftDialogueText;
        [SerializeField] private Button _leftNextButton;

        [Header("Right Speaker")] 
        [SerializeField] private GameObject _rightBlock;
        [SerializeField] private Image _rightSpeakerIcon;
        [SerializeField] private TextMeshProUGUI _rightSpeakerName;
        [SerializeField] private TextMeshProUGUI _rightDialogueText;
        [SerializeField] private Button _rightNextButton;
        
        private DialogueManager _dialogueManager;

        [Inject]
        public void Construct(DialogueManager dialogueManager)
        {
            _dialogueManager = dialogueManager;
            _dialogueManager.SetPanel(this);
        }

        private void OnEnable()
        {
            _leftNextButton.onClick.AddListener(OnNextClicked);
            _rightNextButton.onClick.AddListener(OnNextClicked);
        }

        private void OnDisable()
        {
            _leftNextButton.onClick.RemoveListener(OnNextClicked);
            _rightNextButton.onClick.RemoveListener(OnNextClicked);
        }

        public void Show(DialogueLine line)
        {
            gameObject.SetActive(true);
            OffAllBlocks();
            
            if (line.speakerSide == SpeakerSide.Left)
                SetupBlock(_leftBlock, _leftSpeakerIcon, _leftSpeakerName, _leftDialogueText, line);
            else
                SetupBlock(_rightBlock, _rightSpeakerIcon, _rightSpeakerName, _rightDialogueText, line);
        }
        
        public void Hide() => gameObject.SetActive(false);

        private void OnNextClicked() => _dialogueManager.OnNextPressed();

        private void OffAllBlocks()
        {
            _leftBlock.SetActive(false);
            _rightBlock.SetActive(false);
        }
        
        private void SetupBlock(GameObject block,Image icon, TextMeshProUGUI nameText, TextMeshProUGUI dialogueText, DialogueLine line)
        {
            block.SetActive(true);
            icon.sprite = line.icon;
            nameText.text = line.speakerName;
            dialogueText.text = line.text;
        }
    }
}
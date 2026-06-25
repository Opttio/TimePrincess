using _Project.Scripts.ScriptableObjects;
using _Project.Scripts.UI.Dialogue;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Managers
{
    public class DialogueManager
    {
        private DialoguePanel _dialoguePanel;
        private UniTaskCompletionSource _nextPressed;
        private int _currentLineIndex;
        private DialogueData _currentDialogueData;
        
        public void SetPanel(DialoguePanel dialoguePanel) => _dialoguePanel = dialoguePanel;

        public async UniTask PlayDialogue(DialogueData dialogueData)
        {
            _currentDialogueData = dialogueData;
            _currentLineIndex = 0;

            while (_currentLineIndex < _currentDialogueData.lines.Length)
            {
                ShowCurrentLine();
                
                _nextPressed = new UniTaskCompletionSource();
                await _nextPressed.Task;
                
                _currentLineIndex++;
            }
            
            _dialoguePanel.Hide();
        }

        public void OnNextPressed()
        {
            _nextPressed?.TrySetResult();
        }

        private void ShowCurrentLine()
        {
            _dialoguePanel.Show(_currentDialogueData.lines[_currentLineIndex]);
        }
    }
}
using Core.Tokens;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField dice1Input;
        [SerializeField] private TMP_InputField dice2Input;
        [SerializeField] private Button rollButton;
        
        private ITokenController _tokenController;

        public void Init(ITokenController tokenController)
        {
            _tokenController = tokenController;
        }
        
        public void OnRollClicked()
        {
            if (!int.TryParse(dice1Input.text, out var dice1) || dice1 < 1 || dice1 > 6)
            {
                Debug.LogWarning("[UIController] Dice 1 invalid (1-6)");
                return;
            }

            if (!int.TryParse(dice2Input.text, out var dice2) || dice2 < 1 || dice2 > 6)
            {
                Debug.LogWarning("[UIController] Dice 2 invalid (1-6)");
                return;
            }

            var totalSteps = dice1 + dice2;
            Debug.Log($"[UIController] Rolling: {dice1} + {dice2} = {totalSteps}");

            _tokenController.MoveActiveTokenBySteps(totalSteps);
        }
    }
}
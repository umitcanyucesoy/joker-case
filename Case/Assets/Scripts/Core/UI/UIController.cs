using Core.Dice;
using Core.Tokens;
using Event;
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
        private IDiceController _diceController;

        private void OnEnable()
        {
            EventBus.Subscribe<DiceRollCompletedEvent>(OnDiceRollCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DiceRollCompletedEvent>(OnDiceRollCompleted);
        }

        public void Init(ITokenController tokenController, IDiceController diceController)
        {
            _tokenController = tokenController;
            _diceController = diceController;
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

            Debug.Log($"[UIController] Throwing dice: {dice1}, {dice2}");
            _diceController.RollDice(dice1, dice2);
        }

        private void OnDiceRollCompleted(DiceRollCompletedEvent evt)
        {
            Debug.Log($"[UIController] Dice roll complete: {evt.Dice1Value} + {evt.Dice2Value} = {evt.TotalValue}");
            _tokenController.MoveActiveTokenBySteps(evt.TotalValue);
        }
    }
}
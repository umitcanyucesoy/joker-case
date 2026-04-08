using System.Collections.Generic;
using Core.Dice;
using Core.Pool;
using Core.Tokens;
using Event;
using Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.Core.UI
{
    public class UIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Dropdown diceCountDropdown;
        [SerializeField] private ScrollRect fieldScrollRect;
        [SerializeField] private Transform inputFieldContainer;
        [SerializeField] private TMP_InputField inputFieldPrefab;
        [SerializeField] private Button rollButton;

        [Header("Limits")]
        [SerializeField] private int maxDiceCount = 20;
        [SerializeField] private int inputFieldPrewarmCount = 4;

        private ITokenController _tokenController;
        private IDiceController _diceController;
        private IPoolService _poolService;
        private readonly List<TMP_InputField> _activeInputFields = new();

        private void OnEnable()
        {
            EventBus.Subscribe<DiceRollCompletedEvent>(OnDiceRollCompleted);
            EventBus.Subscribe<TokenSequenceCompletedEvent>(OnTokenSequenceCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<DiceRollCompletedEvent>(OnDiceRollCompleted);
            EventBus.Unsubscribe<TokenSequenceCompletedEvent>(OnTokenSequenceCompleted);
        }

        public void Init(ITokenController tokenController, IDiceController diceController)
        {
            _tokenController = tokenController;
            _diceController = diceController;
            _poolService = ServiceLocator.Get<IPoolService>();
            _poolService.Prewarm(inputFieldPrefab, inputFieldPrewarmCount);

            SetupDropdown();
        }

        private void SetupDropdown()
        {
            diceCountDropdown.ClearOptions();

            var options = new List<string>();
            for (var i = 1; i <= maxDiceCount; i++)
                options.Add(i.ToString());

            diceCountDropdown.AddOptions(options);
            diceCountDropdown.onValueChanged.AddListener(OnDiceCountChanged);

            diceCountDropdown.value = 0;
            RebuildInputFields(1);
        }

        private void OnDiceCountChanged(int index)
        {
            RebuildInputFields(index + 1);
        }


        private void RebuildInputFields(int count)
        {
            while (_activeInputFields.Count > count)
            {
                var last = _activeInputFields[^1];
                _activeInputFields.RemoveAt(_activeInputFields.Count - 1);
                _poolService.Return(inputFieldPrefab, last);
            }

            while (_activeInputFields.Count < count)
            {
                var field = _poolService.Get(inputFieldPrefab, inputFieldContainer);
                field.contentType = TMP_InputField.ContentType.IntegerNumber;
                field.characterLimit = 1;
                field.text = "";

                if (field.placeholder && field.placeholder.TryGetComponent<TMP_Text>(out var ph))
                    ph.text = "1-6";

                _activeInputFields.Add(field);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)inputFieldContainer);
            fieldScrollRect.verticalNormalizedPosition = 0f;
        }

        public void OnRollClicked()
        {
            var values = new int[_activeInputFields.Count];

            for (var i = 0; i < _activeInputFields.Count; i++)
            {
                if (!int.TryParse(_activeInputFields[i].text, out var val) || val < 1 || val > 6)
                {
                    Debug.LogWarning($"[UIController] Dice {i + 1} invalid (1-6)");
                    return;
                }
                values[i] = val;
            }

            SetInputInteractable(false);
            _diceController.RollDice(values);
        }

        private void OnDiceRollCompleted(DiceRollCompletedEvent evt)
        {
            _tokenController.MoveActiveTokenBySteps(evt.TotalValue);
        }

        private void OnTokenSequenceCompleted(TokenSequenceCompletedEvent evt)
        {
            SetInputInteractable(true);
        }

        private void SetInputInteractable(bool interactable)
        {
            rollButton.interactable = interactable;
            diceCountDropdown.interactable = interactable;

            foreach (var field in _activeInputFields)
                field.interactable = interactable;
        }
    }
}
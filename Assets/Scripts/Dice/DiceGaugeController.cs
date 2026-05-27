using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dice
{
    public class DiceGaugeController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static DiceGaugeController Instance;

        [Header("Charging Settings")]
        public float chargeSpeed = 100f; // Percent per second
        public float currentCharge = 0f;
        private bool isCharging = false;
        private int chargeDirection = 1; // 1 = up, -1 = down
        private bool suppressNextClick = false;

        [Header("UI References")]
        public Slider gaugeSlider;
        public TMPro.TextMeshProUGUI labelStatus;
        public TMPro.TextMeshProUGUI labelRange;
        public TMPro.TextMeshProUGUI labelResult;
        public Button rollButton;

        // Callback when a roll completes
        public Action<int> OnDiceRolled;
        public Action<DiceResult> OnDiceResultRolled;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            ResetGauge();
            if (rollButton != null)
            {
                // Click remains a fallback; pointer events provide hold/release for mouse and touch.
                rollButton.onClick.AddListener(OnRollButtonClicked);
                RegisterPointerEvents(rollButton.gameObject);
            }
        }

        private void Update()
        {
            // Only respond to spacebar if it's a human turn and we are waiting for input
            if (CanAcceptHumanInput())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCharging();
                }
                
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    ReleaseGauge();
                }
            }

            if (isCharging)
            {
                // Oscillate charge between 0 and 100
                currentCharge += chargeDirection * chargeSpeed * Time.deltaTime;
                if (currentCharge >= 100f)
                {
                    currentCharge = 100f;
                    chargeDirection = -1;
                }
                else if (currentCharge <= 0f)
                {
                    currentCharge = 0f;
                    chargeDirection = 1;
                }

                UpdateGaugeUI();
            }
        }

        public void StartCharging()
        {
            if (!CanRollNow()) return;
            if (isCharging) return;
            
            isCharging = true;
            currentCharge = 0f;
            chargeDirection = 1;
            
            if (labelResult != null) labelResult.text = "Mengocok...";

            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.diceRollClip);
            }
        }

        public void ReleaseGauge()
        {
            if (!CanRollNow()) return;
            if (!isCharging) return;
            isCharging = false;

            DiceResult result = CalculateDiceRoll(currentCharge);
            
            if (labelResult != null)
            {
                labelResult.text = result.value.ToString();
            }

            Debug.Log($"[Dice] Charged {currentCharge:F1}% ({result.levelLabel}) -> Rolled {result.value}");
            OnDiceResultRolled?.Invoke(result);
            OnDiceRolled?.Invoke(result.value);
        }

        private void OnRollButtonClicked()
        {
            if (suppressNextClick)
            {
                suppressNextClick = false;
                return;
            }

            // Simple click: if not charging, start charging. If charging, release it!
            if (!isCharging)
            {
                StartCharging();
            }
            else
            {
                ReleaseGauge();
            }
        }

        public void ResetGauge()
        {
            isCharging = false;
            currentCharge = 0f;
            UpdateGaugeUI();
            if (labelResult != null) labelResult.text = "-";
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            suppressNextClick = true;
            if (CanAcceptHumanInput())
            {
                StartCharging();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CanAcceptHumanInput())
            {
                ReleaseGauge();
            }
        }

        private void UpdateGaugeUI()
        {
            if (gaugeSlider != null)
            {
                gaugeSlider.value = currentCharge / 100f;
            }

            string label = "Status Percobaan";
            string range = "2 - 3";

            if (currentCharge <= 25f)
            {
                label = "Status Percobaan";
                range = "2 - 3";
            }
            else if (currentCharge <= 50f)
            {
                label = "Mahasiswa Reguler";
                range = "4 - 6";
            }
            else if (currentCharge <= 75f)
            {
                label = "Mahasiswa Teladan";
                range = "7 - 9";
            }
            else
            {
                label = "Duta Tata Tertib";
                range = "10 - 12";
            }

            // Adjust range displays if near finish
            if (Core.GameManager.Instance != null)
            {
                var curPlayer = Core.GameManager.Instance.GetCurrentPlayer();
                if (curPlayer != null)
                {
                    int needed = 100 - curPlayer.currentTile;
                    if (needed > 0 && needed < 12)
                    {
                        range += $" (Butuh {needed})";
                    }
                }
            }

            if (labelStatus != null) labelStatus.text = label;
            if (labelRange != null) labelRange.text = $"Dadu: {range} ({currentCharge:F0}%)";
        }

        private DiceResult CalculateDiceRoll(float charge)
        {
            List<int> possibilities = new List<int>();
            string levelLabel;
            string rangeLabel;
            
            // Map charge to specific ranges
            if (charge <= 25f)
            {
                possibilities.AddRange(new int[] { 2, 3 });
                levelLabel = "Status Percobaan";
                rangeLabel = "2 - 3";
            }
            else if (charge <= 50f)
            {
                possibilities.AddRange(new int[] { 4, 5, 6 });
                levelLabel = "Mahasiswa Reguler";
                rangeLabel = "4 - 6";
            }
            else if (charge <= 75f)
            {
                possibilities.AddRange(new int[] { 7, 8, 9 });
                levelLabel = "Mahasiswa Teladan";
                rangeLabel = "7 - 9";
            }
            else
            {
                possibilities.AddRange(new int[] { 10, 11, 12 });
                levelLabel = "Duta Tata Tertib";
                rangeLabel = "10 - 12";
            }

            // Handle near finish edge case override:
            // If the player is near finish, they must be able to roll the exact value.
            if (Core.GameManager.Instance != null)
            {
                var curPlayer = Core.GameManager.Instance.GetCurrentPlayer();
                if (curPlayer != null)
                {
                    int needed = 100 - curPlayer.currentTile;
                    if (needed > 0 && needed <= 12)
                    {
                        // Dynamic Override: If the exact number needed is 1, let the lowest range contain 1.
                        if (needed == 1 && charge <= 25f)
                        {
                            possibilities.Add(1);
                        }
                        // Otherwise, make sure the exact number needed is always added as a possibility to the rolled range
                        else if (!possibilities.Contains(needed))
                        {
                            possibilities.Add(needed);
                        }
                    }
                }
            }

            // Select random value from possibilities
            int index = UnityEngine.Random.Range(0, possibilities.Count);
            return new DiceResult(possibilities[index], charge, levelLabel, rangeLabel);
        }

        // Automated charging for bots or auto-rolls
        public void TriggerAutoRoll(float targetCharge)
        {
            if (!CanRollNow()) return;
            StartCharging();
            currentCharge = targetCharge;
            UpdateGaugeUI();
            Invoke(nameof(ReleaseGauge), 0.2f);
        }

        private bool CanAcceptHumanInput()
        {
            if (!CanRollNow()) return false;
            var currentPlayer = Core.GameManager.Instance.GetCurrentPlayer();
            return currentPlayer != null && !currentPlayer.isBot;
        }

        private bool CanRollNow()
        {
            return Core.GameManager.Instance != null &&
                   Core.GameManager.Instance.currentState == Core.GameState.WaitingForInput &&
                   Core.GameManager.Instance.GetCurrentPlayer() != null;
        }

        private void RegisterPointerEvents(GameObject target)
        {
            if (target == null) return;

            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = target.AddComponent<EventTrigger>();
            }

            AddTrigger(trigger, EventTriggerType.PointerDown, eventData => OnPointerDown((PointerEventData)eventData));
            AddTrigger(trigger, EventTriggerType.PointerUp, eventData => OnPointerUp((PointerEventData)eventData));
        }

        private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }
    }
}

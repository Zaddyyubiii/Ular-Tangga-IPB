using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Dice
{
    public class DiceGaugeController : MonoBehaviour
    {
        public static DiceGaugeController Instance;

        [Header("Charging Settings")]
        public float chargeSpeed = 80f;
        public float currentCharge = 0f;
        private bool isCharging = false;
        private int chargeDirection = 1;
        private bool holdRegistered = false; // true = button is held down
        private Coroutine tapAutoReleaseCoroutine;

        [Header("UI References")]
        public Slider gaugeSlider;
        public TMPro.TextMeshProUGUI labelStatus;
        public TMPro.TextMeshProUGUI labelRange;
        public TMPro.TextMeshProUGUI labelResult;
        public Button rollButton;

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
                RegisterPointerEventsOnButton(rollButton.gameObject);
            }
        }

        private void Update()
        {
            // Space bar hold support
            if (CanAcceptHumanInput())
            {
                if (Input.GetKeyDown(KeyCode.Space) && !isCharging)
                    BeginCharge(true);
                if (Input.GetKeyUp(KeyCode.Space) && isCharging && holdRegistered)
                    EndCharge();
            }

            if (isCharging)
            {
                currentCharge += chargeDirection * chargeSpeed * Time.deltaTime;
                if (currentCharge >= 100f) { currentCharge = 100f; chargeDirection = -1; }
                else if (currentCharge <= 0f) { currentCharge = 0f; chargeDirection = 1; }
                UpdateGaugeUI();
            }
        }

        // Called when pointer is pressed DOWN on the button
        private void OnButtonPointerDown(BaseEventData data)
        {
            if (!CanAcceptHumanInput()) return;
            holdRegistered = true;
            if (tapAutoReleaseCoroutine != null) StopCoroutine(tapAutoReleaseCoroutine);
            BeginCharge(true);
        }

        // Called when pointer is released UP from the button
        private void OnButtonPointerUp(BaseEventData data)
        {
            if (!CanAcceptHumanInput()) return;
            if (isCharging && holdRegistered)
            {
                holdRegistered = false;
                EndCharge();
            }
        }

        private void BeginCharge(bool fromHold)
        {
            if (!CanRollNow()) return;
            if (isCharging) return;
            holdRegistered = fromHold;
            isCharging = true;
            currentCharge = 0f;
            chargeDirection = 1;
            if (labelResult != null) labelResult.text = "...";
            UpdateGaugeUI();
            if (Audio.AudioManager.Instance != null && Audio.AudioManager.Instance.diceRollClip != null)
                Audio.AudioManager.Instance.PlaySFX(Audio.AudioManager.Instance.diceRollClip);
        }

        private void EndCharge()
        {
            if (!isCharging) return;
            isCharging = false;
            holdRegistered = false;
            DiceResult result = CalculateDiceRoll(currentCharge);
            if (labelResult != null) labelResult.text = result.value.ToString();
            Debug.Log($"[Dice] Released at {currentCharge:F1}% -> Roll: {result.value}");
            OnDiceResultRolled?.Invoke(result);
            OnDiceRolled?.Invoke(result.value);
        }

        public void ResetGauge()
        {
            isCharging = false;
            holdRegistered = false;
            currentCharge = 0f;
            chargeDirection = 1;
            UpdateGaugeUI();
            if (labelResult != null) labelResult.text = "-";
        }

        public void TriggerAutoRoll(float targetCharge)
        {
            if (!CanRollNow()) return;
            StartCoroutine(AutoRollCo(targetCharge));
        }

        private IEnumerator AutoRollCo(float targetCharge)
        {
            BeginCharge(false);
            yield return new WaitForSeconds(0.15f);
            currentCharge = Mathf.Clamp(targetCharge, 0f, 100f);
            UpdateGaugeUI();
            yield return new WaitForSeconds(0.2f);
            EndCharge();
        }

        /// <summary>Called by bots - bypasses human-only check</summary>
        public void ForceAutoRoll(float targetCharge)
        {
            if (Core.GameManager.Instance == null) return;
            if (Core.GameManager.Instance.currentState != Core.GameState.WaitingForInput) return;
            StartCoroutine(ForceAutoRollCo(targetCharge));
        }

        private IEnumerator ForceAutoRollCo(float targetCharge)
        {
            // Force start
            isCharging = true;
            holdRegistered = false;
            currentCharge = 0f;
            chargeDirection = 1;
            if (labelResult != null) labelResult.text = "...";
            UpdateGaugeUI();
            yield return new WaitForSeconds(0.15f);
            currentCharge = Mathf.Clamp(targetCharge, 0f, 100f);
            UpdateGaugeUI();
            yield return new WaitForSeconds(0.25f);
            EndCharge();
        }

        private void UpdateGaugeUI()
        {
            if (gaugeSlider != null)
                gaugeSlider.value = currentCharge / 100f;

            string label, range;
            if (currentCharge <= 25f)      { label = "Status Percobaan"; range = "2 - 3"; }
            else if (currentCharge <= 50f) { label = "Mahasiswa Reguler"; range = "4 - 6"; }
            else if (currentCharge <= 75f) { label = "Mahasiswa Teladan"; range = "7 - 9"; }
            else                            { label = "Duta Tata Tertib";  range = "10 - 12"; }

            if (Core.GameManager.Instance != null)
            {
                var p = Core.GameManager.Instance.GetCurrentPlayer();
                if (p != null)
                {
                    int needed = 100 - p.currentTile;
                    if (needed > 0 && needed < 12) range += $" (Butuh {needed})";
                }
            }

            if (labelStatus != null) labelStatus.text = label;
            if (labelRange != null) labelRange.text = $"Dadu: {range} ({currentCharge:F0}%)";
        }

        private DiceResult CalculateDiceRoll(float charge)
        {
            List<int> possibilities = new List<int>();
            string levelLabel, rangeLabel;

            if (charge <= 25f)      { possibilities.AddRange(new[] {2,3});       levelLabel="Status Percobaan"; rangeLabel="2-3"; }
            else if (charge <= 50f) { possibilities.AddRange(new[] {4,5,6});     levelLabel="Mahasiswa Reguler"; rangeLabel="4-6"; }
            else if (charge <= 75f) { possibilities.AddRange(new[] {7,8,9});     levelLabel="Mahasiswa Teladan"; rangeLabel="7-9"; }
            else                    { possibilities.AddRange(new[] {10,11,12});   levelLabel="Duta Tata Tertib"; rangeLabel="10-12"; }

            if (Core.GameManager.Instance != null)
            {
                var p = Core.GameManager.Instance.GetCurrentPlayer();
                if (p != null)
                {
                    int needed = 100 - p.currentTile;
                    if (needed > 0 && needed <= 12 && !possibilities.Contains(needed))
                        possibilities.Add(needed);
                }
            }

            int idx = UnityEngine.Random.Range(0, possibilities.Count);
            return new DiceResult(possibilities[idx], charge, levelLabel, rangeLabel);
        }

        private bool CanAcceptHumanInput()
        {
            if (!CanRollNow()) return false;
            var p = Core.GameManager.Instance.GetCurrentPlayer();
            return p != null && !p.isBot;
        }

        private bool CanRollNow()
        {
            return Core.GameManager.Instance != null &&
                   Core.GameManager.Instance.currentState == Core.GameState.WaitingForInput &&
                   Core.GameManager.Instance.GetCurrentPlayer() != null;
        }

        private void RegisterPointerEventsOnButton(GameObject target)
        {
            EventTrigger trigger = target.GetComponent<EventTrigger>();
            if (trigger == null) trigger = target.AddComponent<EventTrigger>();

            // Clear old entries
            trigger.triggers.Clear();

            var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            downEntry.callback.AddListener(OnButtonPointerDown);
            trigger.triggers.Add(downEntry);

            var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            upEntry.callback.AddListener(OnButtonPointerUp);
            trigger.triggers.Add(upEntry);
        }
    }
}

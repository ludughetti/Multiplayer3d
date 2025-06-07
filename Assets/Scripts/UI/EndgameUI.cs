using Endzone;
using TMPro;
using UnityEngine;

namespace UI
{
    public class EndgameUI : MonoBehaviour
    {
        [SerializeField] private GameObject endGameScreen;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private GameObject playerEntryPrefab;

        public static EndgameUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            endGameScreen.SetActive(false);
        }
        
        public void Show(FinishEntry[] results)
        {
            Debug.Log("EndgameUI.Show called");
            endGameScreen.SetActive(true);

            // Clear previous entries just in case
            foreach (Transform child in playerListContainer)
                Destroy(child.gameObject);

            // Add new entries
            foreach (var result in results)
            {
                var playerResultText = Instantiate(playerEntryPrefab, playerListContainer);
                var text = playerResultText.GetComponent<TMP_Text>();
                text.text = $"{result.PlayerName} - {FormatTime(result.FinishTime)}";
            }
        }

        private static string FormatTime(float time)
        {
            var seconds = Mathf.FloorToInt(time);
            var milliseconds = Mathf.FloorToInt((time - seconds) * 100);

            return $"{seconds:00}.{milliseconds:00}";
        }
    }
}

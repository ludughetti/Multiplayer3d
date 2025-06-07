using TMPro;
using UnityEngine;
using Utils;

namespace UI
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text lobbyText;

        private GameTimer _timer;
        
        private void Awake()
        {
            _timer = FindAnyObjectByType<GameTimer>();
        }
        
        private void Update()
        {
            if (!_timer) return;

            var time = _timer.GetTimer();

            var minutes = Mathf.FloorToInt(time / 60f);
            var seconds = Mathf.FloorToInt(time % 60f);

            timerText.text = $"{minutes:00}:{seconds:00}";
            
            // Show/hide lobby message
            if (lobbyText.IsActive() && _timer.IsLobbyEnded())
                lobbyText.gameObject.SetActive(false);
        }
    }
}

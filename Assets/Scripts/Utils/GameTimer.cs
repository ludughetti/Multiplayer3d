using Fusion;
using Network;
using UnityEngine;

namespace Utils
{
    public class GameTimer : NetworkBehaviour
    {
        [SerializeField] private float lobbyTime = 30f;
        [SerializeField] private float gameTime = 90f;
        [Networked] private float Timer { get; set; }
        [Networked] private bool IsLobbyPhase { get; set; }
        
        private bool _spawned;

        public override void Spawned()
        {
            Timer = lobbyTime; 
            IsLobbyPhase = true;
            _spawned = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (Timer > 0f)
            {
                Timer -= Runner.DeltaTime;
                if (Timer <= 0f)
                {
                    Timer = 0f;

                    if (IsLobbyPhase)
                    {
                        IsLobbyPhase = false;
                        Timer = gameTime;
                        NetworkManager.Instance.StartGame();
                    }
                    else
                    {
                        // Call end-game logic
                        NetworkManager.Instance.TriggerEndGame();
                    }
                }
            }
        }
        
        public float GetTimer()
        {
            return _spawned ? Timer : lobbyTime;
        }

        public bool IsLobbyEnded()
        {
            return _spawned ? !IsLobbyPhase : false;
        }
    }
}

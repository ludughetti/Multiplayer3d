using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Endzone
{
    public class EndzoneController : MonoBehaviour
    {
        private readonly HashSet<PlayerController> _playersTriggered = new();
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player) && player.HasInputAuthority)
            {
                if (_playersTriggered.Contains(player))
                    return; // already triggered for this player locally
            
                _playersTriggered.Add(player);
                player.RequestFinishRPC();
            }
        }
    }
}

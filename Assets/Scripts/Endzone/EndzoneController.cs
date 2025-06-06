using Fusion;
using Network;
using UnityEngine;

namespace Endzone
{
    public class EndzoneController : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.PlayerController player) && player.HasInputAuthority)
            {
                player.ReachEndzone();
            }
        }
    }
}

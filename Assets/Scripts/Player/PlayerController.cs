using Fusion;
using Network;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        
        private NetworkCharacterController _charControl;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManager.Instance.LocalPlayer = this;
            }
            
            _charControl = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasInputAuthority || !GetInput(out NetworkInputData input)) return;
            
            Debug.Log($"Player Input received: { input.Move }");
            
            var moveDirection = new Vector3(input.Move.x, 0f, input.Move.y);
            _charControl.Move(moveDirection * moveSpeed * Runner.DeltaTime);
        }
    }
}

using Fusion;
using Network;
using UnityEngine;
using Utils;

namespace Player
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.5f;
        
        private NetworkCharacterController _charControl;
        
        [Networked]
        public bool HasReachedEnd { get; private set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManager.Instance.LocalPlayer = this;
                
                // Lock cursor for local player
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Setup camera to follow local player
                var cameraFollow = FindAnyObjectByType<CameraFollow>();
                cameraFollow.AssignTarget(transform);
            }
            
            _charControl = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out NetworkInputData input)) return;
            
            // Move
            var inputDirection = new Vector3(input.Move.x, 0, input.Move.y);
            _charControl.Move(inputDirection * moveSpeed * Runner.DeltaTime);
            
            // Jump
            if (input.IsInputDown(NetworkButtonType.Jump))
                _charControl.Jump();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RequestFinishRPC()
        {
            if (HasReachedEnd) return; // double check server side to avoid dupes
        
            HasReachedEnd = true;     // mark on server that this player finished
            NetworkManager.Instance.PlayerReachedEnd(this);
        }
    }
}

using Fusion;
using Network;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(NetworkCharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Camera Settings")] 
        [SerializeField] private Camera playerCamera;
        
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.5f;
        [SerializeField] private float rotationSpeed = 80f;
        
        private NetworkCharacterController _charControl;
        private float _currentYaw;
        
        public bool HasReachedEnd { get; private set; }
        public float CurrentYaw => _currentYaw;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                NetworkManager.Instance.LocalPlayer = this;
                
                // Lock cursor for local player
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Enable camera only for local player
                if (playerCamera) 
                    playerCamera.gameObject.SetActive(true);
            }
            else
            {
                // Disable camera for network players
                if (playerCamera) 
                    playerCamera.gameObject.SetActive(false);
            }
            
            _charControl = GetComponent<NetworkCharacterController>();
            _currentYaw = transform.rotation.eulerAngles.y;
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasInputAuthority || !GetInput(out NetworkInputData input)) return;
            
            // Move
            var inputDirection = new Vector3(input.Move.x, 0, input.Move.y);
            var moveDirection = Quaternion.Euler(0, _currentYaw, 0) * inputDirection;
            _charControl.Move(moveDirection * moveSpeed * Runner.DeltaTime);
            
            // Apply yaw orbiting
            _currentYaw += input.YawDelta * rotationSpeed * Runner.DeltaTime;
            transform.rotation = Quaternion.Euler(0, _currentYaw, 0);
            
            // Jump
            if (input.IsInputDown(NetworkButtonType.Jump))
                _charControl.Jump();
        }

        public void ReachEndzone()
        {
            if (HasReachedEnd) return;

            HasReachedEnd = true;
            NetworkManager.Instance.PlayerReachedEnd(this);
        }
    }
}

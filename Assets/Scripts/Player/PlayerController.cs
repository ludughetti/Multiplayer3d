using Fusion;
using Network;
using UnityEngine;
using Utils;

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
        
        [Networked] private float NetworkYaw { get; set; }
        
        private NetworkCharacterController _charControl;
        private CameraFollow _cameraFollow;
        
        public bool HasReachedEnd { get; private set; }

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
                {
                    _cameraFollow = playerCamera.GetComponent<CameraFollow>();
                    playerCamera.gameObject.SetActive(true);
                }

            }
            else
            {
                // Disable camera for network players
                if (playerCamera) 
                    playerCamera.gameObject.SetActive(false);
            }
            
            _charControl = GetComponent<NetworkCharacterController>();
            NetworkYaw = transform.rotation.eulerAngles.y;
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out NetworkInputData input)) return;
            
            // Move
            var inputDirection = new Vector3(input.Move.x, 0, input.Move.y);
            var moveDirection = Quaternion.Euler(0, NetworkYaw, 0) * inputDirection;
            _charControl.Move(moveDirection * moveSpeed * Runner.DeltaTime);
            
            // Apply yaw orbiting to networked property
            if (Object.HasInputAuthority)
            {
                NetworkYaw += input.YawDelta * rotationSpeed * Runner.DeltaTime;
            }
            
            // Apply rotation
            //transform.rotation = Quaternion.Euler(0, NetworkYaw, 0);
            _charControl.rotationSpeed = inputDirection.magnitude * Runner.DeltaTime;
            
            // Jump
            if (input.IsInputDown(NetworkButtonType.Jump))
                _charControl.Jump();
            
            // Update camera
            if (_cameraFollow)
                _cameraFollow.FollowPlayer(transform.position, NetworkYaw, Runner.DeltaTime);
        }

        public void ReachEndzone()
        {
            if (HasReachedEnd) return;

            HasReachedEnd = true;
            NetworkManager.Instance.PlayerReachedEnd(this);
        }
    }
}

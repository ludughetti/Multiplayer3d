using Network;
using Player;
using UnityEngine;

namespace Utils
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new (0, 5, -10);
        [SerializeField] private float followSpeed = 5f;

        private Transform _target;
        private PlayerController _playerController;

        private void LateUpdate()
        {
            if (!_target && NetworkManager.Instance.LocalPlayer)
            {
                _target = NetworkManager.Instance.LocalPlayer.transform;
                _playerController = NetworkManager.Instance.LocalPlayer;
            }

            if (!_target) return;
            
            // Rotate offset vector by player's yaw rotation
            var rotatedOffset = Quaternion.Euler(0, _playerController.CurrentYaw, 0) * offset;
            var targetPosition = _target.position + rotatedOffset;

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}

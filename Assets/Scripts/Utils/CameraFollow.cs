using Network;
using Player;
using UnityEngine;

namespace Utils
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new (0, 4, -12);
        [SerializeField] private float followSpeed = 5f;
        
        public void FollowPlayer(Vector3 targetPosition, float yaw, float deltaTime)
        {
            var rotatedOffset = Quaternion.Euler(0, yaw, 0) * offset;
            var desiredPosition = targetPosition + rotatedOffset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * deltaTime);
        }
    }
}

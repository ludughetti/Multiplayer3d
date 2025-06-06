using UnityEngine;

namespace Utils
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smoothSpeed = 5f;
        
        private Transform _target;
        
        private void LateUpdate()
        {
            if (!_target) return;

            var desiredPosition = _target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(_target);
        }
        
        public void AssignTarget(Transform target)
        {
            _target = target;
        }
    }
}

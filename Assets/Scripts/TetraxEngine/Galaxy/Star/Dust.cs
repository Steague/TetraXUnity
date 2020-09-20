using UnityEngine;

namespace TetraxEngine.Galaxy.Star
{
    public class Dust : MonoBehaviour
    {
        // public Transform target;
        public float xOffset = 0f;
        public float yOffset = 0f;
        public float zOffset = 0f;

        private Transform _target;
        private Transform _xFormDust;
        private Vector3 _originalRotation;

        private void Start()
        {
            var thisTransform = transform;
            _xFormDust = thisTransform;
            _originalRotation = _xFormDust.eulerAngles;
        }

        public void SetTarget(Transform t)
        {
            // Debug.Log("wat");
            _target = t;
        }

        public void SetPosition(Vector3 v)
        {
            _xFormDust.position = v;
        }

        private void LateUpdate()
        {
            transform.LookAt(_target.transform.position);
            var v = _xFormDust.eulerAngles;
            v.x += xOffset + _originalRotation.x;
            v.y += yOffset + _originalRotation.y;
            v.z += zOffset + _originalRotation.z;
            _xFormDust.eulerAngles = v;
        }
    }
}

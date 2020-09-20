using UnityEngine;

namespace TetraxEngine.Galaxy
{
    public class CameraOrbit : MonoBehaviour 
    {
        private Transform _xFormCamera;
        private Transform _xFormParent;

        private Vector3 _localRotation;
        private float _cameraDistance = 900f;

        public float mouseSensitivity = 4f;
        public float scrollSensitivity = 2f;
        public float orbitDampening = 10f;
        public float scrollDampening = 6f;
        public bool cameraDisabled;


        // Use this for initialization
        private void Start()
        {
            var thisTransform = transform;
            _xFormCamera = thisTransform;
            _xFormParent = thisTransform.parent;
        }

        private void LateUpdate() {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                cameraDisabled = !cameraDisabled;
            }

            if (!cameraDisabled)
            {
                //Rotation of the Camera based on Mouse Coordinates
                var mouseX = Input.GetAxis("Mouse X");
                var mouseY = Input.GetAxis("Mouse Y");
                if (mouseX != 0 || mouseY != 0)
                {
                    _localRotation.x += mouseX * mouseSensitivity;
                    _localRotation.y -= mouseY * mouseSensitivity;

                    //Clamp the y Rotation to horizon and not flipping over at the top
                    if (_localRotation.y < -90f)
                    {
                        _localRotation.y = -90f;
                    }
                    else if (_localRotation.y > 90f)
                    {
                        _localRotation.y = 90f;
                    }
                }
            
                //Zooming Input from our Mouse Scroll Wheel
                var mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
                if (mouseScrollWheel != 0f)
                {
                    var scrollAmount = mouseScrollWheel * scrollSensitivity;

                    scrollAmount *= (_cameraDistance * 0.3f);

                    _cameraDistance += scrollAmount * -1f;
                    _cameraDistance = Mathf.Clamp(_cameraDistance, 300f, 900f);
                }
            }

            //Actual Camera Rig Transformations
            var qt = Quaternion.Euler(_localRotation.y, _localRotation.x, 0);
            _xFormParent.rotation = Quaternion.Lerp(_xFormParent.rotation, qt, Time.deltaTime * orbitDampening);

            if (_xFormCamera.localPosition.z != _cameraDistance * -1f)
            {
                _xFormCamera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(_xFormCamera.localPosition.z, _cameraDistance * -1f, Time.deltaTime * scrollDampening));
            }
        }
    }
}
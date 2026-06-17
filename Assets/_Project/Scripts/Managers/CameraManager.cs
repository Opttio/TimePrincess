using UnityEngine;

namespace _Project.Scripts.Managers
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;

        public void MoveTo(Transform targetPoint)
        {
            _cameraTransform.position = targetPoint.position;
            _cameraTransform.rotation = targetPoint.rotation;
        }
    }
}
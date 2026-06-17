using UnityEngine;

namespace _Project.Scripts.UI.Enemy
{
    public class EnemyBarFaceToCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.forward = _camera.transform.forward;
        }
    }
}
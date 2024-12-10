using UnityEngine;

namespace Blackopter.UI
{
    public class LoadingCircle : MonoBehaviour
    {
        private RectTransform _rectComponent;
    
        [Range(200f, 1000f)] public float rotateSpeed = 200f;
        [SerializeField] private bool isClockWise;

        private float Factor => isClockWise ? -1 * rotateSpeed: 1 * rotateSpeed;
    
        private void Awake() => _rectComponent = GetComponent<RectTransform>();
        private void FixedUpdate() => _rectComponent.Rotate(0f, 0f, Factor * Time.deltaTime);
    }
}

using UnityEngine;

public class FloatAnimator : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startPos;

    private void OnEnable()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0f, yOffset, 0f);
    }
}
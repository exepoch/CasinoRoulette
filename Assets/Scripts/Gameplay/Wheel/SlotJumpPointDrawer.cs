using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws multiple concentric rings of jump points for roulette slots in the Scene view.
/// </summary>
[ExecuteAlways]
public class SlotJumpPointDrawer : MonoBehaviour
{
    [Tooltip("Toggle gizmo drawing.")]
    public bool enable;

    [Tooltip("Rotating wheel transform.")]
    public Transform wheelTransform;

    [Tooltip("Center of the wheel in local space.")]
    public Vector3 wheelCenter = Vector3.zero;

    [Tooltip("Total number of slots.")]
    public int slotCount = 37;

    [Tooltip("Number of concentric rings.")]
    public int ringCount = 3;

    [Tooltip("Radius of the innermost ring.")]
    public float minRadius = 0.7f;

    [Tooltip("Radius of the outermost ring.")]
    public float maxRadius = 1.3f;

    [Tooltip("Angle gap between slot dividers.")]
    [Range(0f, 15f)] 
    public float dividerAngle = 1f;

    private void OnDrawGizmos()
    {
        if (!enable) return;
        if (wheelTransform == null || slotCount <= 0 || ringCount <= 0) return;

        float radiusStep = (maxRadius - minRadius) / (ringCount - 1);
        float slotAngle = (360f - slotCount * dividerAngle) / slotCount;

        for (int ring = 0; ring < ringCount; ring++)
        {
            float radius = minRadius + radiusStep * ring;

            for (int i = 0; i < slotCount; i++)
            {
                float localAngleDeg = i * (slotAngle + dividerAngle) + slotAngle / 2f;
                float worldAngleDeg = localAngleDeg - wheelTransform.eulerAngles.y;
                float angleRad = worldAngleDeg * Mathf.Deg2Rad;

                float x = wheelCenter.x + wheelTransform.position.x + radius * Mathf.Cos(angleRad);
                float z = wheelCenter.z + wheelTransform.position.z + radius * Mathf.Sin(angleRad);
                Vector3 pos = new Vector3(x, wheelCenter.y, z);

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(pos, 0.035f);

#if UNITY_EDITOR
                Handles.color = Color.red;
                Handles.Label(pos + Vector3.up * 0.02f, $"R{ring + 1}-{i}");
#endif
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wheelCenter, 0.02f);
    }
}

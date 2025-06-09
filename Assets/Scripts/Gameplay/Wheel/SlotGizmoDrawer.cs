using UnityEngine;

/// <summary>
/// Draws gizmos for roulette slots in the Scene view.
/// </summary>
[ExecuteAlways]
public class SlotGizmoDrawer : MonoBehaviour
{
    [Tooltip("Toggle gizmo drawing.")]
    public bool enable;

    [Tooltip("Toggle european or american.")]
    public bool isEuropean = true;
    
    [Tooltip("Rotating wheel transform.")]
    public Transform wheelTransform;

    [Tooltip("Center of the wheel in local space.")]
    public Vector3 wheelCenter = Vector3.zero;

    [Tooltip("Radius from the center to draw the slot markers.")]
    public float radius = 0.9f;

    [Tooltip("Angle between slot dividers.")]
    [Range(0f, 15f)]
    public float dividerAngle = 1.0f;

    // European roulette number order
    private readonly int[] _europeanOrder = {
        0, 26, 3, 35, 12, 28, 7, 29, 18, 22, 9, 31, 14, 20, 1, 33,
        16, 24, 5, 10, 23, 8, 30, 11, 36, 13, 27, 6, 34, 17, 25, 2,
        21, 4, 19, 15, 32
    };
    
    // American roulette number order
    private readonly int[] _americanOrder = {
        0, 26, 3, 35, 12, 28, 7, 29, 18, 22, 9, 31, 14, 20, 1, 33,
        16, 24, 5, 10, 23, 8, 30, 11, 36, 13, 27, 6, 34, 17, 25, 2,
        21, 4, 19, 15, 32,37
    };

    private void OnDrawGizmos()
    {
        if (!enable || wheelTransform == null) return;

        var order = isEuropean ? _europeanOrder : _americanOrder;

        float slotAngle = (360f - order.Length * dividerAngle) / order.Length;

        for (int i = 0; i < order.Length; i++)
        {
            // Calculate angle of current slot
            float localAngleDeg = i * (slotAngle + dividerAngle) + slotAngle / 2f;
            float worldAngleDeg = localAngleDeg - wheelTransform.eulerAngles.y;
            float angleRad = worldAngleDeg * Mathf.Deg2Rad;

            // Calculate position on wheel
            float x = wheelCenter.x + wheelTransform.position.x + radius * Mathf.Cos(angleRad);
            float z = wheelCenter.z + wheelTransform.position.z + radius * Mathf.Sin(angleRad);
            Vector3 pos = new Vector3(x, wheelCenter.y, z);

            // Draw point
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pos, 0.035f);

#if UNITY_EDITOR
            // Draw label
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.Label(pos + Vector3.up * 0.02f, order[i].ToString());
#endif
        }

        // Draw center point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wheelCenter, 0.02f);
    }
}

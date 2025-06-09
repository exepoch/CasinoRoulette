using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Wheel.Helper
{
    public struct JumpPointInitializeData
    {
        public Transform wheelTransform;
        public Vector3 wheelCenter;
        public int slotCount;
        public int ringCount;
        public float minRadius;
        public float maxRadius;
    }
    public static class JumpPointGenerationHelper
    {
        public static List<Vector3> Generate(JumpPointInitializeData initData)
        {
            var generatedList = new List<Vector3>();
            if (initData.wheelTransform == null || initData.slotCount <= 0 || initData.ringCount <= 0) return generatedList;

            // Halkalar arası radius farkı
            float radiusStep = (initData.maxRadius - initData.minRadius) / (initData.ringCount - 1);

            // Her halka için radius hesapla
            for (int ring = 0; ring < initData.ringCount; ring++)
            {
                float radius = initData.minRadius + radiusStep * ring;

                // Bir halkadaki slot açısı hesapla
                float slotAngle = (360f - initData.slotCount) / initData.slotCount;

                for (int i = 0; i < initData.slotCount; i++)
                {
                    // Slotun merkez açısı (derece cinsinden)
                    float localAngleDeg = i * slotAngle + slotAngle / 2f;
                    // Dünya açısını wheelRotation'a göre ayarla
                    float worldAngleDeg = localAngleDeg - initData.wheelTransform.eulerAngles.y;
                    float angleRad = worldAngleDeg * Mathf.Deg2Rad;

                    // Pozisyonu hesapla
                    float x = initData.wheelCenter.x + initData.wheelTransform.position.x + radius * Mathf.Cos(angleRad);
                    float z = initData.wheelCenter.z + initData.wheelTransform.position.z + radius * Mathf.Sin(angleRad);
                    generatedList.Add(new Vector3(x, initData.wheelCenter.y, z));
                }
            }

            return generatedList;
        }
    }
}

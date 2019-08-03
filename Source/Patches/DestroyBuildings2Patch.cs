using System;
using Harmony;
using UnityEngine;

namespace EnhancedDisastersMod.Patches
{
    [HarmonyPatch(typeof(DisasterHelpers))]
    [HarmonyPatch("DestroyBuildings")]
    [HarmonyPatch(new Type[] { typeof(int), typeof(InstanceManager.Group), typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) })]
    class DestroyBuildings2Patch
    {
        static bool Prefix(int seed, InstanceManager.Group group, Vector3 position, float preRadius, float removeRadius,
            float destructionRadiusMin, float destructionRadiusMax, float burnRadiusMin, float burnRadiusMax, float probability)
        {
            DisasterHelpersModified.DestroyBuildings(seed, group, position, preRadius, removeRadius, destructionRadiusMin,
                destructionRadiusMax, burnRadiusMin, burnRadiusMax, probability);

            return false;
        }
    }
}

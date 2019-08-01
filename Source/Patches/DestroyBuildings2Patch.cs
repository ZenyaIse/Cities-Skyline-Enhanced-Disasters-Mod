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
        static bool Prefix()
        {
            return false;
        }
    }
}

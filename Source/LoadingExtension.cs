using ICities;
using UnityEngine;

namespace EnhancedDisastersMod
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            Debug.Log("EnhancedDisastersMod Loaded: 2017/06/13");

            ModOptions.Init();

            //int prefabCount = PrefabCollection<DisasterInfo>.PrefabCount();

            //for (int i = 0; i < prefabCount; i++)
            //{
            //    DisasterInfo disasterInfo = PrefabCollection<DisasterInfo>.GetPrefab((uint)i);
            //    Debug.Log("name: " + disasterInfo.name + ", m_randomProbability: " + disasterInfo.m_randomProbability);
            //}
        }
    }
}

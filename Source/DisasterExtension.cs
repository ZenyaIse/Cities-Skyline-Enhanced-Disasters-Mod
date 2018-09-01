using UnityEngine;
using ColossalFramework;
using ICities;

namespace EnhancedDisastersMod
{
    public class DisasterExtension : IDisasterBase
    {
        public override void OnDisasterCreated(ushort disasterID)
        {
            DisasterData disasterData = Singleton<DisasterManager>.instance.m_disasters.m_buffer[disasterID];
            Debug.Log(">>> EnhancedDisastersMod (OnDisasterCreated): " + disasterData.Info.GetAI().name + "(" + disasterData.m_intensity.ToString() + "), "
                + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString());
            Singleton<EnhancedDisastersManager>.instance.OnDisasterCreated(disasterData.Info.m_disasterAI, disasterData.m_intensity);
        }

        public override void OnDisasterStarted(ushort disasterID)
        {
            DisasterData disasterData = Singleton<DisasterManager>.instance.m_disasters.m_buffer[disasterID];
            Debug.Log(">>> EnhancedDisastersMod (OnDisasterStarted): " + disasterData.Info.GetAI().name + "(" + disasterData.m_intensity.ToString() + "), "
                + Singleton<SimulationManager>.instance.m_currentGameTime.ToShortDateString());
            Singleton<EnhancedDisastersManager>.instance.OnDisasterStarted(disasterData.Info.m_disasterAI, disasterData.m_intensity);
        }
    }
}

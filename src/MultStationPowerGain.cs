using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;




namespace FixEconomy
{
    [HarmonyPatch(typeof(StationSystem), nameof(StationSystem.GetStationPowerGain))]
    public static class MultStationPowerGain
    {
        static int Faction_Prod_PowerGain_Mult_Perc = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Faction_Prod_PowerGain_Mult_Perc", 300);

        static float Faction_Prod_PowerGain_Mult = (float)Faction_Prod_PowerGain_Mult_Perc / 100f;

        public static void Postfix(Station station, Difficulty difficulty, ref int __result)
        {
            __result = (int)(__result * Faction_Prod_PowerGain_Mult);
        }

    }
}

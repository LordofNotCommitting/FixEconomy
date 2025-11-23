using MGSC;
using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FixEconomy
{
    // Token: 0x02000006 RID: 6
    public class ModConfigGeneral
    {
        // Token: 0x0600001D RID: 29 RVA: 0x00002840 File Offset: 0x00000A40



        public ModConfigGeneral(string ModName, string ConfigPath)
        {
            this.ModName = ModName;
            this.ModData = new ModConfigData(ConfigPath);
            this.ModData.AddConfigHeader("STRING:General Settings", "general");
            this.ModData.AddConfigValue("general", "about_final", "STRING:<color=#f51b1b>The game must be restarted after setting then saving this config to take effect.</color>\n");

            this.ModData.AddConfigValue("general", "Price_Ratio_Min", 50, 10, 100, "STRING:Price Ratio Minimum %", "STRING:Minimum possible % value used for consumption multiplier. This number controls rubberbending power when item price is above average. Lower number means stronger rubberbending.");
            this.ModData.AddConfigValue("general", "Price_Ratio_Max", 1000, 101, 2000, "STRING:Price Ratio Maximum %", "STRING:Maximum possible % value used for consumption multiplier. This number controls rubberbending power when item price is below average. Higher number means stronger rubberbending.");
            this.ModData.AddConfigValue("general", "Price_Ratio_Abs_Adj", 100, 10, 1000, "STRING:Price Adjustment Multiplier %", "STRING:This is % multiplier on arbitrary average price the rubberbending is aiming for. Higher number here makes all market item price higher. ");

            
            this.ModData.AddConfigValue("general", "about_1", "STRING: By comparing [Current Item Price] vs (Starting item price x <color=#f51b1b>[Price Adjustment Multiplier %]</color>). Depending on whether price is below or above starting item price, consumption multiplier of <color=#f51b1b>[Price Ratio Minimum %]</color> or <color=#f51b1b>[Price Ratio Maximum %]</color> will be applied. \n");


            //this.ModData.AddConfigValue("general", "Debug_Log_On", false, "STRING:Debug Log", "STRING:For personal debugging. DO NOT TURN IT ON if you don't intend to.");
            
            this.ModData.RegisterModConfigData(ModName);
        }

        // Token: 0x04000011 RID: 17
        private string ModName;

        // Token: 0x04000012 RID: 18
        public ModConfigData ModData;

    }
}

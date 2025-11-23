
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
    [HarmonyPatch(typeof(StationSystem), nameof(StationSystem.RefreshConsumablesPrices))]
    public static class FixStationPrice
    {


        static int price_ratio_min = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Min", 30);
        static int price_ratio_max = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Max", 1000);
        static int price_ratio_abs_adjustment = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Abs_Adj", 100);

        static bool debug_log_on = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Debug_Log_On", false);

        public static bool Prefix(Station station, ItemsPrices itemsPrices)
        {


            float temp_ratio_min_factor = (float)price_ratio_min / 100f;
            float temp_ratio_max_factor = (float)price_ratio_max / 100f;
            float temp_abs_adjustment_factor = (float)price_ratio_abs_adjustment / 100f;

            bool flag = false;

            Dictionary<string, float> Prices = new Dictionary<string, float>();


            StationSystem._stringCache.Clear();
            StationSystem._stringCache.AddRange(station.ConsumableItems.Keys);
            foreach (string text in StationSystem._stringCache)
            {
                if (!(text == station.AdditionalPurchaseItemId))
                {

                    float price_ratio = 1f;
                    float abs_price = ((ItemRecord)(Data.Items.GetRecord(text, true) as CompositeItemRecord).PrimaryRecord).Price;

                    float curr_price = itemsPrices.GetPrice(text);

                    // minimum price gurantee
                    if (curr_price < 10)
                    {
                        curr_price = 10f;
                    }
                    float num3;
                    station.ConsumableItems.TryGetValue(text, out num3);
                    if (num3 <= 0f)
                    {
                        station.ConsumableItems[text] = 1f;
                    }
                    else {
                        price_ratio = Mathf.Clamp(abs_price / curr_price * temp_abs_adjustment_factor, temp_ratio_min_factor, temp_ratio_max_factor);
                        int num = station.InternalStorage.CountItems(text);
                        

                        //Plugin.Logger.Log($"Station {station.Id}. Production log for item {text}. Absolute price of {abs_price} and current price of {curr_price}.");
                        //Plugin.Logger.Log($"Station {station.Id}. This puts ratio at {price_ratio}.");
                        float num2 = station.ItemConsumeRatePerMonth[text];
                        num2 = num2 * price_ratio;
                        //Plugin.Logger.Log($"Station {station.Id}. curr stock of {num} and consuming stock of {num2}");
                        float price_multiplier = Mathf.Clamp(num2 / num, 0.5f, 2f);
                        station.ConsumableItems[text] = (float)Mathf.CeilToInt(curr_price * price_multiplier);

                    }



                }
            }

            return false;
        }


    }
}


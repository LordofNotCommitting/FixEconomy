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
    [HarmonyPatch(typeof(StationSystem), nameof(StationSystem.ProduceItems))]
    public static class FixProduceItems
    {


        static int price_ratio_min = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Min", 50);
        static int price_ratio_max = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Max", 1000);
        static int price_ratio_abs_adjustment = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Abs_Adj", 100);

        static bool debug_log_on = Plugin.ConfigGeneral.ModData.GetConfigValue<bool>("Debug_Log_On", false);

        public static bool Prefix(PopulationDebugData populationDebugData, Factions factions, ItemsPrices itemsPrices, MagnumProgression magnumProgression, SpaceTime spaceTime, Station station, float produceTimeMult)
        {


            float temp_ratio_min_factor = (float)price_ratio_min / 100f;
            float temp_ratio_max_factor = (float)price_ratio_max / 100f;
            float temp_abs_adjustment_factor = (float)price_ratio_abs_adjustment / 100f;

            bool flag = false;

            Dictionary<string, float> Prices = new Dictionary<string, float>();

            ProxyCorpDepartment department = magnumProgression.GetDepartment<ProxyCorpDepartment>();
            foreach (string text in station.CurrentReceipts)
            {
                if (!StationSystem.IsProducingItem(station, text))
                {
                    BarterReceipt record = Data.BarterReceipts.GetRecord(text, true);
                    float num = produceTimeMult;
                    if (ItemProductionSystem.GetAvailableToProduceCount(station.InternalStorage, record.InputItems) == 0 || station.Population < record.InputPopulation)
                    {
                        num *= Data.Global.StationProductionTimeRatio;
                    }

                    //max possible speed as default
                    float num3000 = num / temp_ratio_min_factor;


                    if (department.ProxyFactionId == station.OwnerFactionId)
                    {
                        num /= magnumProgression.ProxyCompanyProduceSpeed;
                    }
                    if (record.InputPopulation != 0)
                    {
                        StationSystem.AddPopulation(populationDebugData, factions, spaceTime, itemsPrices, magnumProgression, station, record.InputPopulation, PopulationChangeReason.BarterReceiptInputPop, false);
                        flag = true;
                    }



                    foreach (ItemQuantity itemQuantity in record.InputItems)
                    {

                        float price_ratio = 1f;
                        string itemId = itemQuantity.ItemId;
                        // ok. first is price comparison.
                        //this contains original price of the item... I think.
                        float abs_price = ((ItemRecord)(Data.Items.GetRecord(itemId, true) as CompositeItemRecord).PrimaryRecord).Price;

                        bool pricechart_exist = Prices.TryGetValue(itemId, out var curr_price);

                        if (pricechart_exist) {
                            price_ratio = Mathf.Clamp(abs_price / curr_price * temp_abs_adjustment_factor, temp_ratio_min_factor, temp_ratio_max_factor);
                        }
                        else
                        {
                            curr_price = abs_price;
                        }
                        // now we re-adjust consumption rate as per whatever.

                        //don't consume less than what we already got, only consume more if ratio allows.
                        int temp_consumption_count = itemQuantity.Count;
                        //if abs_price / curr_price. So when item price is cheaper, consumption increases.
                        // should not decrease consumption if item is expensive. just slow down production cycle.
                        if (price_ratio > 1)
                        {
                            temp_consumption_count = (int)Mathf.Floor((float)itemQuantity.Count * price_ratio);
                        }
                        //adjust production speed accordingly
                        //get lowest speed possible.
                        num3000 = Mathf.Min(num / price_ratio, num3000);
                        if (debug_log_on) {
                            Plugin.Logger.Log($"Station {station.Id}. Production log for item {itemId}. Absolute price of {abs_price} and current price of {curr_price}.");
                            Plugin.Logger.Log($"Station {station.Id}. This puts ratio at {price_ratio}.");
                            Plugin.Logger.Log($"Station {station.Id}. So consumption speed is at {num3000}, Consumption amount is at {temp_consumption_count}. Original amount is {itemQuantity.Count}");
                        }
                        StationSystem.RemoveItemFromStationStorage(station, itemQuantity.ItemId, (short)temp_consumption_count);
                        if (!itemsPrices.ExpenseItemsCount.TryAdd(itemQuantity.ItemId, temp_consumption_count))
                        {
                            Dictionary<string, int> expenseItemsCount = itemsPrices.ExpenseItemsCount;
                            expenseItemsCount[itemId] += temp_consumption_count;
                        }
                    }
                    ItemProductionSystem.StartStationItemProduction(spaceTime, station, record, num3000);
                }
            }
            if (flag)
            {
                StationSystem.RefreshPopulationConsumables(factions, magnumProgression, station, itemsPrices, (float)station.Population);
                return false;
            }
            StationSystem.RefreshConsumablesPrices(station, itemsPrices);



            return false;
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }
            return false;
        }

    }
}

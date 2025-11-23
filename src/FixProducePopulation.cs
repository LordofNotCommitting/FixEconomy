using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




namespace FixEconomy
{
    [HarmonyPatch(typeof(StationSystem), nameof(StationSystem.ProducePopulation))]
    public static class FixProducePopulation
    {


        static int price_ratio_min = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Min", 50);
        static int price_ratio_max = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Max", 1000);
        static int price_ratio_abs_adjustment = Plugin.ConfigGeneral.ModData.GetConfigValue<int>("Price_Ratio_Abs_Adj", 100);

        static Dictionary<string, float> Prices = new Dictionary<string, float>();

        public static bool Prefix(PopulationDebugData populationDebugData, Factions factions, ItemsPrices itemsPrices, MagnumProgression magnumProgression, SpaceTime spaceTime, Station station)
        {


            float temp_ratio_min_factor = (float)price_ratio_min / 100f;
            float temp_ratio_max_factor = (float)price_ratio_max / 100f;
            float temp_abs_adjustment_factor = (float)price_ratio_abs_adjustment / 100f;

            if (station.PopulationProduceOrders.Count > 0)
            {
                return false;
            }
            Faction faction = factions.Get(station.OwnerFactionId, true);
            ProxyCorpDepartment department = magnumProgression.GetDepartment<ProxyCorpDepartment>();
            StationSystem._populationReceiptsCache.Clear();
            foreach (PopulationReceipt populationReceipt in Data.PopulationReceipts)
            {
                if (populationReceipt.FactionType == faction.FactionType)
                {
                    StationSystem._populationReceiptsCache.Insert(0, populationReceipt);
                }
            }
            float num = (float)station.Population / (float)station.Record.MaxPopulation;
            PopulationReceipt populationReceipt2 = null;
            bool flag = false;


            // this is.. population maintainance logic? we will not touch this since it can cause massive pop dropoff.
            foreach (PopulationReceipt populationReceipt3 in StationSystem._populationReceiptsCache)
            {
                populationReceipt2 = populationReceipt3;
                if (num >= populationReceipt3.MinPopulationPercent)
                {
                    BarterReceipt record = Data.BarterReceipts.GetRecord(populationReceipt3.BartherReceiptId, true);
                    if (ItemProductionSystem.GetAvailableToProduceCount(station.InternalStorage, record.InputItems) != 0)
                    {
                        flag = true;
                        break;
                    }
                    StationSystem.AddPopulation(populationDebugData, factions, spaceTime, itemsPrices, magnumProgression, station, -Mathf.RoundToInt(populationReceipt3.FailPopStepLoss * (float)station.Record.MaxPopulation), PopulationChangeReason.FailPopStepLoss, false);
                }
            }
            float num2 = 1f;
            if (!flag)
            {
                num2 = Data.Global.StationProductionTimeRatio;
            }

            //max possible speed as default
            float num3000 = num2 / temp_ratio_min_factor;
            if (department.ProxyFactionId == station.OwnerFactionId)
            {
                num2 /= magnumProgression.ProxyCompanyProduceSpeed;
            }

            foreach (ItemQuantity itemQuantity in Data.BarterReceipts.GetRecord(populationReceipt2.BartherReceiptId, true).InputItems)
            {

                float price_ratio = 1f;
                string itemId = itemQuantity.ItemId;
                // ok. first is price comparison.
                //this contains original price of the item... I think.

                float abs_price = ((ItemRecord)(Data.Items.GetRecord(itemId, true) as CompositeItemRecord).PrimaryRecord).Price;

                bool pricechart_exist = Prices.TryGetValue(itemId, out var curr_price);

                if (pricechart_exist)
                {
                    price_ratio = Mathf.Clamp(abs_price / curr_price * temp_abs_adjustment_factor, temp_ratio_min_factor, temp_ratio_max_factor);
                }
                else
                {
                    curr_price = abs_price;
                }
                // now we re-adjust consumption rate as per whatever.

                //don't consume less than what we already got, only consume more if ratio allows.
                int temp_consumption_count = itemQuantity.Count;
                //if abs_price / curr_price. So when item price is cheaper
                if (price_ratio > 1) {
                    temp_consumption_count = (int)Mathf.Floor((float)itemQuantity.Count * price_ratio);
                }
                //adjust production speed accordingly
                //get lowest speed possible.
                num3000 = Mathf.Min(num2 / price_ratio, num3000);

                StationSystem.RemoveItemFromStationStorage(station, itemQuantity.ItemId, (short)temp_consumption_count);
                if (!itemsPrices.ExpenseItemsCount.TryAdd((string)itemQuantity.ItemId, (int)temp_consumption_count))
                {
                    Dictionary<string, int> expenseItemsCount = itemsPrices.ExpenseItemsCount;
                    expenseItemsCount[itemId] += temp_consumption_count;
                }
            }
            ItemProductionSystem.StartStationPopulationProduction(spaceTime, station, populationReceipt2, num3000);
            int num3 = station.Population + Mathf.RoundToInt(populationReceipt2.SuccessPopAdd * (float)station.Record.MaxPopulation);
            StationSystem.RefreshPopulationConsumables(factions, magnumProgression, station, itemsPrices, (float)num3);

            return false;
        }


    }
}

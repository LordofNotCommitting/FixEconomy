Quasimorph naturally have either all item price crash or all item price rise forever, as demand and supply are mostly static outside of station changing hands to other faction type (and therefore list of produced item). This assumes you to be a not factor, and if you get involved then you can crash market even faster as demand won't increase with you supplying more. Like, if you dumped 500 units of gold into the market from gold dimension, gold price will be ruined semi-permanantly.

This mod put rubberbanding on demanded/excess items, and decrease/increase consumption cycle accordingly. This should prevent consistant long game market crashes which has been common ever since market has been a thing. If you want more explanation, it will be commented below.

How much market-correcting rubberbending force you are applying can be configured on MCM menu, but not really recommending you to touch those if you don't know what you are doing.

**Need to restart the game after MCM config setup for mode change to take effect**

**If your market has already crashed and installed this mod to recover, This mod take time to clean up all the excess stocks that caused the crash. increase Price Ratio Maximum % if you are in this category to make recovery faster.**

**Notes Below is detailed explanation on how vanilla market work. Ignore that if you are not interested.**

How Market works vanilla ingame afaik:
Vanilla game have station with semi-constant item consumption rate (depending on population and etc), which produces items in the same rate. This means there is a cap on consumption rate for each and every item. Depending on production cycle, Each item will have either excess production globally or deficit production globally.

Now, Vanilla game periodical set average price by averaging price of an item from each station. Each station determines the price by how much excess/demanded stock they have.
Putting 1+1 together. For each item, # of global stock is determined to be either increase or decrease naturally due to production cycle. And excess from production drives down item price. There is no mechanic to clear up excess stock, and therefore item price will continue to go down forever if item stock is on global excess, or continue to go up if item stock is on global demand.

This also makes item where # of supply is exactly same as # of demand fragile.
If this item have excess -> supply is exactly same as demand -> excess means item price go down -> Repeat from step 1.


This mod does:
Starting item price / Current item price * Price Adjustment Multiplier % in actual percentage will be set as multiplier on production cycle. To minimum of Price Ratio Minimum % as actual percentage and to maximum of Price Ratio Maximum % as actual percentage.

If this multiplier is greater than 1 (there are excess items), this multiplier will multiply station item consumption, and decrease production cycle time.
If this multiplier is lower than 1 (there are in-demand items),it just increase production cycle time.

Since stock per station is now controlled and consumption now pegged to starting item price, there are no hard crashes. Rubberbending, how does it work?

Oh, and this should in theory increase non-tez faction power since we just juiced up station production cycles. While I did not notice it during testing, it should be disclosed.

using UnityEngine;

public class TraderAI : StructureAI {
    public float bestWeight;
    public Item toTrade;
    public StructureBehaviours from;
    public StructureBehaviours to;
    public bool bought = false;

    public TraderAI () {
        lastUpdated = 0;
        delay = Random.Range (2.0f, 5.0f);
        bestWeight = 0;
    }

    public override void Process (StructureBehaviours structureBehaviours, float deltaTime) {
        HandleRouting (structureBehaviours, deltaTime);
        lastUpdated += deltaTime;
        if (lastUpdated < delay) return;
        lastUpdated = 0;
        delay = Random.Range (2.0f, 5.0f);
        if (!bought && bestWeight == 0) {
            foreach (Item tradable in ItemsHandler.GetInstance ().items) {
                float minSell = float.MaxValue, maxBuy = float.MinValue;
                StructureBehaviours minSellSb = null, maxBuySb = null;
                foreach (StructureBehaviours sb in MarketManager.GetInstance ().GetStructuresTradeItem (tradable)) {
                    long sellPrice = sb.profile.market.GetSellPrice (sb, tradable);
                    long buyPrice = sb.profile.market.GetBuyPrice (sb, tradable);
                    if (sellPrice != -1 && sellPrice < minSell && sb.inventory.GetItemCount (tradable) > 0) {
                        minSell = sellPrice;
                        minSellSb = sb;
                    }
                    if (buyPrice != -1 && buyPrice > maxBuy) {
                        maxBuy = buyPrice;
                        maxBuySb = sb;
                    }
                    if (minSellSb != null && maxBuySb != null) {
                        Route a = NavigationManager.GetInstance ().GetRoute (minSellSb.transform.position, maxBuySb.transform.position);
                        Route b = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, minSellSb.transform.position);
                        if (a != null && b != null) {
                            Debug.Log ("valid route");
                            int jumps = a.waypoints.Count + b.waypoints.Count;
                            float weight = (float) (maxBuy - minSell) / jumps;
                            if (weight > bestWeight) {
                                bestWeight = weight;
                                from = minSellSb;
                                to = maxBuySb;
                                toTrade = tradable;
                                bought = false;
                            }
                        }
                    }
                }
            }
        }
        if (from != null && to != null) {
            if (!bought) structureBehaviours.route = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, from.transform.position);
            if (bought) structureBehaviours.route = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, to.transform.position);
            if (!bought && from.DockerCanDock (structureBehaviours)) {
                from.Dock (structureBehaviours);
                long sellPrice = from.profile.market.GetSellPrice (from, toTrade);
                int canBuy = (int) (FactionsManager.GetInstance ().GetWealth (structureBehaviours.faction) / sellPrice);
                int canHold = (int) (structureBehaviours.inventory.GetAvailableSize () / toTrade.size);
                int theyHave = from.inventory.GetItemCount (toTrade);
                int willBuy = Mathf.Min (canBuy, Mathf.Min (canHold, theyHave));
                FactionsManager.GetInstance ().ChangeWealth (structureBehaviours.faction, -willBuy * sellPrice);
                FactionsManager.GetInstance ().ChangeWealth (from.faction, willBuy * sellPrice);
                from.inventory.RemoveItem (toTrade, willBuy);
                structureBehaviours.inventory.AddItem (toTrade, willBuy);
                bought = true;
            } else if (bought && structureBehaviours.transform.parent == from.transform) {
                from.Undock (structureBehaviours);
            } else if (bought && to.DockerCanDock (structureBehaviours)) {
                to.Dock (structureBehaviours);
                long buyPrice = to.profile.market.GetBuyPrice (to, toTrade);
                int theyCanHold = (int) (to.inventory.GetAvailableSize () / toTrade.size);
                int theyCanBuy = (int) (FactionsManager.GetInstance ().GetWealth (to.faction) / buyPrice);
                int weHave = structureBehaviours.inventory.GetItemCount (toTrade);
                int willSell = Mathf.Min (theyCanBuy, Mathf.Min (theyCanHold, weHave));
                FactionsManager.GetInstance ().ChangeWealth (to.faction, -willSell * buyPrice);
                FactionsManager.GetInstance ().ChangeWealth (structureBehaviours.faction, willSell * buyPrice);
                structureBehaviours.inventory.RemoveItem (toTrade, willSell);
                to.inventory.AddItem (toTrade, willSell);
                bought = false;
                bestWeight = 0;
            } else if (!bought && structureBehaviours.transform.parent == to.transform) {
                to.Undock (structureBehaviours);
            }
        }
        HandleCombat (structureBehaviours, deltaTime);
    }
}

using UnityEngine;

public class TraderAI : StructureAI {
    public float bestWeight;
    public Item toTrade;
    public StructureBehaviours from;
    public StructureBehaviours to;
    public bool bought = false;

    public TraderAI () {}

    public override void Process (StructureBehaviours structureBehaviours, float deltaTime) {
        lastUpdated += deltaTime;
        if (lastUpdated < delay) return;
            lastUpdated = 0;
            delay = Random.Range (2.0f, 5.0f);
            if (!bought && bestWeight == 0) {
            foreach(Item tradable in ItemsHandler.GetInstance().items) {
                float minSell = 0, maxBuy = 0;
                StructureBehaviours minSellSb = null, maxBuySb = null;
                foreach (StructureBehaviours sb in MarketManager.GetInstance().GetStructuresTradeItem(tradable)) {
                    long sellPrice = sb.profile.market.GetSellPrice(sb, tradable);
                    long buyPrice = sb.profile.market.GetBuyPrice(sb, tradable);
                    if (sellPrice != -1 && sellPrice < minSell) {
                        minSell = sellPrice;
                        minSellSb = sb;
                    }
                    if (buyPrice != -1 && buyPrice > maxBuy) {
                        maxBuy = buyPrice;
                        maxBuySb = sb;
                    }
                    float weight = (float) (buyPrice - sellPrice) / NavigationManager.GetInstance().GetRoute(minSellSb.transform.position, maxBuySb.transform.position).waypoints.Count;
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
        if (!bought) structureBehaviours.route = NavigationManager.GetInstance().GetRoute(structureBehaviours.transform.position, from.transform.position);
        if (bought) structureBehaviours.route = NavigationManager.GetInstance().GetRoute(structureBehaviours.transform.position, to.transform.position);
        if (!bought && from.DockerCanDock(structureBehaviours)) {
            from.Dock(structureBehaviours);
            long sellPrice = from.profile.market.GetSellPrice(from, toTrade);
            int canBuy = (int)(FactionsManager.GetInstance().GetWealth(structureBehaviours.faction) / sellPrice);
            int canHold = (int)(structureBehaviours.inventory.GetAvailableSize() / toTrade.size);
            int theyHave = from.inventory.GetItemCount(toTrade);
            int willBuy = Mathf.Min(canBuy, Mathf.Min(canHold, theyHave));
            FactionsManager.GetInstance().ChangeWealth(structureBehaviours.faction, -willBuy * sellPrice);
            FactionsManager.GetInstance().ChangeWealth(from.faction, willBuy * sellPrice);
            from.inventory.RemoveItem(toTrade, willBuy);
            structureBehaviours.inventory.AddItem(toTrade, willBuy);
            bought = true;
        }
        if (bought && to.DockerCanDock(structureBehaviours)) {
            to.Dock(structureBehaviours);
            long buyPrice = to.profile.market.GetBuyPrice(to, toTrade);
            int theyCanHold = (int)(to.inventory.GetAvailableSize() / toTrade.size);
            int theyCanBuy = (int)(FactionsManager.GetInstance().GetWealth(to.faction) / buyPrice);
            int weHave = structureBehaviours.inventory.GetItemCount(toTrade);
            int willSell = Mathf.Min(theyCanBuy, Mathf.Min(theyCanHold, weHave));
            FactionsManager.GetInstance().ChangeWealth(to.faction, -willSell * buyPrice);
            FactionsManager.GetInstance().ChangeWealth(structureBehaviours.faction, willSell * buyPrice);
            structureBehaviours.inventory.RemoveItem(toTrade, willSell);
            to.inventory.AddItem(toTrade, willSell);
            bought = false;
            bestWeight = 0;
        }
        if (structureBehaviours.targeted == null || Vector3.Distance (structureBehaviours.transform.position, structureBehaviours.targeted.transform.position) > optimalRange) {
            float leastWeight = float.MaxValue;
            foreach (StructureBehaviours structure in structureBehaviours.sector.inSector) {
                if (structure != null && structure.CanBeTargeted () && structure.profile.canFireAt) {
                    float distance = Vector3.Distance (structureBehaviours.transform.position, structure.transform.position);
                    if (structure != structureBehaviours &&
                        structureBehaviours.factionsManager.Hostile (structureBehaviours.faction, structure.faction) &&
                        distance < leastWeight) {
                        leastWeight = distance;
                        structureBehaviours.targeted = structure;
                    }
                }
            }
        }
        if (structureBehaviours.targeted != null) {
            float totalRange = 0;
            int effectiveTurrets = 0;
            foreach (TurretHandler turretHandler in structureBehaviours.turrets) {
                turretHandler.Activate (structureBehaviours.targeted.gameObject);
                Turret turret = turretHandler.turret;
                if (turret != null) {
                    totalRange += turret.range;
                    effectiveTurrets ++;
                }
            }
            if (structureBehaviours.route == null) {
                optimalRange = effectiveTurrets == 0 ? 1000 : totalRange / effectiveTurrets * structureBehaviours.profile.engagementRangeMultiplier;
                structureBehaviours.engine.forwardSetting = 1.0f;
                Vector3 heading = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position;
                Vector3 perp = Vector3.Cross (structureBehaviours.transform.forward, heading);
                float leftRight = Vector3.Dot (perp, structureBehaviours.transform.up);
                float angle = structureBehaviours.targeted.transform.position - structureBehaviours.transform.position == Vector3.zero ?
                        0 :
                        Quaternion.Angle (structureBehaviours.transform.rotation, Quaternion.LookRotation (structureBehaviours.targeted.transform.position
                    - structureBehaviours.transform.position)
                );
                float lrMult = leftRight >= 0 ? 1 : -1;
                angle *= lrMult;
                float approachAngle = 90 * lrMult;
                float sqrDis = (structureBehaviours.targeted.transform.position - structureBehaviours.transform.position).sqrMagnitude;
                approachAngle -= sqrDis > optimalRange * optimalRange ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0;
                approachAngle += sqrDis < optimalRange * optimalRange * 0.75f ? structureBehaviours.profile.rangeChangeAngle * lrMult : 0;
                if (angle > approachAngle) structureBehaviours.engine.turnSetting = 1;
                else if (angle > 0 && angle < approachAngle * 0.9f) structureBehaviours.engine.turnSetting = -1;
                else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1;
                else if (angle < 0 && angle > -approachAngle * 0.9f) structureBehaviours.engine.turnSetting = 1;
                else structureBehaviours.engine.turnSetting = 0;
            }
        } else {
            if (structureBehaviours.route == null) {
                structureBehaviours.engine.forwardSetting = 0;
                structureBehaviours.engine.turnSetting = 0;
            }
        }
        if (structureBehaviours.route != null)
            if (structureBehaviours.route.waypoints != null) {
                if (structureBehaviours.route.waypoints.Count == 0)
                    structureBehaviours.route = null;
            } else structureBehaviours.route = null;
        if (structureBehaviours.route != null) {
            Vector3 heading = structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position;
            Vector3 perp = Vector3.Cross (structureBehaviours.transform.forward, heading);
            float leftRight = Vector3.Dot (perp, structureBehaviours.transform.up);
            float angle = structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position == Vector3.zero ?
                    0.0f :
                    Quaternion.Angle (structureBehaviours.transform.rotation, Quaternion.LookRotation (structureBehaviours.route.waypoints[0]
                - structureBehaviours.transform.position)
            );
            float lrMult = leftRight >= 0 ? 1 : -1;
            angle *= lrMult;
            float approachAngle = 0;
            if (angle > approachAngle) structureBehaviours.engine.turnSetting = 1;
            else if (angle > 0 && angle < approachAngle * 0.9f) structureBehaviours.engine.turnSetting = -1;
            else if (angle < -approachAngle) structureBehaviours.engine.turnSetting = -1;
            else if (angle < 0 && angle > -approachAngle * 0.9f) structureBehaviours.engine.turnSetting = 1;
            else structureBehaviours.engine.turnSetting = 0;
            structureBehaviours.engine.forwardSetting = 1;
            if ((structureBehaviours.route.waypoints[0] - structureBehaviours.transform.position).sqrMagnitude <= 2750) structureBehaviours.route.ReachedWaypoint ();
        }
    }
}

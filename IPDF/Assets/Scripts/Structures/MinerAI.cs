using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinerAI : StructureAI {
    public StructureBehaviours mining;
    public bool mined;
    public List<Item> loot = new List<Item> ();
    public StructureBehaviours selling;

    public MinerAI () {
        lastUpdated = 0;
        delay = Random.Range (0.5f, 1.0f);
    }

    public override void Process (StructureBehaviours structureBehaviours, float deltaTime) {
        HandleRouting (structureBehaviours, deltaTime);
        lastUpdated += deltaTime;
        if (lastUpdated < delay) return;
        lastUpdated = 0;
        delay = Random.Range (0.5f, 1.0f);
        if (mined) {
            if (selling == null) {
                float maxBuy = float.MinValue;
                foreach (StructureBehaviours trader in MarketManager.GetInstance ().GetStructuresTradeItem (loot[0])) {
                    long buyPrice = trader.profile.market.GetBuyPrice (trader, loot[0]);
                    if (buyPrice != -1 && buyPrice > maxBuy) {
                        maxBuy = buyPrice;
                        selling = trader;
                    }
                }
                structureBehaviours.route = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, selling.transform.position);
            }
            if (selling.DockerCanDock (structureBehaviours)) {
                selling.Dock (structureBehaviours);
                long buyPrice = selling.profile.market.GetBuyPrice (selling, loot[0]);
                int theyCanHold = (int) (selling.inventory.GetAvailableSize () / loot[0].size);
                int theyCanBuy = (int) (FactionsManager.GetInstance ().GetWealth (selling.faction) / buyPrice);
                int weHave = structureBehaviours.inventory.GetItemCount (loot[0]);
                int willSell = Mathf.Min (theyCanBuy, Mathf.Min (theyCanHold, weHave));
                FactionsManager.GetInstance ().ChangeWealth (selling.faction, -willSell * buyPrice);
                FactionsManager.GetInstance ().ChangeWealth (structureBehaviours.faction, willSell * buyPrice);
                structureBehaviours.inventory.RemoveItem (loot[0], willSell);
                selling.inventory.AddItem (loot[0], willSell);
                if (structureBehaviours.inventory.GetItemCount (loot[0]) == 0) loot.RemoveAt (0);
            } else if (structureBehaviours.transform.parent == selling.transform) {
                selling.Undock (structureBehaviours);
            }
            if (loot.Count == 0) mined = false;
        } else {
            if (mining == null) {
                List<StructureBehaviours> structures = StructuresManager.GetInstance ().structures;
                List<StructureBehaviours> asteroids = new List<StructureBehaviours> ();
                foreach (StructureBehaviours structure in structures)
                    if (structure.profile.structureClass == StructureClass.Asteroid)
                        asteroids.Add (structure);
                int minDis = int.MaxValue;
                foreach (StructureBehaviours asteroid in asteroids) {
                    Route a = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, asteroid.transform.position);
                    if (a != null) {
                        int dis = a.waypoints.Count;
                        if (dis < minDis) {
                            minDis = dis;
                            mining = asteroid;
                        }
                    }
                }
            }
            if (mining != null) {
                structureBehaviours.route = NavigationManager.GetInstance ().GetRoute (structureBehaviours.transform.position, mining.transform.position);
                foreach (TurretHandler turretHandler in structureBehaviours.turrets)
                    if (turretHandler.turret is MiningPulseTurret)
                        turretHandler.Activate (mining.gameObject);
            }
            List<StructureBehaviours> inSector = structureBehaviours.sector.inSector;
            foreach (StructureBehaviours sb in inSector) {
                if (sb.gameObject.GetComponent<CargoPod> ()) {
                    if ((sb.transform.position - structureBehaviours.transform.position).sqrMagnitude < structureBehaviours.tractorBeam.tractorBeam.range * structureBehaviours.tractorBeam.tractorBeam.range) {
                        structureBehaviours.tractorBeam.Activate (sb.gameObject);
                        foreach (Item item in structureBehaviours.inventory.inventory.Keys.ToArray ())
                            if (!loot.Contains (item))
                                loot.Add (item);
                        break;
                    }
                }
            }
            if (structureBehaviours.inventory.GetAvailableSize () <= 1) {
                mined = true;
            }
        }
        HandleCombat (structureBehaviours, deltaTime);
    }
}

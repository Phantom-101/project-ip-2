using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Essentials;

/*

$$$$$$$$\ $$$$$$$$\ $$$$$$$$\  $$$$$$\  
\__$$  __|$$  _____|$$  _____|$$  __$$\ 
   $$ |   $$ |      $$ |      $$ /  $$ |
   $$ |   $$$$$\    $$$$$\    $$ |  $$ |
   $$ |   $$  __|   $$  __|   $$ |  $$ |
   $$ |   $$ |      $$ |      $$ |  $$ |
   $$ |   $$ |      $$$$$$$$\  $$$$$$  |
   \__|   \__|      \________| \______/ 

1. GeneratorHandler.GenerateEnergy (CapacitorHandler capacitor);
2. CapacitorHandler.DistributeEnergy (List<TurretHandler> turrets, ShieldHandler shield, ElectronicsHandler electronics);
3. EngineHandler.ApplySettings (ConstantForce target);
4. ElectronicsHandler.Process (GameObject processor);
5. TractorBeamHandler.Process (GameObject processor);

*/

public class Equipment : Item {
   public int meta;
}
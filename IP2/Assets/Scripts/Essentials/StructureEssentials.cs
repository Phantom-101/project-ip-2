using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

$$\   $$\                     $$\   $$\     $$\       
$$ |  $$ |                    $$ |  $$ |    $$ |      
$$ |  $$ | $$$$$$\   $$$$$$\  $$ |$$$$$$\   $$$$$$$\  
$$$$$$$$ |$$  __$$\  \____$$\ $$ |\_$$  _|  $$  __$$\ 
$$  __$$ |$$$$$$$$ | $$$$$$$ |$$ |  $$ |    $$ |  $$ |
$$ |  $$ |$$   ____|$$  __$$ |$$ |  $$ |$$\ $$ |  $$ |
$$ |  $$ |\$$$$$$$\ \$$$$$$$ |$$ |  \$$$$  |$$ |  $$ |
\__|  \__| \_______| \_______|\__|   \____/ \__|  \__|

*/

public interface IHealth {
    // Getters ------------------------------------------------------------------------------------
    float GetTotalHealth(); // Returns the sum of all of the layer healths
    float GetLayerHealth(int i); // Returns value at hitpoints[i]
    List<HealthChange> GetHealthChanges(); // Returns the health stack
    float GetHealthChangesSum(); // Returns the sum of the health stack
    // Setters ------------------------------------------------------------------------------------
    void AddHealthChange(HealthChange healthChange); // Adds a health change to the health stack
    // Appliers -----------------------------------------------------------------------------------
    void ApplyHealthChanges(); // Apply health changes on the health stack
}

public struct HealthChange {
    public float value;
    public float[] effectiveness;
    public bool[] bypasses;

    // Constructor given variable values
    public HealthChange(float value, float[] effectiveness, bool[] bypasses) {
        this.value = value;
        this.effectiveness = effectiveness;
        this.bypasses = bypasses;
    }

    // Constructor given HealthChangeProfile
    public HealthChange(HealthChangeProfile healthChangeProfile) {
        this.value = healthChangeProfile.value;
        this.effectiveness = healthChangeProfile.effectiveness;
        this.bypasses = healthChangeProfile.bypasses;
    }
}

public interface IRenegerative {
    // Setters ------------------------------------------------------------------------------------
    void RegenerateHealth(); // Add a positive health change to the health stack using IHealth's AddHealthChange
}

/*

$$\      $$\                                                                    $$\     
$$$\    $$$ |                                                                   $$ |    
$$$$\  $$$$ | $$$$$$\  $$\    $$\  $$$$$$\  $$$$$$\$$$$\   $$$$$$\  $$$$$$$\  $$$$$$\   
$$\$$\$$ $$ |$$  __$$\ \$$\  $$  |$$  __$$\ $$  _$$  _$$\ $$  __$$\ $$  __$$\ \_$$  _|  
$$ \$$$  $$ |$$ /  $$ | \$$\$$  / $$$$$$$$ |$$ / $$ / $$ |$$$$$$$$ |$$ |  $$ |  $$ |    
$$ |\$  /$$ |$$ |  $$ |  \$$$  /  $$   ____|$$ | $$ | $$ |$$   ____|$$ |  $$ |  $$ |$$\ 
$$ | \_/ $$ |\$$$$$$  |   \$  /   \$$$$$$$\ $$ | $$ | $$ |\$$$$$$$\ $$ |  $$ |  \$$$$  |
\__|     \__| \______/     \_/     \_______|\__| \__| \__| \_______|\__|  \__|   \____/ 

*/

public interface IMoveable {
    // Getters ------------------------------------------------------------------------------------
    List<MovementVector> GetMovementVectors(); // Gets the movement stack
    List<Vector3> GetTranslations(); // Gets the list of translation vectors
    Vector3 GetTranslationsMagnitude(); // Gets the magnitude of the sums of all translation vectors on the movement stack
    List<Vector3> GetRotations(); // Gets the list of rotation vectors
    Vector3 GetRotationsMagnitude(); // Gets the magnitude of the sums of all rotation vectors on the movement stack
    // Setters ------------------------------------------------------------------------------------
    void AddMovementVector(MovementVector movementVector); // Adds a movement vector to the movement stack
    // Appliers -----------------------------------------------------------------------------------
    void ApplyMovementVectors(); // Moves the gameObject according to the movement stack
}

public struct MovementVector {
    public Vector3 translation;
    public Vector3 rotation;

    // Constructor given variable values
    public MovementVector (Vector3 translation, Vector3 rotation) {
        this.translation = translation;
        this.rotation = rotation;
    }
}

/*

$$$$$$$$\ $$\   $$\       $$\     $$\                     
$$  _____|\__|  $$ |      $$ |    \__|                    
$$ |      $$\ $$$$$$\   $$$$$$\   $$\ $$$$$$$\   $$$$$$\  
$$$$$\    $$ |\_$$  _|  \_$$  _|  $$ |$$  __$$\ $$  __$$\ 
$$  __|   $$ |  $$ |      $$ |    $$ |$$ |  $$ |$$ /  $$ |
$$ |      $$ |  $$ |$$\   $$ |$$\ $$ |$$ |  $$ |$$ |  $$ |
$$ |      $$ |  \$$$$  |  \$$$$  |$$ |$$ |  $$ |\$$$$$$$ |
\__|      \__|   \____/    \____/ \__|\__|  \__| \____$$ |
                                                $$\   $$ |
                                                \$$$$$$  |
                                                 \______/ 

*/

public interface IEquipable {
    // Getters ------------------------------------------------------------------------------------
    List<Equipment> GetAllEquipment(); // Gets all of the equipment
    Equipment GetEquipment(int i); // Gets equipment at index i
    // Setters ------------------------------------------------------------------------------------
    string SetEquipment(int i); // Sets equipment at index i
    // Appliers -----------------------------------------------------------------------------------
    string SetAllEquipmentState(bool state); // Sets the state of all equipment
    string SetEquipmentState(int i, bool state); // Sets state of equipment at index i
}
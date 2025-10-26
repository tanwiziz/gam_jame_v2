using UnityEngine;

// Assuming you made ItemDefinition inherit from ScriptableObject
public class ItemDefinition : ScriptableObject
{
    // THIS is the line you need to add to fix the error.
    public Sprite icon; 

    // Add other item properties here (e.g., public string itemName;)
}
using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    // List to hold all the keys player has collected
    public List<KeyItem> collectedKeys = new List<KeyItem>();

    public void AddKey(KeyItem key)
    {
        if (!collectedKeys.Contains(key))
        {
            collectedKeys.Add(key);
        }
    }

    public bool Contains(KeyItem key)
    {
        return collectedKeys.Contains(key);
    }
}

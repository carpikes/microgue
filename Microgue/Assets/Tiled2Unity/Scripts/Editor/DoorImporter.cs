using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Tiled2Unity.CustomTiledImporter]
public class DoorImporter : Tiled2Unity.ICustomTiledImporter
{
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
        if (!props.ContainsKey("DoorType"))
            return;

        DoorBehavior door = gameObject.AddComponent<DoorBehavior>();
        door.mType = props["DoorType"];
        if (!props.TryGetValue("DoorNeedItem", out door.mNeedItem))
            door.mNeedItem = null;

        door.mPosition = gameObject.transform.position;
        BoxCollider2D b2d = gameObject.GetComponent<BoxCollider2D>();
        if (b2d == null)
            Debug.LogError("I need a box collider");
        else
            b2d.isTrigger = true;
    }

    public void CustomizePrefab(GameObject prefab)
    {
        Debug.Log("Customize prefab");
    }
}

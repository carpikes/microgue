using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Tiled2Unity.CustomTiledImporter]
public class ItemImporter : Tiled2Unity.ICustomTiledImporter
{
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
        if (!props.ContainsKey("ItemCategory"))
            return;

        ItemBehavior s = gameObject.AddComponent<ItemBehavior>();
        s.mWhat = props["ItemName"];

        CircleCollider2D cc = gameObject.GetComponent<CircleCollider2D>();
        if (cc == null)
            throw new System.Exception("VOGLIO UN CIRCLE COLLIDER"); // throw ex

        Vector2 trpos = gameObject.transform.position;
        s.mCenter = trpos + cc.offset;

        // TODO: set all effects in some way
    }

    public void CustomizePrefab(GameObject prefab)
    {
        Debug.Log("Customize prefab");
    }
}

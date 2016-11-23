using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Tiled2Unity.CustomTiledImporter]
public class SpawnImporter : Tiled2Unity.ICustomTiledImporter
{
    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> props)
    {
        if (!props.ContainsKey("SpawnName"))
            return;

        SpawnBehavior s = gameObject.AddComponent<SpawnBehavior>();
        s.mWhat = props["SpawnName"];
        if (props.ContainsKey("SpawnNumber"))
            s.mNumberMin = s.mNumberMax = int.Parse(props["SpawnNumber"]);
        else {
            s.mNumberMin = int.Parse(props["SpawnNumberMin"]);
            s.mNumberMax = int.Parse(props["SpawnNumberMax"]);
        }

        CircleCollider2D cc = gameObject.GetComponent<CircleCollider2D>();
        if (cc == null)
            throw new System.Exception("VOGLIO UN CIRCLE COLLIDER"); // throw ex

        Vector2 trpos = gameObject.transform.position;
        s.mCenter = trpos + cc.offset;
        s.mRadius = cc.radius;
    }

    public void CustomizePrefab(GameObject prefab)
    {
        Debug.Log("Customize prefab");
    }
}

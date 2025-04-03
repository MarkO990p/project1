using UnityEngine;
using System.Collections.Generic;

public class PortalManager : MonoBehaviour
{
    public static PortalManager instance;

    [System.Serializable]
    public class Destination
    {
        public string destinationId;
        public Transform destinationTransform;
    }

    public Destination[] destinations;

    private Dictionary<string, Transform> idMap = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            foreach (var dest in destinations)
            {
                if (!idMap.ContainsKey(dest.destinationId))
                {
                    idMap.Add(dest.destinationId, dest.destinationTransform);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform GetDestination(string id)
    {
        idMap.TryGetValue(id, out Transform dest);
        return dest;
    }
}

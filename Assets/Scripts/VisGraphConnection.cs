using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class VisGraphConnection
{
    // The to node for this connection.
    [SerializeField]
    private GameObject toNode;
    public GameObject ToNode {
        get { return toNode; }
    }
}
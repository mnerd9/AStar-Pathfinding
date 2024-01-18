using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NodeRecord
{
    private GameObject node;
    public GameObject Node
    {
        get { return node; }
        set { node = value; }
    }
    private Connection connection;
    public Connection Connection
    {
        get { return connection; }
        set { connection = value; }
    }
    private float costSoFar;
    public float CostSoFar
    {
        get { return costSoFar; }
        set { costSoFar = value; }
    }
    private float estimatedTotalCost;
    public float EstimatedTotalCost
    {
        get { return estimatedTotalCost; }
        set { estimatedTotalCost = value; }
    }
    public NodeRecord()
    {
    }
}
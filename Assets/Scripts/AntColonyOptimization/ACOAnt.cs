using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ACOAnt
{
    private float antTourLength = 0;
    public float AntTourLength
    {
        set { antTourLength = value; }
        get { return antTourLength; }
    }
    private List<ACOConnection> antTravelledConnections = new List<ACOConnection>();
    public List<ACOConnection> AntTravelledConnections
    {
        get { return antTravelledConnections; }
    }
    private GameObject startNode;
    public GameObject StartNode
    {
        set { startNode = value; }
        get { return startNode; }
    }
    public ACOAnt()
    {
    }
    public void AddAntTourLength(float AntTourLength)
    {
        this.AntTourLength += AntTourLength;
    }
    public void AddTravelledConnection(ACOConnection aConnection)
    {
        antTravelledConnections.Add(aConnection);
    }
}

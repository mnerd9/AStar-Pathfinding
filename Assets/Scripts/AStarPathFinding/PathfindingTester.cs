using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Astar.MyScript;

public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();
    private MyScript myScript;
    // List of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();
    // List of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();
    // The start and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;
    // Debug line offset.
    Vector3 OffSet = new Vector3(0, 0.3f, 0);
    // Movement variables.
    [SerializeField] private float currSpeed = 32;
    [SerializeField] private int logs = 10;
    private int currTarget = 0;
    private Vector3 currTargetPos;
    private int movingDir = 1;
    private bool isAgentMoving = true;

    public TextMeshProUGUI storeDistance;
    public TextMeshProUGUI storeTime;
    public TextMeshProUGUI storeSpeed;
    public TextMeshProUGUI storeItems;

    private float newDist;
    private float newTime;
    private float newSpeed;

    private bool moveNotification;

    private bool isMovingDirection;

    public float accumulatedDistance;
    public float accumulatedTime;

    // Start is called before the first frame update
    void Start()
    {
        myScript = GetComponent<MyScript>();
        if (start == null || end == null)
        {
            myScript.notification("No start or end waypoints.", "error");
            return;
        }
        VisGraphWaypointManager tmpWpM = start.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            myScript.notification("Start is not a waypoint.", "error");
            return;
        }
        tmpWpM = end.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            
            myScript.notification("End is not a waypoint.", "error");
            return;
        }
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            // Loop through a waypoints connections.
            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointMan.Connections)
            {
                if (aVisGraphConnection.ToNode != null)
                {
                    Connection aConnection = new Connection();
                    aConnection.FromNode = waypoint;
                    aConnection.ToNode = aVisGraphConnection.ToNode;
                    AStarManager.AddConnection(aConnection);
                }
                else
                {
                    myScript.notification(waypoint.name + " has a missing to node for a connection!", "error");
                }
            }
        }
        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(start, end);
        if (ConnectionArray.Count == 0)
        {
            myScript.notification("A* did not return a path between the start and end node.", "error");
        }
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
        }
    }
    public float CurrSpeed {
        get { return currSpeed; }
        set { currSpeed = value; }
    }

    public void SetStartNode(GameObject startNode) {
        start = startNode;
    }

    public void SetEndNode(GameObject endNode) {
        end = endNode;
    }

    void Update() {
        if (isAgentMoving) {
            if (movingDir > 0) {
                currTargetPos = ConnectionArray[currTarget].ToNode.transform.position;
            } else {
                currTargetPos = ConnectionArray[currTarget].FromNode.transform.position;
            }
            
            if (!moveNotification) {
                myScript.notification(gameObject.name + " is moving", "success");
                moveNotification = true;
            }

            currTargetPos.y = transform.position.y;
            Vector3 direction = currTargetPos - transform.position;

            float agentDist = direction.magnitude;

            direction.y = 0;
            if (direction.magnitude > 0) {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.smoothDeltaTime * 5f);
            }
            Vector3 normDirection = direction.normalized;
            transform.position = transform.position + normDirection * currSpeed * Time.smoothDeltaTime;
            if (agentDist < 1) {
                currTarget = currTarget + movingDir;

                if (currTarget >= ConnectionArray.Count || currTarget < 0) {
                    movingDir *= -1;
                    currTarget += movingDir;
                    isMovingDirection = true;
                    
                    currTarget = Mathf.Clamp(currTarget, 0, ConnectionArray.Count - 1);
                    if (currTarget <= 0) {
                        isAgentMoving = false;
                        currSpeed = 0f;
                        myScript.notification(gameObject.name + " has returned home!", "success");
                        moveNotification = false;
                    }
                }
            }

            float calcDist = currSpeed * Time.smoothDeltaTime;
            accumulatedDistance += calcDist;
            accumulatedTime += Time.smoothDeltaTime;

            myScript.RotateWheel(currSpeed);

            storeDistance.text = accumulatedDistance.ToString("F2");
            storeTime.text = accumulatedTime.ToString("F2");

            if (accumulatedTime >= 1) {
                newSpeed = accumulatedDistance / accumulatedTime;
                storeSpeed.text = currSpeed.ToString("F2");
            }

            if (currSpeed <= 0 && isAgentMoving) {
                storeSpeed.text = "0.00";
            } else if(isMovingDirection != true) {
                storeSpeed.text = currSpeed.ToString("F2");
            }
        }
    }

    public void GetNotification(string text, string type) {
        if (text == "" && type == "") {
            myScript.notification(gameObject.name + " has collected "+ logs + " logs", "info");
            return;
        }
        myScript.notification(text, type);
        return;
    }
}
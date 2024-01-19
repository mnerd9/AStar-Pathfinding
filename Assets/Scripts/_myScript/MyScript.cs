using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Astar.MyScript {
    
    public class MyScript : MonoBehaviour {
        [SerializeField] private Transform frontWheelL;
        [SerializeField] private Transform frontWheelR;
        [SerializeField] private Transform rearWheel;
        public TextMeshProUGUI setNotification;
        public GameObject notificationGUI;
        public Image notificationBackground;

        private float agent1, agent2, agent3;
        private PathfindingTester slowerAgent;
        private ACOTester slowerAgent2;
        private float originalSpeed;
        private Vector3 storeOldPosition;
        public TextMeshProUGUI collisionText;

        void Start() {
            PathfindingTester pathfindingTesterComponent = GetComponent<PathfindingTester>();
            if (pathfindingTesterComponent != null) {
                agent1 = GameObject.Find("Agent1").GetComponent<PathfindingTester>().CurrSpeed;
                agent2 = GameObject.Find("Agent2").GetComponent<PathfindingTester>().CurrSpeed;
                agent3 = GameObject.Find("Agent3").GetComponent<PathfindingTester>().CurrSpeed;
            } else {
                agent1 = GameObject.Find("Agent1").GetComponent<ACOTester>().CurrSpeed;
                agent2 = GameObject.Find("Agent2").GetComponent<ACOTester>().CurrSpeed;
                agent3 = GameObject.Find("Agent3").GetComponent<ACOTester>().CurrSpeed;
            }
        }
        
        public void notification(string getText, string getType) {
            float paddingX = 20f;
            float paddingY = 10f;
            Color infoColor = new Color(0.0f, 0.0f, 0.5f, 0.8f);
            Color successColor = new Color(0.0f, 0.5f, 0.0f, 0.8f);
            Color errorColor = new Color(0.5f, 0.0f, 0.0f, 0.8f);
            Image backgroundImage = notificationBackground.GetComponent<Image>();
            
            if (getType == null || getType == "") {
                Debug.Log("Notification Type ERROR");
                return;
            }

            if (setNotification != null) {
                notificationGUI.SetActive(true);

                if (getType == "info") {
                    backgroundImage.color = infoColor;
                } else if (getType == "success") {
                    backgroundImage.color = successColor;
                } else if (getType == "error") {
                    backgroundImage.color = errorColor;
                }

                setNotification.text = getText;

                float notiWidth = setNotification.preferredWidth;
                float notiHeight = setNotification.preferredHeight;

                if (notificationBackground != null) {
                    RectTransform backgroundRectTransform = notificationBackground.GetComponent<RectTransform>();
                    backgroundRectTransform.sizeDelta = new Vector2(notiWidth + paddingX, notiHeight + paddingY);
                }
            } else {
                Debug.Log("Notification text component is not assigned");
            }
        }

        public void RotateWheel(float currSpeed) {
            if (frontWheelL == null && frontWheelR == null && rearWheel == null) {
                notification("Cannot find the wheel of the vehicle!", "error");
                return;
            }

            float rotationAngle = 0.4f * currSpeed * Time.smoothDeltaTime * 360f;
            frontWheelL.Rotate(Vector3.left, rotationAngle);
            frontWheelR.Rotate(Vector3.right, rotationAngle);
            rearWheel.Rotate(Vector3.right, rotationAngle);
        }

        private float otherAgentSpeed;
        void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Agent") {
                Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
                if (otherRigidbody != null) {
                    float distance = Vector3.Distance(transform.position, other.transform.position);

                    if (distance < 5f) {
                        PathfindingTester pathfindingTesterComponent = other.gameObject.GetComponent<PathfindingTester>();
                        ACOTester ACOTesterComponent = other.gameObject.GetComponent<ACOTester>();
                        if (pathfindingTesterComponent != null) {
                            // Use PathfindingTester if pathfindingtester is enabled
                            otherAgentSpeed = other.gameObject.GetComponent<PathfindingTester>().CurrSpeed;
                            if (otherAgentSpeed < agent1 || otherAgentSpeed < agent2 || otherAgentSpeed < agent3) {
                                slowerAgent = other.gameObject.GetComponent<PathfindingTester>();
                                collisionText.text = "Collision Detection: " + gameObject.name + " has collided with " + other.gameObject.name;
                                if (slowerAgent != null) {
                                    originalSpeed = slowerAgent.CurrSpeed;
                                    slowerAgent.CurrSpeed = 0f;
                                    // slowerAgent.GetNotification(gameObject.name + " has stopped", "error");
                                    storeOldPosition = slowerAgent.transform.position;
                                    Vector3 newPosition = slowerAgent.transform.position;
                                    newPosition.z += 5f;
                                    slowerAgent.transform.position = newPosition;
                                }
                            }
                        } else {
                            otherAgentSpeed = other.gameObject.GetComponent<ACOTester>().CurrSpeed;
                            if (otherAgentSpeed < agent1 || otherAgentSpeed < agent2 || otherAgentSpeed < agent3) {
                                slowerAgent2 = other.gameObject.GetComponent<ACOTester>();
                                collisionText.text = "Collision Detection: " + gameObject.name + " has collided with " + other.gameObject.name;
                                if (slowerAgent2 != null) {
                                    originalSpeed = slowerAgent2.CurrSpeed;
                                    slowerAgent2.CurrSpeed = 0f;
                                    // slowerAgent2.GetNotification(gameObject.name + " has stopped", "error");
                                    storeOldPosition = slowerAgent2.transform.position;
                                    Vector3 newPosition = slowerAgent2.transform.position;
                                    newPosition.x -= 5f;
                                    slowerAgent2.transform.position = newPosition;
                                }
                            }
                        }
                    }
                }
            }
        }
        void OnTriggerExit(Collider other) {
            if (other.gameObject.tag == "Agent") {
                if (slowerAgent != null || slowerAgent2 != null) {
                    Invoke("ResumeAgent", 3f);
                }
            }
        }

        void ResumeAgent() {
            if (slowerAgent != null || slowerAgent2 != null) {
                PathfindingTester pathfindingTesterComponent = GetComponent<PathfindingTester>();
                if (pathfindingTesterComponent != null) {
                    slowerAgent.transform.position = storeOldPosition;
                    slowerAgent.CurrSpeed = originalSpeed;
                    // slowerAgent.GetNotification("", "");
                    collisionText.text = "Collision Detection:  None";
                    slowerAgent = null;
                } else {
                    slowerAgent2.transform.position = storeOldPosition;
                    slowerAgent2.CurrSpeed = originalSpeed;
                    // slowerAgent2.GetNotification("", "");
                    collisionText.text = "Collision Detection:  None";
                    slowerAgent2 = null;
                }
            }
        }
    }
}

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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject image;
    public Animator left, right, icon;
    bool isAnimationStart = false, imageMovedDown=false, pressed=false;
    float timeToLoadScene = 99999999,timeToRePressed=999999, timeToTurnTextOff=9999999;
    int sceneToLoadID,initYimage=-700;
    string photoTrigger,iconTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        
        sceneToLoadID = (SceneManager.GetActiveScene().buildIndex == 1) ? 0 : 1;
        if (SceneManager.GetActiveScene().buildIndex == 1)
            text.text = "Press B To Exit";
        else
            text.text = "Press B To Enter";
        photoTrigger = "close";
        iconTrigger = "up";
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToTurnTextOff <= Time.time)
        {
            text.gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            pressed = true;
            timeToRePressed = Time.time + 0.15f;
        }
        else if (timeToRePressed <= Time.time)
            pressed = false;

        if(isAnimationStart && Time.time >=timeToLoadScene)
        {
            SceneManager.LoadScene(sceneToLoadID);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "door")
        {
            text.gameObject.SetActive(true);
            timeToTurnTextOff = Time.time + 0.15f;
        }
        if (other.gameObject.tag== "door" && !isAnimationStart && pressed)
        {
            left.SetTrigger(photoTrigger);
            right.SetTrigger(photoTrigger);
            icon.SetTrigger(iconTrigger);
            timeToLoadScene = Time.time + 2f;
            isAnimationStart = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "door" && !isAnimationStart && pressed)
        {
            left.SetTrigger(photoTrigger);
            right.SetTrigger(photoTrigger);
            icon.SetTrigger(iconTrigger);
            timeToLoadScene = Time.time + 2f;
            isAnimationStart = true;
        }
    }

   

}

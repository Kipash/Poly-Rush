using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] int playerStep;
    [SerializeField] int startHazard;
    [SerializeField] int hazardStep;
    [SerializeField] int maxHazard;
    [SerializeField] float hazardT;
    [SerializeField] Text scoreUI;
    int hazard;

    int score;

    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    private void Start()
    {
        Screen.SetResolution(720, 1280, true);
        dragDistance = Screen.height * 10 / 100; //dragDistance is 15% height of the screen

        hazard = startHazard;
        InvokeRepeating("Hazard", hazardStep, hazardStep);
    }
    void Hazard()
    {
        hazard -= hazardStep;
        if (hazard == 0)
            gameManager.IsDead = true;
        hazard = Mathf.Clamp(hazard, 0, maxHazard);
    }
    private void Update()
    {
        gameManager.SetHazardGFX(maxHazard, hazard);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
        }
 
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            MoveRight();
                        }
                        else
                        {   //Left swipe
                            MoveLeft();
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up swipe
                            //Debug.Log("Up Swipe");
                        }
                        else
                        {   //Down swipe
                            //Debug.Log("Down Swipe");
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    //Debug.Log("Tap");
                    Reset();
                }
            }
        }
    }

    void MoveLeft()
    {
        if (!gameManager.IsReady || gameManager.IsDead)
            return;
        if (gameManager.MoveLeft())
        {
            hazard += playerStep;
            hazard = Mathf.Clamp(hazard, 0, maxHazard);
            if ((score / 7f) - (int)(score / 7f) == 0)
                Time.timeScale += 0.25f;
            score++;
            scoreUI.text = score.ToString();
        }
    }
    void MoveRight()
    {
        if (!gameManager.IsReady || gameManager.IsDead)
            return;
        if (gameManager.MoveRight())
        {
            hazard += playerStep;
            hazard = Mathf.Clamp(hazard, 0, maxHazard);
            if ((score / 7f) - (int)(score / 7f) == 0)
                Time.timeScale += 0.25f;
            score++;
            scoreUI.text = score.ToString();
        }
    }

    void Reset()
    {
        if (gameManager.IsDead)
        {
            score = 0;
            hazard = startHazard;
            gameManager.Reset();
        }
    }

}
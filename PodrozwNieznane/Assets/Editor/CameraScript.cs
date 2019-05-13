using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    float speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveRigth();
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDown();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveUp();
        }
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
        {
            if (getSpeed() < 20.0f)
            {
                changeSpeed(getSpeed() + 1);
            }
        }
        if(Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
        {
            if (getSpeed() > 1.0f)
            {
                changeSpeed(getSpeed() - 1);
            }
        }
    }

    void moveRigth()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }
    void moveLeft()
    {
        transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
    }
    void moveUp()
    {
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
    }
    void moveDown()
    {
        transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
    }

    void changeSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }
    float getSpeed()
    {
        return speed;
    }


}

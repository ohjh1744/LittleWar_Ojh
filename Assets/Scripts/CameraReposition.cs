using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReposition : MonoBehaviour
{
    private Vector3 firstMapPos;
    private Vector3 secondMapPos;
    private Vector3 thirdMapPos;
    private Vector3 curCameraPos;
    
    // Start is called before the first frame update
    void Start()
    {
        firstMapPos = new Vector3(0, 0, -10f);
        secondMapPos = new Vector3(17.5f, 0, -10f);
        thirdMapPos = new Vector3(35f, 0, -10f);
        //transform.position = firstMapPos;
        curCameraPos = firstMapPos;
        transform.position = curCameraPos;
    }
    public void MoveRightScreen()
	{
        if (curCameraPos == firstMapPos)
        {
            curCameraPos = secondMapPos;
        }
        else if (curCameraPos == secondMapPos)
        {
            curCameraPos = thirdMapPos;
        }
        else return;

        // 카메라 위치 변경
        transform.position = curCameraPos;
    }
    public void MoveLeftScreen()
	{
        if (curCameraPos == thirdMapPos)
        {
            curCameraPos = secondMapPos;
        }
        else if (curCameraPos == secondMapPos)
        {
            curCameraPos = firstMapPos;
        }
        else return;
        transform.position = curCameraPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

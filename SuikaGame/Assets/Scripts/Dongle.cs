using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public bool isDrag;
    Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrag)
        {
            // 스크린 좌표를 월드 좌표로 변경해준다.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.1f + transform.localScale.x / 2;
            float rightBorder = 4.1f - transform.localScale.x / 2;

            if (mousePos.x < leftBorder)
                mousePos.x = leftBorder;
            else if (rightBorder < mousePos.x)
                mousePos.x = rightBorder;

            mousePos.y = transform.position.y;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.02f);

        }
    }

    public void Drag()
    {
        isDrag = true;
    }
    public void Drop()
    {
        isDrag = false;
        rigidbody.simulated = true;
    }
}

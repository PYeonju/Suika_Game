using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager gameManager;

    public int level;
    public bool isDrag;
    public bool isMerge;

    Rigidbody2D rigidbody;
    CircleCollider2D circle;
    Animator animator;
    

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // 스크립트가 (오브젝트가) 새로생성이 되거나 활성화 될 때 자동으로 활성화되는 이벤트 함수
    void OnEnable()
    {
        animator.SetInteger("Level",level);
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
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigidbody.simulated = false;
        circle.enabled = false;

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        
        while(frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            yield return null;
        }

        isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge = true;

        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        animator.SetInteger("Level", level + 1);

        yield return new WaitForSeconds(0.3f);

        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();
            
            // 합쳐지는 중에 또 하게 되면 문제가 발생하므로
            // 합쳐지지 않는 중일때만 머지를한다.
            if(level == other.level && !isMerge && !other.isMerge && level<7 )
            {
                // 합치기 로직이 이루어져야한다.
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. 내가 아래에 있을때
                // 2. 동일한 높이이지만, 내가 오른쪽에 있을때
                if(meY < otherY || (meY==otherY && otherX < meX))
                {
                    // 상대방은 숨기기
                    other.Hide(transform.position);
                    // 나는 레벨업
                    LevelUp();
                }
            }
        }
    }
}

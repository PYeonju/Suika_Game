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

    // ��ũ��Ʈ�� (������Ʈ��) ���λ����� �ǰų� Ȱ��ȭ �� �� �ڵ����� Ȱ��ȭ�Ǵ� �̺�Ʈ �Լ�
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
            // ��ũ�� ��ǥ�� ���� ��ǥ�� �������ش�.
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
            
            // �������� �߿� �� �ϰ� �Ǹ� ������ �߻��ϹǷ�
            // �������� �ʴ� ���϶��� �������Ѵ�.
            if(level == other.level && !isMerge && !other.isMerge && level<7 )
            {
                // ��ġ�� ������ �̷�������Ѵ�.
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. ���� �Ʒ��� ������
                // 2. ������ ����������, ���� �����ʿ� ������
                if(meY < otherY || (meY==otherY && otherX < meX))
                {
                    // ������ �����
                    other.Hide(transform.position);
                    // ���� ������
                    LevelUp();
                }
            }
        }
    }
}

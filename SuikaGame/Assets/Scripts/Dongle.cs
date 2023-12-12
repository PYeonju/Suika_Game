using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager gameManager;
    public ParticleSystem effect;
    public int level;
    public bool isDrag;
    public bool isMerge;
    public bool isAttack;

    public Rigidbody2D rigidbody;
    CircleCollider2D circle;
    Animator animator;
    SpriteRenderer spriteRenderer;

    float deadTime;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enter���� ������ ��찡 �־� Stay������ ����
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            // �������� �߿� �� �ϰ� �Ǹ� ������ �߻��ϹǷ�
            // �������� �ʴ� ���϶��� �������Ѵ�.
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                // ��ġ�� ������ �̷�������Ѵ�.
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. ���� �Ʒ��� ������
                // 2. ������ ����������, ���� �����ʿ� ������
                if (meY < otherY || (meY == otherY && otherX < meX))
                {
                    // ������ �����
                    other.Hide(transform.position);
                    // ���� ������
                    LevelUp();
                }
            }
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        if (isAttack)
            yield break;

        isAttack = true;

        gameManager.SfxPlay(GameManager.Sfx.Attack);

        yield return new WaitForSeconds(0.2f);
        isAttack = false;
    }

    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigidbody.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
            EffectPlay();

        StartCoroutine(HideRoutine(targetPos));
    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        
        while(frameCount < 20)
        {
            frameCount++;
            if (targetPos != Vector3.up * 100)
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            else if (targetPos == Vector3.up * 100)
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);

            yield return null;
        }

        gameManager.score += (int)Mathf.Pow(2, level);

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
        EffectPlay();
        gameManager.SfxPlay(GameManager.Sfx.LevelUp);

        yield return new WaitForSeconds(0.3f);

        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag =="Finish")
        {
            deadTime += Time.deltaTime;

            if(deadTime > 2)
            {
                spriteRenderer.color = new Color(0.9f,0.5f,0.5f);
            }
            if(deadTime > 5)
            {
                spriteRenderer.color = Color.red;
                gameManager.GameOver();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag=="Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }

    void EffectPlay()
    {
        effect.transform.position = this.transform.position;
        effect.transform.localScale = this.transform.localScale * 5;
        effect.Play();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Dongle")
        {
            Dongle other = collision.gameObject.GetComponent<Dongle>();

            // �������� �߿� �� �ϰ� �Ǹ� ������ �߻��ϹǷ�
            // �������� �ʴ� ���϶��� �������Ѵ�.
            if (level == other.level && !isMerge && !other.isMerge && level < 7)
            {
                // ��ġ�� ������ �̷�������Ѵ�.
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;

                // 1. ���� �Ʒ��� ������
                // 2. ������ ����������, ���� �����ʿ� ������
                if (meY < otherY || (meY == otherY && otherX < meX))
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

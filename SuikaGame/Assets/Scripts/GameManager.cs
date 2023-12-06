using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

    public int maxLevel;

    private void Awake()
    {
        // 프레임 지정
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle()
    {
        // clone이 하나씩 생성된다.
        // 자식으로 생성된다.
        GameObject instant = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = instant.GetComponent<Dongle>();
        return instantDongle;
    }

    void NextDongle()
    {
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        lastDongle.level = Random.Range(0, maxLevel);
        // 애니메이션 재생을 위해 활성화 시켜야한다.
        lastDongle.gameObject.SetActive(true);
        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;
        }

        // 한프레임을 쉬는 코드
        // yield return null;
        yield return new WaitForSeconds(2.5f);

        NextDongle();
    }    

    public void TouchDown()
    {
        if (lastDongle == null)
            return;
        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null)
            return;
        lastDongle.Drop();
        lastDongle = null;
    }
}

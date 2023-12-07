using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int maxLevel;

    private void Awake()
    {
        // ������ ����
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle()
    {
        // ����Ʈ ����
        GameObject effectObject = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instanctEffect = effectObject.GetComponent<ParticleSystem>();

        // clone�� �ϳ��� �����ȴ�.
        // �ڽ����� �����ȴ�.
        GameObject dongleObject = Instantiate(donglePrefab, dongleGroup);
        Dongle instantDongle = dongleObject.GetComponent<Dongle>();

        instantDongle.effect = instanctEffect;

        return instantDongle;
    }

    void NextDongle()
    {
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        lastDongle.level = Random.Range(0, maxLevel);
        // �ִϸ��̼� ����� ���� Ȱ��ȭ ���Ѿ��Ѵ�.
        lastDongle.gameObject.SetActive(true);
        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while(lastDongle != null)
        {
            yield return null;
        }

        // ���������� ���� �ڵ�
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

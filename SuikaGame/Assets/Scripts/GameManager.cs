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

    public AudioSource bgmPlayer;
    // �ν�����â���� ���� �Ҵ��� �� �־ ũ�⸦ �������� �ʴ´�.
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx
    {
        Attack, LevelUp, Next, Button, GameOver
    };

    int sfxCursor;

    public int score;
    public int maxLevel;
    public bool isOver;

    private void Awake()
    {
        // ������ ����
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        bgmPlayer.Play();
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
        if (isOver)
            return;

        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.gameManager = this;
        lastDongle.level = Random.Range(0, maxLevel);
        // �ִϸ��̼� ����� ���� Ȱ��ȭ ���Ѿ��Ѵ�.
        lastDongle.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (lastDongle != null)
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

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            case Sfx.Attack:
                sfxPlayer[sfxCursor].clip = sfxClip[0];
                break;
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(1, 4)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.GameOver:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
        }
        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }

    public void GameOver()
    {
        if (isOver)
            return;
        isOver = true;

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        for(int i=0; i<dongles.Length; i++)
        {
            dongles[i].rigidbody.simulated = false;
        }

        for (int i = 0; i < dongles.Length; i++)
        {
            // ���� ���ü� ���� ������ ��������
            dongles[i].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        SfxPlay(Sfx.GameOver);
    }
}

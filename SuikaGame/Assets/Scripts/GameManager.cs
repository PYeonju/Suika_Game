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
    // 인스펙터창에서 직접 할당할 수 있어서 크기를 지정하지 않는다.
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
        // 프레임 지정
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }

    Dongle GetDongle()
    {
        // 이펙트 생성
        GameObject effectObject = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instanctEffect = effectObject.GetComponent<ParticleSystem>();

        // clone이 하나씩 생성된다.
        // 자식으로 생성된다.
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
        // 애니메이션 재생을 위해 활성화 시켜야한다.
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
            // 절대 나올수 없는 값으로 보내버림
            dongles[i].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        SfxPlay(Sfx.GameOver);
    }
}

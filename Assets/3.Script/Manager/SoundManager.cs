using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region 싱글톤
    public static SoundManager instance = null;

    private void Awake()
    {

        if (instance == null) //instance가 null. 즉, 시스템상에 존재하고 있지 않을때
        {
            instance = this; //내자신을 instance로 넣어줍니다.
            DontDestroyOnLoad(gameObject); //OnLoad(씬이 로드 되었을때) 자신을 파괴하지 않고 유지
        }
        else
        {
            if (instance != this) //instance가 내가 아니라면 이미 instance가 하나 존재하고 있다는 의미
                Destroy(this.gameObject); //둘 이상 존재하면 안되는 객체이니 방금 AWake된 자신을 삭제
        }
    }
    #endregion

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }


    [SerializeField] Sound[] sfx = null;
    [SerializeField] Sound[] bgm = null;
    [SerializeField] Sound[] effect = null;

    [SerializeField] AudioSource bgmPlayer = null;
    [SerializeField] AudioSource sfxPlayer = null;
    [SerializeField] AudioSource effectPlayer = null;

    private void Start()
    {
        PlayBGM("City");
    }

    public void PlayBGM(string p_bgmName)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (p_bgmName == bgm[i].name)
            {
                bgmPlayer.clip = bgm[i].clip;
                bgmPlayer.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
    }

    public void PlaySFX(string p_sfxName)
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (p_sfxName.Contains(sfx[i].name))
            {
                sfxPlayer.clip = sfx[i].clip;
                sfxPlayer.Play();
            }
        }
    }

    public void PlayEffect(string p_sfxName)
    {
        for (int i = 0; i < effect.Length; i++)
        {
            if (p_sfxName == effect[i].name)
            {
                effectPlayer.clip = effect[i].clip;
                effectPlayer.Play();
            }
        }
    }
}

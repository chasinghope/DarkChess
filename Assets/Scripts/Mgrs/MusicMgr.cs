using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region 说明

/// <summary>
/// 音乐管理类
/// BGM
/// Sound           -->   通过对象池实现                      调用  SetRetainNum(int num)     可以设置AudioSource 保留数
/// </summary>
#endregion
public class MusicMgr : InstanceNull<MusicMgr>
{
    private string BGMpath = "Music/BGM/";                   // BGM存放目录                             // Resources 目录下
    private string Soundpath = "Music/Sound/";             // 音效存放目录
    private float bgmVolume =1;
    private float soundVolume = 1;

    private AudioSource bgm;
    public AudioSource BGM
    {
        get
        {
            if( bgm == null)
            {
                GameObject obj = new GameObject("BGM");
                bgm = obj.AddComponent<AudioSource>();
                GameObject.DontDestroyOnLoad(obj);
            }
            return bgm;
        }
    }

    private GameObject soundObj;                              // 音效依赖对象
    public GameObject SoundObj
    {
        get
        {
            if( soundObj == null)
            {
                soundObj = new GameObject("Sound");
                GameObject.DontDestroyOnLoad(soundObj);
            }
            return soundObj;
        }
    }

    private List<AudioSource> soundList = new List<AudioSource>();
    private int retainNum = 30;
    private int sourceNum => soundList.Count;

    public MusicMgr()
    {
        MonoManager.Instance.AddUpdateListener(MusicUpdate);
    }


    /// <summary>
    /// 清除无用的AudioSource
    /// </summary>
    private void MusicUpdate()
    {
        for (int i = 0 ; i < soundList.Count; i++)
        {
            if( retainNum >= sourceNum)
            {
                break;
            }
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    #region BGM

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">音乐文件名</param>
    public void PlayBGM(string name)
    {
        // 异步加载背景音乐并播放
        ResMgr.Instance.LoadAsync<AudioClip>(BGMpath + name, (clip) => {
            BGM.clip = clip;
            BGM.volume = bgmVolume;
            BGM.loop = true;
            BGM.Play();

        });
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBGM()
    {
        if (bgm != null && BGM.isPlaying)
        {
            BGM.Pause();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        if (bgm != null && BGM.isPlaying)
        {
            
            BGM.Stop();
        }       
    }

    /// <summary>
    /// 改变BGM的音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void ChangeBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgm == null)
        {
            return;
        }

        BGM.volume = bgmVolume;
    }

    #endregion


    #region Sound
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名</param>
    public void PlaySound(string name, bool isLoop = false, UnityAction<AudioSource> callback = null)
    {
        
        // 当音效资源异步加载结束后， 再添加一个音效
        ResMgr.Instance.LoadAsync<AudioClip>( Soundpath + name, (clip) => 
        {
            // AudioSource source = SoundObj.AddComponent<AudioSource>();
            AudioSource source = GetFreeAudioSource();
            source.clip = clip;
            source.volume = soundVolume;
            source.loop = isLoop;
            source.Play();
            // soundList.Add(source);
            if (callback != null)
                callback(source);

        });
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="source">要停止的音效源</param>
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }

    /// <summary>
    /// 改变所有音效的音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (var source in soundList)
        {
            source.volume = soundVolume;
        }
    }

    /// <summary>
    /// 设置保留的 音效的 AudioSource 数量
    /// </summary>
    /// <param name="num"> AudioSource 数量</param>
    public void SetRetainNum(int num)
    {
        retainNum = num;
    }


    /// <summary>
    /// 获取AudioSource
    /// </summary>
    /// <returns></returns>
    private AudioSource GetFreeAudioSource()
    {
        AudioSource source = null;
        bool haveFree = false;

        for (int i = 0; i < soundList.Count; i++)
        {
            if(!soundList[i].isPlaying)
            {
                source = soundList[i];
                haveFree = true;
            }
        }

        if (!haveFree)
        {
            source = SoundObj.AddComponent<AudioSource>();
            soundList.Add(source);
        }

        return source;
    }

    public float GetBGMVolume()
    {
        return bgmVolume;
    }

    public float GetSoundVolume()
    {
        return soundVolume;
    }

    #endregion

}

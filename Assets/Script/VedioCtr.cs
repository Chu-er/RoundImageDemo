using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;
using UnityEngine.Events;

public class VedioCtr : MonoBehaviour
{

    public string _floder = "AVProVideoSamples/";

    public string[] fileName = new string[]
    {
        "v1.mp4","v2.mp4"
    };
    /// <summary>
    /// 加载方式
    /// </summary>
    private MediaPlayer.FileLocation _location = MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
    public MediaPlayer.FileLocation Location { get => _location; set => _location = value; }

    public MediaPlayer _medioPlayer;
    public DisplayUGUI _displayUGUI;

    #region Event  有需要的话自定义实现 
    public UnityAction<MediaPlayer> onReadyPlay;
    //TODO
    
    #endregion


    /// <summary>
    /// 注册视频生命周期的事件
    /// </summary>
    void Start()
    {
        _medioPlayer.Events.AddListener(AddEvents);
    }
    /// <summary>
    /// 按钮事件
    /// </summary>
    /// <param name="index"></param>
    public void PlayBtnClick(int index)
    {
        _medioPlayer.m_VideoPath = System.IO.Path.Combine(_floder, fileName[index]);
        PlayVedio();
    }
    /// <summary>
    /// 播放视频调用组件API
    /// </summary>
    public void PlayVedio()
    {

        if (string.IsNullOrEmpty(_medioPlayer.m_VideoPath))
        {
            CloseVedio();
            return;
        }
        _medioPlayer.OpenVideoFromFile(_location, _medioPlayer.m_VideoPath, true);
    }

    /// <summary>
    /// 重播
    /// </summary>
    public void Rewind()
    {
        if ( _medioPlayer.Equals(null))
        {
            return;
        }
        _medioPlayer.Control.Rewind();
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Pause()
    {
        if (_medioPlayer.Equals(null))
        {
            return;
        }
        _medioPlayer.Control.Pause();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void CloseVedio() {
        if (_medioPlayer.Equals(null))
        {
            return;
        }
        _medioPlayer.CloseVideo();
    }
    /// <summary>
    /// 修改进度
    /// </summary>
    /// <param name="value"></param>
    public void Seek(float value)
    {
        if (_medioPlayer.Equals(null))
        {
            return;
        }
        _medioPlayer.Control.Seek(value);

    }

    public void AddEvents(MediaPlayer mp,MediaPlayerEvent.EventType eventType,ErrorCode error)
    {
        Debug.Log("当前事件类型"+eventType);
        switch (eventType)
        {

            case MediaPlayerEvent.EventType.ReadyToPlay:
                //TODO
                onReadyPlay?.Invoke(mp);
                break;

            case MediaPlayerEvent.EventType.Started:
                //TODO

                break;

            case MediaPlayerEvent.EventType.FirstFrameReady:

                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                //TODO

                break;
            default:
                break;
        }
    }




}

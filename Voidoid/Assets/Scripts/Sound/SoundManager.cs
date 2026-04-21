using System;
using UnityEngine;

namespace Voidoid.Sound
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    [SerializeField] private float bpm = 150f;
    [SerializeField] private int beatsPerBar = 4;
    [SerializeField] private double startLeadTime = 0.1;

    public event Action<int> OnBeat;
    public event Action<int> OnBar;
    public event Action<int> OnSongLoop;

    private double _startDspTime;
    private double _beatDuration;
    private double _barDuration;
    private double _songDuration;

    private int _lastBeatIndex = -1;
    private int _lastBarIndex = -1;
    private int _lastSongLoopIndex = -1;

    private bool _started;
    private double _pastDspTime;
    private double _elapsedTime;
    private double _deltaTime;
    public double CurrentSongTime => _started ? _elapsedTime % _songDuration : 0.0;
    public double CurrentBarTime => _started ? _elapsedTime % _barDuration : 0.0;
    public float pitch = 1;
    public double DeltaTime => _deltaTime;

    public void StartMusic()
    {
        if (clip == null || audioSource == null)
        {
            Debug.LogError("AudioSource or AudioClip is missing.");
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = true;

        _beatDuration = 60.0 / bpm;
        _barDuration = _beatDuration * beatsPerBar;
        _songDuration = clip.length;

        _startDspTime = AudioSettings.dspTime + startLeadTime;
        _pastDspTime = _startDspTime;
        _elapsedTime = 0;

        _lastBeatIndex = -1;
        _lastBarIndex = -1;
        _lastSongLoopIndex = -1;

        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.PlayScheduled(_startDspTime);

        _started = true;
    }

    private void Start()
    {
        StartMusic();
    }

    private void Update()
    {
        if (!_started) return;

        _deltaTime = (AudioSettings.dspTime - _pastDspTime) * pitch;
        _elapsedTime += _deltaTime;
        _pastDspTime = AudioSettings.dspTime;
        // _elapsedTime = AudioSettings.dspTime - _startDspTime;
        this.audioSource.pitch = pitch; // ピッチ変更追加
        if (_elapsedTime < 0.0) return;


        int beatIndex = Mathf.FloorToInt((float)(_elapsedTime / _beatDuration));
        int barIndex = Mathf.FloorToInt((float)(_elapsedTime / _barDuration));
        int songLoopIndex = Mathf.FloorToInt((float)(_elapsedTime / _songDuration));

        if (beatIndex != _lastBeatIndex)
        {
            _lastBeatIndex = beatIndex;
            OnBeat?.Invoke(beatIndex);
        }

        if (barIndex != _lastBarIndex)
        {
            _lastBarIndex = barIndex;
            OnBar?.Invoke(barIndex);
        }

        if (songLoopIndex != _lastSongLoopIndex)
        {
            _lastSongLoopIndex = songLoopIndex;
            OnSongLoop?.Invoke(songLoopIndex);
        }
    }
    }
}
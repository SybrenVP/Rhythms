using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShowWaveformInGame : MonoBehaviour
{
    public AudioClip AudioClip = null;

    public int Width = 500;
    public int Height = 200;
    public Color Color = Color.white;

    private AudioClip _prevClip = null;
    private SpriteRenderer _spriteRenderer = null;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateTexture();
    }

    private void Update()
    {
        if (_prevClip != AudioClip)
        {
            UpdateTexture();
            _prevClip = AudioClip;
        }
    }

    private void UpdateTexture()
    {
        Texture2D tex = SequenceUtilities.GetWaveformTextureFromAudioClip(AudioClip, 1f, Width, Height, Color);
        _spriteRenderer.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}

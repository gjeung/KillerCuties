using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource buttonSounds;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void HoverSound()
    {
        buttonSounds.PlayOneShot(hoverSound);
    }

    public void ClickSound()
    {
        buttonSounds.PlayOneShot(clickSound);
    }
}

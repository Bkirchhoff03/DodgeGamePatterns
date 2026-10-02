using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SceneMusic : MonoBehaviour
{
    public AudioClip music;

    private void Start()
    {
        if (music != null && AudioManager.instance() != null)
        {
            AudioManager.instance().PlayMusic(music);
        }
    }
}


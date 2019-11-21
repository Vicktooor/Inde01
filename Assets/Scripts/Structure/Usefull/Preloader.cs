using UnityEngine;
using System.Collections;

public class Preloader : MonoSingleton<Preloader>
{
    override protected void Awake()
    {
        base.Awake();
        Preload.Preloading();
    }
}

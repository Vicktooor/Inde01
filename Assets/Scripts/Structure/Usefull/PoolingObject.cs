using UnityEngine;
using System.Collections;

public class PoolingObject : MonoBehaviour
{
    private bool _usable = true;
    public bool Usable {
        get { return _usable; }
        set { _usable = value; }
    }

    public virtual void Reset()
    {
        PoolingManager.Instance.Reset(this);
    }
}

using UnityEngine;
using System.Collections;

public class RootsUIElement : MonoBehaviour
{
    public bool invert = false;

    virtual public void Hide()
    {
        gameObject.SetActive(invert ? true : false);
    }

    virtual public void Show()
    {
        gameObject.SetActive(invert ? false : true);
    }
}

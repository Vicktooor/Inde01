using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class RosaceDrawer
{
    public List<float> angularSpeeds;
    public Vector3 position = Vector3.zero;
    public bool active = false;

    private List<Vector3> vectors = new List<Vector3>();
    private List<Vector3> activeVectors = new List<Vector3>();
    private float launchTime = 5f;
    private float nTime = 0f;
    public float T { get { return nTime; } }

    public void CustomUpdate(float deltaTime)
    {
        if (active)
        {
            if (nTime == 0f) FillVectors();
            nTime = Mathf.Clamp01(nTime + (deltaTime * (1f / launchTime)));
            position = GetRosacePosition(deltaTime);
        }
        else
        {
            if (nTime == 0f) return;
            nTime = Mathf.Clamp01(nTime - (deltaTime * (1f / launchTime)));
            position = GetRosacePosition(deltaTime);
        }
    }

    private Vector3 GetRosacePosition(float deltaTime)
    {
        for (int i = 0; i < angularSpeeds.Count; i++)
        {
            Vector3 nVector;
            nVector = MathCustom.RotateDirectionAround(vectors[i], deltaTime * angularSpeeds[i], Vector3.forward);
            nVector = nVector.normalized * nTime * 2f;
            vectors[i] = nVector;
            nVector.z = 0f;
            activeVectors.Add(nVector);
        }
        Vector3 pos = MathCustom.GetBarycenter(activeVectors.ToArray());
        activeVectors.Clear();
        return pos;
    }

    private void FillVectors()
    {
        vectors.Clear();
        foreach (float f in angularSpeeds) vectors.Add(new Vector3(1f, 1f, 0f));
    }
}

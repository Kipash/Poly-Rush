using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public Color Color
    {
        get
        {
            return renderers[0].material.color;
        }
        set
        {
            foreach (var x in renderers)
            {
                x.material.color = value;
            }
        }
    }
    public Transform Transform;
    public bool Deadly { get; set; }
    [SerializeField] Renderer[] renderers;
}
   
public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;

    [SerializeField] Color captured;
    [SerializeField] Color deadly;
    [SerializeField] Color @default;
    [SerializeField] float offSet;
    [SerializeField] float lerpSpeed;
    [SerializeField] float lerpColorSpeed;
    [SerializeField] float lerpScaleSpeed;
    [SerializeField] float lerpMinDiff;

    public bool IsReady { get; private set; }

    [SerializeField] Node origin;
    [SerializeField] Node left;
    [SerializeField] Node right;
    [SerializeField] Transform[] hazardTransforms;

    public bool IsDead { get; set; }

    private void Start()
    {
        origin.Transform.position = Vector3.zero;
        StartCoroutine(Spawn());
    }
    IEnumerator Spawn()
    {
        left.Deadly = false;
        right.Deadly = false;
        var i = UnityEngine.Random.Range(0, 3);
        switch(i)
        {
            case 0:
                left.Color = deadly;
                right.Color = @default;
                left.Deadly = true;
                break;
            case 1:
                left.Color = @default;
                right.Color = deadly;
                right.Deadly = true;
                break;
            case 2:
                left.Color = @default;
                right.Color = @default;
                break;
        }
        left.Transform.position = new Vector3(-offSet, 30);
        right.Transform.position = new Vector3(offSet, 30);
        while (true)
        {
            left.Transform.position = Vector3.Lerp(left.Transform.position, new Vector3(-offSet, offSet), Time.deltaTime * lerpSpeed);
            right.Transform.position = Vector3.Lerp(right.Transform.position, new Vector3(offSet, offSet), Time.deltaTime * lerpSpeed);

            if(Vector3.Distance(left.Transform.position, new Vector3(-offSet, offSet)) < lerpMinDiff)
                break;

            yield return null;
        }
        IsReady = true;
    }

    public bool MoveLeft()
    {
        if (left.Deadly)
            return false;

        IsReady = false;
        var o = origin;
        origin = left;
        left = o;
        StartCoroutine(Move());
        return true;
    }
    public bool MoveRight()
    {
        if (right.Deadly)
            return false;

        IsReady = false;
        var o = origin;
        origin = right;
        right = o;
        StartCoroutine(Move());

        return true;
    }
    IEnumerator Move()
    {
        //origin.Color = captured;
        while (true)
        {
            origin.Transform.position = Vector3.Lerp(origin.Transform.position, Vector3.zero, Time.deltaTime * lerpSpeed);
            right.Transform.position = Vector3.Lerp(right.Transform.position, Vector3.right * 20, Time.deltaTime * lerpSpeed);
            left.Transform.position = Vector3.Lerp(left.Transform.position, Vector3.left * 20, Time.deltaTime * lerpSpeed);

            origin.Color = Color32.Lerp(origin.Color, captured, lerpColorSpeed * Time.fixedDeltaTime);
            right.Color = Color32.Lerp(right.Color, @default, lerpColorSpeed * Time.fixedDeltaTime);
            left.Color = Color32.Lerp(left.Color, @default, lerpColorSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(origin.Transform.position, Vector3.zero) < lerpMinDiff)
            {
                StartCoroutine(Spawn());
                break;
            }
            yield return null;
        }
    }

    public void SetHazardGFX(int max, int value)
    {
        if (max == 0)
            max = 1;
        if (value == 0)
            return;

        float s = value / (max / 100f) / 100;
        Vector3 v3 = Vector3.Lerp(hazardTransforms[0].localScale, new Vector3(s, s, s), lerpScaleSpeed * Time.deltaTime);
        foreach(var x in hazardTransforms)
        {
            x.localScale = v3;
        }
    }

    public void Reset()
    {
        IsDead = false;
        IsReady = true;
        Time.timeScale = 1;
    }
}

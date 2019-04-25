using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex
{
    public readonly HexMap HexMap;
    public readonly int Q;
    public readonly int R;
    public readonly int S;
    public readonly float movementDebuff = 1;

    readonly float radius = 1f;



    public Hex (HexMap hexMap, int q, int r)
    {
        this.HexMap = hexMap;

        this.Q = q;
        this.R = r;
        this.S = -(q+r);

    }

    public Vector3 Position()
    {

        return new Vector3(
            HexHorizontalSpacing() * (this.Q + this.R/2f),
            0,
            HexVerticalSpacing() * this.R
            );

    }

    public float HexHeight()
    {
        return radius * 2;
    }
    public float HexWidth()
    {
        return 0.866025404f * HexHeight();
    }

    public float HexHorizontalSpacing()
    {
        return HexWidth();
    }
    public float HexVerticalSpacing()
    {
        return HexHeight() * 0.75f;
    }




}


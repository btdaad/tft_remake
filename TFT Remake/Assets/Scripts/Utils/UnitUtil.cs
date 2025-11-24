using UnityEngine;

public enum UnitType
{
    Capsule, // 1-cost
    Cylinder,
    Ellipsoid,
    Rectangular,
    Trigonal,
    Cone, // 3-cost
    Hexagonal,
    Octahedron,
    Pyramid,
    Icosahedron, // 5-cost
    Tetrahedron,
    Torus,
    TargetDummy // no cost
};
    
public enum Star
{
    OneStar = 0,
    TwoStar = 1,
    ThreeStar = 2,
    None = -1
};

public static class UnitUtil
{
}

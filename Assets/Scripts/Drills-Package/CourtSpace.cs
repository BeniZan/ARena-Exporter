using UnityEngine;

/// <summary>
/// Single definition of how authored 2D court coordinates map into drill-local 3D space.
/// Field-standard coordinates are (length, width) in meters, so world Z runs along the
/// length of the court and world X across its width. Every authored position in a drill
/// (characters, triggers, the local player start) must go through here, otherwise the
/// same pair of numbers lands in two different places.
/// </summary>
public static class CourtSpace {
    public static readonly Vector2 FieldStandardSize = new Vector2(28f, 15f);

    public static Vector3 ToLocal(in Vector2 fieldStandard)
        => new Vector3(fieldStandard.y, 0f, fieldStandard.x);

    public static Vector2 ToField(in Vector3 local)
        => new Vector2(local.z, local.x);
}

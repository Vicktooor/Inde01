using UnityEngine;

// AUTHOR - Victor

[System.Serializable]
public struct Matrix3x3
{
    public float m00;
    public float m01;
    public float m02;

    public float m10;
    public float m11;
    public float m12;

    public float m20;
    public float m21;
    public float m22;
}

/// <summary>
/// Need to be initialise in degree
/// </summary>
public struct CoupleAngleValue
{
    public float positiveAngle;
    public float negativeAngle;   

    public CoupleAngleValue(float cAngle)
    {
        positiveAngle = cAngle;
        negativeAngle = 360 - cAngle;
    }

    public void ToRadian()
    {
        positiveAngle = positiveAngle * Mathf.Deg2Rad;
        negativeAngle = negativeAngle * Mathf.Deg2Rad;   
    }

    public void ToDegree()
    {
        positiveAngle = negativeAngle * Mathf.Rad2Deg;
        negativeAngle = negativeAngle * Mathf.Rad2Deg;
    }
}

[System.Serializable]
public struct IntVector2
{
    public static IntVector2 Zero = new IntVector2(0, 0);
    public static IntVector2 One = new IntVector2(1, 1);
    public static IntVector2 Right = new IntVector2(1, 0);
    public static IntVector2 Top = new IntVector2(0, 1);

    public int x;
    public int y;

    public IntVector2(int px, int py)
    {
        x = px;
        y = py;
    }

    public bool Equals(IntVector2 testPos)
    {
        if (testPos.x == x && testPos.y == y) return true;
        return false;
    }

    public static float Distance(IntVector2 v1, IntVector2 v2)
    {
        Vector2 fv1 = new Vector2(v1.x, v1.y);
        Vector2 fv2 = new Vector2(v2.x, v2.y);
        return Vector2.Distance(fv1, fv2);
    }

    public static IntVector2 operator +(IntVector2 c1, IntVector2 c2)
    {
        return new IntVector2(c1.x + c2.x, c1.y + c2.y);
    }

    public static IntVector2 operator -(IntVector2 c1, IntVector2 c2)
    {
        return new IntVector2(c1.x - c2.x, c1.y - c2.y);
    }

    public static IntVector2 operator -(IntVector2 c)
    {
        return new IntVector2(-c.x, -c.y);
    }

    public static Vector2 operator *(IntVector2 c1, Vector2 c2)
    {
        return new Vector2(c1.x * c2.x, c1.y * c2.y);
    }

    public static IntVector2 operator *(IntVector2 c, int f)
    {
        return new IntVector2(c.x * f, c.y * f);
    }

    public static IntVector2 operator *(int f, IntVector2 c)
    {
        return new IntVector2(c.x * f, c.y * f);
    }

    public static Vector2 operator /(IntVector2 c1, float d)
    {
        return new Vector2(c1.x / d, c1.y / d);
    }

    public static bool operator ==(IntVector2 c1, IntVector2 c2)
    {
        return c1.x == c2.x && c1.y == c2.y;
    }

    public static bool operator !=(IntVector2 c1, IntVector2 c2)
    {
        return c1.x != c2.x || c1.y != c2.y;
    }
}

[System.Serializable]
public struct IntVector3
{
    public static IntVector3 Zero = new IntVector3(0, 0, 0);
    public static IntVector3 One = new IntVector3(1, 1, 1);

    public int x;
    public int y;
    public int z;

    public IntVector3(int px, int py, int pz)
    {
        x = px;
        y = py;
        z = pz;
    }

    public bool Equals(IntVector3 testPos)
    {
        if (testPos.x == x && testPos.y == y) return true;
        return false;
    }

    public static float Distance(IntVector3 v1, IntVector3 v2)
    {
        Vector3 fv1 = new Vector3(v1.x, v1.y, v1.z);
        Vector3 fv2 = new Vector3(v2.x, v2.y, v2.z);
        return Vector3.Distance(fv1, fv2);
    }

    public static IntVector3 operator +(IntVector3 c1, IntVector3 c2)
    {
        return new IntVector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
    }

    public static IntVector3 operator -(IntVector3 c1, IntVector3 c2)
    {
        return new IntVector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
    }
}

public class MathCustom
{
    public static Vector2 Get2DLineComponents(Vector2 p1, Vector2 p2)
    {
        float dx = p1.x - p2.x;
        if (dx == 0) dx = float.NaN;

        float a = (p2.y - p1.y) / dx;
        float b = p1.y - (a * p1.x);

        return new Vector2(a, b);
    }

    /// <summary>
    /// Calcul the a,b,c & d values of the plan equation
    /// </summary>
    public static Vector4 GetPlaneValues(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 AB = B - A;
        Vector3 AC = C - A;

        float x = (AB.y * AC.z) - (AC.y * AB.z);
        float y = (AB.z * AC.x) - (AC.z * AB.x);
        float z = (AB.x * AC.y) - (AC.x * AB.y);
        Vector3 n = new Vector3(x, y, z);

        float d = (A.x * n.x) + (A.y * n.y) + (A.z * n.z);

        return new Vector4(x, y, z, d);
    }

	/// <summary>
	/// Calcul the position of the intersection between plan & vector
	/// </summary>
	/// <param name="A">Vector origin</param>
	/// <param name="B">Vector direction</param>
	/// <param name="planValues">Values a,b,c & d of plan equation</param>
	public static Vector3 LineCutPlaneCoordinates(Vector3 A, Vector3 B, Vector4 planValues, bool asSegment = false)
    {
        Vector3 AB = B - A;

        float numerator = (planValues.x * A.x) + (planValues.y * A.y) + (planValues.z * A.z) - planValues.w;
        float denominator = - (planValues.x * AB.x) - (planValues.y * AB.y) - (planValues.z * AB.z);
        float t = numerator / denominator;

        Vector3 crossPoint = new Vector3(A.x + (AB.x * t), A.y + (AB.y * t), A.z + (AB.z * t));

        if (asSegment)
        {
            Vector3 mid = (A + B) / 2f;
            if (Vector3.Distance(mid, crossPoint) > Vector3.Distance(A, B) / 2f) return new Vector3(float.NaN, float.NaN, float.NaN);
        }

        return crossPoint;
    }

    /// <summary>
    /// Calcul the distance between point & plan
    /// </summary>
    /// <param name="getAbs">Want you to get absolute value ?</param>
    /// <returns></returns>
    public static float GetDistanceToPlane(Vector3 point, Vector4 planValues, bool getAbs = false)
    {
        float numerator;
        float denominator;

        if (!getAbs) numerator = (planValues.x * point.x) + (planValues.y * point.y) + (planValues.z * point.z) - planValues.w;
        else numerator = Mathf.Abs((planValues.x * point.x) + (planValues.y * point.y) + (planValues.z * point.z) - planValues.w);
        denominator = Mathf.Sqrt(Mathf.Pow(planValues.x, 2) + Mathf.Pow(planValues.y, 2) + Mathf.Pow(planValues.z, 2));

        return numerator / denominator;
    }

    public static Vector3 RotateDirectionAround(Vector3 origin, float theta, Vector3 axis)
    {
        float c = Mathf.Cos(theta);
        float s = Mathf.Sin(theta);

        Matrix3x3 transformMatrix = new Matrix3x3();

        transformMatrix.m00 = Mathf.Pow(axis.x, 2) + (1 - Mathf.Pow(axis.x, 2)) * c;
        transformMatrix.m01 = (axis.x * axis.y * (1 - c)) - (axis.z * s);
        transformMatrix.m02 = (axis.x * axis.z * (1 - c)) + (axis.y * s);

        transformMatrix.m10 = (axis.x * axis.y * (1 - c)) + (axis.z * s);
        transformMatrix.m11 = Mathf.Pow(axis.y, 2) + (1 - Mathf.Pow(axis.y, 2)) * c;
        transformMatrix.m12 = (axis.y * axis.z * (1 - c)) - (axis.x * s);

        transformMatrix.m20 = (axis.x * axis.z * (1 - c)) - (axis.y * s);
        transformMatrix.m21 = (axis.y * axis.z * (1 - c)) + (axis.x * s);
        transformMatrix.m22 = Mathf.Pow(axis.z, 2) + (1 - Mathf.Pow(axis.z, 2)) * c;

        return Vector3TransformFromMatrix(origin, transformMatrix);
    }

    private static Vector3 Vector3TransformFromMatrix(Vector3 v, Matrix3x3 m)
    {
        Vector3 result = Vector3.zero;

        result.x = (v.x * m.m00) + (v.y * m.m10) + (v.z * m.m20);
        result.y = (v.x * m.m01) + (v.y * m.m11) + (v.z * m.m21);
        result.z = (v.x * m.m02) + (v.y * m.m12) + (v.z * m.m22);

        return result;
    }

    public static Vector3 GetBarycenter(Vector3[] pointsCloud)
	{
		float numX = 0;
		float numY = 0;
		float numZ = 0;
		float den = 0;

		foreach (Vector3 V in pointsCloud)
		{
			numX += V.x * V.magnitude;
			numY += V.y * V.magnitude;
			numZ += V.z * V.magnitude;
			den += V.magnitude;
		}

        Vector3 barycenter = new Vector3(numX / den, numY / den, numZ / den);
        if (float.IsNaN(barycenter.x)) barycenter.x = 0f;
        if (float.IsNaN(barycenter.y)) barycenter.y = 0f;
        if (float.IsNaN(barycenter.z)) barycenter.z = 0f;
        return barycenter;
    }

	public static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
	{
		float a = radius * Mathf.Cos(elevation);
		outCart.x = a * Mathf.Cos(polar);
		outCart.y = radius * Mathf.Sin(elevation);
		outCart.z = a * Mathf.Sin(polar);
	}

	public static void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
	{
		if (cartCoords.x == 0) cartCoords.x = Mathf.Epsilon;
		outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x) + (cartCoords.y * cartCoords.y) + (cartCoords.z * cartCoords.z));
		outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
		if (cartCoords.x < 0) outPolar += Mathf.PI;
		outElevation = Mathf.Asin(cartCoords.y / outRadius);
	}

	/// <summary>
	/// Calcul normal vector of 3 points forming face
	/// </summary>
	public static Vector3 GetFaceNormalVector(Vector3 A, Vector3 B, Vector3 C)
	{
		Vector3 AB = A + B;
		Vector3 AC = A + C;

		return Vector3.Cross(AB, AC);
	}

	public static bool RandomBool() {
        return Random.value > 0.5;
    }

    /// <summary>
    /// Boucing lerp with elasticity
    /// </summary>
    /// <param name="bounce">Elasticity</param>
    /// <returns></returns>
    public static float Berp(float start, float end, float bounce, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + bounce * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }


    /// <summary>
    /// Circular lerp, avoid 0°/360° lerp problem (euler angles for ex)
    /// </summary>
    /// <returns></returns>
    public static float Clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;

        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    public static Vector3 LerpUnClampVector(Vector3 A, Vector3 B, float t)
    {
        return A + (B - A) * t;
    }

    /// <summary>
    /// Normalize val from [inStart, inEnd] to [outStart, outEnd]
    /// </summary>
    /// <returns></returns>
    public static float NormalizeRange(float val, float inStart, float inEnd, float outStart, float outEnd)
    {
        float res = val - inStart;
        res /= (inEnd - inStart);
        res *= (outEnd - outStart);
        return res - outStart;
    }
}
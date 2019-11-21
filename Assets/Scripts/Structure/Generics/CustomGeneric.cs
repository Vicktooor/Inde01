using System;
using System.Collections.Generic;
using UnityEngine;

public static class CustomGeneric
{
    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)) as IEnumerable<T>;
    }

    public static T CastEnum<T>(string value) where T : struct, IConvertible
    {
        T enumCast = new T();
        try
        {
            enumCast = (T)Enum.Parse(typeof(T), value);
            if (Enum.IsDefined(typeof(T), value)) return enumCast;
            else return enumCast;
        }
        catch (ArgumentException)
        {
            Console.WriteLine(string.Format("{0} is not existing in enum {1}", value, typeof(T)));
            return enumCast;
        }
    }

    public static object GetPropertyValue(object src, string propName)
    {
        return src.GetType().GetField(propName).GetValue(src);
    }

    public static bool HasProperty<T>(string propertyName)
    {
        return typeof(T).GetField(propertyName) != null;
    }

    public static float[] ConvertByteToFloat16(byte[] array)
    {
        float[] floatArr = new float[array.Length / 2];
        for (int i = 0; i < floatArr.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(array, i * 2, 2);
            }
            floatArr[i] = (float)(BitConverter.ToInt16(array, i * 2) / 32767f);
        }
        return floatArr;
    }
}

public static class ArrayListExtensions
{
    public static void CheckIt<T>(this List<T> list)
    {
        T swapObj;
        int iteCheck = Mathf.RoundToInt(list.Count / 2f);
        int lLength = list.Count;
        for (int i = 0; i < iteCheck; i++)
        {
            int rIndex1 = UnityEngine.Random.Range(0, lLength);
            int rIndex2 = UnityEngine.Random.Range(0, lLength);
            if (rIndex1 == rIndex2) i--;
            else
            {
                swapObj = list[rIndex1];
                list[rIndex1] = list[rIndex2];
                list[rIndex2] = swapObj;
            }
        }
    }

    public static void Sum<T>(this List<T> originalArray, List<T> l2)
    {
        foreach (T e in l2) originalArray.Add(e);
    }

    public static List<T> ToList<T>(this T[] originalArray)
    {
        List<T> outList = new List<T>();
        int l = originalArray.Length;
        for (int i = 0; i < l; i++) outList.Add(originalArray[i]);
        return outList;
    }

    public static void Fill<T>(this T[] originalArray, T with)
    {
        int l = originalArray.Length;
        for (int i = 0; i < l; i++)
        {
            originalArray[i] = with;
        }
    }

    public static void Fill<T>(this T[] originalArray, T with, int index)
    {
        int l = originalArray.Length;
        if (index >= originalArray.Length) return;
        else originalArray[index] = with;
    }

    public static int IndexOf<T>(this T[] originalArray, T test)
    {
        int l = originalArray.Length;
        for (int i = 0; i < l; i++)
        {
            if (originalArray[i].Equals(test)) return i;
        }
        return -1;
    }

    public static bool Contain<T>(this T[] originalArray, T test)
    {
        int l = originalArray.Length;
        for (int i = 0; i < l; i++)
        {
            if (originalArray[i].Equals(test)) return true;
        }
        return false;
    }

    public static T Extract<T>(this T[] originalArray, int index)
    {
        T[] newArray = new T[originalArray.Length - 1];
        int l = originalArray.Length - 1;
        int insertIndex = 0;
        for (int i = 0; i <= l; i++)
        {
            if (index > i || index < i)
            {
                newArray[insertIndex] = originalArray[i];
                insertIndex++;
            }
        }
        T e = originalArray[index <= l ? index : l];
        originalArray = newArray;
        return e;
    }

    public static void Fill<T>(this T[,] originalArray, T with)
    {
        int l = originalArray.Length;
        for (int i = 0; i < l; i++)
        {
            for (int j = 0; j < l; j++)
            {
                originalArray[i, j] = with;
            }
        }
    }
}

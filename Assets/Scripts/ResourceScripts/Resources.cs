using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the information of a set of resources.
/// </summary>
[System.Serializable]
public struct Resources {

    [SerializeField]
    private int[] res; //0: gold, 1: wood, 2: iron, 3: food.

    public Resources(params int[] r) {
        
        res = new int[System.Enum.GetValues(typeof(IResources)).Length];
        for (int i = 0; i < res.Length; i++) {
            res[i] = r[i];
        }

    }

    /// <value>The amount of gold</value>
    public int gold {
        get { return res[(int)IResources.GOLD]; }
        set { res[(int)IResources.GOLD] = value; }
    }

    /// <value>The amount of wood</value>
    public int wood {
        get { return res[(int)IResources.WOOD]; }
        set { res[(int)IResources.WOOD] = value; }
    }

    /// <value>The amount of iron</value>
    public int iron {
        get { return res[(int)IResources.IRON]; }
        set { res[(int)IResources.IRON] = value; }
    }

    /// <value>The amount of food</value>
    public int food {
        get { return res[(int)IResources.FOOD]; }
        set { res[(int)IResources.FOOD] = value; }
    }

    /// <value>How many diferent resources there are.</value>
    public int length {
        get { return res.Length; }
    }

    public int this[int index] {
        get { return res[index]; }
        set { res[index] = value; }
    }

    public int Get(int n) {
        return res[n];
    }
    public void Set(int i, int n) {
        res[i] = n;
    }

    public static Resources Zero {
        get { return new Resources(new int[System.Enum.GetValues(typeof(IResources)).Length]); }
    }

    /// <summary>
    /// reduce the given resources in a percentage
    /// </summary>
    /// <param name="ress">The resources</param>
    /// <param name="reduction">The percentage</param>
    /// <returns>The reduced resources</returns>
    public static Resources ReducedRes(Resources ress, int reduction) {
        Resources result = new Resources(new int[ress.length]);
        for (int i = 0; i < ress.length; i++) {
            result[i] -= (ress[i] / 100) * reduction;
        }
        return result;
    }

    public static Resources Sum(params Resources[] ress) {
        Resources result = new Resources(0,0,0,0);
        foreach(Resources elem in ress) {
            for (int i = 0; i < System.Enum.GetValues(typeof(IResources)).Length; i++) {
                result[i] += elem[i];
            }
        }
        return result;
    }
    public static Resources Sub(params Resources[] ress) {
        Resources result = new Resources(0,0,0,0);
        foreach(Resources elem in ress) {
            for (int i = 0; i < System.Enum.GetValues(typeof(IResources)).Length; i++) {
                result[i] -= elem[i];
                if (result[i] < 0) {
                    result[i] = 0;
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Check if the first resources are bigger or equal in all the elements than the second
    /// </summary>
    /// <param name="res1">The supposed bigger</param>
    /// <param name="res2">The supposed smaller</param>
    /// <returns>true if the first is bigger. False otherwise</returns>
    public static bool CompareResources(Resources res1, Resources res2) {
        for (int i = 0; i < res1.length; i++) {
            if (res1[i] - res2[i] > 0) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if the first resources are bigger or equal than a percentage of the second resources
    /// </summary>
    /// <param name="res1">The first resources</param>
    /// <param name="res2">The second resources to which apply de percentage</param>
    /// <param name="percentage">the percentage to apply to the second resources</param>
    /// <returns>true if the first resources are bigger than a percentage of the second</returns>
    public static bool CompareResources(Resources res1, Resources res2, int percentage) {
        return CompareResources(res1, ReduceInPercentage(res2, percentage));
    }

    public static Resources ReduceInPercentage(Resources ress, int percentage) {
        Resources result = new Resources(new int[ress.length]);
        for (int i = 0; i < ress.length; i++) {
            result[i] = (ress[i] / 100) * percentage;
        }
        return result;
    }

    public string ToString() {
        string result = "";
        for (int i = 0; i < res.Length; i++) {
            result += res[i] + ", ";
        }
        return result;
    }

}
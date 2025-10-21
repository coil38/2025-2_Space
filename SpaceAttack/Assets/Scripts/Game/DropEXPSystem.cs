using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropEXPSystem : MonoBehaviour
{
    public static int dropExpCount = 10;   //평균 값 | 표준편차 = 10 * 0.4
    public static float stdDevRate = 0.4f; //표준편차 비울
    public static int i_dropExpCount = 10;

    public static void DropEXP()
    {
        PlayerCore.GetDarkMatter(GenerateGaussian(dropExpCount, dropExpCount * stdDevRate));
    }

    private static int GenerateGaussian(float mean, float stdDev)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;

        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) *
                            Mathf.Sin(2.0f * Mathf.PI * u2);

        return (int)(mean + stdDev * randStdNormal);
    }
}

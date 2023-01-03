using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed) {
        System.Random prng = new System.Random(seed);
        for(int i=0; i<array.Length - 1; i++) {
            int index = prng.Next(i, array.Length);
            T element = array[index];
            array[index] = array[i];
            array[i] = element;
        }
        return array;
    }

}

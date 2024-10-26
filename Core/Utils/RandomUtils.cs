/*
 * RandomUtils
 * 
 * Author: Andre Ferreira
 * 
 * Copyright (c) Uzmi Games. Licensed under the MIT License.
 *    
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/// <summary>
/// Provides utility methods for generating random values and performing random operations.
/// </summary>
public static class RandomUtils
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// Generates a random integer between the specified minimum and maximum values (inclusive).
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns>A random integer between <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static int MinMaxInt(int min, int max)
    {
        if (min >= max)
            return min;

        int range = max - min + 1;
        return _random.Next(range) + min;
    }

    /// <summary>
    /// Determines if a random event occurs based on a given percentage chance.
    /// </summary>
    /// <param name="chance">The probability percentage (0.01 to 100) that the event occurs.</param>
    /// <returns><c>true</c> if the event occurs; otherwise, <c>false</c>.</returns>
    public static bool DropChance(double chance)
    {
        if (chance < 0.01 || chance > 100)
            return false;

        double probability = chance / 100;
        double randomNum = _random.NextDouble();
        return randomNum < probability;
    }

    /// <summary>
    /// Selects a random element from an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array from which to select a random element.</param>
    /// <returns>A random element from the array, or the default value of <typeparamref name="T"/> if the array is empty.</returns>
    public static T ArrRandom<T>(T[] array)
    {
        if (array.Length == 0)
            return default(T);

        int index = _random.Next(array.Length);
        return array[index];
    }

    /// <summary>
    /// Selects a random element from a list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The list from which to select a random element.</param>
    /// <returns>A random element from the list, or the default value of <typeparamref name="T"/> if the list is empty.</returns>
    public static T ArrRandom<T>(List<T> list)
    {
        if (list.Count == 0)
            return default(T);

        int index = _random.Next(list.Count);
        return list[index];
    }
}
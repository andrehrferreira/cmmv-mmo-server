/*
 * Vector3
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
/// Represents a 3D vector with X, Y, and Z components.
/// </summary>
public struct Vector3
{
    /// <summary>
    /// Gets or sets the X component of the vector.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the Y component of the vector.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the Z component of the vector.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector3"/> struct with specified X, Y, and Z components.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Gets the magnitude (length) of the vector.
    /// </summary>
    public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

    /// <summary>
    /// Gets the normalized version of the vector (unit vector).
    /// </summary>
    public Vector3 Normalized => Magnitude > 0 ? this / Magnitude : new Vector3(0, 0, 0);

    /// <summary>
    /// Adds two vectors component-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The sum of the two vectors.</returns>
    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    /// <summary>
    /// Subtracts one vector from another component-wise.
    /// </summary>
    /// <param name="a">The vector to subtract from.</param>
    /// <param name="b">The vector to subtract.</param>
    /// <returns>The difference of the two vectors.</returns>
    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    /// <summary>
    /// Multiplies a vector by a scalar.
    /// </summary>
    /// <param name="v">The vector to scale.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector3 operator *(Vector3 v, float scalar)
    {
        return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
    }

    /// <summary>
    /// Divides a vector by a scalar.
    /// </summary>
    /// <param name="v">The vector to divide.</param>
    /// <param name="scalar">The scalar value.</param>
    /// <returns>The divided vector.</returns>
    /// <exception cref="DivideByZeroException">Thrown when attempting to divide by zero.</exception>
    public static Vector3 operator /(Vector3 v, float scalar)
    {
        if (scalar == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        return new Vector3(v.X / scalar, v.Y / scalar, v.Z / scalar);
    }

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The dot product of the vectors.</returns>
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    /// <summary>
    /// Calculates the cross product of two vectors.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The cross product of the vectors.</returns>
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }

    /// <summary>
    /// Returns a string representation of the vector.
    /// </summary>
    /// <returns>A string in the format "Vector3(X, Y, Z)".</returns>
    public override string ToString()
    {
        return $"Vector3({X}, {Y}, {Z})";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current vector.
    /// </summary>
    /// <param name="obj">The object to compare with the current vector.</param>
    /// <returns><c>true</c> if the specified object is equal to the current vector; otherwise, <c>false</c>.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Vector3 vector)
        {
            return X == vector.X && Y == vector.Y && Z == vector.Z;
        }
        return false;
    }

    /// <summary>
    /// Gets the hash code for the vector.
    /// </summary>
    /// <returns>The hash code for the vector.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }
}

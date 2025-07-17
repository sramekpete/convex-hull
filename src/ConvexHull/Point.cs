namespace ConvexHull;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// Initializes a new instance of the <see cref="Point"/> struct with specified coordinates.
/// </summary>
/// <param name="x">The X coordinate.</param>
/// <param name="y">The Y coordinate.</param>
[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
[DebuggerDisplay("X: {X}, Y: {Y}")]
public readonly struct Point(int x, int y) : IEquatable<Point> {
    public static readonly Point Default = new(0, 0);
    /// <summary>
    /// Gets or sets the X coordinate of the point.
    /// </summary>
    public int X { get; } = x;

    /// <summary>
    /// Gets or sets the Y coordinate of the point.
    /// </summary>
    public int Y { get; } = y;

    /// <summary>
    /// Calculates the distance to another point using the distance formula.
    /// </summary>
    /// <param name="other">
    /// The other point to which the distance is calculated.
    /// </param>
    /// <returns>
    /// Returns the distance between this point and the specified point.
    /// </returns>
    public double DistanceTo(Point other) {
        return Math.Sqrt(Math.Pow(Math.Max(X, other.X) - Math.Min(X, other.X), 2) + Math.Pow(Math.Max(Y, other.Y) - Math.Min(Y, other.Y), 2));
    }

    /// <summary>
    /// Calculates the angle in degrees from origin point to this point.
    /// </summary>
    /// <param name="other">
    /// The other point from which the angle is calculated.
    /// </param>
    /// <returns>
    /// Returns the angle in degrees from origin to the this point.
    /// </returns>
    public double AngleFrom(Point origin) {
        if (this == origin) {
            return 0;
        }

        return Math.Atan2(origin.Y - Y, origin.X - X) * (180 / Math.PI);
    }

    /// <summary>
    /// Returns new point offset from the origin point.
    /// </summary>
    /// <param name="origin">
    /// The other point from which the offset is calculated.
    /// </param>
    /// <returns>
    /// The offset point.
    /// </returns>
    public Point OffsetFrom(Point origin) {
        return new Point(X - origin.X, Y - origin.Y);
    }

    /// <summary>
    /// Returns new point that is rotated around provided origin point.
    /// </summary>
    /// <param name="origin">
    /// The other point around which the rotation is calculated.
    /// </param>
    /// <param name="angle">
    /// The angle to by applied to the rotation of this point.
    /// </param>
    /// <returns>
    /// The rotated point.
    /// </returns>
    public Point Rotate(Point origin, double angle) {
        Debug.Assert(angle is not double.NaN or double.NegativeInfinity or double.PositiveInfinity);

        double radians = Math.PI / 180 * angle;
        double sin = Math.Sin(radians);
        double cos = Math.Cos(radians);

        var x = X - origin.X;
        var y = Y - origin.Y;

        return new Point((int)(x * cos - y * sin) + origin.X, (int)(x * sin + y * cos) + origin.Y);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) {
        return obj is Point point && Equals(point);
    }

    /// <inheritdoc />
    public bool Equals(Point other) {
        return X == other.X &&
               Y == other.Y;
    }

    /// <inheritdoc />
    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    /// <summary>
    /// Determines whether two points are equal.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the two points are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Point left, Point right) {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two points are not equal.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the two points are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Point left, Point right) {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether the left point is greater than or equal to the right point.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the left point is greater than or equal to the right point; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(Point left, Point right) {
        return left == right || left > right;
    }

    /// <summary>
    /// Determines whether the left point is less than or equal to the right point.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the left point is less than or equal to the right point; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(Point left, Point right) {
        return left == right || left < right;
    }

    /// <summary>
    /// Determines whether the left point is greater than the right point.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the left point is greater than the right point; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(Point left, Point right) {
        return
            left.X > right.X && left.Y > right.Y
            || left.X > right.X && left.Y == right.Y
            || left.X == right.X && left.Y > right.Y;
    }

    /// <summary>
    /// Determines whether the left point is less than the right point.
    /// </summary>
    /// <param name="left">
    /// The left point to compare.
    /// </param>
    /// <param name="right">
    /// The right point to compare.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if the left point is less than the right point; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(Point left, Point right) {
        return
            left.X < right.X && left.Y < right.Y
            || left.X < right.X && left.Y == right.Y
            || left.X == right.X && left.Y < right.Y;
    }
}
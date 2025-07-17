namespace ConvexHull.Algorithms;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Provides convex hull computation using Jarvis' March algorithm
/// </summary>
/// <inheritdoc cref="IConvexHullAlgorithm"/>
public class JarvisMarchAlgorithm : IConvexHullAlgorithm {
    public static readonly JarvisMarchAlgorithm Default = new JarvisMarchAlgorithm();

    private JarvisMarchAlgorithm() { }

    public IEnumerable<Point> Compute(IEnumerable<Point> points) {
        Debug.Assert(points != null, "Points collection should not be null.");
        Debug.Assert(points.Count() > 1, "At least three unique points are required to compute a convex hull.");

        Point origin = GetStartingPoint(points, out int count);

        Span<Point> temp = stackalloc Point[count + 1];
        temp[0] = origin;
        int consumed = 0;
        int index = 1;

        Point next;

        while (consumed < count) {
            next = GetNextEdgePoint(points, origin);

            if (next != temp[0]) {
                temp[index++] = origin = next;
            }

            consumed++;
        }

        // When we end up only with two points we don't need to enclose the loop
        if (index != 2) {
            temp[index++] = temp[0];
        }

        return temp[..index]
            .ToArray();
    }

    /// <summary>
    /// Returns direction of the next point
    /// </summary>
    /// <param name="origin">
    /// The last convex hull point.
    /// </param>
    /// <param name="current">
    /// Point that is being currently tested for validity to be the next hull point.
    /// </param>
    /// <param name="next">
    /// The immediate point after <paramref name="current"/>
    /// </param>
    /// <returns>
    /// The direction of the turn between <paramref name="origin"/> and both <paramref name="current"/> and <paramref name="next"/>.
    /// </returns>
    private NextDirection GetNextDirection(Point origin, Point current, Point next) {
        NextDirection direction = NextDirection.None;

        if (origin != current) {
            var angle = origin.AngleFrom(current);

            var normalizedCurrent = current.OffsetFrom(origin);
            var normalizedNext = next.OffsetFrom(origin);

            var rotatedCurrent = normalizedCurrent.Rotate(Point.Default, -angle);
            var rotatedNext = normalizedNext.Rotate(Point.Default, -angle);

            Debug.Assert(Point.Default.AngleFrom(rotatedCurrent) == 0);

            direction = (NextDirection)Point.Default.AngleFrom(rotatedNext).CompareTo(0);
        }

        return direction;
    }

    /// <summary>
    /// Returns next valid hull point
    /// </summary>
    /// <param name="points">
    /// Collection of points to be evaluated.
    /// </param>
    /// <param name="origin">
    /// The last convex hull point.
    /// </param>
    /// <returns>
    /// The next convex hull point.
    /// </returns>
    private Point GetNextEdgePoint(IEnumerable<Point> points, Point origin) {
        Point current = origin;
        NextDirection direction;

        foreach (Point next in points) {
            direction = GetNextDirection(origin, current, next);

            if (direction == NextDirection.Right || direction == NextDirection.None && origin.DistanceTo(next) > origin.DistanceTo(current))
                current = next;
        }

        return current;
    }

    /// <summary>
    /// Returns initial convex hull edge point
    /// </summary>
    /// <param name="points">
    /// Collection of points to be evaluated.
    /// </param>
    /// <param name="count">
    /// Outputs the number of items in provided <paramref name="points"/>.
    /// </param>
    /// <returns>
    /// The initial edge convex hull point.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Throw when <paramref name="points"/> is empty enumeration.
    /// </exception>
    private static Point GetStartingPoint(IEnumerable<Point> points, out int count) {
        Debug.Assert(points.Any(), "Points should not be empty.");

        Point start;
        count = 0;

        using var enumerator = points.GetEnumerator();

        if (enumerator.MoveNext()) {
            start = enumerator.Current;
            count++;
        } else {
            throw new InvalidOperationException();
        }

        while (enumerator.MoveNext()) {
            Point current = enumerator.Current;

            if (start.X > current.X
                || start.X == current.X && start.Y > current.Y) {
                start = current;
            }

            count++;
        }

        return start;
    }

    /// <summary>
    /// Defines direction of next turn of currently tested point
    /// </summary>
    private enum NextDirection : int {
        /// <summary>
        /// The next point is angled right from origin in comparison to the following point
        /// </summary>
        Right = -1,
        /// <summary>
        /// The next point is in line with the following point from origin
        /// </summary>
        None = 0,
        /// <summary>
        /// The next point is angled right from origin in comparison to the following point
        /// </summary>
        Left = 1,
    }

}

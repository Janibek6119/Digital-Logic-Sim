using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DLS.ChipCreation
{
	public static class MathsHelper
	{
		public static (bool intersects, Vector2 intersectionPoint) LineIntersectsLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float d = (a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x);
			if (d == 0) // parallel
			{
				return (false, Vector2.zero);
			}

			float n = (a1.x - b1.x) * (b1.y - b2.y) - (a1.y - b1.y) * (b1.x - b2.x);
			float t = n / d;
			Vector2 intersectionPoint = a1 + (a2 - a1) * t;
			return (true, intersectionPoint);
		}


		public static bool BoundsOverlap2D(Bounds a, Bounds b)
		{
			if (a.size.x * a.size.y == 0 || b.size.x * b.size.y == 0)
			{
				return false;
			}
			bool overlapX = b.min.x < a.max.x && b.max.x > a.min.x;
			bool overlapY = b.min.y < a.max.y && b.max.y > a.min.y;
			return overlapX && overlapY;

		}
		public static bool BoundsOverlap2DAllowNumericalError(Bounds a, Bounds b, float allowedNumericalError = 0.001f)
		{
			if (a.size.x * a.size.y == 0 || b.size.x * b.size.y == 0)
			{
				return false;
			}
			bool overlapX = TwoRangesIntersectionSpan(a.min.x, a.max.x, b.min.x, b.max.x) > allowedNumericalError;
			bool overlapY = TwoRangesIntersectionSpan(a.min.y, a.max.y, b.min.y, b.max.y) > allowedNumericalError;
			return overlapX && overlapY;
		}

		public static float TwoRangesIntersectionSpan(float aMin, float aMax, float bMin, float bMax)
		{
			float containerMinimum = Mathf.Min(aMin, bMin);
			float containerMaximum = Mathf.Max(aMax, bMax);
			float containerLength = containerMaximum - containerMinimum;
			float aLength = aMax - aMin;
			float bLength = bMax - bMin;
			return aLength + bLength - containerLength;
		}

		public static Vector2 GetDiscretizedVector(Vector2 pos, float discretization, Bounds? absBounds = null)
		{
			float discretizedX = GetDiscretizedFloat(pos.x, discretization, absBounds?.min.x, absBounds?.max.x);
			float discretizedY = GetDiscretizedFloat(pos.y, discretization, absBounds?.min.y, absBounds?.max.y);
			return new Vector2(discretizedX, discretizedY);
		}

		public static Vector2 GetDiscretizedVector(Vector2 pos, float discretization, Vector2 offset, Bounds? absBounds = null)
		{
			return GetDiscretizedVector(pos + offset, discretization, absBounds) - offset;
		}

		public static float GetDiscretizedFloat(float value, float discretization, float? absLowerBoundary = null, float? absHigherBoundary = null)
		{
			// If boundaries set, restrict the input value
			if (value > absHigherBoundary) value = absHigherBoundary.Value;
			if (value < absLowerBoundary) value = absLowerBoundary.Value;

			int steps = Mathf.FloorToInt(value / discretization);
			float adjacentLower = discretization * steps;
			float adjacentHigher = adjacentLower + discretization;
			float distanceDown = value - adjacentLower;
			float distanceUp = adjacentHigher - value;

			if (distanceDown < distanceUp)
			{
				// If closer number violates boundary, return another one
				if (adjacentLower < absLowerBoundary)
				{
					return adjacentHigher;
				}
				else
				{
					return adjacentLower;
				}
			}
			else
			{
				// If closer number violates boundary, return another one
				if (adjacentHigher > absHigherBoundary)
				{
					return adjacentLower;
				}
				else
				{
					return adjacentHigher;
				}
			}
		}
	}
}
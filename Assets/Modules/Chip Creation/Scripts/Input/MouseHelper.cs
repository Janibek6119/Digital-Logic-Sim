using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DLS.ChipCreation
{
	public static class MouseHelper
	{
		static Camera _cam;


		public static bool LeftMousePressedThisFrame()
		{
			return Mouse.current.leftButton.wasPressedThisFrame;
		}

		public static bool LeftMouseIsPressed()
		{
			return Mouse.current.leftButton.isPressed;
		}

		public static bool LeftMouseReleasedThisFrame()
		{
			return Mouse.current.leftButton.wasReleasedThisFrame;
		}

		public static bool RightMousePressedThisFrame()
		{
			return Mouse.current.rightButton.wasPressedThisFrame;
		}

		public static Vector3 GetMouseWorldPosition(float z)
		{
			Vector2 pos = GetMouseWorldPosition();
			return new Vector3(pos.x, pos.y, z);
		}

		public static Vector2 GetMouseWorldPosition()
		{
			return Cam.ScreenToWorldPoint(GetMouseScreenPosition());
		}

		public static Vector2 GetMouseScreenPosition()
		{
			if (Application.isEditor)
			{
				return SebInput.Internal.MouseEventSystem.GetMousePos();
			}
			return Mouse.current.position.ReadValue();
		}

		public static Vector2 CalculateAxisSnappedMousePosition(Vector2 origin, bool snap = true)
		{
			Vector2 snappedMousePos = GetMouseWorldPosition();
			if (snap)
			{
				Vector2 delta = snappedMousePos - origin;
				bool snapHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
				snappedMousePos = new Vector2(snapHorizontal ? snappedMousePos.x : origin.x, snapHorizontal ? origin.y : snappedMousePos.y);
			}
			return snappedMousePos;

		}

		public static Vector2 CalculateAxisSnappedMousePosition(Vector2 origin, bool snap, bool gridSnap, float gridDiscretization, Bounds? absBounds = null)
		{
			Vector2 snappedMousePos = CalculateAxisSnappedMousePosition(origin, snap);
			if (gridSnap)
			{
				snappedMousePos = GetDiscretizedVector(snappedMousePos, absBounds, gridDiscretization);
			}
			return snappedMousePos;
		}

		private static Vector2 GetDiscretizedVector(Vector2 pos, Bounds? absBounds, float discretization)
		{
			float discretizedX = GetDiscretizedFloat(pos.x, discretization, absBounds?.min.x, absBounds?.max.x);
			float discretizedY = GetDiscretizedFloat(pos.y, discretization, absBounds?.min.y, absBounds?.max.y);
			return new Vector2(discretizedX, discretizedY);
		}

		public static float GetDiscretizedFloat(float value, float discretization, float? absLowerBoundary, float? absHigherBoundary)
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

		static Camera Cam
		{
			get
			{
				if (_cam == null)
				{
					_cam = Camera.main;
				}
				return _cam;
			}
		}


		[RuntimeInitializeOnLoadMethod]
		static void Init()
		{
			// Ensure static variables are properly initialized when domain reloading is disabled.
			_cam = null;
		}
	}
}
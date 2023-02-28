using DLS.ChipCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundGridRenderer : MonoBehaviour
{
	public GameObject HLine;
	public GameObject VLine;
	public GameObject container;
	WorkArea workArea;

	[SerializeField, Range(0, 0.005f)] float thickness;

	void Start()
	{
		workArea = GetComponent<WorkArea>();
		InstantiateDotGrid();
		container.SetActive(false);
	}

	void Update()
	{
		Keyboard keyboard = Keyboard.current;
		if (keyboard.ctrlKey.wasPressedThisFrame)
		{
			container.SetActive(true);
		}
		else if (keyboard.ctrlKey.wasReleasedThisFrame)
		{
			container.SetActive(false);
		}
	}

	private void InstantiateDotGrid()
	{
		foreach (Transform child in container.transform)
		{
			Destroy(child.gameObject);
		}
		float discretization = workArea.GridDiscretization;
		Vector3 offset = new Vector3(0, 0, -0.01f);
		Bounds bounds = workArea.ColliderBounds;
		Vector2 boundsDiagonal = bounds.max - bounds.min;
		float YThickness = thickness * boundsDiagonal.x / boundsDiagonal.y;

		// Get min x,y of grid
		float stepsToMinX = Mathf.FloorToInt((offset.x - bounds.min.x) / discretization);
		float minDiscreteX = offset.x - stepsToMinX * discretization;
		float stepsToMinY = Mathf.FloorToInt((offset.y - bounds.min.y) / discretization);
		float minDiscreteY = offset.y - stepsToMinY * discretization;

		int cols = Mathf.FloorToInt((bounds.max.x - minDiscreteX) / discretization) + 1;
		int rows = Mathf.FloorToInt((bounds.max.y - minDiscreteY) / discretization) + 1;

		// Row by row, bottom to top / left to right
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				float y = minDiscreteY + i * discretization;
				float x = minDiscreteX + j * discretization;
				GameObject obj = Instantiate(HLine, new Vector3(x, y, -0.01f), Quaternion.identity, container.transform);
				obj.transform.localScale = new Vector3(thickness * 2, YThickness * 2, obj.transform.localScale.z);
			}
		}
	}

	private void InstantiateGrid()
	{
		foreach (Transform child in container.transform)
		{
			Destroy(child.gameObject);
		}
		float discretization = workArea.GridDiscretization;
		Vector3 HOffset = new Vector3(0, 0, -0.01f);
		Vector3 VOffset = new Vector3(container.transform.position.x, container.transform.position.y, -0.01f);
		Bounds bounds = workArea.ColliderBounds;
		Vector2 boundsDiagonal = bounds.max - bounds.min;
		float YThickness = thickness * boundsDiagonal.x / boundsDiagonal.y;

		InstantiateHorizontalLine(HOffset, YThickness);
		InstantiateVerticalLine(VOffset, thickness);

		for (float y = HOffset.y + discretization; y <= bounds.max.y; y += discretization)
		{
			Vector3 pos = new Vector3(HOffset.x, y, HOffset.z);
			InstantiateHorizontalLine(pos, YThickness);
		}
		for (float y = HOffset.y - discretization; y >= bounds.min.y; y -= discretization)
		{
			Vector3 pos = new Vector3(HOffset.x, y, HOffset.z);
			InstantiateHorizontalLine(pos, YThickness);
		}
		for (float x = VOffset.x + discretization; x <= bounds.max.x; x += discretization)
		{
			Vector3 pos = new Vector3(x, VOffset.y, VOffset.z);
			InstantiateVerticalLine(pos, thickness);
		}
		for (float x = VOffset.x - discretization; x >= bounds.min.x; x -= discretization)
		{
			Vector3 pos = new Vector3(x, VOffset.y, VOffset.z);
			InstantiateVerticalLine(pos, thickness);
		}
	}
	private void InstantiateHorizontalLine(Vector3 pos, float YThickness)
	{
		GameObject obj = Instantiate(HLine, pos, Quaternion.identity, container.transform);
		obj.transform.localScale = new Vector3(obj.transform.localScale.x, YThickness, obj.transform.localScale.z);
	}
	private void InstantiateVerticalLine(Vector3 pos, float XThickness)
	{
		GameObject obj = Instantiate(VLine, pos, Quaternion.identity, container.transform);
		obj.transform.localScale = new Vector3(XThickness, obj.transform.localScale.y, obj.transform.localScale.z);
	}
}

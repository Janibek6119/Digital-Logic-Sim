using DLS.ChipCreation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundGridRenderer : MonoBehaviour
{
	public GameObject linePrefab;
	public GameObject container;
	WorkArea workArea;

	[SerializeField, Range(0, 0.005f)] float thickness;
	float YThickness;
	[SerializeField] bool preferDotGrid;
	DisplayOptions.BackgroundGridDisplayMode mode;
	bool gridShow;

	public void SetUp(WorkArea workArea, DisplayOptions.BackgroundGridDisplayMode gridDisplayMode)
	{
		this.workArea = workArea;
		SetGridDisplayMode(gridDisplayMode);
		// NOTE: If we call SpawnGrid() directly here, it gets called twice (apparently because Work Area is cloned)
		workArea.WorkAreaResized += SpawnGrid;
	}

	public void OnDestroy()
	{
		// NOTE: Without this null check, it throws when quitting to main menu (apparently because Work Area is cloned)
		if (workArea != null)
		{
			workArea.WorkAreaResized -= SpawnGrid;
		}
	}

	public void SetGridDisplayMode(DisplayOptions.BackgroundGridDisplayMode gridDisplayMode)
	{
		mode = gridDisplayMode;
		SetGridVisibility(gridDisplayMode == DisplayOptions.BackgroundGridDisplayMode.Always);
	}

	void SpawnGrid()
	{
		foreach (Transform child in container.transform)
		{
			Destroy(child.gameObject);
		}
		InstantiateGrid();
	}

	void SetGridVisibility(bool visible)
	{
		gridShow = visible;
		container.SetActive(gridShow);
	}

	void Update()
	{
		if (mode == DisplayOptions.BackgroundGridDisplayMode.Sync)
		{
			SetGridVisibility(workArea.GridSnap());
		}
		else if (mode == DisplayOptions.BackgroundGridDisplayMode.Toggle)
		{
			if (Keyboard.current.gKey.wasPressedThisFrame)
			{
				SetGridVisibility(!gridShow);
			}
		}
	}

	private void InstantiateGrid()
	{
		float discretization = workArea.GridDiscretization;
		Bounds bounds = workArea.ColliderBounds;
		YThickness = thickness * bounds.size.x / bounds.size.y;

		Vector2 bottomLeftGridPoint = MathsHelper.GetDiscretizedVector(bounds.min, discretization, bounds);

		if (preferDotGrid)
		{
			for (float y = bottomLeftGridPoint.y; y <= bounds.max.y; y += discretization)
			{
				for (float x = bottomLeftGridPoint.x; x <= bounds.max.x; x += discretization)
				{
					InstantiateDot(x, y);
				}
			}
		}
		else
		{
			for (float y = bottomLeftGridPoint.y; y <= bounds.max.y; y += discretization)
			{
				InstantiateHorizontalLine(y);
			}
			for (float x = bottomLeftGridPoint.x; x <= bounds.max.x; x += discretization)
			{
				InstantiateVerticalLine(x);
			}
		}
	}
	private void InstantiateDot(float x, float y)
	{
		GameObject obj = Instantiate(linePrefab, new Vector3(x, y, RenderOrder.BackgroundOutline), Quaternion.identity, container.transform);
		obj.transform.localScale = new Vector3(thickness * 2, YThickness * 2, obj.transform.localScale.z);
	}

	private void InstantiateHorizontalLine(float y)
	{
		Vector3 pos = new Vector3(container.transform.position.x, y, RenderOrder.BackgroundOutline);
		GameObject obj = Instantiate(linePrefab, pos, Quaternion.identity, container.transform);
		obj.transform.localScale = new Vector3(obj.transform.localScale.x, YThickness, obj.transform.localScale.z);
	}

	private void InstantiateVerticalLine(float x)
	{
		Vector3 pos = new Vector3(x, container.transform.position.y, RenderOrder.BackgroundOutline);
		GameObject obj = Instantiate(linePrefab, pos, Quaternion.identity, container.transform);
		obj.transform.localScale = new Vector3(thickness, obj.transform.localScale.y, obj.transform.localScale.z);
	}
}

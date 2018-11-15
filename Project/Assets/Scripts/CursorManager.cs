﻿using System;
using System.Collections.Generic;

using UnityEngine;

using View;

public class CursorManager : MonoBehaviour
{
    public Plot HoveredPlot = null;
    public Position HoveredPlotPosition = Position.Invalid;

    private Dictionary<System.Type, ICursor> cursors = new Dictionary<Type, ICursor>();
    private ICursor currentCursor;

    private Camera currentCamera = null;

    public static CursorManager Instance { get; private set; }

    public void ChangeCursor<T>(object parameter = null) where T : ICursor
    {
        this.currentCursor?.OnDeactivate();

        if (!this.cursors.TryGetValue(typeof(T), out this.currentCursor))
        {
            Debug.LogError($"Unknown cursor: {typeof(T)}");
            return;
        }

        this.currentCursor.OnActivate(parameter);
    }

    private void Awake()
    {
        Debug.Assert(CursorManager.Instance == null);
        CursorManager.Instance = this;

        this.cursors.Add(typeof(PlantPlacementCursor), new PlantPlacementCursor());
        this.cursors.Add(typeof(DefaultCursor), new DefaultCursor());
    }

    private void Update()
    {
        if (this.currentCamera == null)
        {
            this.currentCamera = GameObject.FindObjectOfType<Camera>();
            return;
        }

        this.currentCursor?.Update(this.currentCamera);
    }

    private void OnDrawGizmos()
    {
        this.currentCursor?.OnDrawGizmos();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (this.currentCursor != null)
        {
            GUILayout.Space(20f);
            GUILayout.Label($"Cursor {this.currentCursor.GetType().Name}");
        }
    }
#endif
}
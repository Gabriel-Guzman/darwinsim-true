/*
------------------------------------------------------------------
  This file is part of UnitySharpNEAT
  Copyright 2020, Florian Wolf
  https://github.com/flo-wolf/UnitySharpNEAT
------------------------------------------------------------------
*/

using System;
using System.Drawing;
using UnityEngine;

namespace UI
{
    public class NeatUI : MonoBehaviour
    {
        public GameManager gameManager;

        [SerializeField] private Vector2 topLeft = new Vector2(10, 10);

        public float dragSpeed = 4;
        private Vector3 _dragOrigin;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        /// <summary>
        /// Display simple Onscreen buttons for quickly accessing ceratain lifecycle funtions and to display generation info.
        /// </summary>
        private void OnGUI()
        {
            switch (gameManager.State)
            {
                case GameState.Running:
                    WhileRunningGUI(topLeft);
                    break;
                case GameState.Stopped:
                    PreStartGUI(topLeft);
                    break;
            }
        }

        private void WhileRunningGUI(Vector2 origin)
        {
            var runningTopLeft = origin;
            var mul = 1.5f;
            if (GUI.Button(new Rect(runningTopLeft.x, runningTopLeft.y, 110, 40), "Faster"))
            {
                var newScale = Time.timeScale * mul;
                if (newScale > 100f) newScale = 100f;
                Time.timeScale = newScale;
                Debug.Log($"New timescale: {Time.timeScale}");
            }

            runningTopLeft.y += 50;
            if (GUI.Button(new Rect(runningTopLeft.x, runningTopLeft.y, 110, 40), "Slower"))
            {
                var newScale = Time.timeScale / mul;
                if (newScale < 0.1f)
                    newScale = 0.1f;
                Time.timeScale = newScale;
                Debug.Log($"New timescale: {Time.timeScale}");
            }

            runningTopLeft.y += 50;
            if (GUI.Button(new Rect(runningTopLeft.x, runningTopLeft.y, 110, 40), "Toggle Strategy"))
            {
                gameManager.ToggleStrategy();
            }
        }

        private void PreStartGUI(Vector2 origin)
        {
            var runningTopLeft = origin;
            if (GUI.Button(new Rect(runningTopLeft.x, runningTopLeft.y, 110, 40), "Spawn"))
            {
                gameManager.Init();
            }
        }
    }
}
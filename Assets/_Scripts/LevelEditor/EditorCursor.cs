﻿using System.Linq;
using UnityEngine;

namespace Assets._Scripts.LevelEditor
{
    [UnityComponent]
    public class EditorCursor : MonoBehaviour
    {
        public Tool HoldingTool;

        /// <summary>Clicks won't go to the game world in the palette area.</summary>
        [AssignedInUnity]
        public RectTransform PaletteArea;

        [AssignedInUnity]
        public RectTransform WorldArea;

        private GridPosition lastGridLocation;

        [UnityMessage]
        public void Awake()
        {
            Cursor.visible = false;
        }

        [UnityMessage]
        public void Update()
        {
            SetCursorPosition();
            CheckClick();
            CheckMovement();
        }

        private void SetCursorPosition()
        {
            var screenCursorPosition = Input.mousePosition;
            transform.position = new Vector3(screenCursorPosition.x, screenCursorPosition.y, 0);
        }

        private bool OneMouseButtonWasPressed()
        {
            return Input.GetMouseButtonDown(0) ^ Input.GetMouseButtonDown(1);
        }

        private bool OneMouseButtonIsDown()
        {
            return Input.GetMouseButton(0) ^ Input.GetMouseButton(1);
        }

        private void CheckClick()
        {
            if(OneMouseButtonWasPressed() == false)
                return;
            
            if (CursorIsInPalette())
            {
                HandlePalleteClick(Input.mousePosition);
            }
            else if(CursorIsInWorld())
            {
                HandleWorldClick();
            }
        }

        private bool CursorIsInWorld()
        {
            return WorldArea.GetWorldRect().Contains((Vector2)Input.mousePosition);
        }

        private bool CursorIsInPalette()
        {
            return PaletteArea.GetWorldRect().Contains((Vector2)Input.mousePosition);
        }

        private void CheckMovement()
        {
            var movementWithMouseDown = OneMouseButtonWasPressed() == false && OneMouseButtonIsDown();
            if (movementWithMouseDown == false || HoldingTool == null || HoldingTool.ShouldSnapToGrid == false || !CursorIsInWorld())
                return;
            
            var currentGridPosition = PlacementGrid.Instance.GetGridPosition(HoldingTool.CurrentPosition);

            if (currentGridPosition != lastGridLocation)
            {
                HandleWorldClick();
            }

            lastGridLocation = currentGridPosition;
        }

        private void HandlePalleteClick(Vector2 mousePosition)
        {
            var allTools = PaletteArea.GetComponentsInChildren<PaletteItem>();

            var selectedTool = allTools.FirstOrDefault(x => ((RectTransform)x.transform).GetWorldRect().Contains(mousePosition));

            if (selectedTool == null)
                return;

            if (HoldingTool != null)
                Destroy(HoldingTool.gameObject);

            var toolInstance = (GameObject)Instantiate(selectedTool.ToolPrefab, GetWorldPosition(), Quaternion.identity);
            HoldingTool = toolInstance.GetComponent<Tool>();
            HoldingTool.Cursor = this;
        }

        private void HandleWorldClick()
        {
            if (HoldingTool == null)
                return; //todo?

            if (Input.GetMouseButton(0))
            {
                HoldingTool.ActivateTool(HoldingTool.CurrentPosition);
            }
            else
            {
                HoldingTool.SecondaryActivateTool(HoldingTool.CurrentPosition);
            }
        }

        public static Vector2 GetWorldPosition()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
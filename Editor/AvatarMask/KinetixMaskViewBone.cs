// // ----------------------------------------------------------------------------
// // <copyright file="KinetixMaskViewBone.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Kinetix.Editor
{
	internal static class KinetixMaskViewBone
	{
		public static GUIContent dotFill = EditorGUIUtility.IconContent("AvatarInspector/DotFill");
		public static GUIContent dotFrame = EditorGUIUtility.IconContent("AvatarInspector/DotFrame");
		public static GUIContent dotFrameDotted = EditorGUIUtility.IconContent("AvatarInspector/DotFrameDotted");
		public static GUIContent dotSelection = EditorGUIUtility.IconContent("AvatarInspector/DotSelection");

		public delegate void ClickDelegate(int controlId, Event evt);

		public const int ICON_SIZE = 19;

		public static void BoneDotGUI(Rect rect, Rect selectRect, Color color, ClickDelegate OnClick, string tooltip = null)
		{
			int id = GUIUtility.GetControlID(FocusType.Passive, rect);

			Event evt = Event.current;
			bool isHovered = selectRect.Contains(evt.mousePosition);
			if (isHovered)
			{
				//TODO:
				GUI.tooltip = tooltip;
			}

			if (evt.type == EventType.MouseDown && isHovered)
			{
				evt.Use();
				OnClick?.Invoke(id, evt);
			}

			Color guiColor = GUI.color;

			GUI.color = color;

			// Frame
			GUI.Label(rect, new GUIContent(dotFrame.image, tooltip), EditorStyles.label);
			//GUI.DrawTexture(rect, dotFrame.image);

			// Fill
			GUI.Label(rect, new GUIContent(dotFill.image, tooltip), EditorStyles.label);
			//GUI.DrawTexture(rect, dotFill.image);

			GUI.color = guiColor;
		}
	}
}

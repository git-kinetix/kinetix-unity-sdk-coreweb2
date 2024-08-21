// // ----------------------------------------------------------------------------
// // <copyright file="KinetixMaskEditor.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;

namespace Kinetix.Editor
{

	[CustomEditor(typeof(KinetixMask), true)]
	public class KinetixMaskEditor : UEditor
	{
		private SerializedProperty disabledBones;
		private bool hasChanged;
		private int shownBodyPart;

		private void OnEnable()
		{
			disabledBones = serializedObject.FindProperty( nameof(disabledBones) );
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			hasChanged = false;

			if (disabledBones.arraySize != (int)HumanBodyBones.LastBone)
			{
				disabledBones.arraySize = (int)HumanBodyBones.LastBone;
				serializedObject.ApplyModifiedProperties();
				serializedObject.Update();
			}

			DrawBoneMapping();

			GUILayout.Space(8);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Enable all" , EditorStyles.miniButtonLeft)) 
			{
				HumanBodyBones[] bones = KinetixMaskView.VIEW_BONES[shownBodyPart];
				for (int i = bones.Length - 1; i >= 0; i--)
				{
					disabledBones.GetArrayElementAtIndex((int)bones[i]).boolValue = false;
				}

				hasChanged = true;
			}

			if (GUILayout.Button("Toggle all" , EditorStyles.miniButtonMid))  
			{
				HumanBodyBones[] bones = KinetixMaskView.VIEW_BONES[shownBodyPart];
				for (int i = bones.Length - 1; i >= 0; i--)
				{
					SerializedProperty serializedProperty = disabledBones.GetArrayElementAtIndex((int)bones[i]);
					serializedProperty.boolValue = !serializedProperty.boolValue;
				}

				hasChanged = true;
			}

			if (GUILayout.Button("Disable all", EditorStyles.miniButtonRight))
			{
				HumanBodyBones[] bones = KinetixMaskView.VIEW_BONES[shownBodyPart];
				for (int i = bones.Length - 1; i >= 0; i--)
				{
					disabledBones.GetArrayElementAtIndex((int)bones[i]).boolValue = true;
				}

				hasChanged = true;
			}
			GUILayout.EndHorizontal();


			if (hasChanged)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}


		private void DrawBoneMapping()
		{
			GUILayout.BeginVertical("", "TE NodeBackground");
			{
				shownBodyPart = KinetixMaskView.ShowBoneMapping(shownBodyPart, PartColor, BoneColor, Click);
			}
			GUILayout.EndVertical();

			Color PartColor(BodyPart part)
			{
				HumanBodyBones[] bones = KinetixMaskView.VIEW_PART_BONES[shownBodyPart, (int)part];
				int enabledCount = 0;
				int count = bones.Length;
				bool enabledBone;
				for (int i = 0; i < count; i++)
				{
					enabledBone = !disabledBones.GetArrayElementAtIndex((int)bones[i]).boolValue;
					if (enabledBone)
						++enabledCount;
				}

				if (count == enabledCount)
					return Color.green;
				if (enabledCount > 0)
					return Color.Lerp(Color.red, Color.green, (float)enabledCount / count);

				return Color.red;
			}

			Color BoneColor(HumanBodyBones bone)
			{
				bool enabledBone = !disabledBones.GetArrayElementAtIndex((int)bone).boolValue;
				return enabledBone ? Color.green : Color.red;
			}

			void Click(HumanBodyBones bone)
			{
				SerializedProperty serializedProperty = disabledBones.GetArrayElementAtIndex((int)bone);
				serializedProperty.boolValue = !serializedProperty.boolValue;

				hasChanged = true;
			}
		}
	}
}

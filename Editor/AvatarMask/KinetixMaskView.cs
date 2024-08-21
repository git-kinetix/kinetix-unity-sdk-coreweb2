// // ----------------------------------------------------------------------------
// // <copyright file="KinetixMaskView.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kinetix.Editor
{
	internal static class KinetixMaskView
	{
		private class ViewStyle
		{
			public GUIContent[] Shadow =
			{
				EditorGUIUtility.IconContent("AvatarInspector/BodySilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/HeadZoomSilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoomSilhouette"),
				EditorGUIUtility.IconContent("AvatarInspector/RightHandZoomSilhouette")
			};

			public GUIContent[,] BodyParts =
			{
				{
					null,
					EditorGUIUtility.IconContent("AvatarInspector/Torso"),
					EditorGUIUtility.IconContent("AvatarInspector/Head"),
					EditorGUIUtility.IconContent("AvatarInspector/LeftArm"),
					EditorGUIUtility.IconContent("AvatarInspector/LeftFingers"),
					EditorGUIUtility.IconContent("AvatarInspector/RightArm"),
					EditorGUIUtility.IconContent("AvatarInspector/RightFingers"),
					EditorGUIUtility.IconContent("AvatarInspector/LeftLeg"),
					EditorGUIUtility.IconContent("AvatarInspector/RightLeg")
				},
				{
					null,
					null,
					EditorGUIUtility.IconContent("AvatarInspector/HeadZoom"),
					null,
					null,
					null,
					null,
					null,
					null
				},
				{
					null,
					null,
					null,
					null,
					EditorGUIUtility.IconContent("AvatarInspector/LeftHandZoom"),
					null,
					null,
					null,
					null
				},
				{
					null,
					null,
					null,
					null,
					null,
					null,
					EditorGUIUtility.IconContent("AvatarInspector/RightHandZoom"),
					null,
					null
				},
			};
		}

		private static ViewStyle Style { get { _style ??= new ViewStyle(); return _style; } }
		private static ViewStyle _style;

		private const  float BUTTON_MARGIN_TOP_LEFT = 5;
		private static readonly Vector2 BUTTON_SIZE = new Vector2(80, 18);
		
		public  static readonly string[] VIEW_BUTTONS = new string[] { "Body", "Head", "Left Hand", "Right Hand" };
		private static readonly Vector2[,] BONE_POSTIONS = new Vector2[4, HumanTrait.BoneCount];
		
		/// <summary>
		/// <see cref="HumanBodyBones"/>[] boneArray = <see cref="VIEW_PART_BONES"/> [ <see langword="button_view_id"/>, <see cref="BodyPart"/> ]
		/// </summary>
		public static readonly HumanBodyBones[,][] VIEW_PART_BONES = new HumanBodyBones[4, (int)BodyPart.Last][]
		{
			//view = 0
			{
				new HumanBodyBones[0],
				new HumanBodyBones[]
				{
					HumanBodyBones.Hips,
					HumanBodyBones.Spine,
					HumanBodyBones.Chest,
					HumanBodyBones.UpperChest
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.Neck,
					HumanBodyBones.Head
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.LeftShoulder,
					HumanBodyBones.LeftUpperArm,
					HumanBodyBones.LeftLowerArm
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.LeftHand
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.RightShoulder,
					HumanBodyBones.RightUpperArm,
					HumanBodyBones.RightLowerArm
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.RightHand
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.LeftUpperLeg,
					HumanBodyBones.LeftLowerLeg,
					HumanBodyBones.LeftFoot,
					HumanBodyBones.LeftToes
				},
				new HumanBodyBones[]
				{
					HumanBodyBones.RightUpperLeg,
					HumanBodyBones.RightLowerLeg,
					HumanBodyBones.RightFoot,
					HumanBodyBones.RightToes
				}
			},
			
			//view = 1
			{
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[]
				{
					HumanBodyBones.Neck,
					HumanBodyBones.Head,
					HumanBodyBones.LeftEye,
					HumanBodyBones.RightEye,
					HumanBodyBones.Jaw
				},
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
			},
			
			//view = 2
			{
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[]
				{
					HumanBodyBones.LeftThumbProximal,
					HumanBodyBones.LeftThumbIntermediate,
					HumanBodyBones.LeftThumbDistal,

					HumanBodyBones.LeftIndexProximal,
					HumanBodyBones.LeftIndexIntermediate,
					HumanBodyBones.LeftIndexDistal,

					HumanBodyBones.LeftMiddleProximal,
					HumanBodyBones.LeftMiddleIntermediate,
					HumanBodyBones.LeftMiddleDistal,

					HumanBodyBones.LeftRingProximal,
					HumanBodyBones.LeftRingIntermediate,
					HumanBodyBones.LeftRingDistal,

					HumanBodyBones.LeftLittleProximal,
					HumanBodyBones.LeftLittleIntermediate,
					HumanBodyBones.LeftLittleDistal
				},
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0]
			}, 

			//view = 3
			{
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[0],
				new HumanBodyBones[]
				{
					HumanBodyBones.RightThumbProximal,
					HumanBodyBones.RightThumbIntermediate,
					HumanBodyBones.RightThumbDistal,

					HumanBodyBones.RightIndexProximal,
					HumanBodyBones.RightIndexIntermediate,
					HumanBodyBones.RightIndexDistal,

					HumanBodyBones.RightMiddleProximal,
					HumanBodyBones.RightMiddleIntermediate,
					HumanBodyBones.RightMiddleDistal,

					HumanBodyBones.RightRingProximal,
					HumanBodyBones.RightRingIntermediate,
					HumanBodyBones.RightRingDistal,

					HumanBodyBones.RightLittleProximal,
					HumanBodyBones.RightLittleIntermediate,
					HumanBodyBones.RightLittleDistal
				},
				new HumanBodyBones[0],
				new HumanBodyBones[0]
			}
		};

		public static readonly HumanBodyBones[][] VIEW_BONES = new HumanBodyBones[4][];
		
		public delegate Color BodyPartFeedbackDelegate(BodyPart bodyPart);
		public delegate Color BodyBoneFeedbackDelegate(HumanBodyBones bodyPart);
		public delegate void BodyBoneClickDelegate(HumanBodyBones bone);

		static KinetixMaskView()
		{
			int length = VIEW_BONES.Length;
			for (int i = 0; i < length; i++)
			{
				VIEW_BONES[i] = new HumanBodyBones[0]
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.Avatar])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.Body])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.Head])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.LeftArm])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.LeftFingers])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.RightArm])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.RightFingers])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.LeftLeg])
					.Concat(VIEW_PART_BONES[i, (int)BodyPart.RightLeg])
					.ToArray();
			}

			// ================================ //
			// Body view                        //
			// ================================ //
			int view = 0;
			#region BODY VIEW
			// hips
			BONE_POSTIONS[view, (int)HumanBodyBones.Hips] = new Vector2(0.00f, 0.08f);

			// upper leg
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftUpperLeg] = new Vector2(0.16f, 0.01f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightUpperLeg] = new Vector2(-0.16f, 0.01f);

			// lower leg
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftLowerLeg] = new Vector2(0.21f, -0.40f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightLowerLeg] = new Vector2(-0.21f, -0.40f);

			// foot
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftFoot] = new Vector2(0.23f, -0.80f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightFoot] = new Vector2(-0.23f, -0.80f);

			// spine - head
			BONE_POSTIONS[view, (int)HumanBodyBones.Spine] = new Vector2(0.00f, 0.20f);
			BONE_POSTIONS[view, (int)HumanBodyBones.Chest] = new Vector2(0.00f, 0.35f);
			BONE_POSTIONS[view, (int)HumanBodyBones.UpperChest] = new Vector2(0.00f, 0.50f);
			BONE_POSTIONS[view, (int)HumanBodyBones.Neck] = new Vector2(0.00f, 0.66f);
			BONE_POSTIONS[view, (int)HumanBodyBones.Head] = new Vector2(0.00f, 0.76f);

			// shoulder
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftShoulder] = new Vector2(0.14f, 0.60f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightShoulder] = new Vector2(-0.14f, 0.60f);

			// upper arm
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftUpperArm] = new Vector2(0.30f, 0.57f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightUpperArm] = new Vector2(-0.30f, 0.57f);

			// lower arm
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftLowerArm] = new Vector2(0.48f, 0.30f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightLowerArm] = new Vector2(-0.48f, 0.30f);

			// hand
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftHand] = new Vector2(0.66f, 0.03f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightHand] = new Vector2(-0.66f, 0.03f);

			// toe
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftToes] = new Vector2(0.25f, -0.89f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightToes] = new Vector2(-0.25f, -0.89f);
			#endregion BODY VIEW

			// ================================ //
			// Head view                        //
			// ================================ //
			view = 1;
			#region HEAD VIEW
			// neck - head
			BONE_POSTIONS[view, (int)HumanBodyBones.Neck] = new Vector2(-0.20f, -0.62f);
			BONE_POSTIONS[view, (int)HumanBodyBones.Head] = new Vector2(-0.15f, -0.30f);
			// left, right eye
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftEye] = new Vector2(0.63f, 0.16f);
			BONE_POSTIONS[view, (int)HumanBodyBones.RightEye] = new Vector2(0.15f, 0.16f);
			// jaw
			BONE_POSTIONS[view, (int)HumanBodyBones.Jaw] = new Vector2(0.45f, -0.40f);
			#endregion

			// ================================ //
			// Left hand view                   //
			// ================================ //
			view = 2;
			#region LEFT HAND VIEW
			// finger bases, thumb - little
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbProximal] = new Vector2(-0.35f, 0.11f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftIndexProximal] = new Vector2(0.19f, 0.11f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftMiddleProximal] = new Vector2(0.22f, 0.00f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftRingProximal] = new Vector2(0.16f, -0.12f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftLittleProximal] = new Vector2(0.09f, -0.23f);

			// finger tips, thumb - little
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbDistal] = new Vector2(-0.03f, 0.33f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftIndexDistal] = new Vector2(0.65f, 0.16f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftMiddleDistal] = new Vector2(0.74f, 0.00f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftRingDistal] = new Vector2(0.66f, -0.14f);
			BONE_POSTIONS[view, (int)HumanBodyBones.LeftLittleDistal] = new Vector2(0.45f, -0.25f);

			// finger middles, thumb - little
			for (int i = 0; i < 5; i++)
				BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbIntermediate + i * 3] = Vector2.Lerp(BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbProximal + i * 3], BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbDistal + i * 3], 0.58f);
			#endregion

			// ================================ //
			// Right hand view                  //
			// ================================ //
			view = 3;
			#region RIGHT HAND VIEW
			for (int i = 0; i < 15; i++)
				BONE_POSTIONS[view, (int)HumanBodyBones.LeftThumbProximal + i + 15] = Vector2.Scale(BONE_POSTIONS[view - 1, (int)HumanBodyBones.LeftThumbProximal + i], new Vector2(-1, 1));
			#endregion
		}

		public static int ShowBoneMapping(int viewId, BodyPartFeedbackDelegate colorCallback, BodyBoneFeedbackDelegate boneColorCallback, BodyBoneClickDelegate clickCallback)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (Style.Shadow[viewId].image)
				{
					Rect rect = GUILayoutUtility.GetRect(Style.Shadow[viewId], GUIStyle.none, GUILayout.MaxWidth(Style.Shadow[viewId].image.width));
					DrawBodyParts(rect, viewId, colorCallback);

					for (HumanBodyBones i = 0; i < HumanBodyBones.LastBone; i++)
						DrawBone(viewId, i, rect, boneColorCallback, clickCallback);
				}
				else
					GUILayout.Label("texture missing,\nfix me!");
				
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();

			// Unity-like view buttons
			Rect buttonRect = GUILayoutUtility.GetLastRect();
			buttonRect.x += BUTTON_MARGIN_TOP_LEFT;
			buttonRect.width = BUTTON_SIZE.x;
			buttonRect.yMin = buttonRect.yMax - (BUTTON_SIZE.y * VIEW_BUTTONS.Length + BUTTON_MARGIN_TOP_LEFT);
			buttonRect.height = BUTTON_SIZE.y;

			for (int i = 0; i < VIEW_BUTTONS.Length; i++)
			{
				if (GUI.Toggle(buttonRect, viewId == i, VIEW_BUTTONS[i], EditorStyles.miniButton))
				{
					viewId = i;
				}
				buttonRect.y += BUTTON_SIZE.y;
			}

			return viewId;
		}

		private static void DrawBodyParts(Rect rect, int shownBodyPart, BodyPartFeedbackDelegate colorCallback)
		{
			Color guiColor = GUI.color;
			GUI.color = new Color(0.2f, 0.2f, 0.2f, 1);
			GUIContent shadowContent = Style.Shadow[shownBodyPart];
			if (shadowContent != null)
			{
				GUI.DrawTexture(rect, shadowContent.image);
			}

			for (int i = 1; i < (int)BodyPart.Last; i++)
			{
				GUIContent partContent = Style.BodyParts[shownBodyPart, i];
				if (partContent != null && partContent.image != null)
				{
					GUI.color = colorCallback == null ? Color.gray : colorCallback((BodyPart)i);
					GUI.DrawTexture(rect, partContent.image);
				}
			}

			GUI.color = guiColor;
		}
		
		private static void DrawBone(int shownBodyView, HumanBodyBones bone, Rect rect, BodyBoneFeedbackDelegate boneColorCallback, BodyBoneClickDelegate clickCallback)
		{
			
			Vector2 pos = BONE_POSTIONS[shownBodyView, (int)bone];
			if (pos == Vector2.zero)
				return;

			const float size = KinetixMaskViewBone.ICON_SIZE;
			
			pos.y *= -1; // Unity use "y = up" for bone position. Instead of the "y = down" in imgui
			pos.Scale(new Vector2(rect.width / 2f, rect.height / 2f));
			pos = rect.center + pos;

			Rect r = new Rect(pos.x - size/2f, pos.y - size/2f, size, size);

            Color color = boneColorCallback == null ? Color.gray : boneColorCallback(bone);
            KinetixMaskViewBone.BoneDotGUI(r, r, color, OnClick, ObjectNames.NicifyVariableName(bone.ToString()) );

			void OnClick(int controlId, Event evt) =>clickCallback?.Invoke(bone);
		}
    }
}

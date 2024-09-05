using Kinetix;
using System;
using System.Linq;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IK_ContainmentZone : MonoBehaviour
{
	private static readonly Color GIZMO_COLOR = Color.green;
	private static readonly Color GIZMO_COLOR_HIT = Color.red;
	private const float RAY_COLISION_OFFSET = 0.5f;

	[SerializeField] private StepDisplayController stepDisplayController;
	[SerializeField] private Animator myAnimator;
	[SerializeField] private TMP_InputField apiKeyField;
	[SerializeField] private TMP_InputField usernameField;
	[SerializeField] private TMP_Text usernameLabel;
	[SerializeField] private TMP_Text animationName;
	[SerializeField] private Image animationIcon;
	private string animationID;

	[Header("Physic")]
	[SerializeField] private LayerMask raycastLayerMask;
	[SerializeField] private LayerMask overlapLayerMask;
	[Space]
	[SerializeField] private ARaycastObject leftHandCollider ;
	[SerializeField] private ARaycastObject rightHandCollider;
	[SerializeField] private ARaycastObject leftFootCollider ;
	[SerializeField] private ARaycastObject rightFootCollider;

	private GizmoUtils gizmo;
	private Coroutine routine;

	private void Awake()
	{
		gizmo = GetComponent<GizmoUtils>() ?? gameObject.AddComponent<GizmoUtils>();
	}

	public void OnValidateGameApiKey()
	{
		// Initialization is an async process, 
		// We use a callback to tell when it's finished
		KinetixCore.OnInitialized += OnInitialize;
		KinetixCore.Initialize(new KinetixCoreConfiguration()
		{
			GameAPIKey = apiKeyField.text,
			PlayAutomaticallyAnimationOnAnimators = true,
		});
	}

	/// <summary>
	/// This callback is used for the actions made after the SDK is initialized
	/// Such as initializing the UI and Registering our LocalPlayer's animator
	/// </summary>
	private void OnInitialize()
	{
		// Register local player to receive animation
		// See "Animation System" documentation
		KinetixCore.Animation.RegisterLocalPlayerAnimator(myAnimator, new RootMotionConfig(false, false, false));

		//Register the IK update event
		KinetixCore.Animation.RegisterIkEventOnLocalPlayer(OnBeforeIK);

		stepDisplayController.NextStep();
	}

	public void ConnectAccount()
	{
		// Now, we connect the current user's account to get his emotes
		// The userID is chosen by you, and must be unique to each user
		// See "Account Management" documentation
		KinetixCore.Account.ConnectAccount(usernameField.text, () => {
			Debug.Log("Account connected successfully");
			usernameLabel.text = usernameField.text;

			stepDisplayController.NextStep();
		}, () => {
			Debug.LogError("There was a problem during account connection. Is the GameAPIKey correct?");
		});
	}

	public void OpenUGELink()
	{
		// Example on how to get the link
		KinetixCore.UGC.GetUgcUrl((_Url) => {
			Application.OpenURL(_Url);

			stepDisplayController.NextStep();

			if (routine != null)
				StopCoroutine(routine);

			routine = StartCoroutine(FetchEmotesAtInterval());
		});
	}

	private IEnumerator FetchEmotesAtInterval()
	{
		GetPlayerEmotes();

		while (enabled)
		{
			// Fetch emotes every 5 minutes
			yield return new WaitForSeconds(300);

			GetPlayerEmotes();
		}
	}

	public void GetPlayerEmotes()
	{
		// We get the animation 
		KinetixCore.Metadata.GetUserAnimationMetadatas(OnPlayerEmoteFetched);
	}

	public void OnPlayerEmoteFetched(AnimationMetadata[] _Emotes)
	{
		if (_Emotes.Length == 0)
			return;

		stepDisplayController.NextStep();

		// Let's create a button for the last emote we fetched
		AssignEmoteToButton(_Emotes[_Emotes.Length - 1]);
	}

	public void AssignEmoteToButton(AnimationMetadata _Metadata)
	{
		// We cn load the icon of the emote using this
		KinetixCore.Metadata.LoadIconByAnimationId(_Metadata.Ids.UUID, (_Sprite) => {
			animationIcon.sprite = _Sprite;
			animationName.text = _Metadata.Name;
				animationID = _Metadata.Ids.UUID;
		});
	}

	public void PlayEmote()
	{
		// Finally we can play the animation on our local player
		KinetixCore.Animation.PlayAnimationOnLocalPlayer(animationID);
	}

	//===================================================//
	//                      PHYSICS                      //
	//===================================================//

	private void OnBeforeIK(IKInfo currentPose)
	{
		#region GIZMO SKELETON
		Debug.DrawLine(currentPose.hips.globalPosition, currentPose.leftUpperLeg.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.leftUpperLeg.globalPosition, currentPose.leftLowerLeg.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.leftLowerLeg.globalPosition, currentPose.leftFoot.globalPosition, Color.gray);

		Debug.DrawLine(currentPose.hips.globalPosition, currentPose.leftUpperArm.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.leftUpperArm.globalPosition, currentPose.leftLowerArm.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.leftLowerArm.globalPosition, currentPose.leftHand.globalPosition, Color.gray);

		Debug.DrawLine(currentPose.hips.globalPosition, currentPose.rightUpperLeg.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.rightUpperLeg.globalPosition, currentPose.rightLowerLeg.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.rightLowerLeg.globalPosition, currentPose.rightFoot.globalPosition, Color.gray);

		Debug.DrawLine(currentPose.hips.globalPosition, currentPose.rightUpperArm.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.rightUpperArm.globalPosition, currentPose.rightLowerArm.globalPosition, Color.gray);
		Debug.DrawLine(currentPose.rightLowerArm.globalPosition, currentPose.rightHand.globalPosition, Color.gray);
		#endregion

		ApplyIK(currentPose, leftHandCollider, currentPose.leftHand, currentPose.leftLowerArm, AvatarIKGoal.LeftHand, AvatarIKHint.LeftElbow);
		ApplyIK(currentPose, leftFootCollider, currentPose.leftFoot, currentPose.leftLowerLeg, AvatarIKGoal.LeftFoot, AvatarIKHint.LeftKnee);

		ApplyIK(currentPose, rightHandCollider, currentPose.rightHand, currentPose.rightLowerArm, AvatarIKGoal.RightHand, AvatarIKHint.RightElbow);
		ApplyIK(currentPose, rightFootCollider, currentPose.rightFoot, currentPose.rightLowerLeg, AvatarIKGoal.RightFoot, AvatarIKHint.RightKnee);
	}


	/// <summary>
	/// Method to apply IK on end bone
	/// </summary>
	private void ApplyIK(IKInfo currentPose, ARaycastObject raycast, IKTransformInfo transformGoal, IKTransformInfo transformHint, AvatarIKGoal goal, AvatarIKHint hint)
	{
		bool isFoot = goal.ToString().EndsWith("Foot");

		//Check collision for goals (hands / foot)
		PhysicCast(currentPose, raycast, transformGoal, transformHint, (v) =>
		{
			if (v != null)
			{
				KinetixCore.Animation.SetIKPositionAndWeightOnLocalPlayer(goal, v.Value, 1);
				KinetixCore.Animation.SetIKHintPositionOnLocalPlayer(hint, transformGoal.globalPosition + (transformHint.globalPosition - transformGoal.globalPosition) * 2);

				if (isFoot)
					KinetixCore.Animation.SetIKRotationAndWeightOnLocalPlayer(goal, transformGoal.globalRotation, 1);

				//We didn't compute any rotation for the hands
			}
			else
			{
				//Reset weight
				KinetixCore.Animation.SetIKPositionWeightOnLocalPlayer(goal, 0);
				KinetixCore.Animation.SetIKRotationWeightOnLocalPlayer(goal, 0);
			}
		});

		//We can add for hint collision for example.
	}

	/// <summary>
	/// Method to compute the IK physic on a goal
	/// </summary>
	/// <param name="currentPose">Current IK pose info</param>
	/// <param name="raycast">Raycast object (to compute overlap and other physic stuff)</param>
	/// <param name="transformGoal">Transform info of the Goal (hand / foot)</param>
	/// <param name="transformHint">Transform info of the Hint (elbow / knee)</param>
	/// <param name="applyPhysic"></param>
	private void PhysicCast(IKInfo currentPose, ARaycastObject raycast, IKTransformInfo transformGoal, IKTransformInfo transformHint, Action<Vector3?> applyPhysic)
	{
		/*
			--- Gizmo colors ---
			Wired BOX / Wired Sphere : RayCast object (red when there is an overlap)
			Gray line : Skeleton of the pose (not in this function)
			
			Yellow solid sphere : Position to return (applyPhysic)
			Green line (near yellow sphere) : Previous position to corrected position
			
			Small Red sphere and red line : Nearest point on overlapped colliders
			
			White and magenta line : 'confirmation raycast' (box/sphere cast to get the actual collision point)

			Cyan solid sphere and cyan line : Actual collision point and normal

			Green sphere : Farthest point in the collided wall
		*/

		gizmo.Color(GIZMO_COLOR);

		//Check for colliders
		Collider[] colliders = raycast.Overlap(transformGoal.globalPosition, transformGoal.globalRotation, overlapLayerMask).Distinct().ToArray();
		int collidersLenght = colliders.Length;

		bool defaultQuery = Physics.queriesHitBackfaces; //We need to restore physics after we're done computing the physic

		Vector3? toReturn = null;
		//Compute the offset of the raycast
		Vector3 offset = transformGoal.globalRotation * raycast.offset;
		//Compute hips -> goal
		Vector3 hipsToGoal = transformGoal.globalPosition - currentPose.hips.globalPosition;
		//Compute hint -> goal (+offset)
		Vector3 fromHintToGoal = (transformGoal.globalPosition + offset) - transformHint.globalPosition;

		//If an overlap has been detected
		if (collidersLenght > 0)
		{
			Physics.queriesHitBackfaces = false; //disable back faces

			Vector3 point, fromTo, hitPosition, hitNormal;
			Vector3 moveDirection = Vector3.zero;//To be computed
			for (int i = collidersLenght - 1; i >= 0; i--)
			{
				Collider collider = colliders[i];
				bool colliderIsSelf = collider.GetComponentInParent<Animator>() == myAnimator;

				Vector3 rayOrigin = colliderIsSelf ? transformGoal.globalPosition : currentPose.hips.globalPosition;

				point = collider.ClosestPointOnBounds(transformGoal.globalPosition + offset);
				fromTo = point - rayOrigin;

				//If it's not interpenetrating, make the raycast point in the direction of the leg/arm.
				//This is to fix backface normal that are on the opposite side
				if (!colliderIsSelf)
					fromTo *= Mathf.Sign(Vector3.Dot(hipsToGoal, fromTo));

				if (fromTo == Vector3.zero) //Security
					fromTo = offset;

				#region GIZMO
				//Nearest point on collider's bounding box
				Debug.DrawLine(transformGoal.globalPosition + offset, point, new Color(1, 0, 0, 0.2f));
				gizmo.Color(GIZMO_COLOR_HIT);
				gizmo.DrawSphere(point, 0.005f);

				//Raycast
				Debug.DrawRay(offset + rayOrigin - fromTo.normalized * RAY_COLISION_OFFSET, fromTo.normalized * (fromTo.magnitude + RAY_COLISION_OFFSET), Color.white);
				Debug.DrawRay(offset + rayOrigin - fromTo.normalized * RAY_COLISION_OFFSET, fromTo.normalized * (fromTo.magnitude + RAY_COLISION_OFFSET) * 0.2f, Color.magenta);
				#endregion

				//Do a raycast to get the actual position of the hit
				if (!raycast.Raycast(rayOrigin - fromTo.normalized * RAY_COLISION_OFFSET, transformGoal.globalRotation, fromTo, overlapLayerMask, out RaycastHit hit, fromTo.magnitude + RAY_COLISION_OFFSET, collider))
				{
					--collidersLenght;
					continue;
				}

				hitNormal = hit.normal;
				hitPosition = hit.point;

				//If it's not interpenetrating, make the normal point in the opposite direction of the leg/arm.
				//This is to fix backface normal that are on the opposite side
				if (!colliderIsSelf)
					hitNormal *= Mathf.Sign(Vector3.Dot(hipsToGoal, -hit.normal));

				//Get the point on our RaycastObject
				Vector3 pointOnCollider = raycast.FarthestPointOnCollider(transformGoal.globalPosition, transformGoal.globalRotation, -hitNormal);
				//Add a 'velocity' to correct the position in the direction of the normal
				Vector3 moveVelocity = Vector3.Dot(hitNormal, hitPosition - pointOnCollider) * hitNormal;
				moveDirection += moveVelocity;

				#region GIZMO
				//Normal
				gizmo.Color(Color.cyan);
				Debug.DrawRay(hitPosition, hitNormal, Color.cyan);
				gizmo.DrawSphere(hitPosition, 0.01f);
				//Move velocity
				Debug.DrawRay(hitPosition, moveVelocity, Color.green);

				//Point on collider
				gizmo.Color(Color.green);
				gizmo.DrawSphere(pointOnCollider, 0.01f);
				#endregion
			}

			//If we got at least 1 good raycast, add the moveDirection to "toReturn"
			if (collidersLenght > 0)
			{
				toReturn = transformGoal.globalPosition + moveDirection;

				//Reset color to "color hit" (for the raycastObject's gizmo)
				gizmo.Color(GIZMO_COLOR_HIT);
			}
			else
			{
				toReturn = null;
			}
		}
		else if (Physics.Raycast(transformHint.globalPosition - fromHintToGoal, fromHintToGoal, out RaycastHit hit, fromHintToGoal.magnitude * 2, raycastLayerMask))
		{
			//Draw the raycast
			Debug.DrawRay(transformHint.globalPosition - fromHintToGoal, fromHintToGoal * 2, Color.blue);

			//Init some variables
			Vector3 hitPosition = hit.point;
			Vector3 hitNormal = hit.normal;
			Vector3 fromTo = hitPosition - transformHint.globalPosition;
			//Test if the Hit is in the direction of the raycast (minimum -45deg, maximum +225deg)
			if (Vector3.Dot(fromTo, fromHintToGoal) > -0.5f)
			{
				//Get the point on our RaycastObject
				Vector3 pointOnCollider = raycast.FarthestPointOnCollider(transformGoal.globalPosition, transformGoal.globalRotation, -hitNormal);
				//Add a 'velocity' to correct the position in the direction of the normal
				Vector3 moveVelocity = Vector3.Dot(hitNormal, hitPosition - pointOnCollider) * hitNormal;
				toReturn = transformGoal.globalPosition + moveVelocity;

				#region GIZMO
				//Point on collider
				gizmo.Color(Color.green);
				gizmo.DrawSphere(pointOnCollider, 0.01f);

				//Hit
				gizmo.Color(GIZMO_COLOR_HIT);
				gizmo.DrawSphere(hitPosition, 0.01f);
				#endregion
			}

			gizmo.Color(GIZMO_COLOR);
		}
		Physics.queriesHitBackfaces = defaultQuery;

		#region GIZMO
		//Draw the raycastObject's gizmo
		gizmo.AddGizmo(DrawGizmo);
		void DrawGizmo() => raycast.DrawGizmo(transformGoal.globalPosition, transformGoal.globalRotation);

		//Draw the final position
		if (toReturn.HasValue)
		{
			gizmo.Color(Color.yellow);
			gizmo.DrawSphere(toReturn.Value, 0.01f);
			Debug.DrawLine(transformGoal.globalPosition, toReturn.Value, GIZMO_COLOR);
		}

		gizmo.Color(Color.white);
		#endregion

		applyPhysic(toReturn);
	}
}

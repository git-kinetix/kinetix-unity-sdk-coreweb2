using Kinetix;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IK_NoGroundClipping : MonoBehaviour
{
	[SerializeField] private StepDisplayController stepDisplayController;
	[SerializeField] private Animator myAnimator;
	[SerializeField] private TMP_InputField apiKeyField;
	[SerializeField] private TMP_InputField usernameField;
	[SerializeField] private TMP_Text usernameLabel;
	[SerializeField] private TMP_Text animationName;
	[SerializeField] private Image animationIcon;
	[SerializeField] private bool rootMotionEnabled = true;
	private string animationID;

	[Header("Physic")]
	[SerializeField] private LayerMask raycastLayerMask;
	[SerializeField] private float raycastSphereRadius = 0.1f;
	[SerializeField] private float footHeight = 0.1f;
	[SerializeField] private float footOffset = 0.1f;

	[Header("Smooth Physic")]
	[SerializeField] private float lerpIkWeight = 10;
	[SerializeField] private float ikInterpolationSpeedPosition = 50f;
	[SerializeField] private float ikInterpolationSpeedRotation = 180f;

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
		KinetixCore.Animation.RegisterLocalPlayerAnimator(myAnimator, new RootMotionConfig(rootMotionEnabled, false, rootMotionEnabled));

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

	//Lerp variables
	float ikWeightL = 0;
	float ikWeightR = 0;

	Vector3 previousLeftFootIK;
	Vector3 previousRightFootIK;
	Quaternion previousLeftFootIKRot;
	Quaternion previousRightFootIKRot;
	private void OnBeforeIK(IKInfo currentPose)
	{
		//Set backface to true and keep previous queries
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		
		//There is no weight so we reset ik position to current animation position
		if (ikWeightL == 0)
		{
			previousLeftFootIK = currentPose.leftFoot.globalPosition;
			previousLeftFootIKRot = currentPose.leftFoot.globalRotation;
		}

		if (ikWeightR == 0)
		{
			previousRightFootIK = currentPose.rightFoot.globalPosition;
			previousRightFootIKRot = currentPose.rightFoot.globalRotation;
		}

		//Left foot
		ComputeFootRaycast(AvatarIKHint.LeftKnee, AvatarIKGoal.LeftFoot, currentPose.leftFoot, currentPose.leftLowerLeg, ref ikWeightL);

		//Right foot
		ComputeFootRaycast(AvatarIKHint.RightKnee, AvatarIKGoal.RightFoot, currentPose.rightFoot, currentPose.rightLowerLeg, ref ikWeightR);

		//----------------//
		// Lerp
		//----------------//
		if (ikWeightL != 0)
		{
			//Interpolate rotation and position based on ikWeightL
			KinetixCore.Animation.SetIKPositionAndWeightOnLocalPlayer(AvatarIKGoal.LeftFoot,
				previousLeftFootIK = Vector3.MoveTowards(previousLeftFootIK, KinetixCore.Animation.GetIKPositionOnLocalPlayer(AvatarIKGoal.LeftFoot), ikInterpolationSpeedPosition * Time.deltaTime),
				ikWeightL
			);
			KinetixCore.Animation.SetIKRotationAndWeightOnLocalPlayer(AvatarIKGoal.LeftFoot,
				previousLeftFootIKRot = Quaternion.RotateTowards(previousLeftFootIKRot, KinetixCore.Animation.GetIKRotationOnLocalPlayer(AvatarIKGoal.LeftFoot), ikInterpolationSpeedRotation * Time.deltaTime),
				ikWeightL
			);
		}
		
		if (ikWeightR != 0)
		{
			//Interpolate rotation and position based on ikWeightR
			KinetixCore.Animation.SetIKPositionAndWeightOnLocalPlayer(AvatarIKGoal.RightFoot,
				previousRightFootIK = Vector3.MoveTowards(previousRightFootIK, KinetixCore.Animation.GetIKPositionOnLocalPlayer(AvatarIKGoal.RightFoot), ikInterpolationSpeedPosition * Time.deltaTime),
				ikWeightR
			);
			KinetixCore.Animation.SetIKRotationAndWeightOnLocalPlayer(AvatarIKGoal.RightFoot,
				previousRightFootIKRot = Quaternion.RotateTowards(previousRightFootIKRot, KinetixCore.Animation.GetIKRotationOnLocalPlayer(AvatarIKGoal.RightFoot), ikInterpolationSpeedRotation * Time.deltaTime),
				ikWeightR
			);
		}
		
		Physics.queriesHitBackfaces = queriesHitBackfaces; //Restore previous setting
		gizmo.Color(Color.white);

		//COMPUTE RAYCAST
		void ComputeFootRaycast(AvatarIKHint hint, AvatarIKGoal goal, IKTransformInfo foot, IKTransformInfo lowerLeg, ref float ikWeight)
		{
			//Some physic constants
			const float DOWN_CAST = 0.12f;
			const float UP_CAST = 4f;
			const float NORMAL_SIZE = 1.5f;

			bool hasCollied = false;
			
			//If the foot is near the ground, fix it to the grouns
			if (Physics.Raycast(new Ray(foot.globalPosition, Vector3.down * DOWN_CAST), out RaycastHit hit, DOWN_CAST, raycastLayerMask))
			{
				Vector3 hitNormal = hit.normal;
				hitNormal *= Mathf.Sign(Vector3.Dot(Vector3.up, hitNormal)); //Put the normal in the direction of the raycast
				hasCollied = true;

				#region GIZMO
				Debug.DrawRay(hit.point, hitNormal * NORMAL_SIZE, Color.magenta);
				Debug.DrawLine(foot.globalPosition, hit.point, Color.red);
				#endregion

				//Call IK api
				KinetixCore.Animation.SetIKHintPositionOnLocalPlayer(hint, lowerLeg.globalPosition);
				KinetixCore.Animation.SetIKPositionAndWeightOnLocalPlayer(goal, hit.point + footHeight * hitNormal.normalized, 1);
				KinetixCore.Animation.SetIKRotationAndWeightOnLocalPlayer(goal, Quaternion.FromToRotation(Vector3.up, hitNormal) * Quaternion.Euler(0, foot.globalRotation.eulerAngles.y, 0), 1);

			}
			else
			{
				Debug.DrawRay(foot.globalPosition, Vector3.down * DOWN_CAST, Color.green);
			}

			Vector3 footOrigin = foot.globalPosition + currentPose.hips.globalRotation * Vector3.down * footOffset;

			#region GIZMO
			gizmo.Color(Color.green);
			gizmo.DrawWireSphere(footOrigin, raycastSphereRadius);
			#endregion

			//If the foot is inside a collider, put it on top of it
			if (Physics.SphereCast(new Ray(footOrigin, Vector3.up), raycastSphereRadius, out hit, UP_CAST, raycastLayerMask))
			{
				Vector3 hitNormal = hit.normal;
				hitNormal *= Mathf.Sign(Vector3.Dot(Vector3.up, hitNormal)); //Put the normal in the direction of the raycast
				hasCollied = true;

				//Call IK api
				KinetixCore.Animation.SetIKHintPositionOnLocalPlayer(hint, lowerLeg.globalPosition + hit.point - foot.globalPosition);
				KinetixCore.Animation.SetIKPositionAndWeightOnLocalPlayer(goal, hit.point + footHeight * hitNormal.normalized, 1);
				KinetixCore.Animation.SetIKRotationAndWeightOnLocalPlayer(goal, Quaternion.FromToRotation(Vector3.up, hitNormal) * Quaternion.Euler(0, foot.globalRotation.eulerAngles.y, 0), 1);

				#region GIZMO
				gizmo.Color(Color.red);
				gizmo.DrawWireSphere(hit.point, raycastSphereRadius + 0.01f);
				Debug.DrawRay(hit.point, hitNormal * NORMAL_SIZE, Color.cyan);

				Debug.DrawRay(hit.point, Quaternion.FromToRotation(Vector3.up, hitNormal) * Vector3.up, Color.green);
				Debug.DrawRay(hit.point, Quaternion.FromToRotation(Vector3.up, hitNormal) * Vector3.right, Color.red);
				Debug.DrawRay(hit.point, Quaternion.FromToRotation(Vector3.up, hitNormal) * Vector3.forward, Color.blue);
				#endregion
			}

			//No collision :
			// - Decrease weight
			//Else
			// - Increase weight
			if (!hasCollied)
			{
				ikWeight -= lerpIkWeight * Time.deltaTime;
				if (ikWeight < 0)
					ikWeight = 0;

				KinetixCore.Animation.SetIKPositionWeightOnLocalPlayer(goal, ikWeight);
				KinetixCore.Animation.SetIKRotationWeightOnLocalPlayer(goal, ikWeight);
			}
			else
			{
				ikWeight += lerpIkWeight * Time.deltaTime;
				if (ikWeight > 1)
					ikWeight = 1;
			}
		}
	}
}

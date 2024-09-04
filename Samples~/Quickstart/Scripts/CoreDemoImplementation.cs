// // ----------------------------------------------------------------------------
// // <copyright file="BasicImplementation.cs" company="Kinetix SAS">
// // Kinetix Unity SDK - Copyright (C) 2022 Kinetix SAS
// // </copyright>
// // ----------------------------------------------------------------------------
using UnityEngine;
using Kinetix;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class CoreDemoImplementation : MonoBehaviour
{
    [SerializeField] protected StepDisplayController stepDisplayController;
    [SerializeField] protected Animator myAnimator;
    [SerializeField] protected TMP_InputField apiKeyField;
    [SerializeField] protected TMP_InputField usernameField;
    [SerializeField] protected TMP_Text usernameLabel;
    [SerializeField] protected TMP_Text animationName;
    [SerializeField] protected Image animationIcon;
    protected string animationID;


    public void OnValidateGameApiKey() 
    {
        KinetixCore.OnInitialized           += OnInitialize;
        KinetixCore.Initialize(new KinetixCoreConfiguration()
        {
            GameAPIKey = apiKeyField.text,
            PlayAutomaticallyAnimationOnAnimators = true,
            ShowLogs = true
        });
    }

    // This callback is used for the actions made after the SDK is initialized
    // Such as initializing the UI and Registering our LocalPlayer's animator
    protected void OnInitialize()
    {
        // Register local player to receive animation
        // See "Animation System" documentation
        KinetixCore.Animation.RegisterLocalPlayerAnimator(myAnimator);

        stepDisplayController.NextStep();
    }

    protected IEnumerator FetchEmotesAtInterval()
    {
        GetPlayerEmotes();

        while (enabled)
        {
            // Fetch emotes every 5 minutes
            yield return new WaitForSeconds(300);
            
            GetPlayerEmotes();
        }
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

            StartCoroutine(FetchEmotesAtInterval());
        });
    }
    // Maybe use a coroutine to fetch the anim every 5 minutes ?
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
}

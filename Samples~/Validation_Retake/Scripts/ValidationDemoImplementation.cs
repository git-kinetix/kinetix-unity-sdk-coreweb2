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
using Kinetix.Internal;
using System.Collections.Generic;

public class ValidationDemoImplementation : CoreDemoImplementation
{
    private bool hasFiredEventThisCycle = false;
    private List<SdkApiProcess> sdkApiProcesses;
    private SdkApiProcess lastProcess;
    private Coroutine processCoroutine;

    public new void OpenUGELink()
    {
        // Example on how to get the link
        KinetixCore.UGC.GetUgcUrl((_Url) => {
            Application.OpenURL(_Url);

            stepDisplayController.NextStep();

            processCoroutine = StartCoroutine(FetchProcessesAtInterval());
        });
    }

    private IEnumerator FetchProcessesAtInterval()
    {
        GetPlayerProcesses();

        while (enabled)
        {
            yield return new WaitForSeconds(5f);
            
            GetPlayerProcesses();
        }
    }

    private void RefreshProcessList(SdkApiProcess[] _Processes)
    {
        List<SdkApiProcess> filteredProcesses = GetFilteredProcesses(_Processes);
        sdkApiProcesses ??= new List<SdkApiProcess>();

        int previousProcessCount = sdkApiProcesses.Count;

        hasFiredEventThisCycle = false;

        sdkApiProcesses.Clear();

        foreach (SdkApiProcess apiProcess in filteredProcesses)
        {
            if (!apiProcess.CanBeValidatedOrRejected)
                continue;
            
            if (!hasFiredEventThisCycle && apiProcess.Progression == 100)
            {
                hasFiredEventThisCycle = true;

                lastProcess = apiProcess;

                KinetixCore.Metadata.GetAnimationMetadataByAnimationIds(apiProcess.Emote.ToString(), (_AnimationMetadata) => {
                    StopCoroutine(processCoroutine);
                    
                    stepDisplayController.NextStep();
                });
            }
        }
    }

    public void OnProcessValidated()
    {
        OnProcessValidated(lastProcess);
    }

    public void OnProcessValidated(SdkApiProcess _Process)
    {
        KinetixCore.Process.ValidateEmoteProcess(_Process.Uuid.ToString(), (_Process) => {
            Debug.Log("Process validated");

            stepDisplayController.NextStep();
        }, () => {
            Debug.LogError("Unable to validate the emote");
        });
    }

    public void OnProcessRejected()
    {
        OnProcessRejected(lastProcess);
    }
    
    public void OnProcessRejected(SdkApiProcess _Process)
    {
        KinetixCore.Process.RetakeEmoteProcess(_Process.Uuid.ToString(), (_RetakeUGCUrl) => {
            Application.OpenURL(_RetakeUGCUrl);
            stepDisplayController.PreviousStep();
        }, () => {
            Debug.LogError("Unable to retake the emote");
        });
    }

    public void GetPlayerProcesses()
    {
        // We get the animation 
        KinetixCore.Process.GetConnectedAccountProcesses(RefreshProcessList, () => {
            Debug.LogError("Unable to get processes");
        });
    }

    private List<SdkApiProcess> GetFilteredProcesses(SdkApiProcess[] _Processes)
    {
        List<SdkApiProcess> filteredProcesses = new List<SdkApiProcess>();
        
        foreach (SdkApiProcess process in _Processes)
        {
            if (!process.CanBeValidatedOrRejected)
                continue;
            
            filteredProcesses.Add(process);
        }

        return filteredProcesses;
    }

    public void PreviewEmote()
    {
        PlayEmote(lastProcess.Emote.ToString());
    }

    public void PlayEmote(string _EmoteId)
    {
        KinetixCore.Metadata.GetAnimationMetadataByAnimationIds(_EmoteId, (_AnimationMetadata) => {
            // Finally we can play the animation on our local player
            KinetixCore.Animation.PlayAnimationOnLocalPlayer(_AnimationMetadata.Ids.UUID);
        });
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepDisplayController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] stepsPanels;
    private int currentStepIndex = 0;

    public void NextStep()
    {
        if (currentStepIndex == stepsPanels.Length - 1)
            return;

        stepsPanels[currentStepIndex].SetActive(false);
        
        currentStepIndex++;

        stepsPanels[currentStepIndex].SetActive(true);
    }

    public void PreviousStep()
    {
        if (currentStepIndex == 0)
            return;

        stepsPanels[currentStepIndex].SetActive(false);
        
        currentStepIndex--;

        stepsPanels[currentStepIndex].SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentationLinkController : MonoBehaviour
{
    public void GoToLink()
    {
        Application.OpenURL("https://docs.kinetix.tech/integration/integrate-kinetix-sdk/sdk-integration-in-unity/quickstart-unity-sdk");
    }
}

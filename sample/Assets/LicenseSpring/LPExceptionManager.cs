using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LicenseSpring.Unity
{

    /// <summary>
    /// License Spring Exception Manager
    /// </summary>

    [DefaultExecutionOrder(-5)]
    public class LPExceptionManager : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            Application.logMessageReceived += LicenseSpringLogMessage;
            DontDestroyOnLoad(this);
        }

        private void LicenseSpringLogMessage(string condition, string stackTrace, LogType type)
        {
            throw new NotImplementedException();
        }

    } 
}

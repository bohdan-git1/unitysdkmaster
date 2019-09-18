using LicenseSpring;
using LicenseSpring.Unity.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// InGame UI
/// </summary>
public class LicenseSpringUI : MonoBehaviour
{
    [SerializeField]
    private InputField Email;
    [SerializeField]
    private InputField Key;

    [SerializeField]
    private Button Submit;
    [SerializeField]
    private GameObject MainPanel;
    [SerializeField]
    private GameObject TrialPanel;

    private bool            _isTrial = false;
    private bool            _isActive = false;

    private License         _currentLicense = null;
    private LicenseStatus   _currentLicenseStatus = LicenseStatus.Unknown;

    public void SetStatus(License licenseData)
    {
        if(licenseData == null)
        {
            _currentLicenseStatus = LicenseStatus.Unknown;
            _currentLicense = null;
        }
        else
        {
            _currentLicense = licenseData;
            _currentLicenseStatus = licenseData.Status();

        }
    }

    private void Awake()
    {
        //events registrations
        Submit.onClick.AddListener(new UnityEngine.Events.UnityAction(SubmitClick));
    }

    private void Update()
    {
        if (_currentLicense == null)
        {
            MainPanel.SetActive(true);
            TrialPanel.SetActive(false);
            return;
        }

        if (_currentLicenseStatus == LicenseStatus.Active)
        {
            MainPanel.SetActive(false);

            if (_currentLicense.IsTrial())
            {
                TrialPanel.SetActive(true);
            }
        }
        else
        {
            MainPanel.SetActive(true);
            TrialPanel.SetActive(false);
        }
    }

    private void SubmitClick()
    {
        if(!string.IsNullOrEmpty( Email.text))
        {
            string trialKey = LicenseSpringUnityManager.Instance.AppLicenseManager.GetTrialKey(email: Email.text);
            _currentLicense = (License) LicenseSpringUnityManager.Instance.AppLicenseManager.ActivateLicense(trialKey);
        }

        if(string.IsNullOrEmpty(Key.text))
        {
            _currentLicense =(License) LicenseSpringUnityManager.Instance.AppLicenseManager.ActivateLicense(Key.text);
        }

        var installationFile = _currentLicense.Check();
        if(installationFile != null)
            Application.Quit();

    }
}

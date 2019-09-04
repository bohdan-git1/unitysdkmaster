using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LicenseSpring.Unity
{
    /// <summary>
    /// Internal UI, 
    /// This popup is only run at runtime when asset or game bought without proper key/license.
    /// </summary>

    public class LicenseSpringUI : MonoBehaviour, ILicenseSpringMessaging
    {
        public const string UI_NAME = "LICENSESPRING";
        public Font CustomFont;

        public Sprite BackgroundWarning,
            BackgroundAskLicense;

        // Start is called before the first frame update
        public GameObject PanelWarning;
        public GameObject PanelLicense;
        public GameObject PanelTrialIndicator;

        public Button   SubmitLicense;

        private LicenseSpringInfo _messageSender;

        private Image _BgWarning;
        private Image _BgLicense;

        private Text _TextWarning, 
            _TextError;

        private InputField EmailInput;
        private InputField SerialInput;

        public void Message(UnityLicenseMessageType messageType, string content)
        {
            switch (messageType)
            {
                case UnityLicenseMessageType.Trial:

                    break;
                case UnityLicenseMessageType.LicenseInActive:
                    PanelLicense.SetActive(true);
                    break;
                case UnityLicenseMessageType.LicenseExpired:
                    break;
                case UnityLicenseMessageType.LicenseInvalid:
                    PanelLicense.SetActive(true);

                    break;
                default:
                    break;
            }
        }

        public void SetSender(GameObject sender)
        {
            _messageSender = (LicenseSpringInfo)sender.GetComponent<LicenseSpringInfo>();

        }

        private void Awake()
        {
            _BgWarning = PanelWarning.GetComponent<Image>();
            _BgLicense = PanelLicense.GetComponent<Image>();

            _BgLicense.sprite = BackgroundAskLicense == null ? _BgLicense.sprite : BackgroundAskLicense;
            _BgWarning.sprite = BackgroundWarning == null ? _BgWarning.sprite : BackgroundWarning;

            SubmitLicense.onClick.AddListener(new UnityEngine.Events.UnityAction(SubmitLicenseClick));

            var inputFields = GetComponentsInChildren<InputField>();

            EmailInput = inputFields.ToList().SingleOrDefault(s => s.name == "EmailInput");
            SerialInput = inputFields.ToList().SingleOrDefault(s => s.name == "SerialInput");

            PanelLicense.SetActive(false);
            PanelTrialIndicator.SetActive(false);
            PanelWarning.SetActive(false);
        }

        private void SubmitLicenseClick()
        {
            if (!string.IsNullOrEmpty(SerialInput.text))
                _messageSender.RegisterTrial(EmailInput.text);
            else
                _messageSender.Register(EmailInput.text);

        }
    } 
}

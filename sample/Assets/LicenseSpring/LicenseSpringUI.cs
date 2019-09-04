using UnityEngine;
using UnityEngine.UI;

namespace LicenseSpring.Unity
{

    public class LicenseSpringUI : MonoBehaviour, ILicenseSpringMessaging
    {
        public Font CustomFont;

        public Sprite BackgroundWarning,
            BackgroundAskLicense;

        // Start is called before the first frame update
        public GameObject PanelWarning;
        public GameObject PanelLicense;
        public GameObject PanelTrialIndicator;

        private Image BgWarning;
        private Image BgLicense;

        private void Awake()
        {
            BgWarning = PanelWarning.GetComponent<Image>();
            BgLicense = PanelLicense.GetComponent<Image>();

            BgLicense.sprite = BackgroundAskLicense == null ? BgLicense.sprite : BackgroundAskLicense;
            BgWarning.sprite = BackgroundWarning == null ? BgWarning.sprite : BackgroundWarning;
        }

    } 
}

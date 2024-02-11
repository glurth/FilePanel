using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace EyE.Unity.UI
{
    /// <summary>
    /// Attached as a component of a Canvas element.  This canvas element is required to contain, and reference, the appropriate text and yes/no buttons.
    /// The buttons themselves, will have listeners added to in their OnClick events.  These will invoke the appropriate OnClickYes and OnClickNo functions in this class.
    /// To call the Open members one must pass a callback function, that takes an integer as a parameter.  This function will be called when the YesNo window is closed.
    /// Note, this object that opens this Panel will NOT be automatically hidden.  But, that object may choose to hide/unhide *itself* when it opens the YesNoPanel and when its callback function is invoked.
    /// </summary>
    public class YesNoPanel : MonoBehaviour
    {
        const string defaultMessageText = "Confirm";
        const string defaultYesButtonText = "Yes";
        const string defaultNoButtonText = "No";

        static public YesNoPanel _instance = null;
        static public YesNoPanel instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<YesNoPanel>(true);
                if (_instance == null)
                {
                    Debug.LogError("Unable To Find a YesNoModal in the scene.");
                    throw new System.ArgumentNullException("Unable To Find a YesNoModal in the scene.");
                }
                return _instance;
            }
            set { _instance = value; }
        }

        /// <summary>
        /// assigned when Open is called.  Will be invoked when the user clicks the yes or no button.
        /// </summary>
        System.Action<int> callbackFunction;

        public TextMeshProUGUI messageTextControl;
        public Button yesButtonControl;
        public Button noButtonControl;

        TextMeshProUGUI yesButtonTextControl;
        TextMeshProUGUI noButtonTextControl;
        private void Awake()
        {
            yesButtonControl.onClick.AddListener(OnClickYes);
            noButtonControl.onClick.AddListener(OnClickNo);

            yesButtonTextControl = yesButtonControl.GetComponentInChildren<TextMeshProUGUI>();
            noButtonTextControl = noButtonControl.GetComponentInChildren<TextMeshProUGUI>();
        }
        private void Update()
        {
            if (callbackFunction == null)
                gameObject.SetActive(false);
        }
        /// <summary>
        /// Used to open the YesNoPanel if you have a reference to it.
        /// </summary>
        /// <param name="callOnClose">pass a function that takes an int as it's parameter.  This function will be invoked and passed in the users selection, when made.</param>
        /// <param name="messageText">the text that will be displayed for the user</param>
        /// <param name="yesButtonText">optional: override this button's text</param>
        /// <param name="noButtonText">optional: override this button's text</param>
        public void OpenNow(System.Action<int> callOnClose, string messageText = defaultMessageText, string yesButtonText = defaultYesButtonText, string noButtonText = defaultNoButtonText)
        {
            messageTextControl.text = messageText;
            if (yesButtonTextControl != null)
                yesButtonTextControl.text = yesButtonText;
            if (noButtonTextControl != null)
                noButtonTextControl.text = noButtonText;
            callbackFunction = callOnClose;
            gameObject.SetActive(true);
        }
        /// <summary>
        /// Used to open the YesNoPanel singleton.  At least one YesNoPanel must exist in the scene for this static function to work. (It will use the first one it happens to find with FindObjectOfType)
        /// </summary>
        /// <param name="callOnClose">pass a function that takes an int as it's parameter.  This function will be invoked and passed in the users selection, when made.</param>
        /// <param name="messageText">the text that will be displayed for the user</param>
        /// <param name="yesButtonText">optional: override this button's text</param>
        /// <param name="noButtonText">optional: override this button's text</param>
        static public void Open(System.Action<int> callOnClose, string messageText = defaultMessageText, string yesButtonText = defaultYesButtonText, string noButtonText = defaultNoButtonText)
        {
            instance.OpenNow(callOnClose, messageText, yesButtonText, noButtonText);
            //ensure it's drawn on top of any siblings
            instance.transform.SetSiblingIndex(instance.transform.parent.childCount - 1);
        }
        /// <summary>
        /// this callback is attached to button in scene
        /// </summary>
        public void OnClickYes()
        {
            gameObject.SetActive(false);
            callbackFunction(1);
        }
        /// <summary>
        /// this callback is attached to button in scene
        /// </summary>
        public void OnClickNo()
        {
            gameObject.SetActive(false);
            callbackFunction(0);
        }
    }
}
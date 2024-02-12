using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

namespace EyE.Unity.UI
{

    static class GiveHoverDetect
    {
        /// <summary>
        /// This function will find any event triggers on the specified object (and, optionally, it's children) and make them respond to PointerEnter and PointerExit UI events, such that they will call the parameter UnityActions when triggered.
        /// This functionality requires that the EventTriggers be associated with (on the same object as) some kind of canvas graphic component that.
        /// If no EventTrigger is found (children searched, optionally) one will be added to the provided GameObject.  
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handlePointerEnter">This function will be called when PointerEnter is triggered.</param>
        /// <param name="handlePointerExit">This function will be called when PointerExit is triggered.</param>
        /// <param name="childrenEventTriggersToo">when set to true, this function will search and use EventTrigger's found in child objects</param>
        static public void GiveHover(this GameObject go, UnityAction<BaseEventData> handlePointerEnter, UnityAction<BaseEventData> handlePointerExit, bool childrenEventTriggersToo = false)
        {
            EventTrigger eventTriggerComponent = go.GetComponent<EventTrigger>();
            bool hasUsableChildrenEventTriggers = false;
            EventTrigger[] childrenEventTriggers = null;
            if (childrenEventTriggersToo)
            {
                childrenEventTriggers = go.GetComponentsInChildren<EventTrigger>();
                hasUsableChildrenEventTriggers = childrenEventTriggers.Length > 0;
            }
            
            if (eventTriggerComponent == null && !hasUsableChildrenEventTriggers)
                eventTriggerComponent = go.AddComponent<EventTrigger>();

            EventTrigger.Entry onPointerEnterTriggerr=null;
            if (handlePointerEnter != null)
            {
                onPointerEnterTriggerr = new EventTrigger.Entry();
                onPointerEnterTriggerr.eventID = EventTriggerType.PointerEnter;
                onPointerEnterTriggerr.callback.AddListener(handlePointerEnter);
                eventTriggerComponent.triggers.Add(onPointerEnterTriggerr);
            }

            EventTrigger.Entry onPointerExitTrigger=null;
            if (handlePointerExit != null)
            {
                onPointerExitTrigger = new EventTrigger.Entry();
                onPointerExitTrigger.eventID = EventTriggerType.PointerExit;
                onPointerExitTrigger.callback.AddListener(handlePointerExit);
                eventTriggerComponent.triggers.Add(onPointerExitTrigger);
            }

            if (childrenEventTriggersToo)
            {
                foreach (EventTrigger cet in childrenEventTriggers)
                {
                    if (cet != eventTriggerComponent)//we added this one above
                    {
                        if(onPointerEnterTriggerr!=null)
                            cet.triggers.Add(onPointerEnterTriggerr);
                        if(onPointerExitTrigger!=null)
                            cet.triggers.Add(onPointerExitTrigger);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Class is used to display the OS filesystem, allowing the user to select existing files and folders or enter a new filename.
    /// </summary>
    public class FilePanel : MonoBehaviour
    {
        #region memberVariables
        #region UI References
        public TextMeshProUGUI title;
        public TMP_InputField fileNameDisplayAndInput;
        public TMP_InputField searchPatternInput;
        public Button actionButton;
        public TextMeshProUGUI actionButtonTextComponent;
        public Button action2Button;
        public TextMeshProUGUI action2ButtonTextComponent;
        public Button cancelButton;
        public TextMeshProUGUI cancelButtonTextComponent;

        public Button fileItemButtonPreFab; // this prebfab will be used for each line item in the displayed list
        public Transform fileListParent;// this will probably be a "content" object in a scroll view
        public TextMeshProUGUI hoverAndSelectedFileDetails;
        #endregion

        #region inspector config variables

        
        public bool doubleClickFileEqualsActionButton = true;
        public float doubleClickMaxInterval = 0.25f;
        #endregion

        FileSystemInfo selectedFile
        {
            get

            {
                if (displayedFiles == null || selectedFileIndex == -1) return null;
                return displayedFiles[selectedFileIndex];
            }
        }
        int selectedFileIndex = -1;
        UnityAction<FileSystemInfo> fileConfirmedActionCallback;
        UnityAction<FileSystemInfo> fileConfirmedAction2Callback;
        UnityAction canceledCallback;
        //  bool selectExistingFileOnly;
        string defaultFilename;
        bool warnOnExistingFileSelected;
        string existingFileSelectedWarningText;
        bool filterForExtension;
        string fileExtension;
        bool showDirectories = true; //weather or not folder should be shown, if not- directory changing will not be possible.


        string currentPath;
        float lastClickTime = 0; //used to detect doubleclick
        Color unselectedColor; // holds the background color of the last selected object, from BEFORE it was changed to the selected color


        
        public System.Func<FileSystemInfo, string> customGetFileDisplayStringFunction = null;
        public System.Func<FileSystemInfo, string> customGetFileDetailsDisplayStringFunction = null;

        List<Button> instantiatedFileButtons = new List<Button>();
        List<FileSystemInfo> displayedFiles = new List<FileSystemInfo>();
        #endregion
        //exists just for initial test purposes
        private void Start()
        {
            //test open
            Open(
                    titleText: "test Title",
                    selectExistingFileOnly: true,
                    defaultFilename: "",
                    warnOnExistingFileSelected: true,
                    existingFileSelectedWarningText: "Loading this file will lose all unsaved work. Proceed?",
                    filterForExtension: true,
                    fileExtension: "sim",
                    showDirectories: true,
                    actionText: "Load",
                    fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name); },
                    action2Text: "Delete",
                    fileConfirmedAction2Callback: (FileSystemInfo f) => { Debug.Log("Action2 confirmed. file: " + f.Name); },
                    cancelText: "Cancel",
                    canceledCallback: () => { Debug.Log("Cancel confirmed"); }
                );

        }

        public void Open(
            string titleText, //will be displayed at the top of the window
            bool selectExistingFileOnly, //when true user will not be able to enter a filename, they will only be able to choose an existing file
            string defaultFilename, //when allowing file save to new file, this is the default file name it is given.  
            bool warnOnExistingFileSelected, string existingFileSelectedWarningText,  //if and what warning should be shown when an existing file is selected, and accept is clicked
            bool filterForExtension, string fileExtension, //if the starting search filter should be by extension only, and what that extension is
            bool showDirectories,
            string actionText, UnityAction<FileSystemInfo> fileConfirmedActionCallback, //text that should be shown on the action button, and what to do after user has confirmed selection.
            string action2Text = null, UnityAction<FileSystemInfo> fileConfirmedAction2Callback = null,
            string cancelText = null, UnityAction canceledCallback = null,
            System.Func<FileSystemInfo, string> customGetFileDisplayStringFunction = null,
            System.Func<FileSystemInfo, string> customGetFileDetailsDisplayStringFunction = null,
            string startingPath = null)
        {
            #region assignLocalVar
            //assign params to local variables to remember them
            this.defaultFilename = defaultFilename;
            this.filterForExtension = filterForExtension;
            this.fileExtension = fileExtension;
            this.fileConfirmedActionCallback = fileConfirmedActionCallback;
            this.fileConfirmedAction2Callback = fileConfirmedAction2Callback;
            this.canceledCallback = canceledCallback;
            this.warnOnExistingFileSelected = warnOnExistingFileSelected;
            this.existingFileSelectedWarningText = existingFileSelectedWarningText;
            this.showDirectories = showDirectories;
            this.currentPath = startingPath;
            this.customGetFileDisplayStringFunction = customGetFileDisplayStringFunction;
            this.customGetFileDetailsDisplayStringFunction = customGetFileDetailsDisplayStringFunction;
            if (startingPath == null)
                currentPath = Application.dataPath;
            else
                currentPath = Path.Combine(Application.dataPath, startingPath);
            this.selectedFileIndex = -1;
            #endregion

            #region setup display components, subscribe to events, etc..
            //setup display components title, action buttons, etc
            title.text = titleText;
            title.gameObject.SetActive(!string.IsNullOrEmpty(titleText));

            actionButton.onClick.AddListener(HandleActionButtonClicked);
            if (actionButtonTextComponent != null)
                if (!string.IsNullOrEmpty(actionText))
                    actionButtonTextComponent.text = actionText;
                else
                    actionButtonTextComponent.text = "Go";

            if (action2Button != null)
            {
                action2Button.onClick.AddListener(HandleActionButton2Clicked);
                if (action2ButtonTextComponent != null)
                    if (!string.IsNullOrEmpty(action2Text))
                        action2ButtonTextComponent.text = action2Text;
                    else
                        action2ButtonTextComponent.text = "Go";
            }

            cancelButton.onClick.AddListener(canceledCallback);
            cancelButton.onClick.AddListener(() => gameObject.SetActive(false));

            if (cancelButtonTextComponent != null)
                if (!string.IsNullOrEmpty(cancelText))
                    cancelButtonTextComponent.text = cancelText;
                else
                    cancelButtonTextComponent.text = "Cancel";

            // this.selectExistingFileOnly= selectExistingFileOnly;
            fileNameDisplayAndInput.interactable = !selectExistingFileOnly;
            fileNameDisplayAndInput.text = defaultFilename;
            searchPatternInput.text = "*." + fileExtension;
            searchPatternInput.onValueChanged.AddListener(HandleSearchFieldChanged);
            #endregion

            SetupFileDisplayList();



            gameObject.SetActive(true);
        }
        
        void SetupFileDisplayList()
        {
            
            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
            if (!currentDirectory.Exists)
                throw new DirectoryNotFoundException("Trying to show files in non-existent directory.");
            string searchPattern = "*";
            if (!string.IsNullOrEmpty(searchPatternInput.text)) searchPattern = searchPatternInput.text;
            List<FileSystemInfo> filesFound = new List<FileSystemInfo>(currentDirectory.GetFileSystemInfos(searchPattern));
            filesFound.Sort(CompareFilesystemEntries);

           // List<FileSystemInfoWithNameOverride> displayArray;
            
            //insert "up" folder after sort, so it' always on top
            if (showDirectories)
            {
                DirectoryInfo parentDir = currentDirectory.Parent;
                if (parentDir != null && parentDir.Exists)
                {
                    filesFound.Insert(0, parentDir);
                }
            }
            
            void DisplayFile(Button b,FileSystemInfo info)
            {
                IDisplayFileInfo fileDisplay = b.GetComponent<IDisplayFileInfo>();
                string overrideName = null;
                // Debug.Log("current element full name: " + info.FullName + "\n currentDirParent fullname: " + currentDirectory.Parent.FullName);
                if (currentDirectory.Parent != null)
                {
                    if (info.FullName.Equals(currentDirectory.Parent.FullName))
                        overrideName = "..";
                }
                if (fileDisplay != null)
                    fileDisplay.Display(info, overrideName);
                else
                    b.GetComponentInChildren<TextMeshProUGUI>().text = overrideName != null ? overrideName : FileButtonText(info);

            }

            for (int i=0; i < filesFound.Count; i++)
            {
                if (!IsADirectory(filesFound[i]) || showDirectories)
                {
                    if (i < instantiatedFileButtons.Count)
                    {
                        instantiatedFileButtons[i].gameObject.SetActive(true);

                        DisplayFile(instantiatedFileButtons[i], filesFound[i]);
                    }
                    else
                    {
                        Button newElement = Instantiate<Button>(fileItemButtonPreFab, fileListParent.transform);
                        DisplayFile(newElement, filesFound[i]);

                        int deScopingInt = i;
                        ITriggerOnSelect selectTrigger = newElement.GetComponent<ITriggerOnSelect>();
                        if(selectTrigger!=null)
                            selectTrigger.onSelectEvent.AddListener(()=> HandleFileSelected(deScopingInt));

                        newElement.onClick.AddListener(() => { HandleFileClicked(deScopingInt); });

                        newElement.gameObject.GiveHover(
                            (ignored) => { HandleFileMouseOver(deScopingInt); },
                            (ignored) => { HandleFileMouseExit(deScopingInt); });
                        

                        newElement.gameObject.SetActive(true);
                        instantiatedFileButtons.Add(newElement);

                    }
                }
            }
            for (int i = filesFound.Count; i < instantiatedFileButtons.Count; i++)
                instantiatedFileButtons[i].gameObject.SetActive(false);
         
            displayedFiles = filesFound;// new List<FileSystemInfo>(filesFound);
            
        }
        void HandleActionButtonClicked()
        {
            if (selectedFile.Exists && warnOnExistingFileSelected)
                YesNoPanel.Open(HandleConfirmActionClicked, existingFileSelectedWarningText);
            else
                HandleConfirmActionClicked(1);
        }
        void HandleActionButton2Clicked()
        {
            if (selectedFile.Exists && warnOnExistingFileSelected)
                YesNoPanel.Open(HandleConfirmAction2Clicked, existingFileSelectedWarningText);
            else
                HandleConfirmAction2Clicked(1);
        }
        void HandleConfirmActionClicked(int confirm)
        {
            if (confirm != 0)
            {
                fileConfirmedActionCallback.Invoke(selectedFile);
                gameObject.SetActive(false);
            }
        }
        void HandleConfirmAction2Clicked(int confirm)
        {
            if (confirm != 0)
                fileConfirmedAction2Callback.Invoke(selectedFile);
        }


        void HandleFileClicked(int fileIndex)
        {

            if (Time.realtimeSinceStartup - lastClickTime < doubleClickMaxInterval)
                if (selectedFileIndex == fileIndex)
                {
                    HandleFileDoubleClicked(fileIndex);
                    lastClickTime = Time.realtimeSinceStartup;
                    return;
                }

            lastClickTime = Time.realtimeSinceStartup;
            HandleFileSelected(fileIndex);
            //FileSystemInfo fileClicked = displayedFiles[fileIndex];
        }
        void HandleFileSelected(int fileIndex)
        {
            if (selectedFileIndex != fileIndex)
            {
                if (selectedFileIndex != -1)//restore color to previous selected
                {
                    Image previousSelectionBackGround = instantiatedFileButtons[selectedFileIndex].gameObject.GetComponent<Image>();
                    if(previousSelectionBackGround!=null)
                        previousSelectionBackGround.color = unselectedColor;
                }
                selectedFileIndex = fileIndex;
                Image backGround = instantiatedFileButtons[fileIndex].gameObject.GetComponent<Image>();
                if (backGround != null)
                {
                    unselectedColor = backGround.color;
                    backGround.color = Color.yellow;
                }
                if (IsADirectory(displayedFiles[fileIndex]))
                    fileNameDisplayAndInput.text = null; 
                else
                    fileNameDisplayAndInput.text = selectedFile.Name;
            }
            else
            {
                //selectedFileIndex = -1;
                //fileNameDisplayAndInput.text = "";
            }
        }
        void HandleFileDoubleClicked(int fileIndex)
        {
            //we ignore param in favor of "selectedFile"- should be the same after fileIndex lookup
            FileSystemInfo fileDblClicked = selectedFile;
            if (IsADirectory(fileDblClicked))
            {
                currentPath = fileDblClicked.FullName;
                if (selectedFileIndex != -1)//restore color to previous selected
                {
                    Image previousSelectionBackGround = instantiatedFileButtons[selectedFileIndex].gameObject.GetComponent<Image>();
                    if (previousSelectionBackGround != null)
                        previousSelectionBackGround.color = unselectedColor;
                }
                selectedFileIndex = -1;
                SetupFileDisplayList();
                EventSystem.current.SetSelectedGameObject(instantiatedFileButtons[0].gameObject);
                if (displayedFiles.Count > 0)
                    HandleFileSelected(0);
                
            }
            else if (doubleClickFileEqualsActionButton)
                HandleActionButtonClicked();
        }
        void HandleFileMouseOver(int fileIndex)
        {
            hoverAndSelectedFileDetails.text = FileDetailsText(displayedFiles[fileIndex]);
            //hoverAndSelectedFileDetails.DisplayValue = instantiatedFileButtons[fileIndex].FileDisplayed;
        }
        void HandleFileMouseExit(int fileIndex)
        {
            if (selectedFile == displayedFiles[fileIndex]) return;
            hoverAndSelectedFileDetails.text = "";// instantiatedFileButtons[fileIndex].text;
            if (selectedFile != null)
                hoverAndSelectedFileDetails.text = FileDetailsText(selectedFile);
            //hoverAndSelectedFileDetails.DisplayValue = instantiatedFileButtons[fileIndex].FileDisplayed;
        }

        void HandleSearchFieldChanged(string ignored)
        {
            SetupFileDisplayList();
            if (selectedFileIndex != -1)
            {
                Image previousSelectionBackGround = instantiatedFileButtons[selectedFileIndex].gameObject.GetComponent<Image>();
                if (previousSelectionBackGround != null)
                    previousSelectionBackGround.color = unselectedColor;
                selectedFileIndex = -1;
            }
        }

        string FileButtonText(FileSystemInfo fileInfo)
        {
            if (customGetFileDisplayStringFunction != null)
                return customGetFileDisplayStringFunction(fileInfo);
            else
                if (!IsADirectory(fileInfo))
                    return fileInfo.Name + "\t" + fileInfo.LastWriteTime;
                else
                    return "DIR\t" + fileInfo.Name + "\t" + fileInfo.LastWriteTime;
        }
        string FileDetailsText(FileSystemInfo fileInfo)
        {
            if (customGetFileDetailsDisplayStringFunction != null) return customGetFileDetailsDisplayStringFunction(fileInfo);
            FileInfo file = fileInfo as FileInfo;
            if (file != null)
                return
                "<B>Path:</B>\n" +
                Path.GetDirectoryName(fileInfo.FullName) + "\n" + //.DirectoryName + "\n" +
                "<B>Created:</B>\n" +
                fileInfo.CreationTime + "\n" +
                "<B>Modified:</B>\n" +
                fileInfo.LastWriteTime + "\n" +
                "<B>Size:</B>\n" +
                file.Length.FormatLargeNumberSI() + "bytes";
            else
                return
                "<B>Path:</B>\n" +
                Path.GetDirectoryName(fileInfo.FullName) + "\n" + //.DirectoryName + "\n" +
                "<B>Created:</B>\n" +
                fileInfo.CreationTime + "\n" +
                "<B>Modified:</B>\n" +
                fileInfo.LastWriteTime + "\n" +
                "<B>Is a Directory:</B>\nyes";

        }

        bool IsADirectory(FileSystemInfo fileInfo)
        {
            return (fileInfo.Attributes & FileAttributes.Directory) != 0;
        }

        enum SortableFileSystemInfoProperties
        {
            Name, Creation, Modified, Size
        }
        SortableFileSystemInfoProperties sortByProperty = SortableFileSystemInfoProperties.Name;
        int CompareFilesystemEntries(FileSystemInfo x, FileSystemInfo y)
        {
            if (IsADirectory(x) && !IsADirectory(y)) return -1;
            if (!IsADirectory(x) && IsADirectory(y)) return 1;
            switch (sortByProperty)
            {
                case SortableFileSystemInfoProperties.Name:
                    return CompareStrings(x.Name, y.Name);
                case SortableFileSystemInfoProperties.Creation:
                    return System.DateTime.Compare(x.CreationTime, y.CreationTime);
                case SortableFileSystemInfoProperties.Modified:
                    return System.DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
                case SortableFileSystemInfoProperties.Size:
                    {
                        if (IsADirectory(x)) return 0;
                        else
                            return ((FileInfo)x).Length.CompareTo(((FileInfo)y).Length);
                    }
                default:
                    // Default case, in case sortByProperty is not recognized
                    return CompareStrings(x.Name, y.Name);
            }

        }
        int CompareStrings(string x, string y)
        {
            // Extract numeric and non-numeric parts from strings
            var regex = new System.Text.RegularExpressions.Regex(@"([^\d]+)(\d+)?");
            var matchX = regex.Match(x);
            var matchY = regex.Match(y);

            // Compare non-numeric parts
            int nonNumericComparison = string.Compare(matchX.Groups[1].Value, matchY.Groups[1].Value, System.StringComparison.Ordinal);

            if (nonNumericComparison != 0)
            {
                return nonNumericComparison;
            }

            // If both strings have numeric parts
            if (matchX.Groups[2].Success && matchY.Groups[2].Success)
            {
                int numX = int.Parse(matchX.Groups[2].Value);
                int numY = int.Parse(matchY.Groups[2].Value);

                // Compare numeric parts
                return numX.CompareTo(numY);
            }

            // If only one string has numeric part, the one with numeric part comes first
            return matchX.Groups[2].Success ? 1 : (matchY.Groups[2].Success ? -1 : 0);
            //return x.CompareTo(y);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (selectedFileIndex != -1)
                    HandleFileDoubleClicked(selectedFileIndex);
            }
        }
    }
}
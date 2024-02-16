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
    /// <summary>
    /// Use this class to open a file dialog window with customizable options. A single instance of a LimitedListFilePanel must exist in the scene.
    /// </summary>
    public static class FilePanel
    {
        static LimitedListFilePanel instance = null;
        static void FindInstance()
        {
            if (instance != null) return;
            instance = GameObject.FindObjectOfType<LimitedListFilePanel>(true);
            if (instance == null)
                throw new System.ArgumentNullException("At least one instance of a LimitedListFilePanel must exist in the scene.");
        }
        /// <summary>
        /// Opens a file dialog window with customizable options. A single instance of a LimitedListFilePanel must exist in the scene.
        /// </summary>
        /// <param name="titleText">The title text displayed at the top of the window.</param>
        /// <param name="selectExistingFileOnly">When true, the user can only choose an existing file and cannot enter a filename.</param>
        /// <param name="defaultFilename">The default filename given when allowing file save to a new file.</param>
        /// <param name="filterForExtension">Specifies if the starting search filter should be by extension only.</param>
        /// <param name="fileExtension">The file extension used for filtering if filterForExtension is true. Do not include dot.</param>
        /// <param name="showDirectories">Determines whether directories should be shown in the file dialog.</param>
        /// <param name="actionText">The text to display on the action button.</param>
        /// <param name="fileConfirmedActionCallback">The action to perform after the user confirms the file selection.</param>
        /// <param name="canceledCallback">Action to perform when the dialog is canceled.</param>
        /// <param name="warnOnAction1ExistingFileSelected">Determines whether a warning should be shown when an existing file is selected for action1.</param>
        /// <param name="existingFileSelectedOnAction1WarningText">The warning text to display when an existing file is selected and action1 is clicked.</param>
        /// <param name="action2Text">Optional text for a second action button.</param>
        /// <param name="fileConfirmedAction2Callback">Optional action for the second action button.</param>
        /// <param name="warnOnAction2ExistingFileSelected">Determines whether a warning should be shown when an existing file is selected for action2.</param>
        /// <param name="existingFileSelectedOnAction2WarningText">The warning text to display when an existing file is selected and action2 is clicked.</param>
        /// <param name="cancelText">Optional text for the cancel button.</param>
        /// <param name="customGetFileDetailsDisplayStringFunction">Optional function to customize the display string for file details.</param>
        /// <param name="startingPath">Optional starting path for the file dialog.</param>
        public static void Open(
            string titleText, //will be displayed at the top of the window
            bool selectExistingFileOnly, //when true user will not be able to enter a filename, they will only be able to choose an existing file
            string defaultFilename, //when allowing file save to new file, this is the default file name it is given.  
            bool filterForExtension, string fileExtension, bool allowExtensionChange,//if the starting search filter should be by extension only, and what that extension is
            bool showDirectories,
            string actionText, UnityAction<FileSystemInfo> fileConfirmedActionCallback, //text that should be shown on the action button, and what to do after user has confirmed selection.
            UnityAction canceledCallback,
            bool warnOnAction1ExistingFileSelected = false, string existingFileSelectedOnAction1WarningText = null,  //if and what warning should be shown when an existing file is selected, and accept is clicked
            string action2Text = null, UnityAction<FileSystemInfo> fileConfirmedAction2Callback = null,
            bool warnOnAction2ExistingFileSelected = false, string existingFileSelectedOnAction2WarningText = null,  //if and what warning should be shown when an existing file is selected, and accept is clicked
            string cancelText = null,
            System.Func<FileSystemInfo, string> customGetFileDetailsDisplayStringFunction = null,
            string startingPath = null)
        {
            FindInstance();
            instance.Open( titleText, //will be displayed at the top of the window
             selectExistingFileOnly, //when true user will not be able to enter a filename, they will only be able to choose an existing file
             defaultFilename, //when allowing file save to new file, this is the default file name it is given.  
             filterForExtension,  fileExtension, allowExtensionChange, //if the starting search filter should be by extension only, and what that extension is
             showDirectories,
             actionText,fileConfirmedActionCallback, //text that should be shown on the action button, and what to do after user has confirmed selection.
             canceledCallback,
             warnOnAction1ExistingFileSelected,  existingFileSelectedOnAction1WarningText,  //if and what warning should be shown when an existing file is selected, and accept is clicked
             action2Text, fileConfirmedAction2Callback,
             warnOnAction2ExistingFileSelected,  existingFileSelectedOnAction2WarningText,  //if and what warning should be shown when an existing file is selected, and accept is clicked
             cancelText,
             customGetFileDetailsDisplayStringFunction,
             startingPath);
        }
    }

    /// <summary>
    /// Class is used to display the OS filesystem, allowing the user to select existing files and folders or enter a new filename.
    /// </summary>
    public class LimitedListFilePanel : MonoBehaviour
    {
        #region memberVariables
        #region UI References
        public LimitedFileScrollList limitedFileScrollList;

        public TextMeshProUGUI title;
        public TMP_InputField fileNameDisplayAndInput;
        public TMP_InputField searchPatternInput;
        public Button actionButton;
        public TextMeshProUGUI actionButtonTextComponent;
        public Button action2Button;
        public TextMeshProUGUI action2ButtonTextComponent;
        public Button cancelButton;
        public TextMeshProUGUI cancelButtonTextComponent;

    //    public Button fileItemButtonPreFab; // this prebfab will be used for each line item in the displayed list
        public Transform fileListParent;// this will probably be a "content" object in a scroll view
        public TextMeshProUGUI hoverAndSelectedFileDetails;
        #endregion

        #region inspector config variables


        public bool doubleClickFileEqualsActionButton = true;
        public float doubleClickMaxInterval = 0.25f;
        public Color selectionColor = Color.yellow;
        public Color unselectedColor = Color.gray;
        #endregion

        //assigned in Open
        UnityAction<FileSystemInfo> fileConfirmedActionCallback;
        UnityAction<FileSystemInfo> fileConfirmedAction2Callback;
        UnityAction canceledCallback;

        string defaultFilename;
        bool warnOnAction1ExistingFileSelected;
        string existingFileSelectedOnAction1WarningText;
        bool warnOnAction2ExistingFileSelected;
        string existingFileSelectedOnAction2WarningText;
        bool filterForExtension;
        string fileExtension;
        bool allowExtensionChange;
        bool showDirectories = true; //weather or not folder should be shown, if not- directory changing will not be possible.
        bool closeWindowOnAction2Confirmed = false;
        
        //state info
        FileSystemInfo selectedFile = null;
        string currentPath;

        string currentSearchString
        {
            get
            {
                if (searchPatternInput == null) return "*." + fileExtension;
                if (!allowExtensionChange)
                    return searchPatternInput.text +"."+ fileExtension;
                return searchPatternInput.text;
            }
            set
            {
                if (searchPatternInput == null) return;
                if (!allowExtensionChange)
                {
                    int lastDotIndex = value.LastIndexOf('.');
                    if (lastDotIndex != -1)  // If '.' is found
                    {
                        value = value.Substring(0, lastDotIndex);
                    }
                }
                //searchPattern += fileExtension;
                searchPatternInput.text = value;
            }
        }
        float lastClickTime = 0; //used to detect doubleclick
 
        public System.Func<FileSystemInfo, string> customGetFileDetailsDisplayStringFunction = null;

        
        List<FileSystemInfo> displayedFiles = new List<FileSystemInfo>();
        #endregion

        /// <summary>
        /// Opens a file dialog window with customizable options. A single instance of a LimitedListFilePanel must exist in the scene.
        /// </summary>
        /// <param name="titleText">The title text displayed at the top of the window.</param>
        /// <param name="selectExistingFileOnly">When true, the user can only choose an existing file and cannot enter a filename.</param>
        /// <param name="defaultFilename">The default filename given when allowing file save to a new file.</param>
        /// <param name="filterForExtension">Specifies if the starting search filter should be by extension only.</param>
        /// <param name="fileExtension">The file extension used for filtering if filterForExtension is true. Do not include dot.</param>
        /// <param name="allowExtensionChange">Determines weather or not the user can change the extension in either the search field or filename field.</param>
        /// <param name="showDirectories">Determines whether directories should be shown in the file dialog.</param>
        /// <param name="actionText">The text to display on the action button.</param>
        /// <param name="fileConfirmedActionCallback">The action to perform after the user confirms the file selection.</param>
        /// <param name="canceledCallback">Action to perform when the dialog is canceled.</param>
        /// <param name="warnOnAction1ExistingFileSelected">Determines whether a warning should be shown when an existing file is selected for action1.</param>
        /// <param name="existingFileSelectedOnAction1WarningText">The warning text to display when an existing file is selected and action1 is clicked.</param>
        /// <param name="action2Text">Optional text for a second action button.</param>
        /// <param name="fileConfirmedAction2Callback">Optional action for the second action button.</param>
        /// <param name="warnOnAction2ExistingFileSelected">Determines whether a warning should be shown when an existing file is selected for action2.</param>
        /// <param name="existingFileSelectedOnAction2WarningText">The warning text to display when an existing file is selected and action2 is clicked.</param>
        /// <param name="cancelText">Optional text for the cancel button.</param>
        /// <param name="customGetFileDetailsDisplayStringFunction">Optional function to customize the display string for file details.</param>
        /// <param name="startingPath">Optional starting path for the file dialog.</param>
        public void Open(
            string titleText, //will be displayed at the top of the window
            bool selectExistingFileOnly, //when true user will not be able to enter a filename, they will only be able to choose an existing file
            string defaultFilename, //when allowing file save to new file, this is the default file name it is given.  
            bool filterForExtension, string fileExtension, bool allowExtensionChange,//if the starting search filter should be by extension only, and what that extension is
            bool showDirectories,
            string actionText, UnityAction<FileSystemInfo> fileConfirmedActionCallback, //text that should be shown on the action button, and what to do after user has confirmed selection.
            UnityAction canceledCallback,
            bool warnOnAction1ExistingFileSelected=false, string existingFileSelectedOnAction1WarningText=null,  //if and what warning should be shown when an existing file is selected, and accept is clicked
            string action2Text = null, UnityAction<FileSystemInfo> fileConfirmedAction2Callback = null,
            bool warnOnAction2ExistingFileSelected=false, string existingFileSelectedOnAction2WarningText=null,  //if and what warning should be shown when an existing file is selected, and accept is clicked
            string cancelText = null,
            System.Func<FileSystemInfo, string> customGetFileDetailsDisplayStringFunction = null,
            string startingPath = null,
            bool closeWindowOnAction2Confirmed=false)
        {
            #region assignLocalVar
            //assign params to local variables to remember them
            this.defaultFilename = defaultFilename;
            this.filterForExtension = filterForExtension;
            this.fileExtension = fileExtension;
            this.allowExtensionChange = allowExtensionChange;
            this.fileConfirmedActionCallback = fileConfirmedActionCallback;
            this.fileConfirmedAction2Callback = fileConfirmedAction2Callback;
            this.canceledCallback = canceledCallback;
            this.warnOnAction1ExistingFileSelected = warnOnAction1ExistingFileSelected;
            this.existingFileSelectedOnAction1WarningText = existingFileSelectedOnAction1WarningText;
            this.warnOnAction2ExistingFileSelected = warnOnAction2ExistingFileSelected;
            this.existingFileSelectedOnAction2WarningText = existingFileSelectedOnAction2WarningText;

            this.showDirectories = showDirectories;

            this.currentPath = startingPath;
            this.customGetFileDetailsDisplayStringFunction = customGetFileDetailsDisplayStringFunction;
            if (startingPath == null)
                currentPath = Application.dataPath;
            else
                currentPath = Path.Combine(Application.dataPath, startingPath);
            //this.selectedFileIndex = -1;
            this.selectedFile = null;

            this.closeWindowOnAction2Confirmed = closeWindowOnAction2Confirmed;
            #endregion

            #region setup display components, subscribe to events, etc..
            //setup display components title, action buttons, etc
            title.text = titleText;
            title.gameObject.SetActive(!string.IsNullOrEmpty(titleText));
            
            

            //actionButton.onClick.AddListener(HandleActionButtonClicked);
            if (actionButtonTextComponent != null)
                if (!string.IsNullOrEmpty(actionText))
                    actionButtonTextComponent.text = actionText;
                else
                    actionButtonTextComponent.text = "Go";

            if (action2Button != null)
            {
                if (fileConfirmedAction2Callback != null)
                {
                   // action2Button.onClick.AddListener(HandleActionButton2Clicked);
                    action2Button.gameObject.SetActive(true);
                    if (action2ButtonTextComponent != null)
                        if (!string.IsNullOrEmpty(action2Text))
                            action2ButtonTextComponent.text = action2Text;
                        else
                            action2ButtonTextComponent.text = "Action 2";
                }
                else
                    action2Button.gameObject.SetActive(false);
            }

           // cancelButton.onClick.AddListener(canceledCallback);
           // cancelButton.onClick.AddListener(() => gameObject.SetActive(false));

            if (cancelButtonTextComponent != null)
                if (!string.IsNullOrEmpty(cancelText))
                    cancelButtonTextComponent.text = cancelText;
                else
                    cancelButtonTextComponent.text = "Cancel";
            currentSearchString = "*." + fileExtension;

            /*if (searchPatternInput != null)
                if (allowExtensionChange)
                    searchPatternInput.text = "*." + fileExtension;
                else
                    searchPatternInput.text = "*";*/

            fileNameDisplayAndInput.interactable = !selectExistingFileOnly;
            fileNameDisplayAndInput.text = defaultFilename;
            // fileNameDisplayAndInput.onValueChanged.AddListener(HandleFileNameChanged);

            //searchPatternInput.onValueChanged.AddListener(HandleSearchFieldChanged);
            AddListeners();
            #endregion

            SetupFileDisplayList();



            gameObject.SetActive(true);
        }

        bool listenersAdded = false;
        void AddListeners()
        {
            if (!listenersAdded)
            {
                actionButton.onClick.AddListener(HandleActionButtonClicked);
                if (action2Button != null)
                {
                    action2Button.onClick.AddListener(HandleActionButton2Clicked);
                }
                cancelButton.onClick.AddListener(canceledCallback);
                cancelButton.onClick.AddListener(Deactivate);
                fileNameDisplayAndInput.onValueChanged.AddListener(HandleFileNameChanged);
                if(searchPatternInput!=null)
                    searchPatternInput.onValueChanged.AddListener(HandleSearchFieldChanged);

                limitedFileScrollList.onClickEvent.AddListener(HandleFileClicked);
                limitedFileScrollList.onPointerEnterEvent.AddListener(HandleFileMouseOver);
                limitedFileScrollList.onPointerExitEvent.AddListener(HandleFileMouseExit);
                limitedFileScrollList.onSelectEvent.AddListener(HandleFileSelected);
                limitedFileScrollList.onElementInView.AddListener(HandleFileInView);

                listenersAdded = true;
            }
        }
        void Deactivate()
        {
            gameObject.SetActive(false);
        }
        void SetupFileDisplayList()
        {

            DirectoryInfo currentDirectory = new DirectoryInfo(currentPath);
            if (!currentDirectory.Exists)
                throw new DirectoryNotFoundException("Trying to show files in non-existent directory.");

            List<FileSystemInfo> filesFound = new List<FileSystemInfo>(currentDirectory.GetFileSystemInfos(currentSearchString));
            filesFound.Sort(CompareFilesystemEntries);
            //insert "up" folder after sort, so it' always on top
            if (showDirectories)
            {
                DirectoryInfo parentDir = currentDirectory.Parent;
                if (parentDir != null && parentDir.Exists)
                {
                    filesFound.Insert(0, parentDir);
                }
            }
            displayedFiles = filesFound;

            //convert List<FileSystemInfo> filesFound, into a new List<FileSystemInfoWithNameOverride> displayArray
            List<FileSystemInfoWithNameOverride> displayArray = new List<FileSystemInfoWithNameOverride>();
            foreach (FileSystemInfo info in filesFound)
            {
                string overrideName = null;
                if (currentDirectory.Parent != null)
                {
                    if (info.FullName.Equals(currentDirectory.Parent.FullName))
                        overrideName = "..";
                }
                displayArray.Add(new FileSystemInfoWithNameOverride(info, overrideName));
            }
            
            limitedFileScrollList.SetList(displayArray);
            /*
            limitedFileScrollList.onClickEvent.RemoveAllListeners();
            limitedFileScrollList.onClickEvent.AddListener(HandleFileClicked);

            limitedFileScrollList.onPointerEnterEvent.RemoveAllListeners();
            limitedFileScrollList.onPointerEnterEvent.AddListener(HandleFileMouseOver);

            limitedFileScrollList.onPointerExitEvent.RemoveAllListeners(); 
            limitedFileScrollList.onPointerExitEvent.AddListener(HandleFileMouseExit);
            
            limitedFileScrollList.onSelectEvent.RemoveAllListeners(); 
            limitedFileScrollList.onSelectEvent.AddListener(HandleFileSelected);

            limitedFileScrollList.onElementInView.RemoveAllListeners();
            limitedFileScrollList.onElementInView.AddListener(HandleFileInView);*/
        }
        void HandleActionButtonClicked()
        {
            if (selectedFile!=null && selectedFile.Exists && warnOnAction1ExistingFileSelected)
                YesNoPanel.Open(HandleConfirmActionClicked, existingFileSelectedOnAction1WarningText);
            else
                HandleConfirmActionClicked(1);
        }
        void HandleActionButton2Clicked()
        {
            if (selectedFile.Exists && warnOnAction2ExistingFileSelected)
                YesNoPanel.Open(HandleConfirmAction2Clicked, existingFileSelectedOnAction2WarningText);
            else
                HandleConfirmAction2Clicked(1);
        }
        void HandleConfirmActionClicked(int confirm)
        {
            if (confirm != 0)
            {
                if (selectedFile != null)
                    fileConfirmedActionCallback.Invoke(selectedFile);
                else
                {
                    string filename = fileNameDisplayAndInput.text;
                    if (!allowExtensionChange)
                        filename += "." + fileExtension;
                    FileInfo a =new FileInfo(filename);
                    fileConfirmedActionCallback.Invoke(a);
                }
                gameObject.SetActive(false);
            }
        }
        void HandleConfirmAction2Clicked(int confirm)
        {
            if (confirm != 0)
            {
                fileConfirmedAction2Callback.Invoke(selectedFile);
                if(closeWindowOnAction2Confirmed)
                    gameObject.SetActive(false);
            }
        }

        bool isSelectedFile(int fileIndex)
        {
            return (selectedFile != null && selectedFile.FullName == displayedFiles[fileIndex].FullName);
        }

        void ColorListElementBySelection(int fileIndex)
        {
            if (fileIndex == -1) return;

            DisplayFileMono viewElement =limitedFileScrollList.GetDisplayElement(fileIndex);
            if (viewElement == null) return;
            Image backGround = viewElement.gameObject.GetComponent<Image>();
            if (backGround == null) return;
            if (!isSelectedFile(fileIndex))
            {
                backGround.color = unselectedColor;
            }
            else
            {
                backGround.color = selectionColor;
            }
        }
        void HandleFileInView(int fileIndex)
        {
            Debug.Log("FileInView recieved: " + fileIndex + "    isSelected: " + isSelectedFile(fileIndex));
            if (selectedFile == null)
                Debug.Log("selected file == null");
            else
                Debug.Log("selected file: " + selectedFile.FullName);
            Debug.Log("in view file: " + displayedFiles[fileIndex].FullName);
            ColorListElementBySelection(fileIndex);
            if (isSelectedFile(fileIndex)) //selectedFile != null && selectedFile.Equals(displayedFiles[fileIndex]))//selectedFileIndex == fileIndex)
            {
                DisplayFileMono viewElement = limitedFileScrollList.GetDisplayElement(fileIndex);
                if(limitedFileScrollList.HasFocus())
                    EventSystem.current.SetSelectedGameObject(viewElement.gameObject);
            }
        }
        void HandleFileClicked(int fileIndex)
        {
            Debug.Log("filepanel onclick.  fileIndex: "+ fileIndex);
            if (Time.realtimeSinceStartup - lastClickTime < doubleClickMaxInterval)
                if (isSelectedFile(fileIndex)) //(selectedFile.Equals(displayedFiles[fileIndex]))//selectedFileIndex == fileIndex)
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
            if (!isSelectedFile(fileIndex))// (selectedFile==null || !selectedFile.Equals(displayedFiles[fileIndex]))//selectedFileIndex != fileIndex)
            {

                FileSystemInfo previouslySelectedFile = selectedFile;
                int previousSelectedFileInex = displayedFiles.IndexOf(previouslySelectedFile);
                selectedFile = displayedFiles[fileIndex];

                ColorListElementBySelection(fileIndex);
                if(previousSelectedFileInex != -1)
                    ColorListElementBySelection(previousSelectedFileInex);//  previousSelectedFileInex);
                if (IsADirectory(selectedFile))
                    fileNameDisplayAndInput.text = null;
                else
                {
                    if (allowExtensionChange)
                        fileNameDisplayAndInput.text = selectedFile.Name;
                    else
                    {
                        string filename = selectedFile.Name;
                        int lastDotIndex = filename.LastIndexOf('.');
                        if (lastDotIndex != -1)  // If '.' is found
                        {
                            filename = filename.Substring(0, lastDotIndex);
                        }
                        fileNameDisplayAndInput.text = filename;
                    }
                }
            }
        }
        void HandleFileDoubleClicked(int fileIndex)
        {
            //we ignore param in favor of "selectedFile"- should be the same after fileIndex lookup
            FileSystemInfo fileDblClicked = selectedFile;
            if (IsADirectory(fileDblClicked))
            {
                currentPath = fileDblClicked.FullName;
                SetupFileDisplayList();
                if (displayedFiles.Count > 0)
                    HandleFileSelected(0);

            }
            else if (doubleClickFileEqualsActionButton)
                HandleActionButtonClicked();
        }
        void HandleFileMouseOver(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= displayedFiles.Count) return;
            hoverAndSelectedFileDetails.text = FileDetailsText(displayedFiles[fileIndex]);
            //hoverAndSelectedFileDetails.DisplayValue = instantiatedFileButtons[fileIndex].FileDisplayed;
        }
        void HandleFileMouseExit(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= displayedFiles.Count) return;
            if (selectedFile == displayedFiles[fileIndex]) return;
            hoverAndSelectedFileDetails.text = "";// instantiatedFileButtons[fileIndex].text;
            if (selectedFile != null)
                hoverAndSelectedFileDetails.text = FileDetailsText(selectedFile);
            //hoverAndSelectedFileDetails.DisplayValue = instantiatedFileButtons[fileIndex].FileDisplayed;
        }

        void HandleSearchFieldChanged(string ignored)
        {

            SetupFileDisplayList();
            if (selectedFile != null) //selectedFile != null && selectedFile.Equals(displayedFiles[fileIndex]))//selectedFileIndex == fileIndex)
            {
                int slectedFileIndex = displayedFiles.IndexOf(selectedFile);
                if (slectedFileIndex != -1)
                {
                    DisplayFileMono viewElement = limitedFileScrollList.GetDisplayElement(slectedFileIndex);
                    // if (limitedFileScrollList.HasFocus())
                    EventSystem.current.SetSelectedGameObject(viewElement.gameObject);
                    hoverAndSelectedFileDetails.text = FileDetailsText(selectedFile);
                }
                else
                    selectedFile = null;
            }
            // selectedFile = null;
            /*
            if (selectedFileIndex != -1)
            {
                selectedFileIndex = -1;
            }
            */
        }
        void HandleSortFieldChange()
        {
            SetupFileDisplayList();
        }

        void HandleFileNameChanged(string ignored)
        {
            string newFilename = fileNameDisplayAndInput.text;
            if (!allowExtensionChange) newFilename += "." + fileExtension;
            //search files for one with this filename- if found, select it
            for(int i=0;i<displayedFiles.Count;i++)
            {
                FileSystemInfo info = displayedFiles[i];
                if (info.Name == newFilename)
                {
                   // selectedFile = info;
                    HandleFileSelected(i);
//                    SetupFileDisplayList();
                    return;
                }
            }
          //  selectedFile = null;
          //  SetupFileDisplayList();
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
                fileInfo.FullName + "\n" +//Path.GetDirectoryName(fileInfo.FullName) + "\n" + //.DirectoryName + "\n" +
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

        public enum SortableFileSystemInfoProperties
        {
            Name, Extension, Creation, Modified, Size
        }
        SortableFileSystemInfoProperties sortByProperty = SortableFileSystemInfoProperties.Name;
        public void SetSortBy(int newSort)
        {
            SetSortBy((SortableFileSystemInfoProperties)newSort);
        }
        public void SetSortBy(SortableFileSystemInfoProperties newSort)
        {
            if (sortByProperty != newSort)
            {
                sortByProperty = newSort;
                HandleSortFieldChange();
            }
        }
        int CompareFilesystemEntries(FileSystemInfo x, FileSystemInfo y)
        {
            if (IsADirectory(x) && !IsADirectory(y)) return -1;
            if (!IsADirectory(x) && IsADirectory(y)) return 1;
            switch (sortByProperty)
            {
                case SortableFileSystemInfoProperties.Name:
                    return CompareStrings(x.Name, y.Name);
                case SortableFileSystemInfoProperties.Extension:
                    return CompareStrings(x.Extension, y.Extension);
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
                //if (selectedFileIndex != -1)
                //  HandleFileDoubleClicked(selectedFileIndex);
                if (selectedFile != null)
                {
                    HandleFileDoubleClicked(displayedFiles.IndexOf(selectedFile));
                }

            }
        }
    }
}
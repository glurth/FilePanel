using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

namespace EyE.Unity.UI.Example
{
    public class FilePanelExamples : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //   SaveAnyFilePanel();
        }

        public void SaveAnyFilePanel()
        {
            FilePanel.Open(
                titleText: "Save File",
                selectExistingFileOnly: false,
                defaultFilename: "save-" + System.DateTime.Now.ToString("dd-mm-yy") + ".sim",
                filterForExtension: false,
                fileExtension: "*",
                allowExtensionChange: true,
                showDirectories: true,
                actionText: "Save",
                fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name + " .. SAVED"); gameObject.SetActive(true); },
                canceledCallback: () => { Debug.Log("Cancel confirmed"); gameObject.SetActive(true); },
                warnOnAction1ExistingFileSelected: true,
                existingFileSelectedOnAction1WarningText: "Saving to this file will overwrite it contents.  Existing content will be lost. Proceed?",
                action2Text: "Fake Delete",
                fileConfirmedAction2Callback: (FileSystemInfo f) => { Debug.Log("Action2 confirmed. file: " + f.Name + " will now be NOT ACTUALLY deleted"); },
                warnOnAction2ExistingFileSelected: true,
                existingFileSelectedOnAction2WarningText: "This will permanently delete the selected file. Proceed?",
                cancelText: null,
                customGetFileDetailsDisplayStringFunction: null,
                startingPath: null
                ); 
            gameObject.SetActive(false);
        }
        public void SaveSimGameFilePanel()
        {
            FilePanel.Open(
                titleText: "Save Sim",
                selectExistingFileOnly: false,
                defaultFilename: "save-" + System.DateTime.Now.ToString("dd-mm-yy"),
                filterForExtension: true,
                fileExtension: "sim",
                allowExtensionChange: false,
                showDirectories: false,
                actionText: "Save",
                fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name + " .. SAVED"); gameObject.SetActive(true); },
                canceledCallback: () => { Debug.Log("Cancel confirmed"); gameObject.SetActive(true); },
                warnOnAction1ExistingFileSelected: true,
                existingFileSelectedOnAction1WarningText: "Saving to this file will overwrite it contents.  Existing content will be lost. Proceed?",
                action2Text: "Fake Delete",
                fileConfirmedAction2Callback: (FileSystemInfo f) => { Debug.Log("Action2 confirmed. file: " + f.Name + " will now be NOT ACTUALLY deleted"); },
                warnOnAction2ExistingFileSelected: true,
                existingFileSelectedOnAction2WarningText: "This will permanently delete the selected file. Proceed?",
                cancelText: null,
                customGetFileDetailsDisplayStringFunction: null,
                startingPath: null
                );
            gameObject.SetActive(false);
        }
        public void LoadAnyFilePanel()
        {
            FilePanel.Open(
                titleText: "Load File",
                selectExistingFileOnly: true,
                defaultFilename: null,
                filterForExtension: false,
                fileExtension: "*",
                allowExtensionChange: true,
                showDirectories: true,
                actionText: "Load",
                fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name + " .. LOADING"); gameObject.SetActive(true); },
                canceledCallback: () => { Debug.Log("Cancel confirmed"); gameObject.SetActive(true); },
                warnOnAction1ExistingFileSelected: false,
                existingFileSelectedOnAction1WarningText: "Loading this file will lose all unsaved work. Proceed?",
                action2Text: "Fake Delete",
                fileConfirmedAction2Callback: (FileSystemInfo f) => { Debug.Log("Action2 confirmed. file: " + f.Name + " will now be NOT ACTUALLY deleted"); },
                warnOnAction2ExistingFileSelected: true,
                existingFileSelectedOnAction2WarningText: "This will permanently delete the selected file. Proceed?",
                cancelText: null,
                customGetFileDetailsDisplayStringFunction: null,
                startingPath: null
                );
            gameObject.SetActive(false);
        }
        public void LoadSimGameFilePanel()
        {
            FilePanel.Open(
                titleText: "Load Sim",
                selectExistingFileOnly: true,
                defaultFilename: "",
                filterForExtension: true,
                fileExtension: "sim",
                allowExtensionChange: false,
                showDirectories: false,
                actionText: "Load",
                fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name + " .. LOADING"); gameObject.SetActive(true); },
                canceledCallback: () => { Debug.Log("Cancel confirmed"); gameObject.SetActive(true); },
                warnOnAction1ExistingFileSelected: false,
                existingFileSelectedOnAction1WarningText: "Loading this file will lose all unsaved work. Proceed?",
                action2Text: "Fake Delete",
                fileConfirmedAction2Callback: (FileSystemInfo f) => { Debug.Log("Action2 confirmed. file: " + f.Name + " will now be NOT ACTUALLY deleted"); },
                warnOnAction2ExistingFileSelected: true,
                existingFileSelectedOnAction2WarningText: "This will permanently delete the selected file. Proceed?",
                cancelText: null,
                customGetFileDetailsDisplayStringFunction: null,
                startingPath: null
                );
            gameObject.SetActive(false);
        }
    }
}
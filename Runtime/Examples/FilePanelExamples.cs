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
            OpenSaveGamePanel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OpenSaveGamePanel()
        {
            FilePanel.Open(
                titleText: "test Title",
                selectExistingFileOnly: true,
                defaultFilename: "",
                filterForExtension: true,
                fileExtension: "sim",
                showDirectories: true,
                actionText: "Load",
                fileConfirmedActionCallback: (FileSystemInfo f) => { Debug.Log("Action1 confirmed. file: " + f.Name); },
                canceledCallback: () => { Debug.Log("Cancel confirmed"); },
                warnOnAction1ExistingFileSelected: true,
                existingFileSelectedOnAction1WarningText: "Loading this file will lose all unsaved work. Proceed?",
                action2Text: "Delete",
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
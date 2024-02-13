using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using EyE.Unity.UI;
using UnityEngine.Events;

namespace EyE.Unity.UI.Templates
{
    /// <summary>
    /// This component should be added to the preFab for the line item in the File Panel's list.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class DisplayFileButton : DisplayFileMono
    {
        public bool showName;
        public bool extension;
        public bool modifiedDate;
        public bool creationDate;
        public bool size;

        public string dateFormat = "";
        const int numFilePropertyControls = 4;
        public TextMeshProUGUI[] filePropertiesTextControls = new TextMeshProUGUI[numFilePropertyControls];
        public Image folderIcon;
        
        //lets this class work with objects that use IDisplay<T>
        override public void Display(FileSystemInfoWithNameOverride displayInfo)
        {
            Display(displayInfo.info, displayInfo.overrideName);
        }
        override public void Display(FileSystemInfo fileInfo, string overrideName = null)
        {
            
            int numPropertiesDisplayed = 0;
            if (showName || extension) numPropertiesDisplayed++;
            if (modifiedDate) numPropertiesDisplayed++;
            if (creationDate) numPropertiesDisplayed++;
            if (size) numPropertiesDisplayed++;
           
            int controlCounter = 0;


            
            string PossibleAddRightAlign(string str)
            {
                if (controlCounter-1 >= numPropertiesDisplayed / 2) //need to use -1 cuz we use ++ below
                    return "<align=right>" + str + "</align>";
                return str;
            }

            string s = "";
            
            if (showName && extension) s += overrideName != null ? overrideName: fileInfo.Name;
            //if (showName && extension) s += fileInfo.Name;
            if (showName && !extension) s += Path.GetFileNameWithoutExtension(fileInfo.FullName);
            if (!showName && extension) s += "." + fileInfo.Extension;
            if (showName || extension)
                filePropertiesTextControls[controlCounter++].text = PossibleAddRightAlign(s);

            if (modifiedDate)
                filePropertiesTextControls[controlCounter++].text = PossibleAddRightAlign(fileInfo.LastWriteTime.ToString(dateFormat));

            if (creationDate)
                filePropertiesTextControls[controlCounter++].text = PossibleAddRightAlign(fileInfo.CreationTime.ToString(dateFormat));

            if ((fileInfo.Attributes & FileAttributes.Directory) == 0)
            {
                if (size)// folders dont have a size
                {
                    FileInfo file = fileInfo as FileInfo;
                    filePropertiesTextControls[controlCounter++].text = PossibleAddRightAlign(file.Length.FormatLargeNumberSI() + "bytes");
                }
                folderIcon.gameObject.SetActive(false);
            }
            else 
            {
                folderIcon.gameObject.SetActive(true);
            }
            for (int i = controlCounter; i < filePropertiesTextControls.Length; i++)
                filePropertiesTextControls[i].gameObject.SetActive(false);
        }

    }
}
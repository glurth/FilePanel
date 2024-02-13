using System.IO;
using UnityEngine;

namespace EyE.Unity.UI
{
    /// <summary>
    /// This struct contains the data that will be Displayed in the file panel scroll list
    /// </summary>
    public struct FileSystemInfoWithNameOverride
    {
        public FileSystemInfo info;
        public string overrideName;

        public FileSystemInfoWithNameOverride(FileSystemInfo info, string overrideName)
        {
            this.info = info;
            this.overrideName = overrideName;
        }
    }


    /// <summary>
    /// this interface will be implemented by a component on the preFab (like the template class, DisplayFileButton).  When Display is called by the system, it will show it on screen.
    /// </summary>
    public interface IDisplayFileInfo : IDisplay<FileSystemInfoWithNameOverride>
    {
        public void Display(FileSystemInfo fileInfo, string overrideName = null);
        //public void Display(FileSystemInfoWithNameOverride fileInfo);
    }

    /// <summary>
    /// abstract base class for components that will be displayed in a LimitedFileScrollList.  User must implement the abstract IDisplayFileInfo and IDisplay<FileSystemInfoWithNameOverride> functions.
    /// </summary>
    abstract public class DisplayFileMono : MonoBehaviour, IDisplayFileInfo
    {
        abstract public void Display(FileSystemInfo fileInfo, string overrideName = null);
        abstract public void Display(FileSystemInfoWithNameOverride obj);
    }
    /// <summary>
    /// A scene-object efficient ScrollList that displays a list of FileSystemInfoWithNameOverride objects, using a DisplayFileButton preFab.
    /// Utilizes the LimitedObjectScrollList package.
    /// </summary>
    public class LimitedFileScrollList : LimitedObjectScrollList<FileSystemInfoWithNameOverride, DisplayFileMono>
    {

    }
}
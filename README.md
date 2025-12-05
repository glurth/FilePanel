# FilePanel Package

This Unity package provides a flexible, customizable file panel UI system for file selection, saving, and loading.  
It is **Canvas-based** and uses standard **Unity UI components** like `Button`, `TMP_InputField`, and `ScrollRect`.

Designed for in-game file dialogs with support for thousands of entries and real-time filtering.

## Install

### Main Package

1. Open Unity.
2. Go to **Window ? Package Manager**.
3. Click the **+** button (top left) ? **Add package from Git URL…**
4. Paste the following URL:  
   `https://github.com/glurth/FilePanel.git`

### Dependency

This package **requires** the [LimitedObjectScrollList package](https://github.com/glurth/LimitedObjectScrollList).

Repeat the steps above and use:  
`https://github.com/glurth/LimitedObjectScrollList.git`

## Features

- Select or create files.
- Optional second action button.
- Overwrite and delete warnings.
- Extension filtering (with optional locking).
- Optional directory browsing.
- Fully customizable via prefab.
- Handles thousands of files efficiently via pooled scroll list.

## Quick Start

### Example – Save Sim Game File

```
FilePanel.Open(
    titleText: "Save Sim",                         // Title shown at top of the panel
    selectExistingFileOnly: false,                // Allows new filenames
    defaultFilename: "save-" + DateTime.Now.ToString("dd-mm-yy"),
    filterForExtension: true,                     // Filter by extension
    fileExtension: "sim",                         // Only show *.sim files
    allowExtensionChange: false,                  // Lock extension
    showDirectories: false,                       // Disable folder navigation
    actionText: "Save",                           // Text on main action button
    fileConfirmedActionCallback: (FileSystemInfo f) => {
        // Call your save function here, saving to f.FullName
        Debug.Log("Saving file: " + f.Name);
    },
    canceledCallback: () => {
        Debug.Log("User cancelled");
    },
    warnOnAction1ExistingFileSelected: true,
    existingFileSelectedOnAction1WarningText: "Saving to this file will overwrite its contents.",
    action2Text: "Fake Delete",
    fileConfirmedAction2Callback: (FileSystemInfo f) => {
        // Optional secondary action
        Debug.Log("Pretending to delete: " + f.Name);
    },
    warnOnAction2ExistingFileSelected: true,
    existingFileSelectedOnAction2WarningText: "This will permanently delete the file. Proceed?",
    cancelText: null,
    customGetFileDetailsDisplayStringFunction: null,
    startingPath: null
);
```

## Customization

- Modify the included prefab directly to match your UI.
- Use `customGetFileDetailsDisplayStringFunction` to change how file info appears.
- All behavior and visuals are configurable via `Open()` parameters.
- For advanced prefab customization, consider rebuilding the UI using  
  [UIPrefabGenerator](https://github.com/glurth/UIPrefabGenerator).  
  It was not used here to minimize dependencies, but works well with this system.

## Known Limitations

- No multi-file selection.
- Designed for one panel active at a time.

## Contributions

Contributions, issues, and feature requests are welcome! Please submit them via the GitHub repository. Note: Due to licensing, contributions can only be included with explicit written permission from the copyright holder.

## License

This package is licensed under the EyE Dual-Licensing Agreement.

It provides free, perpetual use for indie developers and non-commercial projects whose teams had Total Gross Receipts under $100,000 USD in the previous fiscal year.

Organizations exceeding this threshold must obtain a Perpetual Commercial License (PCL) for each named commercial project.

Please review the full terms in [LICENSE.md] before commercial use.

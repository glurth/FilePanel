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

## License

**All rights reserved.**  
No license is granted for use, modification, distribution, or any other purpose without prior written permission.

If you're an independent developer and would like to use this software, email glurth at gmail.com to request a license. I usually approve such requests for free.  
Businesses may contact me for pricing.

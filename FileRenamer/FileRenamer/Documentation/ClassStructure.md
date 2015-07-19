# Project Structure

The core of this project is structured around the strategy pattern.  The main model class `RenamerModel`
has a `RenameStrategy` property which allows someone to plug in an `IFileRenamerStrategy` which informs
the model on how to rename a file.  These different strategies are stored in the Strategies directory.

This project uses the MVVM pattern so it is structured into three parts: Models, ViewModels, and Views.
There are three directories holding each of these.  There is another directory, IO, which abstracts
the IO that this program uses.  Finally there is the strategies directory which holds the models
for the various renaming strategies that this program implements.

In order to extend this program with another renaming strategy the following must be done:
1. Add a new class to the Strategies directory which implements `IFileRenamerStrategy`
2. Add a viewmodel to the ViewModel directory which implements `StrategyViewModel, IDataErrorInfo`
3. Add a view to the View directory, this should be a usercontrol
4. In the file `MainWindowViewModel.cs` in the ViewModel directory add your new viewmodel to the `Strategies` property.
5. Link the new view and viewmodel in the file `MainWindowResources.xaml` by adding a datatemplate.


# Class Notes

## App.xaml

The XAML file for the App.  Contains startup logic in its code behind.

## MainWindow.xaml

The view for the main window.

## MainWindowResources.xaml

Links the views to the viewmodels, and sets up some datatemplates and styles.

## RelayCommand

Relaying commands for the MVVM pattern.

## Models

### RenamerModel

This is the main model in the project.  It represents a list of files which are being renamed,
and it has the current strategy used to rename the file, as well as a NameExtensionHelper to
determine what part of the filename to rename.

### FileMetaData

The interface `IFileMetaData` represents all the metadata of a file.
The class `FileMetaData` implements this interface.

### FileModel

Represents a file in the process of being renamed.  Its `Clashes` property is set when it
clashes with another file.

### EXIFData

Model representing EXIF data in an image file.

### ID3Tag

Model representing the ID3v1 tag of an MP3 file.

## Strategies

### FileRenamerStrategy

Contains the interface `IFileRenamerStrategy` which all other strategies implement.  Also contains
the identity strategy.

### NameExtensionHelper

Classes which are used to work on only part of a filename, like the extension or just the name without
the extension.

### Other files

The other files all implement their own specific strategy.

## ViewModel

The various viewmodels of the application.  See `ViewModelClassStructure.cd` for the class layouts.

### ViewModelBase

The base for all viewmodels.

### MainWindowViewModel

The viewmodel for the main window.  This is the main viewmodel for the whole application.

### FileListViewModel

Viewmodel for the filelist.

### FileViewModel

A simple viewmodel associated with a file.

### CommandViewModel

A viewmodel for an actionable item.  Part of MVVM.

### StrategyViewModel

Abstract class for all the strategy view models.

### Other files

The other files are viewmodels for the various strategies.

## View

XAML files representing the various visual components of the application.  Note that the `MainWindow` view is
stored in the root directory.

### FileListView

Displays the filelist, which shows the original and the new filename.  The code behind implements drag and drop.

### ErrorDialog

Displays the error dialog when a file cannot be renamed.

### Other files

The other files are views for the renaming strategies.

## IO

Various abstractions to encapsulate IO from the rest of the program.

### IOCommand

A class which implements `IIOCommand` represents some action that does IO.  There are currently two such commands:
* RenameCommand    - for renaming a single file
* RenameAllCommand - for renaming multiple files

The `RenameFailureBehaviour` enum gives the different behaviours that RenameAllCommand can take when it hits an error.

### IOMove

This encapsulates a file system move command, i.e. `File.Move`.
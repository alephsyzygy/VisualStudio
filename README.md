# VisualStudio Repository

This repository contains a few C# projects that I have been working on.  These projects are all experimental and should not be used in production.

## FileRenamer

FileRenamer is a clean-room clone of the [Thunar](http://docs.xfce.org/xfce/thunar/start) plugin [Bulk-Renamer](http://docs.xfce.org/xfce/thunar/bulk-renamer/start).  This project copies and modifies the functionality of Bulk-Renamer, but does not use any of the source code.

This program experiments with some OOP design patterns, especially the strategy pattern, and it also experiments with the MVVM pattern in WPF.  This project is possibly over-engineered but since it was created for learning purposes this is justified.

Not all of the code is mine, some of it has been taken from the internet, such as the drag and drop code in WPF.  All cases should be marked in the comments with their source.

This project also experiments with Ninject, for dependency inversion, and Moq, for mocking.

### Use

When starting up FileRenamer a select file dialog box should appear.  Multiple files can be selected using Ctrl-Click or Shift-Click.  Once `Open` is selected the main window will appear.  The main window consists of a two-column list of file names and their new names.  This list can be rearranged using drag and drop. Below this is a tab control with different ways of renaming the files.  Finally there are two buttons at the bottom, one to close the application and the other to begin renaming the files.

The application will warn the user if it can detect that renaming the files will lead to an error.  However it cannot detect all such errors, so while renaming the files an error may occur.  There are a number of options to recover from this error: skip once, skip all errors, undo everything, or abort.

### Using from File Explorer

I haven't managed to get the shell extension working properly.  Under testing conditions it is fine, but under normal use the program only runs once.  Until I get this working correctly there is no way to install FileRenamer as a shell extension.

However, there is a slightly different way of getting it to work.

In File Explorer type `shell:sendto`.  Then add a shortcut to FileRenamer.exe.  Now whenever you select a bunch of files you can `Send to` the shortcut which then opens FileRenamer with those files you have selected.  

## AsyncLogic

This project is an experiment with Tasks and async methods in order to implement logic based on computablility.  The idea is that we associate `true` with a terminating computer program and `false` with a non-terminating program.  We can form the conjunction (`and`) by running all the programs and halting when all of the halt.  To from the disjunction (`or`) we run them all concurrently and we halt when any one of them halts.  In fact we can form an infinite disjunction by continually spawning more tasks, then we halt when any one of these tasks has completed.  Note that some logical connectives cannot be impletement, such as negation (`not`), and by extension `implies`, since this would allow us to solve the Halting Problem.

There is currently support for this form of computable logic, plus there is some support for naturals numbers, pairing, and lambda expressions with logical target.  An evaluator has been written which can evaluate a closed expression, but not that it could possibly run forever if the result is `false`.

There is also a parser for parsing text into an expression.  The parser uses the Sprache library for parsing, since I am familiar with monadic parsing.  Due to some earlier design decisions the type obtained from the parser cannot be used with the evaluator.  This should be fixed soon.

This project cannot be run however there is a large number of unit tests.

## FizzBuzz

An implementation of FizBuzz using enumerators and tuples.  Also experimented with the new add extension methods, but these only work in C#6.

This is a console application so it can be run from VS.
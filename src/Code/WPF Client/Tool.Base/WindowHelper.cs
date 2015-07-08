#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic.FileIO;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base.Windows.Dialogs;
using TaskDialogInterop;
using Control = System.Windows.Controls.Control;
using FileSystem = SIM.Base.FileSystem;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

#endregion

namespace SIM.Tool.Base
{
  #region



  #endregion

  /// <summary>
  ///   The window helper.
  /// </summary>
  public static class WindowHelper
  {
    #region Public Methods

    [CanBeNull]
    public static string AskForSelection([NotNull] string title, [CanBeNull] string header, [NotNull] string message, [NotNull] IEnumerable<string> options, [CanBeNull] Window owner, [CanBeNull] string defaultValue = null, [CanBeNull] bool? allowMultiSelect = null, [CanBeNull] bool? forceShinyDialog = null)
    {
      Assert.ArgumentNotNull(title, "title");
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(options, "options");

      var optionsArray = options.ToArray();
      if (forceShinyDialog == true || (optionsArray.Length < 5 && allowMultiSelect != true))
      {
        TaskDialogOptions config = new TaskDialogOptions
        {
          Owner = owner,
          Title = title,
          MainInstruction = header ?? title,
          Content = message,
          CommandButtons = optionsArray,
          MainIcon = VistaTaskDialogIcon.Information,
          AllowDialogCancellation = true
        };


        TaskDialogResult res = null;
        if (owner == null)
        {
          res = TaskDialog.Show(config);
        }
        else
        {
          owner.Dispatcher.Invoke(new Action(() => { res = TaskDialog.Show(config); }));
        }
        if (res == null) return null;
        var resultIndex = res.CommandButtonResult;
        if (resultIndex == null) return null;

        return optionsArray[(int)resultIndex];
      }

      SelectDialog dialog = new SelectDialog
      {
        DataContext = optionsArray,
        Title = message,
        AllowMultiSelect = allowMultiSelect ?? false
      };
      if (defaultValue != null)
      {
        dialog.DefaultValue = defaultValue;
      }

      object result = null;
      if (owner == null)
      {
        result = ShowDialog(dialog, null);
      }
      else
      {
        owner.Dispatcher.Invoke(new Action(() => { result = ShowDialog(dialog, owner); }));
      }

      return result as string;
    }

    public static TaskDialogResult LongRunningTask(Action longRunningTask, string title, Window owner, string content = null, string technicalInformation = null, bool allowHidingWindow = false, bool dirtyCancelationMode = false, bool allowSkip = false)
    {
      bool canceled = false;
      using (new ProfileSection("Long running task", typeof(WindowHelper)))
      {
        ProfileSection.Argument("longRunningTask", longRunningTask);
        ProfileSection.Argument("title", title);
        ProfileSection.Argument("owner", owner);
        ProfileSection.Argument("content", content);
        ProfileSection.Argument("technicalInformation", technicalInformation);
        ProfileSection.Argument("allowHidingWindow", allowHidingWindow);
        ProfileSection.Argument("dirtyCancelationMode", dirtyCancelationMode);
        ProfileSection.Argument("allowSkip", allowSkip);

        bool isDone = false;
        var thread = new Thread(() =>
        {
          using (new ProfileSection("{0} (background thread)".FormatWith(title), typeof(WindowHelper)))
          {
            try
            {
              if (!dirtyCancelationMode)
              {
                longRunningTask();
              }
              else
              {
                // this may be required when some of underlying code ignores ThreadAbortException
                // so we just letting it to complete and just stop waiting
                var innerDone = false;
                var async = new Action(() =>
                {
                  longRunningTask();
                  innerDone = true;
                });
                async.BeginInvoke(null, null);

                // waiting until it is done or ThreadAbortException is thrown
                while (!innerDone)
                {
                  Thread.Sleep(100);
                }
              }
            }
            catch (ThreadAbortException ex)
            {
              Log.Warn("Long running task \"{0}\" failed with exception".FormatWith(title), typeof(WindowHelper), ex);
            }
            catch (UserInformationException ex)
            {
              HandleError(ex.Message, false);
            }
            catch (Exception ex)
            {
              HandleError("Long running task \"{0}\" failed with exception".FormatWith(title), true, ex);
            }
            isDone = true;
          }
        });

        const string inerrupt = "&Cancel";
        const string skip = "&Skip";

        var options = allowSkip ? new[] { inerrupt, skip } : new[] { inerrupt };
        
        //const string hide = "&Hide";
        TaskDialogOptions config = new TaskDialogOptions
        {
          Owner = owner,
          Title = title,
          MainInstruction = content ?? title,
          ExpandedInfo = technicalInformation ?? String.Empty,
          CustomButtons = options, /* ButtonId=500; dialog.ClickCustomButton(0)  }, *///allowHidingWindow ? new[] { hide /* ButtonId=501 */, inerrupt /* ButtonId=500; dialog.ClickCustomButton(0) */ } : 
          AllowDialogCancellation = true,
          ShowMarqueeProgressBar = true,
          EnableCallbackTimer = true,
          Callback = (dialog, args, obj) =>
          {
            switch (args.Notification)
            {
              // initialization 
              case VistaTaskDialogNotification.Created:
                dialog.SetProgressBarMarquee(true, 0); // 0 is ignored
                break;

              // dialog is hidden
              case VistaTaskDialogNotification.ButtonClicked:
              case VistaTaskDialogNotification.Destroyed:
                // do not shutdown thread if button Hide was clicked
                if (thread.IsAlive)
                {
                  switch (args.ButtonId)
                  {
                    case 500:
                      thread.Abort();
                      canceled = true;
                      break;
                    case 501:
                      thread.Abort();
                      break;
                  }
                }
                break;
              case VistaTaskDialogNotification.Timer:
                dialog.SetContent(String.Format("Time elapsed: {0}", TimeSpan.FromMilliseconds(args.TimerTickCount).ToString(@"h\:mm\:ss")));
                if (isDone)
                {
                  dialog.ClickCustomButton(0);
                }
                break;
            }

            return false;
          }
        };

        try
        {
          thread.Start();
          var result = TaskDialog.Show(config);
          return canceled ? null : result;
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
          HandleError("The long running task caused an exception", true, ex, typeof(WindowHelper));
        }

        return null;
      }
    }

    /// <summary>
    /// Picks the file.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="textBox">
    /// The text box. 
    /// </param>
    /// <param name="otherControl">
    /// The other control. 
    /// </param>
    /// <param name="pattern">
    /// The pattern. 
    /// </param>
    /// <returns>
    /// The file. 
    /// </returns>
    [CanBeNull]
    public static string PickFile([NotNull] string message, [CanBeNull] TextBox textBox, [CanBeNull] Control otherControl, [NotNull] string pattern)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");
      Assert.ArgumentNotNullOrEmpty(pattern, "pattern");

      OpenFileDialog fileBrowserDialog = new OpenFileDialog
      {
        Title = message,
        Multiselect = false,
        CheckFileExists = true,
        Filter = pattern
      };

      string filePath = textBox != null ? textBox.Text : String.Empty;
      string fileName = Path.GetFileName(filePath);
      if (!String.IsNullOrEmpty(fileName) && FileSystem.Local.File.Exists(filePath))
      {
        fileBrowserDialog.FileName = fileName;
        fileBrowserDialog.InitialDirectory = Path.GetDirectoryName(filePath);
      }

      if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        if (textBox != null)
        {
          textBox.Text = fileBrowserDialog.FileName;
          textBox.Focus();
          if (otherControl != null)
          {
            otherControl.Focus();
          }
        }

        return fileBrowserDialog.FileName;
      }

      return null;
    }

    /// <summary>
    /// Picks the folder.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="textBox">
    /// The text box. 
    /// </param>
    /// <param name="otherControl">
    /// The other control. 
    /// </param>
    public static void PickFolder([NotNull] string message, [NotNull] TextBox textBox, [CanBeNull] Control otherControl, string initialPath = null)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(textBox, "textBox");

      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
      {
        Description = message
      };

      string path = textBox.Text.EmptyToNull() ?? initialPath;
      if (!String.IsNullOrEmpty(path))
      {
        if (!String.IsNullOrEmpty(path) && FileSystem.Local.Directory.Exists(path))
        {
          folderBrowserDialog.SelectedPath = path;
        }
      }

      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
      {
        string value = folderBrowserDialog.SelectedPath;
        SetTextboxTextValue(textBox, value, otherControl);
      }
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <typeparam name="TWindow">
    /// The type of the window. 
    /// </typeparam>
    /// <param name="dataContext">
    /// The data context. 
    /// </param>
    /// <param name="owner">
    /// The owner. 
    /// </param>
    /// <returns>
    /// The dialog. 
    /// </returns>
    [CanBeNull]
    public static object ShowDialog<TWindow>([CanBeNull] object dataContext, [CanBeNull] Window owner) where TWindow : Window, new()
    {
      TWindow window = new TWindow();

      if (dataContext != null)
      {
        window.DataContext = dataContext;
      }

      return ShowDialog(window, owner);
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <param name="window">
    /// The window. 
    /// </param>
    /// <param name="owner">
    /// The owner. 
    /// </param>
    /// <returns>
    /// The dialog. 
    /// </returns>
    [CanBeNull]
    public static object ShowDialog([NotNull] Window window, [CanBeNull] Window owner)
    {
      Assert.ArgumentNotNull(window, "window");

      if (window.Owner == null)
      {
        try
        {
          window.Owner = owner;
        }
        catch (Exception )
        {
        }
      }

      window.Left = Screen.PrimaryScreen.WorkingArea.Width / 2.0 - window.Width / 2.0;
      window.Top = Screen.PrimaryScreen.WorkingArea.Height / 2.0 - window.Height / 2.0;
      bool? result = null;
      try
      {
        result = window.ShowDialog();
        if (result == true)
        {
          return window.DataContext ?? true;
        }
      }
      catch (Exception ex)
      {
        HandleError(ex.Message, !(ex is UserInformationException), ex, typeof(WindowHelper));
      }

      return null;
    }

    /// <summary>
    /// Shows the error message and writes it to log.
    /// </summary>
    public static void HandleError([NotNull] string fullmessage, bool isError, [CanBeNull] Exception ex = null, [CanBeNull] object typeOwner = null)
    {
      Assert.ArgumentNotNull(fullmessage, "fullmessage");

      isError = !(ex is UserInformationException || !isError);
      var type = typeOwner.With(t => t.GetType()) ?? typeof(WindowHelper);
      if (ex != null)
      {
        if (!isError)
        {
          Log.Info(fullmessage, type);
          Log.Warn("Exception details", type, ex);
        }
        else
        {
          Log.Error(fullmessage, type, ex);
        }
      }
      else
      {
        Log.Error(fullmessage, type);
      }

      if (isError)
      {
        var message = ex != null ? fullmessage.TrimEnd(".".ToCharArray()) + ". " + ex.Message : fullmessage;
        if (ShowMessage(message + "\n\nYou can find details in the log file. Would you like to open it?", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel) == MessageBoxResult.OK)
        {
          OpenFile(Log.LogFilePath);
        }
      }
      else
      {
        ShowMessage(fullmessage, MessageBoxButton.OK, MessageBoxImage.Warning);
      }
    }

    public static MessageBoxResult ShowMessage(string message)
    {
      return MessageBox.Show(message, "Sitecore Instance Manager");
    }

    public static MessageBoxResult ShowMessage(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
    {
      return MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage);
    }

    public static MessageBoxResult ShowMessage(string message, string p, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
    {
      return MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage);
    }

    public static MessageBoxResult ShowMessage(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, MessageBoxResult messageBoxResult)
    {
      return MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage, messageBoxResult);
    }

    /// <summary>
    /// Shows the window.
    /// </summary>
    /// <param name="window">
    /// The window. 
    /// </param>
    /// <param name="owner">
    /// The owner. 
    /// </param>
    public static void ShowWindow([NotNull] Window window, [CanBeNull] Window owner)
    {
      Assert.ArgumentNotNull(window, "window");

      try
      {
        window.Owner = owner;
      }
      catch (Exception )
      {
      }

      try
      {
        window.Show();
      }
      catch (Exception ex)
      {
        HandleError("Cannot show {0} window. {1}".FormatWith(window.Name, ex.Message), true, ex, typeof(WindowHelper));
      }
    }

    // UI Hook helper
    /// <summary>
    /// Visuals the upward search.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    /// <param name="source">
    /// The source. 
    /// </param>
    /// <returns>
    /// The upward search. 
    /// </returns>
    [CanBeNull]
    public static DependencyObject VisualUpwardSearch<T>([NotNull] DependencyObject source)
    {
      Assert.ArgumentNotNull(source, "source");

      while (source != null && source.GetType() != typeof(T))
      {
        source = VisualTreeHelper.GetParent(source);
      }

      return source;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Focuses the clicked node.
    /// </summary>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data. 
    /// </param>
    public static void FocusClickedNode([NotNull] MouseButtonEventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");

      DependencyObject originalSource = e.OriginalSource as DependencyObject;
      if (originalSource != null)
      {
        TreeViewItem treeViewItem = VisualUpwardSearch<TreeViewItem>(originalSource) as TreeViewItem;
        if (treeViewItem != null)
        {
          treeViewItem.Focus();
        }
      }
    }

    /// <summary>
    /// Sets the textbox text value.
    /// </summary>
    /// <param name="textBox">
    /// The text box. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <param name="otherControl">
    /// The other control. 
    /// </param>
    public static void SetTextboxTextValue([NotNull] TextBox textBox, [NotNull] string value, [CanBeNull] Control otherControl)
    {
      Assert.ArgumentNotNull(textBox, "textBox");
      Assert.ArgumentNotNull(value, "value");

      textBox.Text = value;
      textBox.Focus();
      if (otherControl != null)
      {
        otherControl.Focus();
      }
    }

    #endregion

    // UI Hook for selecting instance when instantly right-clicking on it (instead of left-click and then right-click)
    public static void OpenInBrowser(string url, bool isFrontEnd, string browser = null, [CanBeNull] string[] parameters = null)
    {
      string app = browser.EmptyToNull() ?? (isFrontEnd ? AppSettings.AppBrowsersFrontend.Value : AppSettings.AppBrowsersBackend.Value);
      if (!string.IsNullOrEmpty(app))
      {
        parameters = parameters ?? new string[0];
        parameters = parameters.Insert(0, url).ToArray();
        RunApp(app, parameters);
        return;
      }

      OpenFile(url);
    }

    public static Process RunApp(string app, params string[] @params)
    {
      using (new ProfileSection("Running app", typeof(WindowHelper)))
      {
        ProfileSection.Argument("app", app);
        ProfileSection.Argument("@params", @params);

        var resultParams = string.Join(" ", @params.Select(x => x.Trim('\"')).Select(x => x.Contains(" ") || x.Contains("=") ? "\"" + x + "\"" : x));
        Log.Debug("resultParams: " + resultParams);

        var process = Process.Start(app, resultParams);

        return ProfileSection.Result(process); 
      }
    }

    public static void OpenFolder(string path)
    {
      OpenFile(path);
    }

    public static ImageSource GetImage(string image, string assembly)
    {
      try
      {
        string uri = "pack://application:,,,/{0};component/{1}"
           .FormatWith(
             assembly.Trim().TrimStart('/'),
             image.Trim().TrimStart('/'));

        return new BitmapImage(new Uri(uri));
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("The {0} image cannot be retrieved from {1} assembly".FormatWith(image, assembly), ex);
      }
    }

    public static string Ask(string title, string defaultValue, Window window)
    {
      Assert.ArgumentNotNull(title, "title");

      var dialog = new InputDialog
      {
        DataContext = new InputDialogArgs
        {
          Title = title,
          DefaultValue = defaultValue
        }
      };

      return ShowDialog(dialog, window) as string;
    }

    public static void OpenFile(string path)
    {
      WindowHelper.RunApp("explorer.exe", path.Replace('/', '\\'));
    }
    public static void CopyFileUI(string sourceFileName, string destFileName, UIOption? showUI = null, UICancelOption? onUserCancel = null)
    {
      Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sourceFileName, destFileName, showUI ?? UIOption.AllDialogs, onUserCancel ?? UICancelOption.ThrowException);
    }

    public static Process RunApp(ProcessStartInfo startInfo)
    {
      return Process.Start(startInfo);
    }
  }
}

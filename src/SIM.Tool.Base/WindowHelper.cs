namespace SIM.Tool.Base
{
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
  using SIM.Tool.Base.Windows.Dialogs;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;
  using TaskDialogInterop;

  #region

  #endregion

  public static class WindowHelper
  {
    private static readonly Dictionary<string, BitmapImage> Images = new Dictionary<string, BitmapImage>(); 

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

        if (res == null)
        {
          return null;
        }

        var resultIndex = res.CommandButtonResult;
        if (resultIndex == null)
        {
          return null;
        }

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

    public static void HandleError([NotNull] string fullmessage, bool isError, [CanBeNull] Exception ex = null, [CanBeNull] object typeOwner = null)
    {
      Assert.ArgumentNotNull(fullmessage, "fullmessage");

      if (ex != null)
      {
        Log.Error(ex, fullmessage);
      }
      else
      {
        Log.Error(fullmessage);
      }

      if (isError)
      {
        var message = ex != null ? fullmessage.TrimEnd(".".ToCharArray()) + ". " + ex.Message : fullmessage;
        if (ShowMessage(message + "\n\nYou can find details in the log file. Would you like to open it?", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.Cancel) == MessageBoxResult.OK)
        {
          CoreApp.OpenFile(ApplicationManager.LogsFolder);
        }
      }
      else
      {
        ShowMessage(fullmessage, MessageBoxButton.OK, MessageBoxImage.Warning);
      }
    }

    public static TaskDialogResult LongRunningTask(Action longRunningTask, string title, Window owner, string content = null, string technicalInformation = null, bool allowHidingWindow = false, bool dirtyCancelationMode = false, bool allowSkip = false)
    {
      bool canceled = false;
      using (new ProfileSection("Long running task"))
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
          using (new ProfileSection("{0} (background thread)".FormatWith(title)))
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
              Log.Warn(ex, "Long running task \"{0}\" failed with exception", title);
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

        var options = allowSkip ? new[]
        {
          inerrupt, skip
        } : new[]
        {
          inerrupt
        };

        // const string hide = "&Hide";
        TaskDialogOptions config = new TaskDialogOptions
        {
          Owner = owner, 
          Title = title, 
          MainInstruction = content ?? title, 
          ExpandedInfo = technicalInformation ?? string.Empty, 
          CustomButtons = options, /* ButtonId=500; dialog.ClickCustomButton(0)  }, */ // allowHidingWindow ? new[] { hide /* ButtonId=501 */, inerrupt /* ButtonId=500; dialog.ClickCustomButton(0) */ } : 
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
                dialog.SetContent(string.Format("Time elapsed: {0}", TimeSpan.FromMilliseconds(args.TimerTickCount).ToString(@"h\:mm\:ss")));
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
          HandleError("The long running task caused an exception", true, ex);
        }

        return null;
      }
    }

    [CanBeNull]
    public static string PickFile([NotNull] string message, [CanBeNull] System.Windows.Controls.TextBox textBox, [CanBeNull] System.Windows.Controls.Control otherControl, [NotNull] string pattern)
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

      string filePath = textBox != null ? textBox.Text : string.Empty;
      string fileName = Path.GetFileName(filePath);
      if (!string.IsNullOrEmpty(fileName) && SIM.FileSystem.FileSystem.Local.File.Exists(filePath))
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

    public static void PickFolder([NotNull] string message, [NotNull] System.Windows.Controls.TextBox textBox, [CanBeNull] System.Windows.Controls.Control otherControl, string initialPath = null)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(textBox, "textBox");

      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
      {
        Description = message
      };

      string path = textBox.Text.EmptyToNull() ?? initialPath;
      if (!string.IsNullOrEmpty(path))
      {
        if (!string.IsNullOrEmpty(path) && SIM.FileSystem.FileSystem.Local.Directory.Exists(path))
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
        catch (Exception)
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
        HandleError(ex.Message, true, ex);
      }

      return null;
    }

    public static MessageBoxResult ShowMessage(string message)
    {
      return System.Windows.MessageBox.Show(message, "Sitecore Instance Manager");
    }

    public static MessageBoxResult ShowMessage(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
    {
      return System.Windows.MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage);
    }

    public static MessageBoxResult ShowMessage(string message, string p, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
    {
      return System.Windows.MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage);
    }

    public static MessageBoxResult ShowMessage(string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, MessageBoxResult messageBoxResult)
    {
      return System.Windows.MessageBox.Show(message, "Sitecore Instance Manager", messageBoxButton, messageBoxImage, messageBoxResult);
    }

    public static void ShowWindow([NotNull] Window window, [CanBeNull] Window owner)
    {
      Assert.ArgumentNotNull(window, "window");

      try
      {
        window.Owner = owner;
      }
      catch (Exception)
      {
      }

      try
      {
        window.Show();
      }
      catch (Exception ex)
      {
        HandleError("Cannot show {0} window. {1}".FormatWith(window.Name, ex.Message), true, ex);
      }
    }

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

    public static void SetTextboxTextValue([NotNull] System.Windows.Controls.TextBox textBox, [NotNull] string value, [CanBeNull] System.Windows.Controls.Control otherControl)
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
    #region Public methods

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

    public static void CopyFileUI(string sourceFileName, string destFileName, UIOption? showUI = null, UICancelOption? onUserCancel = null)
    {
      Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sourceFileName, destFileName, showUI ?? UIOption.AllDialogs, onUserCancel ?? UICancelOption.ThrowException);
    }
   
    public static ImageSource GetImage(string imageName, string assemblyName)
    {
      Assert.ArgumentNotNull(imageName, "imageName");
      Assert.ArgumentNotNull(assemblyName, "assemblyName");

      assemblyName = assemblyName.Trim().TrimStart('/');
      var result = GetImageInternal(imageName, assemblyName, WinAppSettings.AppUiHighDpiEnabled.Value);
      if (result != null)
      {
        return result;
      }

      result = GetImageInternal(imageName, assemblyName, false);
      if (result != null)
      {
        return result;
      }

      throw new InvalidOperationException("The {0} image cannot be retrieved from {1} assembly".FormatWith(imageName, assemblyName));
    }

    private static ImageSource GetImageInternal(string imageName, string assemblyName, bool highDpi)
    {
      Assert.ArgumentNotNull(imageName, "imageName");
      Assert.ArgumentNotNull(assemblyName, "assemblyName");

      var uri = "pack://application:,,,/{0};component/{1}"
        .FormatWith(
          assemblyName, imageName.Trim().TrimStart('/')
        .Replace("$sm", highDpi ? "24" : "16")
        .Replace("$lg", highDpi ? "48" : "32"));

      try
      {
        var uriLower = uri.ToLower();
        BitmapImage img;
        if (Images.TryGetValue(uriLower, out img))
        {
          return img;
        }

        img = new BitmapImage(new Uri(uri));
        Images.Add(uriLower, img);

        return img;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "The {0} image cannot be retrieved from {1} assembly", imageName, assemblyName);

        return null;
      }
    }

    #endregion
  }
}
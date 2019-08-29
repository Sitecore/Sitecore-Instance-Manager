using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SIM.Tool.Windows.UserControls.Install.ParametersEditor
{
  /// <summary>
  /// Interaction logic for Install9ParametersEditor.xaml
  /// </summary>
  public partial class Install9ParametersEditor : Window
  {
    public Install9ParametersEditor()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      var tasker = this.DataContext as Tasker;
      List<TasksModel> model = new List<TasksModel>();
      model.Add(new TasksModel("Global", tasker.GlobalParams));
      foreach (PowerShellTask task in tasker.Tasks.Where(t=>t.ShouldRun&&t.LocalParams.Any()))
      {
        if (!tasker.UnInstall || (tasker.UnInstall && task.SupportsUninstall()))
        {
          model.Add(new TasksModel(task.Name, task.LocalParams));
        }
      }

      this.InstallationParameters.DataContext = model;
      this.InstallationParameters.SelectedIndex = 0;
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      ScrollViewer scrollviewer = sender as ScrollViewer;
      if (e.Delta > 0)
        scrollviewer.LineLeft();
      else
        scrollviewer.LineRight();
      e.Handled = true;
    }

    private class TasksModel
    {
      public TasksModel(string Name, List<InstallParam> Params)
      {
        this.Name = Name;
        this.Params = Params;
      }

      public string Name { get; }
      public List<InstallParam> Params { get; }
    }

    private void Btn_Close_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}

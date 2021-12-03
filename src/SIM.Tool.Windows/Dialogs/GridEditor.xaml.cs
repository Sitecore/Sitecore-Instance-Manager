using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using SIM.Tool.Base;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Windows.MainWindowComponents.Buttons;

namespace SIM.Tool.Windows.Dialogs
{
  /// <summary>
  /// Interaction logic for GridEditor.xaml
  /// </summary>
  public partial class GridEditor : Window
  {
    public GridEditor()
    {
      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      Assert.ArgumentNotNull(editContext, nameof(editContext));
      this.Title = editContext.Title;
      this.DescriptionText.Text = editContext.Description;
      
      //Bind properties
      IEnumerable<PropertyInfo> propertiesToRender = editContext.ElementType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(RenderInDataGreedAttribute)));
      foreach (var propertyToRender in propertiesToRender)
      {
        this.DataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding(propertyToRender.Name), Header = propertyToRender.Name});
      }

      if (editContext.ElementType.Name == "SolrDefinition")
      {
        this.Add.Content = "Add existing";
        this.InstallSolr.Visibility = Visibility.Visible;
        this.CheckSolr.Visibility = Visibility.Visible;
      }

      if (editContext.ElementType.Name == "SolrState")
      {
        this.Add.Content = "Refresh";
        this.Add.Click -= this.AddRow_Click;
        this.Add.Click += this.RefreshSolrState_OnClick;
        this.DataGrid.IsReadOnly = true;
        this.DataGrid.CanUserSortColumns = false;
        this.DataGrid.Columns[0].Visibility = Visibility.Hidden; // hides the first column with the '-' buttons
        this.Width = 550;
        this.Left += 50; // this is needed to center window position after changing width
        this.UpdateDataGridRowColor();
      }
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      if ((this.DataContext as GridEditorContext).ElementType.Name == "SolrState")
      {
        this.Close();
        return;
      }

      var errors = (from c in
            (from object i in DataGrid.ItemsSource
              select DataGrid.ItemContainerGenerator.ContainerFromItem(i))
          where c != null
          select Validation.GetHasError(c))
        .FirstOrDefault(x => x);

      //In case of empty row was added but wasn't changed. Just remove it.
      if (!errors)
      {
        GridEditorContext editContext = this.DataContext as GridEditorContext;
        for (int i = 0; i < editContext.GridItems.Count; i++)
        {
          if ((editContext.GridItems[i] as IValidateable).HasAnyValuesInTheFields())
          { 
            continue;
          }
          editContext.GridItems.RemoveAt(i);
          i--;
        }
      }

      if (errors)
      {
        if (MessageBox.Show("There are validation errors. Data will not be saved.\nProceed anyway?", "Invalid data",
              MessageBoxButton.YesNo) == MessageBoxResult.No)
        {
          return;
        }
      }
      this.DialogResult = !errors;
      this.Close();
    }

    private void DeleteRow_Click(object sender, RoutedEventArgs e)
    {
      Button b = sender as Button;
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      editContext.GridItems.Remove(b.DataContext);
      this.ReinitializeDataContext(editContext);
    }

    private void AddRow_Click(object sender, RoutedEventArgs e)
    {
      Button b = sender as Button;
      GridEditorContext editContext = this.DataContext as GridEditorContext;
      editContext.GridItems.Add(Activator.CreateInstance(editContext.ElementType));
    }

    private void InstallSolr_OnClick(object sender, RoutedEventArgs e)
    {
      InstallSolrButton installSolrButton = new InstallSolrButton();
      // Refresh the list of Solr servers after installing the new one 
      installSolrButton.InstallationCompleted += (o, args) =>
      {
        GridEditorContext editContext = this.DataContext as GridEditorContext;
        editContext.GridItems.Clear();

        foreach (var solr in ProfileManager.Profile.Solrs)
        {
          editContext.GridItems.Add((SolrDefinition)solr.Clone());
        }

        this.ReinitializeDataContext(editContext);
      };
        
      installSolrButton.InstallSolr(this);
    }

    private void ReinitializeDataContext(GridEditorContext editContext)
    {
      this.DataGrid.DataContext = null;
      this.DataGrid.DataContext = editContext;
    }

    private void CheckSolr_OnClick(object sender, RoutedEventArgs e)
    {
      List<SolrState> solrStates = GetSolrStates();
      if (this.IsSolrStatesCountValid(solrStates))
      {
        GridEditorContext context = this.GetSolrStatesContext(solrStates);
        WindowHelper.ShowDialog<GridEditor>(context, this);
      }
    }

    private void RefreshSolrState_OnClick(object sender, RoutedEventArgs e)
    {
      List<SolrState> solrStates = GetSolrStates();
      this.ReinitializeDataContext(this.GetSolrStatesContext(solrStates));
      this.UpdateDataGridRowColor();
    }

    private List<SolrState> GetSolrStates()
    {
      List<SolrState> solrStates = new List<SolrState>();

      WindowHelper.LongRunningTask(() =>
      {
        SolrStateResolver solrStateResolver = new SolrStateResolver();
        foreach (SolrDefinition solrDefinition in ProfileManager.Profile.Solrs)
        {
          SolrState solrState = new SolrState();
          solrState.Name = solrDefinition.Name;
          solrState.Url = solrDefinition.Url;
          solrState.State = solrStateResolver.GetServiceState(solrStateResolver.GetService(solrDefinition.Service));
          if (solrState.State == SolrState.CurrentState.Running)
          {
            solrState.Version = solrStateResolver.GetVersion(solrDefinition.Url);
            solrState.Type = SolrState.CurrentType.Service;
          }
          else
          {
            solrState.Version = "N/A";
            solrState.Type = SolrState.CurrentType.Unknown;
          }
          solrStates.Add(solrState);
        }
        // If Solr service is not running, possibly Solr is started using CMD, in this case Solr Url accesibility can be checked
        foreach (SolrState solrState in solrStates)
        {
          if (solrState.State != SolrState.CurrentState.Running && !solrStates.Any(s =>
            s.State == SolrState.CurrentState.Running && s.Url == solrState.Url))
          {
            solrState.State = solrStateResolver.GetUrlState(solrState.Url);
            if (solrState.State == SolrState.CurrentState.Running)
            {
              solrState.Version = solrStateResolver.GetVersion(solrState.Url);
              solrState.Type = SolrState.CurrentType.Local;
            }
          }
        }
      }, "Checking states of Solr instances", this);

      return solrStates;
    }

    private bool IsSolrStatesCountValid(List<SolrState> solrStates)
    {
      if (solrStates.Count == 0)
      {
        WindowHelper.ShowMessage("Unable to get states of Solr instances.", MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
      }

      return true;
    }

    private GridEditorContext GetSolrStatesContext(List<SolrState> solrStates)
    {
      return new GridEditorContext(typeof(SolrState), solrStates, "Solr", "States of Solr instances.");
    }

    private void UpdateDataGridRowColor()
    {
      this.DataGrid.UpdateLayout();
      for (int i = 0; i < this.DataGrid.Items.Count; i++)
      {
        DataGridRow row = (DataGridRow)this.DataGrid.ItemContainerGenerator.ContainerFromIndex(i);
        if (row != null)
        {
          SolrState.CurrentState state = (this.DataGrid.Items[i] as SolrState).State;
          switch (state)
          {
            case SolrState.CurrentState.Running:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(WindowsSettings.AppThemeBackgroundSuccess.Value);
              break;
            }
            case SolrState.CurrentState.Stopped:
            {
              row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom(WindowsSettings.AppThemeBackgroundDisabled.Value);
              break;
            }
            default: break;
          }
        }
      }
    }
  }

  internal class GridObjectValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      IValidateable entry = (value as BindingGroup).Items[0] as IValidateable;
      if (entry == null)
      {
        return ValidationResult.ValidResult;
      }

      string error = entry.ValidateAndGetError();
      if (string.IsNullOrEmpty(error))
      {
        return ValidationResult.ValidResult;
      }

      return new ValidationResult(false, error);
    }
  }
}

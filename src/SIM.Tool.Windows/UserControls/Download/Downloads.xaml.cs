﻿namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;

  [UsedImplicitly]
  public partial class Downloads : IWizardStep, ICustomButton, IFlowControl
  {
    #region Fields

    private readonly List<IProductDownloadCheckBox> _CheckBoxItems = new List<IProductDownloadCheckBox>();

    #endregion

    #region Constructors

    public Downloads()
    {
      InitializeComponent();
    }

    #endregion

    #region Properties

    public string CustomButtonText
    {
      get
      {
        return "Open Folder";
      }
    }

    #endregion

    #region Public Methods

    #region ICustomButton Members

    public void CustomButtonClick()
    {
      CoreApp.OpenFolder(ProfileManager.Profile.LocalRepository);
    }

    #endregion

    #region IFlowControl Members

    #region Public methods

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      bool canMoveNext = args.Products.Count > 0;
      if (!canMoveNext)
      {
        WindowHelper.HandleError("You didn't select any download, please select one to go further.", false);
      }
      else
      {
        WindowHelper.LongRunningTask(() => PrepareData(args), "Sitecore Versions Downloader", Window.GetWindow(this), "Preparing for downloading");
      }

      return canMoveNext;
    }

    #endregion

    #region Private methods

    private ReadOnlyCollection<Uri> GetLinks(DownloadWizardArgs args)
    {
      return new ReadOnlyCollection<Uri>(args.Products.Select(product => product.Value).ToArray());
    }

    private void PrepareData(DownloadWizardArgs args)
    {
      try
      {
        var links = GetLinks(args);
        args.Links = links;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error while preparing data");
      }
    }

    #endregion

    #endregion

    #region IStateControl Members

    #region Public methods

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      var selected = _CheckBoxItems.Where(mm => mm.IsChecked);
      args.Products.AddRange(selected);

      return true;
    }

    #endregion

    #endregion

    #endregion

    #region Methods

    #region Public methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      _CheckBoxItems.Clear();
      Append(args.Releases);

      foreach (var product in args.Products)
      {
        var selectedPRoduct = product;
        IProductDownloadCheckBox checkBoxItem = _CheckBoxItems.SingleOrDefault(cbi => cbi.Value.Equals(selectedPRoduct));
        if (checkBoxItem != null)
        {
          checkBoxItem.IsChecked = true;
        }
      }

      filePackages.DataContext = _CheckBoxItems;
    }

    #endregion

    #region Private methods

    private void Append([NotNull] IEnumerable<IRelease> releases)
    {
      Assert.ArgumentNotNull(releases, nameof(releases));

      foreach (IRelease release in releases.OrderByDescending(r => r.Version.MajorMinorUpdateInt))
      {
        _CheckBoxItems.AddRange(GetProductCheckBoxes(release));
      }
    }

    private IEnumerable<IProductDownloadCheckBox> GetProductCheckBoxes(IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));

      ICheckBoxCreator creator;

      if (release.Version.Major < 9)
      {
        creator = new CheckBoxCreatorSC8AndEarlier();
      }
      else
      {
        creator = new CheckBoxCreatorSC9AndLater();
      }

      return creator.Create(release);
    }

    private void ModuleSelected([CanBeNull] object sender, [CanBeNull] SelectionChangedEventArgs e)
    {
      filePackages.SelectedIndex = -1;
    }

    private void UserControlLoaded(object sender, RoutedEventArgs e)
    {
    }

    #endregion

    #endregion
  }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SIM.ContainerInstaller.Repositories.TagRepository.Models;
using Newtonsoft.Json;
using Sitecore.Diagnostics.Logging;

namespace SIM.ContainerInstaller.Repositories.TagRepository.Parsers
{
  public class SitecoreTagsParser
  {
    private const string SitecoreTagsPath = "https://raw.githubusercontent.com/Sitecore/docker-images/master/tags/sitecore-tags.json";

    private readonly int _SitecoreTagsHashFileCheckingDays = 1;

    private string SitecoreTagsFilePath => Path.Combine(ApplicationManager.TempFolder, "sitecore-tags.json");

    private string SitecoreTagsHashFilePath => Path.Combine(ApplicationManager.TempFolder, "sitecore-tags-hash.json");

    public SitecoreTagsParser()
    {
    }

    public IEnumerable<SitecoreTagsEntity> GetTagsEntities()
    {
      return JsonConvert.DeserializeObject<IEnumerable<SitecoreTagsEntity>>(this.GetTagsData());
    }

    public string GetTagsData()
    {
      string json;

      if (!this.IsLocalTagsFileValid())
      {
        json = this.DownloadSitecoreTagsFile();
        if (this.IsValidJson(json))
        {
          File.WriteAllText(this.SitecoreTagsFilePath, json);
          SitecoreTagsHashEntity sitecoreTagsHashEntity = new SitecoreTagsHashEntity
          {
            CheckingDate = DateTime.Now,
            Hash = this.GenerateFileHash(json)
          };
          File.WriteAllText(this.SitecoreTagsHashFilePath, JsonConvert.SerializeObject(sitecoreTagsHashEntity));
        }
      }
      else
      {
        using (StreamReader streamReader = new StreamReader(this.SitecoreTagsFilePath))
        {
          json = streamReader.ReadToEnd();
        }
      }

      return json;
    }

    private bool IsLocalTagsFileValid()
    {
      if (File.Exists(this.SitecoreTagsFilePath) && File.Exists(this.SitecoreTagsHashFilePath))
      {
        string data;
        using (StreamReader streamReader = new StreamReader(this.SitecoreTagsHashFilePath))
        {
          data = streamReader.ReadToEnd();
        }

        SitecoreTagsHashEntity sitecoreTagsHashEntity = JsonConvert.DeserializeObject<SitecoreTagsHashEntity>(data);
        if (sitecoreTagsHashEntity != null)
        {
          if ((DateTime.Now - sitecoreTagsHashEntity.CheckingDate).TotalDays > _SitecoreTagsHashFileCheckingDays)
          {
            data = this.DownloadSitecoreTagsFile();
            if (!string.IsNullOrEmpty(data))
            {
              if (this.GenerateFileHash(data) == sitecoreTagsHashEntity.Hash)
              {
                return true;
              }

              return false;
            }
          }

          return true;
        }
      }

      return false;
    }

    private bool IsValidJson(string json)
    {
      if (!string.IsNullOrEmpty(json))
      {
        IEnumerable<SitecoreTagsEntity> sitecoreTagsEntities = JsonConvert.DeserializeObject<IEnumerable<SitecoreTagsEntity>>(json);
        if (sitecoreTagsEntities != null && sitecoreTagsEntities.Any())
        {
          return true;
        }
      }

      return false;
    }

    private string GenerateFileHash(string data)
    {
      var md5 = MD5.Create();
      var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
      return Convert.ToBase64String(hash);
    }

    private string DownloadSitecoreTagsFile()
    {
      try
      {
        return new WebClient().DownloadString(SitecoreTagsPath);
      }
      catch (Exception ex)
      {
        Log.Error(ex, string.Format("The '{0}' error occurred during getting the list of tags:{1}{2}", 
          ex.Message, Environment.NewLine, ex.StackTrace));

        return null;
      }
    }
  }
}
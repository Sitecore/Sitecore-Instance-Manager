using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SIM.DockerImages.Models;

namespace SIM.DockerImages.Helpers
{
  public class FileHelper
  {
    private const string SitecoreTagsPath = "https://raw.githubusercontent.com/Sitecore/docker-images/master/tags/sitecore-tags.json";

    private readonly int _SitecoreTagsHashFileCheckingDays = 1;

    private string SitecoreTagsFilePath => Path.Combine(ApplicationManager.TempFolder, "sitecore-tags.json");

    private string SitecoreTagsHashFilePath => Path.Combine(ApplicationManager.TempFolder, "sitecore-tags-hash.json");

    public string GetTagsData()
    {
      string json;

      if (!this.IsLocalTagsFileValid())
      {
        json = this.DownloadSitecoreTagsFile();
        if (!string.IsNullOrEmpty(json))
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
      catch
      {
        return null;
      }
    }
  }
}

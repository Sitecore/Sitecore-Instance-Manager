﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SIM.DockerImages.Models
{
  public class Tags
  {
    public string Tag { get; set; }

    public string Digest { get; set; }

    public string Architecture { get; set; }

    public string OS { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime LastUpdateTime { get; set; }

    public string OSVersion { get; set; }

    public string TargetOS { get; set; }
  }
}
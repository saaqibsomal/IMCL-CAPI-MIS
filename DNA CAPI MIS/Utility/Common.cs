
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using DNA_CAPI_MIS.Models;
using Microsoft.AspNet.Identity;
using System.IO;

namespace DNA_CAPI_MIS.Utility
{
    public class Common
    {

        public byte[] Photo(string Path)
        {
            byte[] Img = File.ReadAllBytes(Path);

            return Img;
        }
    }
}
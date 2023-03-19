using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Mappers
{
    public static class ImageMapper
    {
        public static void TransferToViewModel(Image source, ImageViewModel destination)
        {
            destination.AltText = source.AltText;
            destination.Description = source.Caption;
            destination.Name = source.Name;

            // tolower is a quick fix because rewriting impacts static resources as well
            destination.LargeImage = source.LargeImage?.ToLower();
            destination.SmallImage = source.SmallImage?.ToLower();
            destination.Id = source.Id;
        }
    }
}
using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Mappers
{
    public static class PageMapper
    {
        public static void TransferToViewModel(Page entity, PageViewModel viewModel)
        {
            viewModel.Content = entity.Content;
            viewModel.Description = entity.Description;
            viewModel.Slug = entity.Slug;
            viewModel.Title = entity.Title;
            viewModel.Published = entity.Published;
            viewModel.Id = entity.Id;
            viewModel.Status = entity.Status;
            viewModel.Order = entity.Order;

            TransferImagesToViewModelIfPresent(entity, viewModel);
        }

        private static void TransferImagesToViewModelIfPresent(Page entity, PageViewModel viewModel)
        {
            if (entity.PreviewImage != null)
            {
                viewModel.PreviewImage = new ImageViewModel();
                ImageMapper.TransferToViewModel(entity.PreviewImage, viewModel.PreviewImage);
            }

            if (entity.DetailImage != null)
            {
                viewModel.DetailImage = new ImageViewModel();
                ImageMapper.TransferToViewModel(entity.DetailImage, viewModel.DetailImage);
            }
        }
    }
}
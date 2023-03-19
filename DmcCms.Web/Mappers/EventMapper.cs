using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Mappers
{
    public static class EventMapper
    {
        public static void TransferToViewModel(Event entity, EventViewModel viewModel)
        {
            viewModel.Content = entity.Content;
            viewModel.Description = entity.Description;
            viewModel.Slug = entity.Slug;
            viewModel.Title = entity.Title;
            viewModel.Published = entity.Published;
            viewModel.Id = entity.Id;
            viewModel.EventDate = entity.EventDate;
            viewModel.EventType = entity.EventType;
            viewModel.Order = entity.Order;
            viewModel.Status = entity.Status;

            TransferImagesToViewModelIfPresent(entity, viewModel);
        }

        private static void TransferImagesToViewModelIfPresent(Event entity, EventViewModel viewModel)
        {
            if (entity.Image != null)
            {
                viewModel.Image = new ImageViewModel();
                ImageMapper.TransferToViewModel(entity.Image, viewModel.Image);
            }
        }
    }
}
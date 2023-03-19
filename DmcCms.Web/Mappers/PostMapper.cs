using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Mappers
{
    public static class PostMapper
    {
        public static void TransferToViewModel(Post entity, PostViewModel viewModel)
        {
            viewModel.Content = entity.Content;
            viewModel.Description = entity.Description;
            viewModel.IsCommentingEnabled = entity.IsCommentingEnabled;
            viewModel.Slug = entity.Slug;
            viewModel.Title = entity.Title;
            viewModel.Published = entity.Published;
            viewModel.Id = entity.Id;
            viewModel.NumberOfComments = (entity.Comments?.Count).GetValueOrDefault();

            //TODO: more ... 

            TransferImagesToViewModelIfPresent(entity, viewModel);
            TransferTagsToViewModelIfPresent(entity, viewModel);
            TransferCategoriesToViewModelIfPresent(entity, viewModel);
        }

        private static void TransferCategoriesToViewModelIfPresent(Post entity, PostViewModel viewModel)
        {
            if (entity.Categories == null)
            {
                return;
            }

            foreach (var item in entity.Categories)
            {
                viewModel.Categories.Add(new CategoryListViewModel
                {
                    Name = item.Title,
                    Slug = item.Slug
                });
            }
        }

        private static void TransferTagsToViewModelIfPresent(Post entity, PostViewModel viewModel)
        {
            if (entity.Tags == null)
            {
                return;
            }

            foreach (var item in entity.Tags)
            {
                viewModel.Tags.Add(new TagListViewModel
                {
                    Name = item.Title,
                    Slug = item.Slug
                });
            }
        }

        private static void TransferImagesToViewModelIfPresent(Post entity, PostViewModel viewModel)
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
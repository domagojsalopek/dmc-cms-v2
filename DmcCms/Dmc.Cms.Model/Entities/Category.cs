using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class Category : ContentBase 
    {
        // TODO: VALIDATORS SHOULD BE IN THIS PROJECT. OTHERWISE WE DEPEND ON VALIDATION IN REPOSITORY.EF.
        // TODO: Think about this
        // We don't need author on category or tag ...

        #region Constructors
        
        public Category()
        {
            Children = new List<Category>();
            Posts = new List<Post>();
        }

        #endregion

        #region Relation Key Properties

        public int? ParentId // this shouldn't maybe be part of the model ...
        {
            get;
            set;
        }

        public int? IntroImageId
        {
            get;
            set;
        }

        #endregion

        #region Properties

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Category Parent
        {
            get;
            set;
        }

        public ICollection<Category> Children
        {
            get;
            private set;
        }

        public ICollection<Post> Posts
        {
            get;
            private set;
        }

        public Image IntroImage
        {
            get;
            set;
        }

        #endregion

        #region Abstract Implementation

        public override bool CanBeDisplayed => Status == ContentStatus.Published;

        #endregion

        #region Non Database Properties

        public bool HasChildren => Children != null && Children.Count > 0;

        public bool IsRoot => !ParentId.HasValue;

        #endregion
    }
}

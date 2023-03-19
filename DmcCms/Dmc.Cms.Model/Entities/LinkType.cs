using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public enum LinkType
    {
        Undefined = 0,
        Website = 1,
        Email = 2,
        Anchor = 3,
        Page = 4,
        Post = 5,
        Category = 6,
        Content = 7,
        ContentGroup = 8
        // different types fb, twitter ... ?
    }
}

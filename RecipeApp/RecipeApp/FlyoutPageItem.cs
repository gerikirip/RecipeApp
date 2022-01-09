using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeApp
{
    class FlyoutPageItem
    {
        public string Title { get; set; }

        public string IconSource { get; set; }

        public Type TargetPage { get; set; }
    }
}

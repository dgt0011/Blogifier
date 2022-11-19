
using System.Collections.Generic;

namespace Blogifier.Shared
{
    public class SnippetModel
    {
        public BlogItem Blog { get; set; }

        public IEnumerable<SnippetCategoryItem> Categories { get; set; }

        public string SelectedSnippet { get; set; }

    }
}

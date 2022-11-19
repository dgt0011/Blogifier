using System.Collections.Generic;

namespace Blogifier.Shared
{
    public class SnippetCategoryItem
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public List<SnippetCategoryItem> SubCategories { get; set; }

        public List<SnippetItem> Snippets { get; set; }
    }
}

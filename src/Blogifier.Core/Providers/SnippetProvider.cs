using System;
using Blogifier.Core.Data;
using Blogifier.Shared;
using Blogifier.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogifier.Core.Providers
{
    public interface ISnippetProvider
    {
        Task<List<TreeBranch>> GetSnippets();

        Task<string> GetSnippet(int id);

        Task<bool> AddSnippetCategory(int? parentCategoryId, string categoryTitle);
    }

    public class SnippetProvider : ISnippetProvider
    {
        private readonly AppDbContext _db;

        public SnippetProvider(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<TreeBranch>> GetSnippets()
        {
            // get all snippet branches and leaves so that there aren't repeated calls to the db - work 'in memory' instead
            var allSnippetLeaves = await _db.TreeLeaves.AsNoTracking().Where(a => a.CategoryType == CategoryType.Snippet).ToListAsync();
            var allSnippetBranches = await _db.TreeBranches.AsNoTracking().Where(a => a.CategoryType == CategoryType.Snippet).ToListAsync();

            // get all the top level branches without a parent Id set
            var retVal = new List<TreeBranch>();
            foreach(var branch in allSnippetBranches.Where(a => a.ParentBranchId == null))
            {
                retVal.Add(branch);

                // find any branches that have this branch as its parent
                var childBranches = FindSubBranchesFor(branch.Id, allSnippetBranches);
                if(childBranches != null && childBranches.Any())
                {
                    branch.Branches = new List<TreeBranch>(childBranches);
                }

                // find any leaves that have this branch as a parent
                FindLeavesFor(branch, allSnippetLeaves);
            }

            return retVal;
        }

        public async Task<string> GetSnippet(int id)
        {
            var snippetLeaf = await _db.TreeLeaves.AsNoTracking()
                .Where(a => a.CategoryType == CategoryType.Snippet && a.Id == id).FirstOrDefaultAsync();

            if (snippetLeaf != null)
            {
                return snippetLeaf.Content;
            }

            return string.Empty;
        }

        public async Task<bool> AddSnippetCategory(int? parentCategoryId, string categoryTitle)
        {
            var newTreeBranch = new TreeBranch { CategoryType = CategoryType.Snippet, Title = categoryTitle, ParentBranchId = parentCategoryId };
            await _db.TreeBranches.AddAsync(newTreeBranch);
            return await _db.SaveChangesAsync() > 0;
        }

        private List<TreeBranch> FindSubBranchesFor(int parentBranchId, List<TreeBranch> allBranches)
        {
            var result = new List<TreeBranch>();
            var branches = allBranches.Where(a=>a.ParentBranchId == parentBranchId);
            foreach(var branch in branches)
            {
                result.Add(branch);

                var childBranches = FindSubBranchesFor(branch.Id, allBranches);
                if (childBranches != null && childBranches.Any())
                {
                    branch.Branches = new List<TreeBranch>(childBranches);
                }
            }
            return result;
        }

        private void FindLeavesFor(TreeBranch branch, List<TreeLeaf> allLeaves)
        {
            var result = new List<TreeLeaf>();

            if(allLeaves.Any(a=>a.BranchId == branch.Id))
            {
                branch.Leaves = new List<TreeLeaf>(allLeaves.Where(a => a.BranchId == branch.Id));
            }

            if(branch.Branches != null)
            {
                foreach(var subBranch in branch.Branches)
                {
                    FindLeavesFor(subBranch, allLeaves);
                }
            }
        }
    }
}

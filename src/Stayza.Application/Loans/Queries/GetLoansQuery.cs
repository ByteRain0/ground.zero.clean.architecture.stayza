using Stayza.Core.PagingAndSorting;

namespace Stayza.Application.Loans.Queries;

public class GetLoansQuery : IPagedQuery, ISortedQuery
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public string? SortColumn { get; set; }
    
    public SortOrder? SortOrder { get; set; }

    public string? UserId { get; set; }
}
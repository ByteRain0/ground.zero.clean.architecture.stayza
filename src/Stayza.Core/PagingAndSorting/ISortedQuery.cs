namespace Stayza.Core.PagingAndSorting;

public interface ISortedQuery
{
    public string? SortColumn { get; set; }

    public SortOrder? SortOrder { get; set; }
}
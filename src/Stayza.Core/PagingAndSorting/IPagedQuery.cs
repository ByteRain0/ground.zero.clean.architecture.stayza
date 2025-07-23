namespace Stayza.Core.PagingAndSorting;

public interface IPagedQuery
{
    int Page { get; set; }

    int PageSize { get; set; }
}
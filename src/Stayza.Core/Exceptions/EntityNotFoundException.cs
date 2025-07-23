namespace Stayza.Core.Exceptions;

public class EntityNotFoundException : Exception
{
    public string EntityType { get; set; }

    public string SearchKey { get; set; }
    
    public EntityNotFoundException(string entityType, string searchKey) 
        : base($"Entity {entityType} by search query / or id : {searchKey} has not been found")
    {
        EntityType = entityType;
        SearchKey = searchKey;
    }
}
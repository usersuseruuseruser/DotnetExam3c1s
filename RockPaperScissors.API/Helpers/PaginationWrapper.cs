namespace api.Helpers;

public record PaginationWrapper<T>(List<T> Data, int Page, int PageSize, long TotalCount);
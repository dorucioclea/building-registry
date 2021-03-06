namespace BuildingRegistry.Api.Legacy.Infrastructure
{
    using System;
    using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;

    public static class PaginationInfoExtension
    {
        public static Uri BuildNextUri(this PaginationInfo paginationInfo, string volgendeUrlBase)
        {
            var offset = paginationInfo.Offset;
            var limit = paginationInfo.Limit;

            return paginationInfo.HasNextPage
                ? new Uri(string.Format(volgendeUrlBase, offset + limit, limit))
                : null;
        }
    }
}

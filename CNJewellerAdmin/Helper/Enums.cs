namespace CNJewellerAdmin.Helper
{
    public enum RowStatus
    {
        Deleted = -1,
        Inactive = 0,
        Active = 1
    }

    public enum AcknowledgeType
    {
        Failure = 0,
        Success = 1,
        Warning = 2,
        Confirmation = 3,
        Information = 4
    }

    public enum SortDirection
    {
        Descending = 0,
        Ascending = 1
    }

    public enum Genders
    {
        Male = 1,
        Female = 2
    }

    public enum OperationTypes
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}

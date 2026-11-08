namespace LibraryStore.Application.Common.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string errorMessage) : base($"BusinessRuleException: {errorMessage}")
        {

        }
    }
}

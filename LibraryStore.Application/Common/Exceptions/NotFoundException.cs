namespace LibraryStore.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key) : base($"{name} with Id :{key} was not found")
        {

        }
    }
}

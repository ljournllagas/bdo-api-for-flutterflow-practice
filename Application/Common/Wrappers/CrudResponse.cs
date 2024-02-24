using Application.Constants;

namespace Application.Common.Wrappers
{
    public class CrudResponse
    {
        public CrudOperation CrudOperation { get; set; }

        public CrudResponse(CrudOperation crudOperation)
        {
            CrudOperation = crudOperation;
        }

        public override string ToString()
        {
            return CrudOperation switch
            {
                CrudOperation.Create => $"Successfully added the record",
                CrudOperation.Update => $"Successfully updated the record",
                CrudOperation.Delete => $"Successfully deleted the record",
                CrudOperation.Patch => $"Successfully patched the record",
                _ => "Successfully completed the operation",
            };
        }
    }
}

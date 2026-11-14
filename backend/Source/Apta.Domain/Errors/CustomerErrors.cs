namespace Apta.Domain.Errors;

public static class CustomerErrors
{
    public static readonly Error AlreadyExists = new(
        Code: "#CUSTOMERS-ERROR-C7E5F",
        Description: "A customer with the same document already exists."
    );

    public static readonly Error DoesNotExist = new(
        Code: "#CUSTOMERS-ERROR-60D0A",
        Description: "The specified customer does not exist."
    );
}

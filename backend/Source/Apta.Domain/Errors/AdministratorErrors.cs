namespace Apta.Domain.Errors;

public static class AdministratorErrors
{
    public static readonly Error AlreadyExists = new(
        Code: "#ADMINISTRATORS-ERROR-5F4EC",
        Description: "An administrator with the same code already exists."
    );

    public static readonly Error DoesNotExist = new(
        Code: "#ADMINISTRATORS-ERROR-F1F4B",
        Description: "The specified administrator does not exist."
    );
}

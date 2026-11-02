namespace Apta.Domain.Errors;

public static class AdministratorErrors
{
    public static readonly Error AlreadyExists = new(
        Code: "#ADMINISTRATORS-ERROR-5F4EC",
        Description: "An administrator with the same code already exists."
    );
}

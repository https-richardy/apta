namespace Apta.Infrastructure.Persistence.Configurations;

public sealed class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.HasKey(administrator => administrator.Id);
        builder.Property(administrator => administrator.Id)
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(administrator => administrator.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(administrator => administrator.Code)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(administrator => administrator.CreatedAt)
            .IsRequired();

        builder.Property(administrator => administrator.UpdatedAt)
            .IsRequired();

        builder.Property(administrator => administrator.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        /* in this section, configure the entity's indexes */
        /* https://learn.microsoft.com/pt-br/ef/core/modeling/indexes */
        builder.HasIndex(administrator => administrator.Code)
            .IsUnique();

        builder.HasIndex(administrator => administrator.Name);
        builder.HasIndex(administrator => administrator.CreatedAt);
    }
}

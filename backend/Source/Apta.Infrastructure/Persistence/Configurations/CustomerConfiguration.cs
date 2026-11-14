namespace Apta.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);
        builder.Property(customer => customer.Id)
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(customer => customer.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(customer => customer.Phone)
            .IsRequired(false)
            .HasMaxLength(20);

        builder.Property(customer => customer.Document)
            .IsRequired(false)
            .HasMaxLength(14);

        builder.Property(customer => customer.CreatedAt)
            .IsRequired();

        builder.Property(customer => customer.UpdatedAt)
            .IsRequired();

        builder.Property(customer => customer.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        /* in this section, configure the entity's indexes */
        /* https://learn.microsoft.com/pt-br/ef/core/modeling/indexes */
        builder.HasIndex(customer => customer.Document)
            .IsUnique();

        builder.HasIndex(customer => customer.Name);
        builder.HasIndex(customer => customer.Phone);
        builder.HasIndex(customer => customer.CreatedAt);
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Parking.Models
{
    public class ParkingDbContext : IdentityDbContext<Usuario>
    {

        public ParkingDbContext(DbContextOptions<ParkingDbContext> options) : base(options) { }

        public DbSet<TipoVehiculo> TipoVehiculo { get; set; }
        public DbSet<Espacio> Espacios { get; set; }
        public DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);// Identity configurado + tablas configuradas

            builder.Entity<TipoIdentificacion>(entity =>
            {
                entity.ToTable("tipo_identificacion");
                entity.Property(ti => ti.Id).UseIdentityAlwaysColumn();
                entity.Property(ti => ti.Sigla).HasDefaultValue("A");
                entity.Property(ti => ti.FechaCreacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(ti => ti.FechaActualizacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.Entity<TipoVehiculo>(entity =>
            {
                entity.Property(tv => tv.Id).UseIdentityAlwaysColumn();
                entity.Property(tv => tv.Estado).HasDefaultValue("A");
                entity.Property(tv => tv.FechaCreacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(tv => tv.FechaActualizacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            builder.Entity<Espacio>(entity =>
            {
                entity.ToTable("espacio");
                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
                entity.Property(e => e.Estado).HasDefaultValue("A");
                entity.Property(e => e.FechaCreacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.FechaActualizacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(e => e.TipoVehiculoNavigation).WithMany(tv => tv.Espacios).HasForeignKey(e => e.IdTipoVehiculo);
                entity.HasOne(e => e.UsuarioNavigation).WithMany(u => u.Espacios).HasForeignKey(e => e.IdUsuarioActualizacion);
            });

            builder.Entity<Servicio>(entity =>
            {
                entity.ToTable("servicio");
                entity.Property(s => s.Id).UseIdentityAlwaysColumn();
                entity.HasOne(s => s.TipoVehiculoNavigation).WithMany(tv => tv.Servicios).HasForeignKey(s => s.IdTipoVehiculo);
                entity.HasOne(s => s.EspacioNavigation).WithOne(e => e.ServicioNavigation).HasForeignKey<Servicio>(s => s.EspacioNumero).HasPrincipalKey<Espacio>(e => e.Numero);
                entity.Property(s => s.FechaHoraIngreso).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(s => s.FechaHoraSalida).HasColumnType("timestamp without time zone");
                entity.Property(s => s.Estado).HasDefaultValue("A");
                entity.Property(s => s.FechaCreacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(s => s.FechaActualizacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.HasOne(s => s.UsuarioCreacionNavigation).WithMany(u => u.ServiciosCreados).HasForeignKey(s => s.IdUsuarioCreacion);
                entity.HasOne(s => s.UsuarioActualizacionNavigation).WithMany(u => u.ServiciosActualizados).HasForeignKey(s => s.IdUsuarioActualizacion);
                entity.HasOne(s => s.UsuarioFinalizacionNavigation).WithMany(u => u.ServiciosFinalizados).HasForeignKey(s => s.IdUsuarioFinalizacion);
            });

            builder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("tarifa");
                entity.Property(t => t.Id).UseIdentityByDefaultColumn();
                entity.Property(t => t.FechaCreacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(t => t.FechaActualizacion).HasColumnType("timestamp without time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}
﻿namespace Core.Entities
{
    public partial class Rol
    {
        public Rol()
        {
            People = new HashSet<User>();
        }

        public int RolId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<User> People { get; set; }

        public static Rol Administrador { get; private set; } = new() { RolId = 1, Name = nameof(Administrador) };
        public static Rol Cliente { get; private set; } = new() { RolId = 2, Name = nameof(Cliente) };
    }
}

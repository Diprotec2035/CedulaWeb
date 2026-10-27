namespace Modelo.v1.ModuloPerfil
{
    public class PermisoModuloUsuarioModelo
    {
        public uint IdModulo { get; set; }

        public string NombreModulo { get; set; }

        public sbyte Estado { get; set; }

        public sbyte Consulta { get; set; }

        public sbyte Nuevo { get; set; }

        public sbyte Modifica { get; set; }

        public bool PuedeVer
        {
            get { return Estado == 0; }
        }

        public bool PuedeConsultar
        {
            get { return Consulta == 0; }
        }

        public bool PuedeNuevo
        {
            get { return Nuevo == 0; }
        }

        public bool PuedeModificar
        {
            get { return Modifica == 0; }
        }
    }
}
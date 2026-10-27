using System;

namespace Modelo.v1.VersionAplicacion
{
    public class VersionAplicacionModelo
    {
        public uint Id { get; set; }

        public int VersionCode { get; set; }

        public string VersionName { get; set; }

        public int MinSupportedVersionCode { get; set; }

        public sbyte ForceUpdate { get; set; }

        public sbyte IsActive { get; set; }

        public sbyte Ispublished { get; set; }

        public string Fabricante { get; set; }

        public string FabricanteApk { get; set; }

        public string Modelo { get; set; }

        public string ModeloApk { get; set; }

        public int AndroidVersion { get; set; }

        public string ModeloKey { get; set; }

        public string ApkFileName { get; set; }

        public string ApkRelativePath { get; set; }

        public string ApkSha256 { get; set; }

        public long FileSizeBytes { get; set; }

        public string ReleaseNotes { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string PublishedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string RutUsuario { get; set; }
    }
}
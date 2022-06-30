namespace PosStoneNfce.API.Portal.App.Common.Services
{
    public class AppSettings
    {   
        public string PosStoneNfceApiAutenticacaoUrl { get; set; }

        public string MideEvoApiUrl { get; set; }
        
        public string CertificadoApi { get; set; }
        
        public string KeyAzure { get; set; }

        public string IdUsuarioMide { get; set; }

        public string DbAutenticacaoConnection { get; set; }

        public string VersionApiCertificado { get; set; }
        
        public string MideApplicationId { get; set; }
    }

    public class SettingsCertificado
    {   
        public string EncryptPassword { get; set; }
    }
}
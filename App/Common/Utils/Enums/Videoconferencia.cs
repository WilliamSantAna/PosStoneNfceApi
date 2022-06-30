using System.ComponentModel;

namespace PosStoneNfce.API.Portal.App.Modules.Videoconferencia
{
    public enum VideoconferenciaRota
    {
        [Description("ImportarPedidoRequest")]
        ImportarPedido,

        [Description("NovaSolicitacaoRequest")]
        NovaSolicitacao,

        [Description("ListarHorariosRequest")]
        ListaHorariosDisponiveis,

        [Description("AgendarVideoRequest")]
        AgendarVideoconferencia,

        [Description("ImportarDocumentoRequest")]
        ImportarDocumentos,
            
        [Description("VoucherExistenteRequest")]
        VoucherExistente,
            
        [Description("ListarSolicitacoesRequest")]
        ListarSolicitacoes,

    }
}
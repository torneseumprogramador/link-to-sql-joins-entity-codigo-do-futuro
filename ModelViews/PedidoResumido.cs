namespace locacao_veiculos.ModelViews;

public struct PedidoResumido
{
    public int PedidoId { get;set; }
    public string NomeCliente { get;set; }
    public string NomeCarro { get;set; }
    public string ModeloDoCarro { get;set; }
    public string MarcaDoCarro { get;set; }
    public DateTime DataLocacaoPedido { get;set; }
    public DateTime DataEntregaPedido { get;set; }
}
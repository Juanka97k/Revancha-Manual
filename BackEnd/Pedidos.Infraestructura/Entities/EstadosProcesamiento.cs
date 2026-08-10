namespace Pedidos.Infraestructura.Entities
{
    public enum EstadoOutbox
    {
        SinProcesar = 0,
        Publicado = 1,
        Procesado = 2,
    }
}
namespace Pedidos.Dominio.Entidades
{
    public enum EstadoOutbox
    {
        SinProcesar = 0,
        Publicado = 1,
        Procesado = 2,
        Fallo = 3
    }
}

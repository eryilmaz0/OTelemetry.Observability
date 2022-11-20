namespace Core.Model.Client;

public class ClientDataResponse<TData> : ClientResponse
{
    public TData Payload { get; set; }
}
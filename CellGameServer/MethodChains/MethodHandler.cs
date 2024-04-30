namespace CellGameServer.MethodChains;

public abstract class MethodHandler
{
    protected MethodHandler? Successor;

    public void SetSuccessor(MethodHandler successor)
    {
        Successor = successor;
    }

    public abstract Task HandleRequest(MethodRequestModel request, CancellationToken cancellationToken);
}
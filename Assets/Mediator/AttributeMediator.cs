using System;

public struct MediatorMessage<TRequest, TResponse>
    where TRequest : struct
    where TResponse : struct
{
    public TRequest Request { get;  }
    public TResponse Response { get; }    
    
    public MediatorMessage(TRequest request)
    {
        Request = request;
        Response = default;
    }
    
    public MediatorMessage(TRequest request, TResponse response)
    {
        Request = request;
        Response = response;
    }
}

public static class Sandbox
{
    public static void Do() 
    {
        var mediator = new AttributeMediator();
        var message = new MediatorMessage<NewGame, DetectLoot>(new NewGame());
        var response = mediator.Send(message);
    }
}

public class AttributeMediator
{
    public TResponse Send<TRequest, TResponse>(MediatorMessage<TRequest, TResponse> message)
        where TRequest : struct
        where TResponse : struct
    {
        return default(TResponse);
    }
}

public class MediatorMessageAttribute : Attribute
{
    public MediatorMessageAttribute()
    {
    }
}

public class MediatorHandlerAttribute : Attribute
{
    public string Name { get; }
    
    public MediatorHandlerAttribute(string name)
    {
        Name = name;
    }
}
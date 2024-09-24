namespace VCard.Common.Application.RequestContext;

internal sealed class RequestContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public IRequestContext? Context
    {
        get => Holder.Value?.Context;
        set
        {
            var holder = Holder.Value;
            if (holder is not null) holder.Context = null;

            if (value is not null) Holder.Value = new ContextHolder { Context = value };
        }
    }

    private class ContextHolder
    {
        public IRequestContext? Context;
    }
}
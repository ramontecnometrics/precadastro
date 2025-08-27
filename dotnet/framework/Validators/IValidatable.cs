using System;

namespace framework.Validators
{
    public interface IValidatable
    {
        AggregateException Validate();
    }
}

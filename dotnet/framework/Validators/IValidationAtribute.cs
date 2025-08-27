namespace framework.Validators
{
    public enum GroupValidationType
    {
        /// <summary>
        /// Se pelo menos um for válido então o grupo é valido
        /// </summary>
        AtLeastOneValid,
        /// <summary>
        /// Se um for informado então todos tem que ser válidos
        /// </summary>
        ValidateAllIfNotNull
    }

    public interface IValidationAttribute
    {
        bool IsValid(object value);
        string ErrorMessage { get;  }        
    }

    public interface IGroupValidationAtribute: IValidationAttribute
    {
        string GroupId { get; }
        GroupValidationType GroupValidationType { get; }
    }

}

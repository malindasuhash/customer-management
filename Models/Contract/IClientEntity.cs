namespace Models.Contract
{
    public interface IClientEntity : IEntity, ICloneable
    {
        public int DraftVersion { get; set; }

        public int LastSubmittedVersion { get; set; }
    }
}

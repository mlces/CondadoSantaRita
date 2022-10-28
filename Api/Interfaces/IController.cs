namespace Api.Interfaces
{
    public interface IController
    {
        public int PersonId { get; set; }

        public Guid TokenId { get; set; }
    }
}

namespace Api.Interfaces
{
    public interface IController
    {
        public int PersonId { get; set; }

        public int TokenId { get; set; }

        public ApplicationContext DbContext { get; set; }
    }
}

namespace BlazorAppDemo01.Data
{
    public class DogsService
    {
        private static readonly string[] Races = new[]
        {
        "Pastore tedesco", "Pitbull", "Maltese","Barboncini","Chihuahua","Bulldog"
        };

        public List<Dog> GetDogs()
        {
            List<Dog> retDog = new List<Dog>();
            Random rnd = new Random();

            for (int i = 0; i < 10000; i++)
            {
                int rndNumber = rnd.Next(0, 5);

                retDog.Add(new Dog()
                {
                    Name = "dog" + i,
                    Race = Races[rndNumber]
                });
            }

            return retDog;
        }
    }
}
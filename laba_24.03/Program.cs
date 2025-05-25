// See https://aka.ms/new-console-template for more information
using laba_24._03;


class program
{
    static void Main(string[] args)
    {
        using (game game = new game(1700, 900))
        {
            game.Run();

        }

    }
}
//vec4(aPosition, 1.0) * model * view * projection; 
using System.Threading.Channels;
using System.Xml.Linq;


namespace LAB3 
{
public struct Token
{
    public string data;
    public int recipient;
    public int ttl;
}

public class TokenRing
{
    public int N { get; set;}
    public static int Nnew = 0;
    public TokenRing()
    {
        N = Nnew + 1;
    }
    public static async Task GetTokenRingTask(int n, Token t)
    {
            List<Channel<Token>> CreateChanel = new List<Channel<Token>>();
            CreateChanel.Add(Channel.CreateBounded<Token>(new BoundedChannelOptions(1)));
            await CreateChanel[0].Writer.WriteAsync(t);

            List<Task> tasks = new List<Task>();
            for (int i = 1; i < n; i++)
            {
                CreateChanel.Add(Channel.CreateBounded<Token>(new BoundedChannelOptions(1)));
                int number = i;
                tasks.Add(ViewProgress(number, CreateChanel[number],CreateChanel[number - 1]));
            }
            await Task.WhenAll(tasks);
    }
    public static async Task ViewProgress(int tasknumber, Channel<Token> NextStep, Channel<Token, Token> PreviousStep)
    {
            await PreviousStep.Reader.WaitToReadAsync();
            var token = await PreviousStep.Reader.ReadAsync();
            if (token.recipient == tasknumber)
            {
                Console.WriteLine("This is number: {0}  This is message from number: {1} ", tasknumber, token.data);
            }
            else
            {
                Console.WriteLine("This is not token of a number: {0} ", tasknumber);
            }
            await NextStep.Writer.WriteAsync(token);
    }
}

public class program
{
    static async Task Main(string[] args)
    {
            int NfromUser;
            NfromUser = Convert.ToInt32(Console.ReadLine());
            TokenRing[] tokenRings = new TokenRing[NfromUser];
            Token tokendefault = new Token
            {
                data = "message something",
                recipient = NfromUser - 1,
                ttl = 1,
            };
            await TokenRing.GetTokenRingTask(NfromUser, tokendefault);
    }
}

}
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SeverinoBot
{
    public class Program
    {
        public static IConfigurationRoot configuration;

        public static async Task Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var discordBotKey = configuration["discordBotKey"];
            var defaultChannelId = configuration["defaultChannelId"];

            var discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = discordBotKey,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            DiscordChannel defaultChannel = null;
            if (ulong.TryParse(defaultChannelId, out var channelId))
                defaultChannel = await discordClient.GetChannelAsync(channelId);
            if (defaultChannel != null)
                await defaultChannel.SendMessageAsync("Salve rapaziada o ousado chegou");

            discordClient.MessageCreated += OnMessageCreated;

            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
        }

        private static async Task OnMessageCreated(MessageCreateEventArgs e)
        {
            if (!e.Message.Author.IsBot && !e.Message.Content.Trim().StartsWith("-"))
            {
                if (e.Message.Content.Trim().ToLower().Equals("parabens"))
                {
                    await e.Message.RespondAsync("po cara, parabens ai velho, de boa, muito legal isso. contei pra todos aqui da minha familia, todos acharam muito surpreendente e pediram pra te dar os parabens, queriam falar com você pessoalmente se possivel para lhe parabenizar. disseram também que na festa de natal irão contar para os parentes mais distantes e no ano novo lançarão baterias de fogos com seu nome. contei esse seu feito também para alguns outros parentes mais próximos, reagiram tal qual minha familia, pediram seu endereço para mandar cartões e mensagem de parabenização. meus amigos não acreditaram quando eu disse que conhecia o dono desse feito tão imenso, sério, ficaram todos de boca aberta, disseram que farão seu nome ecoar por anos e anos. quando os vizinhos ficaram sabendo do feito, ficaram todos boquiabertos, quiseram saber quem é você, pediu se, caso você tiver tempo, é claro, de poderia passar aqui para receber presentes, congratulações e apertos de mãos. com o esparrame da sua noticia, um grande empresario da região decidiu te contratar como presidente da empresa graças a esse seu surepreendente feito e ao mesmo tempo um grande acionista internacional quer patrocinar shows para você para palestrar e ensinar todos a fazerem igual para que o mundo seja um lugar melhor. você não só está famoso aqui na região quanto aí mas também em todas as partes, todos sabem quem é você graças ao rápido esparrame da notícia, prefeitos de todas as cidades estão pendurando faixas, balões, teleféricos, instalando aparelhos de som, tudo o que possa fazer seu nome vibrar para ver qual cidade te consagra mais por esse seu feito magnifico. aqui na minha cidade mesmo cada rua terá seu sobrenome a partir da próxima gestão da administração municipal. muitos países que antes viam o brasil com maus olhos, agora, graças ao seu feito, vêm o brasil como um exemplo, como uma nova capacitação, os grandes sortudos que sabem sobre você diz \"ei, aquele cara é brasileiro\" e todos replicam imediatamente \"é! é! é! o brasil é ");
                    await e.Message.RespondAsync("um bom lugar\". Graças a isso o turismo aumentou no brasil, todos vieram para cá graças a você, a entrada de moedas internacionais foi grande fazendo as bolsas e ações brasileiras decolarem e assim o brasil se tornou o pilar para solução da crise mundial. Graças a isso somos bem vistos e, claro, somos a maior potencia economica do mundo. todos os madeireiros se comoveram com seu feito e decidiram parar de explorar a amazonia para que o mundo viva mais e mais. o caos por conta do presidente paixão nos estados unidos foi cessado graças ao fato do brasil ser o lider economico mundial, uma vez sendo um país de varias etnias, todos passaram a aceitar as diferenças com amor no coração. o papa mandou todos os seus representantes pelo mundo falar sobre seu nome e sobre seus feitos para que a palavra sobre vossa pessoa chegue aos ouvidos de cada criatura que ande sobre a face desse planeta. Também, graças ao seu feito, decidiram cessar os experimentos com o LHC já que a origem do universo se torna sem importancia perto da magnitude desse seu ato. Os Maias voltaram de andromeda e disseram que como existe um humano tão magnifico vivo eles iriam dar a chance de nós sobrevivermos em 2012, contaram então sobre o que poderia causar o fim do mundo, e todos os lideres de todas as nações, inspirados nesse seu feito, estão tomando providencias para que não ocorra. a magnitude desse seu feito acabou até com o magnetismo que expulsou o corpo celeste alfa que habitava a órbita da terra. Em nome desse seu feito, Akira Toryama resolveu continuar com as sagas de dragon ball, desta vez com um personagem dedicado a você. Willian Bonner e Jô Soares ao se despedirem toda noite mandam uma saudação para o Brasil e uma somente para você. Continue sempre assim, essa pessoa linda, koshervilhosa, esforçada, inspiradora, magnifica, espitufenda, criativa, etc. E continue sempre fazendo atos como estes que o mundo será cada vez mais um lugar melhor para se viver.");
                    return;
                }

                string mensagem = e.Message.Content.Trim();
                await e.Message.RespondAsync(RandomizeMessageOrder(mensagem));
                return;
            }
        }
        private static string RandomizeMessageOrder(string message)
        {
            string fimMensagem = "";
            if (message.EndsWith("?") || message.EndsWith("!"))
            {
                fimMensagem = message.Last().ToString();
                message = message.Remove(message.Length - 1);
            }

            string randomizedMessage = "";
            string[] splitString = message.Trim().Split(" ");
            int[] randomOrder = RandomOrder(splitString.Length);
            for (int i = 0; i < splitString.Length; i++)
            {
                randomizedMessage += $"{splitString[randomOrder[i]]} ";
            }

            randomizedMessage = randomizedMessage.Trim();
            if (randomizedMessage.EndsWith(",") || randomizedMessage.EndsWith("?") || randomizedMessage.EndsWith("!"))
            {
                randomizedMessage = randomizedMessage.Remove(randomizedMessage.Length - 1);
            }

            randomizedMessage += fimMensagem;
            return randomizedMessage;
        }
        private static int[] RandomOrder(int quantity)
        {
            var randomizer = new Random();
            HashSet<int> randomOrder = new HashSet<int>();

            do
            {
                randomOrder.Add(randomizer.Next(0, quantity));
            }
            while (randomOrder.Count != quantity);

            return randomOrder.ToArray();
        }
    }
}
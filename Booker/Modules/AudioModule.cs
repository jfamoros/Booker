using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using PartyBot.Handlers;
using PartyBot.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PartyBot.Modules
{
    public class AudioModule : InteractiveBase
    {
        public LavaLinkAudio AudioService { get; set; }
        private readonly LavaNode _lavaNode;
        private readonly DiscordSocketClient _client;

        public AudioModule(LavaNode lavaNode, DiscordSocketClient client)
        {
            _lavaNode = lavaNode;
            _client = client;
        }

        [Command("Join")]
        public async Task JoinAndPlay()
            => await ReplyAsync(embed: await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));

        [Command("Stop")]
        public async Task Stop()
             => await ReplyAsync(embed: await AudioService.StopAsync(Context.Guild));

        [Command("search", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync([Remainder] string query)
        {
            try
            {
                var descriptionBuilder = new StringBuilder();
                var search = await _lavaNode.SearchYouTubeAsync(query);
                int trackNum = 1;
                foreach (LavaTrack track in search.Tracks.Take(5))
                {
                    descriptionBuilder.Append($"{trackNum}: [{track.Title}]({track.Url})\n");
                    trackNum++;
                }
                var test = await EmbedHandler.CreateBasicEmbed("Music Search", $"Results: \n{descriptionBuilder}", Color.Blue);
                await Context.Channel.SendMessageAsync(null, false, test);
                var response = await NextMessageAsync();
                await Context.Channel.SendMessageAsync($"You selected: {response}");
                var test2 = search.Tracks[int.Parse(response.Content) - 1].Url;

                await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel);

                await AudioService.PlayAsync(Context.User as SocketGuildUser , Context.Guild, test2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

using BotData.Entity;
using BotData.Repository;
using CasinoBot.Common;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CasinoBot.Services
{
    public class MessageProcessService
    {
        private readonly ITelegramBotClient botClient;
        private readonly Update update;
        public MessageProcessService(ITelegramBotClient botClient, Update update)
        {
            this.botClient = botClient;
            this.update = update;
        }

        public async Task ProcessMessage(CancellationToken cancellationToken)
        {
            if (update.Message is not { } message) return;

            if (message.Text is not { } messageText) return;

            var chatId = message.Chat.Id;

            using var context = new BotContext();

            if (await context.GetUser(message.Chat.Id) is not { } user)
            {
                user = new BotUser() { Id = message.Chat.Id, Score = 100 };
                await context.AddUser(user);
            }

            switch (message.Text)
            {
                case "Да": await SendBetKeyboard(chatId, user); break;
                case "/start": await SendWelkomeKeyboard(chatId, user); break;

                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10": await SendResult(chatId, user, context); break;

                case "Ставка: 5":
                case "Ставка: 10":
                case "Ставка: 20":
                case "Ставка: 50":
                case "Ставка: 100":
                case "Ставка: 200":
                case "Ставка: 500":
                case "Ставка: 1000":
                case "Ставка: 2000":
                case "Ставка: 5000":
                case "Ставка: 10000":
                case "Ставка: 20000": await SendValuseKeyboard(chatId, user, context, int.Parse(message.Text.Remove(0, 8))); break;
                default: await SendWrongMessage(chatId, user); break;
            };
        }

        private async Task<Message> SendBetKeyboard(long chatId, BotUser user)
        {
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: Messages.ScoreAndChooseBet(user.Score),
                replyMarkup: ReplyKeyboards.Bet());
        }

        private async Task<Message> SendWelkomeKeyboard(long chatId, BotUser user)
        {
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: Messages.Welcome,
                replyMarkup: ReplyKeyboards.Yes());
        }

        private async Task<Message> SendWrongMessage(long chatId, BotUser user)
        {
            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: Messages.WrongCommand);
        }

        private async Task<Message> SendValuseKeyboard(long chatId, BotUser user, BotContext context, int bet)
        {
            await context.UpdateBet(chatId, bet);

            if (bet <= user.Score)
            {
                return await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Messages.PickNumber,
                    replyMarkup: ReplyKeyboards.PickNumber());
            }
            else
            {
                return await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: Messages.NotEnoughMoney(user.Score),
                    replyMarkup: ReplyKeyboards.Bet());
            }
        }

        private async Task<Message> SendResult(long chatId, BotUser user, BotContext context)
        {
            Random rnd = new Random();
            if (user.LastBet == null)
                return await SendWelkomeKeyboard(chatId, user);

            int value = (rnd.Next() % 5) + 1;

            var text = "";

            if (value == 1)
            {
                text += "\n" + Messages.YouWin;

                await context.UpdateScore(user.Id, user.LastBet.Value * 10);

            }
            else
            {
                text += "\n" + Messages.YouLoose;

                await context.UpdateScore(user.Id, user.LastBet.Value * -1);
            }

            await context.UpdateBet(user.Id, null);

            if (user.Score == 0)
            {
                text += "\n" + Messages.HaveNoMoney;

                await context.UpdateScore(user.Id, 100);
            }

            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text + "\n" + Messages.ScoreAndPlayAgain(user.Score),
                replyMarkup: ReplyKeyboards.Yes());
        }
    }
}

using BotData.Entity;
using BotData.Repository;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("5673316704:AAFdXayDtsncq_431DwIimr-Cvu0B4hEY_M");

using var cts = new CancellationTokenSource();

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
        case "10": await SendResultKeyboard(chatId, user, context); break;

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
        default:
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Неверная команда, напишите /start для того чтобы попробовать снова",
                cancellationToken: cancellationToken);
            break;
    };
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

async Task<Message> SendBetKeyboard(long chatId, BotUser user)
{
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
   {
        new KeyboardButton[] { "Ставка: 5", "Ставка: 10", "Ставка: 20"},
        new KeyboardButton[] { "Ставка: 50","Ставка: 100","Ставка: 200"  },
        new KeyboardButton[] { "Ставка: 500","Ставка: 1000","Ставка: 2000"  },
        new KeyboardButton[] { "Ставка: 5000","Ставка: 10000","Ставка: 20000"  }
         })
    {
        ResizeKeyboard = true
    };

    return await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: $"Твой баланс: {user.Score}\nВыбери ставку",
        replyMarkup: replyKeyboardMarkup);
}

async Task<Message> SendValuseKeyboard(long chatId, BotUser user, BotContext context, int bet)
{
    await context.UpdateBet(chatId, bet);
    if (bet <= user.Score)
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
       {
        new KeyboardButton[] { "1", "2", "3" },
        new KeyboardButton[] { "4", "5", "6" },
        new KeyboardButton[] { "7", "8", "9" },
        new KeyboardButton[] { "10" }
         })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"Выбери число от 1 до 10",
            replyMarkup: replyKeyboardMarkup);
    }
    else
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
       {
        new KeyboardButton[] { "Ставка: 5", "Ставка: 10", "Ставка: 20"},
        new KeyboardButton[] { "Ставка: 50","Ставка: 100","Ставка: 200"  },
        new KeyboardButton[] { "Ставка: 500","Ставка: 1000","Ставка: 2000"  },
        new KeyboardButton[] { "Ставка: 5000","Ставка: 10000","Ставка: 20000"  }
         })
        {
            ResizeKeyboard = true
        };

        return await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"У тебя не хватает денег.\nТвой баланс: {user.Score}\nВыбери ставку",
            replyMarkup: replyKeyboardMarkup);
    }
}

async Task<Message> SendResultKeyboard(long chatId, BotUser user, BotContext context)
{
    Random rnd = new Random();
    if (user.LastBet == null)
    {
        return await SendWelkomeKeyboard(chatId, user);
    }

    int value = (rnd.Next() % 10)+1;

    var text = "Вы ";

    if (value == 1)
    {
        text += "выиграли. ";
        await context.UpdateScore(user.Id, user.LastBet.Value*10);

    }
    else
    {
        text += "проиграли. ";
        await context.UpdateScore(user.Id, user.LastBet.Value * -1);
    }

    await context.UpdateBet(user.Id, null);

    if (user.Score == 0)
    {
        text += "\nУ вас закончились деньги.\n\nМы дали Тебе в долг 100р, как поднимешь бабок - отдашь";

        await context.UpdateScore(user.Id, 100);
    }

    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
   {
        new KeyboardButton[] { "Да" }
         })
    {
        ResizeKeyboard = true
    };

    return await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: text + $"\nТвой баланс: {user.Score}\nСыграть еще раз?",
        replyMarkup: replyKeyboardMarkup);
}

async Task<Message> SendWelkomeKeyboard(long chatId, BotUser user)
{
    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
   {
        new KeyboardButton[] { "Да" }
         })
    {
        ResizeKeyboard = true
    };

    return await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: $"Добро пожаловать в игру.\n\nСуть проста - сделай ставку, и угадай число от одного до 10.\nЕсли угадаешь то твой выигрышь увеличится в 10 раз.\n\nСыграем?",
        replyMarkup: replyKeyboardMarkup);
}
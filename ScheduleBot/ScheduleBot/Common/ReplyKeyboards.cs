using Telegram.Bot.Types.ReplyMarkups;

namespace CasinoBot.Common
{
    public static class ReplyKeyboards
    {
        public static ReplyKeyboardMarkup Yes()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Да" }
            })
            {
                ResizeKeyboard = true
            };
        }
        
        public static ReplyKeyboardMarkup Bet()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Ставка: 5", "Ставка: 10", "Ставка: 20"},
                new KeyboardButton[] { "Ставка: 50","Ставка: 100","Ставка: 200"  },
                new KeyboardButton[] { "Ставка: 500","Ставка: 1000","Ставка: 2000"  },
                new KeyboardButton[] { "Ставка: 5000","Ставка: 10000","Ставка: 20000"  }
            })
            {
                ResizeKeyboard = true
            };
        }

        public static ReplyKeyboardMarkup PickNumber()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "1", "2", "3" },
                new KeyboardButton[] { "4", "5", "6" },
                new KeyboardButton[] { "7", "8", "9" },
                new KeyboardButton[] { "10" }
            })
            {
                ResizeKeyboard = true
            };
        }
    }
}

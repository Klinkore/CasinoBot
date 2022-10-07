namespace CasinoBot.Common
{
    public static class Messages
    {        
        public static readonly string Welcome = $"Добро пожаловать в игру.\n\nСуть проста - сделай ставку, и угадай число от одного до 10.\nЕсли угадаешь то твой выигрышь увеличится в 10 раз.\n\nСыграем?\n";
        public static readonly string HaveNoMoney = $"У вас закончились деньги.\n\nМы дали Тебе в долг 100р, как поднимешь бабок - отдашь\n";
        public static readonly string YouLoose = $"Вы проиграли\n";
        public static readonly string YouWin = $"Вы выиграли\n";
        public static readonly string PickNumber = $"Выбери число от 1 до 10\n";
        public static readonly string WrongCommand = "Неверная команда, напишите /start для того чтобы попробовать снова\n";
        
        public static string ScoreAndPlayAgain(int score)
        {
            return $"Твой баланс: {score}\nСыграть еще раз?";
        }

        public static string ScoreAndChooseBet(int score)
        {
            return $"Твой баланс: {score}\nВыбери ставку";
        }

        public static string NotEnoughMoney(int score)
        {
            return $"У тебя не хватает денег.\n\nТвой баланс: {score}\nВыбери ставку";
        }
        
    }
}

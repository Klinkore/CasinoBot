using BotData.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotData.Repository
{
    public static class UserReposotory
    {
        public static async Task<long> AddUser(this BotContext context, BotUser user)
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync();

            return user.Id;
        }

        public static async Task<BotUser?> GetUser(this BotContext context, long userId)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public static async Task<int> UpdateScore(this BotContext context, long userId, int point)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) throw new Exception("Пользователь не найден");

            user.Score += point;
            context.SaveChanges();

            return user.Score;
        }

        public static async Task<int?> UpdateBet(this BotContext context, long userId, int? bet)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) throw new Exception("Пользователь не найден");

            user.LastBet = bet;
            context.SaveChanges();

            return user.LastBet;
        }
    }
}

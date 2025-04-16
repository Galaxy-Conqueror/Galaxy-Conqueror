using Battle;
using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.GameModels;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Handlers
{
    public static class BattleService
    {
        public static async Task<BattleResponse?> GetBattleAsync(int planetId)
        {
            try
            {
                var battleResponse = await RequestHelper.GetRequestAsync($"/api/battle/{planetId}");

                var data = await battleResponse.Content.ReadFromJsonAsync<BattleResponse>();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch battle info: {ex.Message}", ex);
            }
        }

        public static async Task<BattleResultResponse?> LogBattleAsync(int planetId, BattleResult battleResult)
        {
            try
            {
                var requestBody = new { 
                    battleResult.StartedAt,
                    battleResult.EndedAt,
                    battleResult.DamageToSpaceship,
                    battleResult.DamageToTurret,
                    battleResult.ResourcesLooted,
                };

                var response = await RequestHelper.PostRequestAsync($"/api/battle/{planetId}/log", "", JsonSerializer.Serialize(requestBody));

                var data = await response.Content.ReadFromJsonAsync<BattleResultResponse>();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to log battle: {ex.Message}", ex);
            }
        }


    }
}

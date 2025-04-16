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
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Handlers
{
    public static class ApiService
    {
        public static async Task<IEnumerable<Planet>> GetPlanetsAsync()
        {
            try
            {
                var planetResponse = await RequestHelper.GetRequestAsync("/api/planets");

                var content = await planetResponse.Content.ReadFromJsonAsync<IEnumerable<Planet>>();

                return content ?? [];
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get operation: {ex.Message}", ex);
            }
        }

        public static async Task<Planet> GetPlanetAsync()
        {
            try
            {
                var planetResponse = await RequestHelper.GetRequestAsync("/api/planet");

                var content = await planetResponse.Content.ReadFromJsonAsync<Planet>();

                return content;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get operation: {ex.Message}", ex);
            }
        }

        public static async Task<Planet> GetPlanetByIdAsync(int planetId)
        {
            try
            {
                var planetResponse = await RequestHelper.GetRequestAsync($"/api/planet/{planetId - 999}");

                var content = await planetResponse.Content.ReadFromJsonAsync<Planet>();

                return content;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get operation: {ex.Message}", ex);
            }
        }

        public static async Task<Spaceship> GetSpaceshipAsync()
        {
            try
            {
                var spaceshipResponse = await RequestHelper.GetRequestAsync("/api/spaceship");

                var spaceship = await spaceshipResponse.Content.ReadFromJsonAsync<Spaceship>();

                return spaceship;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the get spaceship operation: {ex.Message}", ex);
            }
        }

        public static async Task<ResourceExtractorUpgradedResponse> UpgradeResourceExtractorAsync()
        {
            try
            {
                var extractorUpgradeResponse = await RequestHelper.PutRequestAsync("/api/extractor/upgrade");

                var extractorUpgrade = await extractorUpgradeResponse.Content.ReadFromJsonAsync<ResourceExtractorUpgradedResponse>();

                return extractorUpgrade;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the extractor upgrade operation: {ex.Message}", ex);
            }
        }

        public static async Task<TurretUpgradedResponse> UpgradeTurretAsync()
        {
            try
            {
                var turretUpgradeResponse = await RequestHelper.PutRequestAsync("/api/turret/upgrade");

                var turretUpgrade = await turretUpgradeResponse.Content.ReadFromJsonAsync<TurretUpgradedResponse>();

                return turretUpgrade;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the turret upgrade operation: {ex.Message}", ex);
            }
        }

        public static async Task<SpaceshipRefuelResponse> RefuelSpaceshipAsync()
        {
            try
            {
                var response = await RequestHelper.PutRequestAsync("/api/spaceship/refuel");

                var data = await response.Content.ReadFromJsonAsync<SpaceshipRefuelResponse>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }

        public static async Task<SpaceshipUpgradedResponse> UpgradeSpaceshipAsync()
        {
            try
            {
                var response = await RequestHelper.PutRequestAsync("/api/spaceship/upgrade");

                var data = await response.Content.ReadFromJsonAsync<SpaceshipUpgradedResponse>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }

        public static async Task<SpaceshipRepairResponse> RepairSpaceshipAsync()
        {
            try
            {
                var response = await RequestHelper.PutRequestAsync("/api/spaceship/repair");

                var data = await response.Content.ReadFromJsonAsync<SpaceshipRepairResponse>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }

        public static async Task<Planet> DepositAsync()
        {
            try
            {
                var response = await RequestHelper.PutRequestAsync("/api/spaceship/deposit");

                var data = await response.Content.ReadFromJsonAsync<Planet>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }

        public static async Task<ResourceExtractorDetailResponse> GetOwnExtractor()
        {
            try
            {
                var response = await RequestHelper.GetRequestAsync("/api/extractor");

                var data = await response.Content.ReadFromJsonAsync<ResourceExtractorDetailResponse>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }

        public static async Task<TurretDetailResponse> GetOwnTurret()
        {
            try
            {
                var response = await RequestHelper.GetRequestAsync("/api/turret");

                var data = await response.Content.ReadFromJsonAsync<TurretDetailResponse>();

                return data;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unknown error occurred during the operation: {ex.Message}", ex);
            }
        }
    }
}

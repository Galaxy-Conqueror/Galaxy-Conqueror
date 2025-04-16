﻿using Battle;
using Galaxy.Conqueror.Client.Handlers;
using Galaxy.Conqueror.Client.Managers;
using Galaxy.Conqueror.Client.Models.Menu;
using Galaxy.Conqueror.Client.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models.GameModels
{
    public class Planet : Entity
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Design { get; set; }
        public string? Description { get; set; }
        public int ResourceReserve { get; set; }

        public int X { get; set; }

        public int Y { get; set; }


        public Planet(int id, Guid userId, string name, Glyph glyph, Vector2I position, string description, int resourceReserve) : base(id, name, glyph, position)
        {
            this.Id = id;
            this.UserId = userId;
            this.Name = name;
            this.Glyph = glyph;
            this.Position = position;
            this.Description = description;
            this.ResourceReserve = resourceReserve;
        }

        public async Task<List<MenuItem>> GetPlanetOperations(List<MenuItem> menuItems)
        {
            var extractor = await StateManager.UpdateExtractor();
            var turret = await StateManager.UpdateTurret();
            var isOwnPlanet = UserId == AuthHelper.UserId;

            if (isOwnPlanet)
            {
                menuItems.Add(new MenuItem($"Upgrade extractor [Cost: {extractor.UpgradeCost}]", UpgradeResourceExtractor));
                menuItems.Add(new MenuItem($"Upgrade turret [Cost: {turret.UpgradeCost}]", UpgradeTurret));
            } else
            {
                menuItems.Add(new MenuItem("Attack", AttackPlanet));
            }
            return menuItems;
        }

        public async void UpgradeResourceExtractor()
        {
            var response = await ApiService.UpgradeResourceExtractorAsync();
        }

        public async void UpgradeTurret()
        {
            var response = await ApiService.UpgradeTurretAsync();
        }

        public void TravelToPlanet()
        {
            StateManager.State = GameState.MAP_VIEW;
        }

        public void AttackPlanet()
        {
            StateManager.State = GameState.BATTLE;

        }
    }
}

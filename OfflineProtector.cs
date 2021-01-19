using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Libraries;
using System.Net;
using Oxide.Core.Configuration;
using Rust;
using Oxide.Game.Rust.Cui;
using UnityEngine;
using Oxide.Core.Plugins;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Oxide.Plugins
{
    [Info("OfflineProtector", "mayfairr#2022", 0.2)]
    [Description("Prevent offline raids using a currency based system.")]
    public class OfflineProtector : RustPlugin
    {
        [PluginReference] private Plugin Clans;
        private List<BasePlayer> refundList;
        #region GUI
        private static string GUI = @"[
          {
            ""name"": ""Main"",
            ""parent"": ""Hud"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.12 0.12 0.12 0.995""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              },
                {
                ""type"":""NeedsCursor""
                }
            ]
          },
          {
            ""name"": ""Logo"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.RawImage"",
                ""color"": ""1 1 1 1"",
                ""url"": ""https://prodigy.wtf/logo.png""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.461 0.792"",
                ""anchormax"": ""0.539 0.931""
              }
            ]
          },
          {
            ""name"": ""Header"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 24,
                ""align"": ""MiddleCenter"",
                ""text"": ""Prodigy Offline Protector""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.397 0.714"",
                ""anchormax"": ""0.602 0.753""
              }
            ]
          },
          {
            ""name"": ""Name1"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.2 0.2 0.2 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.227 0.611"",
                ""anchormax"": ""0.383 0.681""
              }
            ]
          },
          {
            ""name"": ""Name1"",
            ""parent"": ""Name1"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 26,
                ""align"": ""MiddleCenter"",
                ""text"": ""4 Hours""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.265 0.24"",
                ""anchormax"": ""0.795 0.84""
              }
            ]
          },
          {
            ""name"": ""Name2"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.2 0.2 0.2 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.422 0.611"",
                ""anchormax"": ""0.578 0.681""
              }
            ]
          },
          {
            ""name"": ""Text"",
            ""parent"": ""Name2"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 26,
                ""align"": ""MiddleCenter"",
                ""text"": "" 8 Hours""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.265 0.24"",
                ""anchormax"": ""0.795 0.84""
              }
            ]
          },
          {
            ""name"": ""Name3"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.2 0.2 0.2 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.617 0.611"",
                ""anchormax"": ""0.773 0.681""
              }
            ]
          },
          {
            ""name"": ""Text"",
            ""parent"": ""Name3"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 26,
                ""align"": ""MiddleCenter"",
                ""text"": ""16 Hours""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.265 0.24"",
                ""anchormax"": ""0.795 0.84""
              }
            ]
          },
          {
            ""name"": ""Cooldown1"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.24 0.24 0.24 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.227 0.444"",
                ""anchormax"": ""0.383 0.583""
              }
            ]
          },
          {
            ""name"": ""CooldownText"",
            ""parent"": ""Cooldown1"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""cooldown:""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.33 0.74"",
                ""anchormax"": ""0.665 0.9""
              }
            ]
          },
          {
            ""name"": ""CooldownValue"",
            ""parent"": ""Cooldown1"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 0.78 0 1"",
                ""fontSize"": 24,
                ""align"": ""MiddleCenter"",
                ""text"": ""{timeout4}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.275 0.34"",
                ""anchormax"": ""0.7 0.62""
              }
            ]
          },
          {
            ""name"": ""Cooldown2"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.24 0.24 0.24 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.422 0.444"",
                ""anchormax"": ""0.578 0.583""
              }
            ]
          },
          {
            ""name"": ""CooldownText"",
            ""parent"": ""Cooldown2"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""cooldown:""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.33 0.74"",
                ""anchormax"": ""0.665 0.9""
              }
            ]
          },
          {
            ""name"": ""CooldownValue"",
            ""parent"": ""Cooldown2"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 0.78 0 1"",
                ""fontSize"": 24,
                ""align"": ""MiddleCenter"",
                ""text"": ""{timeout8}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.275 0.34"",
                ""anchormax"": ""0.7 0.62""
              }
            ]
          },
          {
            ""name"": ""Cooldown3"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Image"",
                ""color"": ""0.24 0.24 0.24 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.617 0.444"",
                ""anchormax"": ""0.773 0.583""
              }
            ]
          },
          {
            ""name"": ""CooldownText"",
            ""parent"": ""Cooldown3"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""cooldown:""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.33 0.74"",
                ""anchormax"": ""0.665 0.9""
              }
            ]
          },
          {
            ""name"": ""Cooldown3"",
            ""parent"": ""Cooldown3"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 0.78 0 1"",
                ""fontSize"": 24, 
                ""align"": ""MiddleCenter"",
                ""text"": ""{timeout16}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.275 0.34"",
                ""anchormax"": ""0.7 0.62""
              }
            ]
          },
          {
            ""name"": ""Buy1"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""op.buy1"",
                ""close"": """",
                ""color"": ""0.31 0.31 0.31 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.227 0.347"",
                ""anchormax"": ""0.383 0.417""
              }
            ]
          },
          {
            ""name"": ""CurrencyIcon"",
            ""parent"": ""Buy1"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.RawImage"",
                ""color"": ""1 1 1 1"",
                ""url"": ""https://prodigy.wtf/scrap.png""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.01 0.04"",
                ""anchormax"": ""0.24 0.96""
              }
            ]
          },
          {
            ""name"": ""Buy1"",
            ""parent"": ""Buy1"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 16,
                ""align"": ""MiddleCenter"",
                ""text"": ""Buy - {price4}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          },
          {
            ""name"": ""Buy2"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""op.buy2"",
                ""close"": """",
                ""color"": ""0.31 0.31 0.31 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.422 0.347"",
                ""anchormax"": ""0.578 0.417""
              }
            ]
          },
          {
            ""name"": ""2b3c-f93f-7602"",
            ""parent"": ""Buy2"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 16,
                ""align"": ""MiddleCenter"",
                ""text"": ""Buy - {price8}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          },
          {
            ""name"": ""cf0a-4dc4-faef"",
            ""parent"": ""Buy2"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.RawImage"",
                ""color"": ""1 1 1 1"",
                ""url"": ""https://prodigy.wtf/scrap.png""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.01 0.04"",
                ""anchormax"": ""0.24 0.96""
              }
            ]
          },
          {
            ""name"": ""Buy3"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""op.buy3"",
                ""close"": """",
                ""color"": ""0.31 0.31 0.31 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.617 0.347"",
                ""anchormax"": ""0.773 0.417""
              }
            ]
          },
          {
            ""name"": ""72e9-8840-eaf8"",
            ""parent"": ""Buy3"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 16,
                ""align"": ""MiddleCenter"",
                ""text"": ""Buy - {price16}""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          },
          {
            ""name"": ""e1a9-7278-67de"",
            ""parent"": ""Buy3"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.RawImage"",
                ""color"": ""1 1 1 1"",
                ""url"": ""https://prodigy.wtf/scrap.png""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.01 0.04"",
                ""anchormax"": ""0.24 0.96""
              }
            ]
          },
          {
            ""name"": ""Close"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Button"",
                ""command"": ""gui.close"",
                ""close"": """",
                ""color"": ""0.31 0.31 0.31 1""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.461 0.236"",
                ""anchormax"": ""0.539 0.306""
              }
            ]
          },
          {
            ""name"": ""56f7-cb58-8d4b"",
            ""parent"": ""Close"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""Close""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0 0"",
                ""anchormax"": ""1 1""
              }
            ]
          },
          {
            ""name"": ""Hours"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""{balance} Hrs""
              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.855 0.936"",
                ""anchormax"": ""0.966 0.958""
              }
            ]
          },
          {
            ""name"": ""d652-1e83-a417"",
            ""parent"": ""Main"",
            ""components"": [
              {
                ""type"": ""UnityEngine.UI.Text"",
                ""color"": ""1 1 1 1"",
                ""fontSize"": 14,
                ""align"": ""MiddleCenter"",
                ""text"": ""Developed by mayfairr#2022""

              },
              {
                ""type"": ""RectTransform"",
                ""anchormin"": ""0.432 0.176"",
                ""anchormax"": ""0.569 0.199""
              }
            ]
          }
        ]";
        #endregion

        #region BoomList
        private Dictionary<string, string> raidtools = new Dictionary<string, string>
        {
            {"ammo.rocket.fire", "rocket_fire" },
            {"ammo.rocket.hv", "rocket_hv" },
            {"ammo.rocket.basic", "rocket_basic" },
            {"explosive.timed", "explosive.timed.deployed" },
            {"surveycharge", "survey_charge.deployed" },
            {"explosive.satchel", "explosive.satchel.deployed" },
            {"grenade.beancan", "grenade.beancan.deployed" },
            {"grenade.f1", "grenade.f1.deployed" },
            {"ammo.grenadelauncher.he", "40mm_grenade_he"},
            {"ammo.rifle", "riflebullet" },
            {"ammo.rifle.explosive", "riflebullet_explosive" },
            {"ammo.rifle.incendiary", "riflebullet_fire" },
            {"ammo.pistol", "pistolbullet" },
            {"ammo.pistol.fire", "pistolbullet_fire" },
            {"ammo.shotgun", "shotgunbullet" },
            {"ammo.shotgun.fire", "shotgunbullet_fire" },
            {"ammo.shotgun.slug", "shotgunslug" }
        };
        #endregion

        #region Config
        Configuration config;

        class Configuration
        {
            //Price per category
            public int Price_4 = 2100;
            public int Price_8 = 4100;
            public int Price_16 = 9100;

            //Cooldown In hours
            public int cooldown_4 = 3;
            public int cooldown_8 = 4;
            public int cooldown_16 = 8;

            public float StartingBalance = 4; //In hours
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                config = Config.ReadObject<Configuration>();
                Puts("Config Loaded Succesfully");
                if (config == null) throw new Exception();
            }
            catch
            {
                Config.WriteObject(config, false, $"{Interface.Oxide.ConfigDirectory}/{Name}.jsonError");
                PrintError("The configuration file contains an error and has been replaced with a default config.\n" +
                           "The error configuration file was saved in the .jsonError extension");
                LoadDefaultConfig();
            }

            SaveConfig();
        }

        protected override void LoadDefaultConfig() => config = new Configuration();

        protected override void SaveConfig() => Config.WriteObject(config);
        #endregion

        #region Clans
        private void OnClanCreate(string tag)
        {
            CreateClanData(tag);
        }
        private void OnClanUpdate(string tag)
        {
            globalInfo[tag].members_online = anyMemberOnline(tag);
        }
        private IEnumerable<ulong> GetClanMembers(string clanName)
        {
            var clan = Clans.Call("GetClan", clanName) as JObject;
            var members = clan?.GetValue("members") as JArray;
            return members?.Select(x => ulong.Parse(x.ToString()));
        }
        private IEnumerable<ulong> GetClanMembers(ulong playerID)
        {
            if (Clans == null) return null;
            //Clans Reborn
            var members = Clans.Call("GetClanMembers", playerID);
            if (members != null) return (members as List<string>)?.Select(ulong.Parse);
            //Clans
            var clanName = Clans.Call("GetClanOf", playerID) as string;
            return clanName != null ? GetClanMembers(clanName) : null;
        }
        private string getClan(ulong uid)
        {
            var clanName = Clans.Call("GetClanOf", uid).ToString();
            return clanName != null ? clanName : null;
        }
        private int anyMemberOnline(string tag)
        {
            int count = 0;
            var members = GetClanMembers(tag);
            foreach (var member in members)
            {
                IPlayer player = this.covalence.Players.FindPlayer(member.ToString());
                if (player.IsConnected)
                {
                    count++;
                }
            }
            return count;
        }
        private void OnClanDestroy(string tag)
        {
            globalInfo.Remove(tag);
            SaveData();
        }
        #endregion

        #region CreateDataFile
        private DataFile dataFile;
        private Dictionary<string, info> globalInfo => dataFile.GlobalInfo;

        private class info
        {
            public float Balance;
            public float Cooldown_4;
            public float Cooldown_8;
            public float Cooldown_16;
            public int members_online;

            public string sinceLoggedOff;
            public bool loggedOn;
        }

        private class DataFile
        {
            public Dictionary<string, info> GlobalInfo { get; private set; } = new Dictionary<string, info>();
        }
        private void SaveData() => Interface.Oxide.DataFileSystem.WriteObject(Name, dataFile);
        private void OnServerSave() => SaveData();
        private void Unload() => SaveData();

        private void CreateClanData(string tag)
        {
            if (!globalInfo.ContainsKey(tag))
            {
                globalInfo[tag] = new info
                {
                    Balance = config.StartingBalance,
                    Cooldown_4 = 0,
                    Cooldown_8 = 0,
                    Cooldown_16 = 0,
                    members_online = anyMemberOnline(tag),
                    sinceLoggedOff = "",
                    loggedOn = true
                };
            }
            SaveData();
        }
        #endregion 

        #region Hooks
        private void Init()
        {
            LoadConfig();
            refundList = new List<BasePlayer>();
            DataFileSystem ds = new DataFileSystem($"{Interface.Oxide.DataDirectory}");
            dataFile = ds.ReadObject<DataFile>("OfflineProtector");
            cmd.AddChatCommand("op", this, "ShowGUI");
            cmd.AddConsoleCommand("gui.close", this, "CloseGUI");
            cmd.AddConsoleCommand("op.buy1", this, "Buy_4Hours");
            cmd.AddConsoleCommand("op.buy2", this, "Buy_8Hours");
            cmd.AddConsoleCommand("op.buy3", this, "Buy_16Hours");
            permission.RegisterPermission("OfflineProtector.use", this);
            UpdateData();
        }
        private void OnUserDisconnected(IPlayer player)
        {
            try
            {


                string clan = getClan(Convert.ToUInt64(player.Id));
                if (globalInfo[clan].members_online > 0)
                {
                    globalInfo[clan].members_online -= 1;
                }
                if (globalInfo[clan].members_online == 0)
                {
                    globalInfo[clan].sinceLoggedOff = DateTime.Now.ToString();
                }
                SaveData();
            }
            catch
            {

            }
        }
        private void OnUserConnected(IPlayer player)
        {
            try
            {


                string clan = getClan(Convert.ToUInt64(player.Id));
                if (globalInfo[clan].members_online < 3)
                {
                    globalInfo[clan].members_online += 1;
                }
                SaveData();
            }
            catch
            {

            }
        }
        #endregion

        #region GUIHooks 
        private void ShowGUI(BasePlayer player, string par, string[] args)
        {
             try
            {
                string clan = getClan(player.userID);
                var newGUI = GUI;
                newGUI = newGUI.Replace("{price4}", config.Price_4.ToString());
                newGUI = newGUI.Replace("{price8}", config.Price_8.ToString());
                newGUI = newGUI.Replace("{price16}", config.Price_16.ToString());

                newGUI = newGUI.Replace("{timeout4}", globalInfo[clan].Cooldown_4.ToString("0.0") != "0.0" ? globalInfo[clan].Cooldown_4.ToString("0.0") + " Hrs" : "None");
                newGUI = newGUI.Replace("{timeout8}", globalInfo[clan].Cooldown_8.ToString("0.0") != "0.0" ? globalInfo[clan].Cooldown_8.ToString("0.0") + " Hrs" : "None");
                newGUI = newGUI.Replace("{timeout16}", globalInfo[clan].Cooldown_16.ToString("0.0") != "0.0" ? globalInfo[clan].Cooldown_16.ToString("0.0") + " Hrs" : "None");

                newGUI = newGUI.Replace("{balance}", "Balance: " + globalInfo[clan].Balance.ToString("0.0"));

                CuiHelper.DestroyUi(player, "main");
                CuiHelper.AddUi(player, newGUI);
            }
            catch
            {
                SendReply(player, "Please use <color=orange>/clan</color> to create a clan first.");
            }
        }
        private void CloseGUI(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();
            CuiHelper.DestroyUi(player, "Main");
        }
        #endregion  

        #region onDamage
        private object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo hitinfo)
        {
            try
            {
                if (hitinfo == null || entity == null || entity.OwnerID == hitinfo?.InitiatorPlayer?.userID)
                    return null;
                if (!(entity is BuildingBlock || entity is Door || entity.PrefabName.Contains("deployable")))
                    return null;

                BasePlayer attacker = hitinfo.InitiatorPlayer;
                string name = null;

                if (hitinfo?.WeaponPrefab?.ShortPrefabName == "rocket_fire"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "rocket_fire"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "rocket_hv"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "rocket_basic"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "explosive.timed.deployed"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "survey_charge.deployed"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "explosive.satchel.deployed"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "grenade.beancan.deployed"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "grenade.f1.deployed"
                || hitinfo?.WeaponPrefab?.ShortPrefabName == "40mm_grenade_he")
                {
                    name = hitinfo?.WeaponPrefab?.ShortPrefabName;
                }
                else
                {
                    name = hitinfo?.ProjectilePrefab?.name.ToString();
                }
                string clan = getClan(entity.OwnerID);
                try
                {
                    if (getClan(attacker.userID) == getClan(entity.OwnerID)) { return null; }
                }
                catch { }
                if (globalInfo[clan].members_online != 0) { return null; }
                if (globalInfo.ContainsKey(clan))
                {
                    if (refundList.Contains(attacker))
                    {
                        refundCooldownOver(attacker);
                        hitinfo.damageTypes = new DamageTypeList();
                        hitinfo.DoHitEffects = false;
                        hitinfo.HitMaterial = 0;
                        return true;

                    };

                    DateTime canRaidTime = Convert.ToDateTime(globalInfo[clan].sinceLoggedOff);
                    canRaidTime = canRaidTime.AddHours(globalInfo[clan].Balance + 1);
                    if (DateTime.Now < Convert.ToDateTime(globalInfo[clan].sinceLoggedOff).AddHours(1)) { return null; }
                    if (DateTime.Now < canRaidTime)
                    {
                        SendReply(attacker, "This player has purchased raid protection and has <color=orange>" + globalInfo[clan].Balance.ToString("0.0") + "</color> hours. You can either raid them when they come online, or once this timer has completed.");
                        refundPlayer(attacker, name, entity);
                        hitinfo.damageTypes = new DamageTypeList();
                        hitinfo.DoHitEffects = false;
                        hitinfo.HitMaterial = 0;
                        refundList.Add(attacker);

                        refundCooldownOver(attacker);
                        return true;
                    }
                    return null;

                }
                return null;
            }
            catch
            {
                return true;
            }
        }

        private void refundPlayer(BasePlayer player, string name, BaseEntity ent)
        {
            foreach (var entry in raidtools)
            {
                if (name == entry.Value)
                {
                    Item item = ItemManager.CreateByName(entry.Key, 1);
                    player.GiveItem(item);
                    SendReply(player, "You have been refunded" + item.name);
                }
            }
        }
        private void refundCooldownOver(BasePlayer player)
        {
            if (player == null) return;

            timer.In(0.1f, () =>
            {
                if (refundList.Contains(player)) refundList.Remove(player);
            });
        }
        #endregion

        #region GUICommands
        private void Buy_4Hours(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();

            if (!(permission.UserHasPermission(player.userID.ToString(), "OfflineProtector.use"))) return;

            string clan = getClan(player.userID);
            var items = player?.inventory?.AllItems();
            if (globalInfo[clan].Cooldown_4.ToString("0.0") != "0.0")
            {
                SendReply(player, $"You or a clan member need to stay <color=orange>Online</color> for <color=orange>{globalInfo[clan].Cooldown_4.ToString("0.0")} more hours.");
                CloseGUI(arg);
                return;
            }

            if (globalInfo[clan].Balance < 16)
            {
                if (player?.inventory?.GetAmount(-932201673) >= config.Price_4)
                {
                    player.inventory.Take(null, -932201673, config.Price_4);
                    globalInfo[clan].Balance += 4;
                    globalInfo[clan].Cooldown_4 += 4;

                    //Partial Refund
                    if (globalInfo[clan].Balance > 16)
                    {
                        SendReply(player, "A partial refund has been recived because you have reached the maximum balance.");
                        var refundAmount = (int)(((globalInfo[clan].Balance - 16) / 16) * config.Price_8);
                        Item item = ItemManager.CreateByName("scrap", refundAmount);
                        player.GiveItem(item);
                        globalInfo[clan].Balance = 16;
                    }
                    SaveData();
                    SendReply(player, "You have just purchased<color=orange> 4 </color>more hours of offline raid protection.\n <color=orange>--------------------------------</color> \nNew balance: <color=orange>" + globalInfo[clan].Balance.ToString("0.0") + " hours</color>\n <color=orange>--------------------------------</color> \n");
                    CloseGUI(arg);
                    return;
                }

                SendReply(player, "You do not have enough scrap");
                CloseGUI(arg);
                return;
            }
            CloseGUI(arg);
            SendReply(player, "You have reached the maximum raid protection: <color=orange>16 hours.</color>");
            return;
        }
        private void Buy_8Hours(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();

            if (!(permission.UserHasPermission(player.userID.ToString(), "OfflineProtector.use"))) return;

            string clan = getClan(player.userID);
            var items = player?.inventory?.AllItems();
            if (globalInfo[clan].Cooldown_8.ToString("0.0") != "0.0")
            {
                SendReply(player, $"You or a clan member need to stay <color=orange>Online</color> for <color=orange>{globalInfo[clan].Cooldown_8.ToString("0.0")} more hours.");
                CloseGUI(arg);
                return;
            }

            if (globalInfo[clan].Balance < 16)
            {
                if (player?.inventory?.GetAmount(-932201673) >= config.Price_8)
                {
                    player.inventory.Take(null, -932201673, config.Price_8);
                    globalInfo[clan].Balance += 8;
                    globalInfo[clan].Cooldown_8 += 8;

                    //Partial Refund
                    if (globalInfo[clan].Balance > 16)
                    {
                        SendReply(player, "A partial refund has been recived because you have reached the maximum balance.");
                        var refundAmount = (int)(((globalInfo[clan].Balance - 16) / 16) * config.Price_8);
                        Item item = ItemManager.CreateByName("scrap", refundAmount);
                        player.GiveItem(item);
                        globalInfo[clan].Balance = 16;
                    }
                    SaveData();
                    SendReply(player, "You have just purchased<color=orange> 8 </color>more hours of offline raid protection.\n <color=orange>--------------------------------</color> \nNew balance: <color=orange>" + globalInfo[clan].Balance.ToString("0.0") + " hours</color>\n <color=orange>--------------------------------</color> \n");
                    CloseGUI(arg);
                    return;
                }

                SendReply(player, "You do not have enough scrap");
                CloseGUI(arg);
                return;
            }
            CloseGUI(arg);
            SendReply(player, "You have reached the maximum raid protection: <color=orange>16 hours.</color>");
            return;
        }
        private void Buy_16Hours(ConsoleSystem.Arg arg)
        {
            var player = arg?.Player();

            if (!(permission.UserHasPermission(player.userID.ToString(), "OfflineProtector.use"))) return;

            string clan = getClan(player.userID);
            var items = player?.inventory?.AllItems();
            if (globalInfo[clan].Cooldown_16.ToString("0.0") != "0.0")
            {
                SendReply(player, $"You or a clan member need to stay <color=orange>Online</color> for <color=orange>{globalInfo[clan].Cooldown_16.ToString("0.0")} more hours.");
                CloseGUI(arg);
                return;
            }

            if (globalInfo[clan].Balance < 16)
            {
                if (player?.inventory?.GetAmount(-932201673) >= config.Price_16)
                {
                    player.inventory.Take(null, -932201673, config.Price_16);
                    globalInfo[clan].Balance += 16;
                    globalInfo[clan].Cooldown_16 += 16;

                    //Partial Refund
                    if (globalInfo[clan].Balance > 16)
                    {
                        SendReply(player, "A partial refund has been recived because you have reached the maximum balance.");
                        var refundAmount = (int)(((globalInfo[clan].Balance - 16) / 16) * config.Price_16);
                        Item item = ItemManager.CreateByName("scrap", refundAmount);
                        player.GiveItem(item);
                        globalInfo[clan].Balance = 16;
                    }
                    SaveData();
                    SendReply(player, "You have just purchased<color=orange> 16 </color>more hours of offline raid protection.\n <color=orange>--------------------------------</color> \nNew balance: <color=orange>" + globalInfo[clan].Balance.ToString("0.0") + " hours</color>\n <color=orange>--------------------------------</color> \n");
                    CloseGUI(arg);
                    return;
                }

                SendReply(player, "You do not have enough scrap");
                CloseGUI(arg);
                return;
            }
            CloseGUI(arg);
            SendReply(player, "You have reached the maximum raid protection: <color=orange>16 hours.</color>");
            return;
        }
        #endregion

        #region Timer
        private void UpdateData()
        {
            timer.Every(60f, () =>
            {
                foreach (var clan in globalInfo)
                {
                    //If a clan member is not online begin raid protection  
                    if (globalInfo[clan.Key].members_online == 0)
                    {
                        globalInfo[clan.Key].Balance = ((globalInfo[clan.Key].Balance * 60 - 1) / 60);
                        if (globalInfo[clan.Key].Balance < 0) globalInfo[clan.Key].Balance = 0;
                    }
                    //Otherwise cooldown
                    else
                    {
                        if (globalInfo[clan.Key].Cooldown_4 >= 0)
                        {
                            globalInfo[clan.Key].Cooldown_4 = ((globalInfo[clan.Key].Cooldown_4 * 60 - 1) / 60);
                            if (globalInfo[clan.Key].Cooldown_4 < 0) globalInfo[clan.Key].Cooldown_4 = 0;
                        }
                        if (globalInfo[clan.Key].Cooldown_8 >= 0)
                        {
                            globalInfo[clan.Key].Cooldown_8 = ((globalInfo[clan.Key].Cooldown_8 * 60 - 1) / 60);
                            if (globalInfo[clan.Key].Cooldown_8 < 0) globalInfo[clan.Key].Cooldown_8 = 0;
                        }
                        if (globalInfo[clan.Key].Cooldown_16 >= 0)
                        {
                            globalInfo[clan.Key].Cooldown_16 = ((globalInfo[clan.Key].Cooldown_16 * 60 - 1) / 60);
                            if (globalInfo[clan.Key].Cooldown_16 < 0) globalInfo[clan.Key].Cooldown_16 = 0;
                        }
                    }
                }

            });
        }

        #endregion
    }
}

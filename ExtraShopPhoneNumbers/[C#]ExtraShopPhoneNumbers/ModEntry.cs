using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using static System.Net.Mime.MediaTypeNames;

namespace ExtraShopPhoneNumbers.ExtraShopPhoneNumbers
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        public static IMonitor ModMonitor { get; private set; } = null!;

        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Phone.PhoneHandlers.Add(new ESPNPhoneHandler());
        }
    }

    /// <summary>A custom phone handler.</summary>
    internal sealed class ESPNPhoneHandler : IPhoneHandler
    {

        public IEnumerable<KeyValuePair<string, string>> GetOutgoingNumbers()
        {
            List<KeyValuePair<string, string>> numbers = new List<KeyValuePair<string, string>>
            { };
            if (true/*Game1.player.mailReceived.Contains("ESPN_Annuary")*/)
            {
                /// <summary>An outgoing call to the Traveling Cart.</summary>
                numbers.Add(new KeyValuePair<string, string>("Traveler", Game1.content.LoadString("Strings\\Characters:ESPN.TravelingCart")));
                /// <summary>An outgoing call to Krobus.</summary>
                numbers.Add(new KeyValuePair<string, string>("Krobus", Game1.content.LoadString("Strings\\Characters:ESPN.Krobus")));
                /// <summary>An outgoing call to Sandy.</summary>
                numbers.Add(new KeyValuePair<string, string>("Sandy", Game1.content.LoadString("Strings\\Characters:ESPN.SandysOasis")));
            }
            return numbers;
        }

        public bool TryHandleOutgoingCall(string callId)
        {
            switch (callId)
            {
                case "Traveler":
                    CallTraveler();
                    return true;
                case "Krobus":
                    CallKrobus();
                    return true;
                case "Sandy":
                    CallSandy();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary> Handle a call to the Traveling Cart</summary>
        public void CallTraveler()
        {
            GameLocation location = Game1.currentLocation;
            location.playShopPhoneNumberSounds("Traveler");
            Game1.player.freezePause = 4950;
            DelayedAction.functionAfterDelay(delegate
            {
                Game1.playSound("bigSelect");
                NPC characterFromName = new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16, 16), Vector2.Zero, "", 0, Game1.content.LoadString("Strings\\Characters:ESPN.TravelingCartMerchantName"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"), eventActor: false);
                if (Game1.dayOfMonth is 8 && Game1.season is Season.Winter) //Festival of Ice?
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_FoI");
                }
                else if (Game1.dayOfMonth is 15 or 16 or 17 && Game1.season is Season.Winter) //Night Market?
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_NM");
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        List<Response> list = new List<Response>
                        {
                            new Response("Traveler_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                            new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                        };
                        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                        void handleAnswer(Farmer who, string whichAnswer)
                        {
                            if (whichAnswer == "Traveler_ShopStock")
                            {
                                if (Utility.TryOpenShopMenu("Traveler", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                                {
                                    menu.readOnly = true;
                                    Phone.HangUp(); //Not sure this is necessary... Trying to replicate a fancied down version of the code below, *just in case* it's actually necessary for a reason or another.
                                    /*menu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(menu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                                    {
                                        answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>());
                                    });*/
                                }
                            }

                        }
                    });
                }
                else if (Game1.dayOfMonth is 15 or 16 or 17 && Game1.season is Season.Spring) //Desert Festival?
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_DF");
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        List<Response> list = new List<Response>
                        {
                            new Response("Traveler_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                            new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                        };
                        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                        void handleAnswer(Farmer who, string whichAnswer)
                        {
                            if (whichAnswer == "Traveler_ShopStock")
                            {
                                if (Utility.TryOpenShopMenu("Traveler", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                                {
                                    menu.readOnly = true;
                                    Phone.HangUp(); //Not sure this is necessary... Trying to replicate a fancied down version of the code below, *just in case* it's actually necessary for a reason or another.
                                    /*menu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(menu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                                    {
                                        answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>());
                                    });*/
                                }
                            }

                        }
                    });
                }
                else if (!(Game1.dayOfMonth % 7 % 5 == 0)) //Not a Traveling Cart day
                {
                    //Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_wrong_day");
                    Game1.DrawDialogue(new Dialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_wrong_day"));
                }
                else if (Game1.dayOfMonth % 7 % 5 == 0 && Game1.timeOfDay < 2000)  //Traveling Cart in town
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler");
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        List<Response> list = new List<Response>
                        {
                            new Response("Traveler_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                            new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                        };
                        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                        void handleAnswer(Farmer who, string whichAnswer)
                        {
                            if (whichAnswer == "Traveler_ShopStock")
                            {
                                if (Utility.TryOpenShopMenu("Traveler", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                                {
                                    menu.readOnly = true;
                                    Phone.HangUp(); //Not sure this is necessary... Trying to replicate a fancied down version of the code below, *just in case* it's actually necessary for a reason or another.
                                    /*menu.behaviorBeforeCleanup = (Action<IClickableMenu>)Delegate.Combine(menu.behaviorBeforeCleanup, (Action<IClickableMenu>)delegate
                                    {
                                        answerDialogueAction("HangUp", LegacyShims.EmptyArray<string>());
                                    });*/
                                }
                            }

                        }
                    });
                }
                else if (Game1.dayOfMonth % 7 % 5 == 0 && Game1.timeOfDay > 2000) //Traveling Cart day but past 8pm
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Traveler_too_late");
                }
                else
                {
                    //Should never happen.
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "If you're reading this, something went wrong with ESPN. Take a screenshot of your time and day and contact PrincessFelicie on NexusMods.");
                }

            }, 4950);
        }

        public void CallKrobus()
        {
            GameLocation location = Game1.currentLocation;
            location.playShopPhoneNumberSounds("Krobus");
            Game1.player.freezePause = 4950;
            DelayedAction.functionAfterDelay(delegate
            {
                Game1.playSound("bigSelect");
                NPC characterFromName = Game1.getCharacterFromName("Krobus");
                if (Game1.dayOfMonth % 7 == 5) //Friday?
                {
                    NPC Krobusrobocall = new NPC(new AnimatedSprite("Characters\\Abigail", 0, 16, 16), Vector2.Zero, "", 0, Game1.content.LoadString("Strings\\Characters:ESPN.Krobus_Friday"), Game1.temporaryContent.Load<Texture2D>("Portraits\\AnsweringMachine"), eventActor: false);
                    Game1.DrawAnsweringMachineDialogue(Krobusrobocall, "Strings\\Characters:ESPN.Phone_Krobus_Friday");
                }
                else if (GameStateQuery.CheckConditions("PLAYER_NPC_RELATIONSHIP Any Krobus Roommate")) //Krobus is roommate?
                {
                    Game1.DrawDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Krobus_onFarm");
                }
                else
                {
                    Game1.DrawDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Krobus");
                }
                Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                {
                    List<Response> list = new List<Response>
                            {
                                new Response("Krobus_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                                new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                            };
                    location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                    void handleAnswer(Farmer who, string whichAnswer)
                    {
                        if (whichAnswer == "Krobus_ShopStock")
                        {
                            if (Utility.TryOpenShopMenu("ShadowShop", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                            {
                                menu.readOnly = true;
                                menu.potraitPersonDialogue = null;
                                Phone.HangUp();
                            }
                        }

                    }
                });
            }, 4950);
        }

        public void CallSandy()
        {
            GameLocation location = Game1.currentLocation;
            location.playShopPhoneNumberSounds("SandysOasis");
            Game1.player.freezePause = 4950;
            DelayedAction.functionAfterDelay(delegate
            {
                Game1.playSound("bigSelect");
                NPC characterFromName = Game1.getCharacterFromName("Sandy");
                if ((Game1.dayOfMonth is 15 or 16 or 17) && Game1.season == Season.Spring) //Desert Festival? Can't check shop.
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Sandy_DF");
                }
                else if (Game1.timeOfDay > 2350) //too late? Can't check shop.
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Sandy_toolate");
                }
                else if (Game1.timeOfDay < 900) //too early? Can still check shop.
                {
                    Game1.DrawAnsweringMachineDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Sandy_tooearly");
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        List<Response> list = new List<Response>
                        {
                            new Response("Sandy_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                            new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                        };
                        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                        void handleAnswer(Farmer who, string whichAnswer)
                        {
                            if (whichAnswer == "Sandy_ShopStock")
                            {
                                if (Utility.TryOpenShopMenu("Sandy", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                                {
                                    menu.readOnly = true;
                                    Phone.HangUp();
                                }
                            }

                        }
                    });
                }
                else //Can check shop.
                {
                    Game1.DrawDialogue(characterFromName, "Strings\\Characters:ESPN.Phone_Sandy");
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        List<Response> list = new List<Response>
                        {
                            new Response("Sandy_ShopStock", Game1.content.LoadString("Strings\\Characters:Phone_CheckSeedStock")),
                            new Response("HangUp", Game1.content.LoadString("Strings\\Characters:Phone_HangUp"))
                        };
                        location.createQuestionDialogue(Game1.content.LoadString("Strings\\Characters:Phone_SelectOption"), list.ToArray(), handleAnswer);

                        void handleAnswer(Farmer who, string whichAnswer)
                        {
                            if (whichAnswer == "Sandy_ShopStock")
                            {
                                if (Utility.TryOpenShopMenu("Sandy", null, playOpenSound: true) && Game1.activeClickableMenu is ShopMenu menu)
                                {
                                    menu.readOnly = true;
                                    Phone.HangUp();
                                }
                            }

                        }
                    });
                }

            }, 4950);
        }
        public string? CheckForIncomingCall(Random random)
        {
            return null;
        }

        public bool TryHandleIncomingCall(string callId, out Action showDialogue)
        {
            showDialogue = null;
            return false;
        }
    }



}
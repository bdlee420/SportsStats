using SportsStats.Common;
using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.DTOObjects;
using SportsStats.Models.ServiceObjects;
using System.Collections.Generic;
using System.Linq;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Services
{
	public class BaseballService
	{
		private static readonly BaseballService _service = new BaseballService();

		public static BaseballService GetInstance()
		{
			if (_service == null)
			{
				return new BaseballService();
			}
			return _service;
		}

		public BaseballGameState GetExistingGameState(int gameID, int leagueID, DataCache dataCache = null)
		{
			var gameState = ConvertObjects.ConvertType(GameDataProvider.GetInstance().GetBaseballGameState(gameID, dataCache));
			if (gameState.Inning == 0)
			{
				gameState.Inning = 1;
				gameState.TopOfInning = true;
			}

			var players = PlayersService.GetInstance().GetPlayers(gameID: gameID, leagueID: leagueID, dataCache: dataCache);
			if (gameState.PlayerOnFirst != null)
			{
				var player = players.First(p => p.ID == gameState.PlayerOnFirst.ID);
				gameState.PlayerOnFirst.Name = player.Name;
				gameState.PlayerOnFirst.Number = player.Number;
			}
			if (gameState.PlayerOnSecond != null)
			{
				var player = players.First(p => p.ID == gameState.PlayerOnSecond.ID);
				gameState.PlayerOnSecond.Name = player.Name;
				gameState.PlayerOnSecond.Number = player.Number;
			}
			if (gameState.PlayerOnThird != null)
			{
				var player = players.First(p => p.ID == gameState.PlayerOnThird.ID);
				gameState.PlayerOnThird.Name = player.Name;
				gameState.PlayerOnThird.Number = player.Number;
			}
			return gameState;
		}

		public AtBatStates GetGameStates(PlayerGameStat stat, bool isLatestGroup, DataCache dataCache = null)
		{
			AtBatStates states = new AtBatStates();
			var players = PlayersService.GetInstance().GetPlayers(stat.TeamID, gameID: stat.GameID, dataCache: dataCache);
			var player = players.First(p => p.ID == stat.PlayerID);
			states.OriginalState = GetExistingGameState(gameID: stat.GameID, leagueID: stat.LeagueID, dataCache: dataCache);
			states.OriginalState.GameID = stat.GameID;
			if (states.OriginalState.Inning == 0)
			{
				states.OriginalState.Inning = 1;
				states.OriginalState.TopOfInning = true;
			}
			BaseballGameState newState = new BaseballGameState()
			{
				GameID = states.OriginalState.GameID,
				NumberOfOuts = states.OriginalState.NumberOfOuts,
				Inning = states.OriginalState.Inning,
				TopOfInning = states.OriginalState.TopOfInning,
				RunnersScored = new List<PlayerBase>(),
				RunnersOut = new List<PlayerBase>(),
				Team1Player = states.OriginalState.TopOfInning ? new PlayerBase() { ID = stat.PlayerID } : states.OriginalState.Team1Player,
				Team2Player = !states.OriginalState.TopOfInning ? new PlayerBase() { ID = stat.PlayerID } : states.OriginalState.Team2Player,
			};

			bool changeState = true;
			bool nextAtBat = false;
			bool potentialAdjustment = false;
			if (stat.Value == -1 || !isLatestGroup)
			{
				newState.PlayerOnFirst = states.OriginalState.PlayerOnFirst;
				newState.PlayerOnSecond = states.OriginalState.PlayerOnSecond;
				newState.PlayerOnThird = states.OriginalState.PlayerOnThird;
				changeState = false;
				potentialAdjustment = false;
			}
			else
			{
				if (isLatestGroup)
				{
					if (stat.StatTypeID == (int)CalculatedStatTypes.Single || stat.StatTypeID == (int)CalculatedStatTypes.FC)
					{
						newState.PlayerOnFirst = player;
						if (states.OriginalState.PlayerOnThird != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnThird);
						}
						if (states.OriginalState.PlayerOnSecond != null)
						{
							potentialAdjustment = true;
							newState.PlayerOnThird = states.OriginalState.PlayerOnSecond;
						}
						if (states.OriginalState.PlayerOnFirst != null)
						{
							potentialAdjustment = true;
							newState.PlayerOnSecond = states.OriginalState.PlayerOnFirst;
						}
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.Double)
					{
						newState.PlayerOnSecond = player;
						if (states.OriginalState.PlayerOnThird != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnThird);
						}
						if (states.OriginalState.PlayerOnSecond != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnSecond);
						}
						if (states.OriginalState.PlayerOnFirst != null)
						{
							potentialAdjustment = true;
							newState.PlayerOnThird = states.OriginalState.PlayerOnFirst;
						}
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.Triple)
					{
						newState.PlayerOnThird = player;
						if (states.OriginalState.PlayerOnFirst != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnFirst);
						}
						if (states.OriginalState.PlayerOnSecond != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnSecond);
						}
						if (states.OriginalState.PlayerOnThird != null)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnThird);
						}
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.HomeRun)
					{
						newState.RunnersScored.Add(player);
						potentialAdjustment = true;
						if (states.OriginalState.PlayerOnFirst != null)
						{
							newState.RunnersScored.Add(states.OriginalState.PlayerOnFirst);
						}
						if (states.OriginalState.PlayerOnSecond != null)
						{
							newState.RunnersScored.Add(states.OriginalState.PlayerOnSecond);
						}
						if (states.OriginalState.PlayerOnThird != null)
						{
							newState.RunnersScored.Add(states.OriginalState.PlayerOnThird);
						}
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.GroundOut)
					{
						newState.NumberOfOuts++;
						if (states.OriginalState.PlayerOnFirst != null)
						{
							potentialAdjustment = true;
							newState.PlayerOnSecond = states.OriginalState.PlayerOnFirst;
						}
						if (states.OriginalState.PlayerOnSecond != null)
						{
							potentialAdjustment = true;
							newState.PlayerOnThird = states.OriginalState.PlayerOnSecond;
						}
						if (states.OriginalState.PlayerOnThird != null && newState.NumberOfOuts < 3)
						{
							potentialAdjustment = true;
							newState.RunnersScored.Add(states.OriginalState.PlayerOnThird);
						}
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.FlyOut || stat.StatTypeID == (int)CalculatedStatTypes.SF)
					{
						potentialAdjustment = true;
						newState.PlayerOnFirst = states.OriginalState.PlayerOnFirst;
						newState.PlayerOnSecond = states.OriginalState.PlayerOnSecond;
						newState.PlayerOnThird = states.OriginalState.PlayerOnThird;
						newState.NumberOfOuts++;
						nextAtBat = true;
					}
					else if (stat.StatTypeID == (int)CalculatedStatTypes.Strikeout)
					{
						newState.PlayerOnFirst = states.OriginalState.PlayerOnFirst;
						newState.PlayerOnSecond = states.OriginalState.PlayerOnSecond;
						newState.PlayerOnThird = states.OriginalState.PlayerOnThird;
						newState.NumberOfOuts++;
						nextAtBat = true;
					}
					else
					{
						newState.PlayerOnFirst = states.OriginalState.PlayerOnFirst;
						newState.PlayerOnSecond = states.OriginalState.PlayerOnSecond;
						newState.PlayerOnThird = states.OriginalState.PlayerOnThird;
						changeState = false;
					}
				}
			}
			bool inningChange = false;
			if (newState.NumberOfOuts == 3)
			{
				inningChange = true;
			}

			//Modifying old at-bat, done mess with the state
			if (!isLatestGroup)
			{
				inningChange = false;
				nextAtBat = false;
				potentialAdjustment = false;
				changeState = false;
			}

			newState.InningChanged = inningChange;
			newState.PotentialAdjustment = potentialAdjustment;
			newState.NextAtBat = nextAtBat;
			newState.ChangeState = changeState;

			states.NewState = newState;

			return states;
		}

		public NextBatter SaveNewGameState(BaseballGameState state, bool manualAdjustment, DataCache dataCache = null)
		{
			var nextBatterResult = new NextBatter();
			if (UserHelper.HasUpdatePermissions())
			{
				var game = GamesService.GetInstance().GetGame(state.GameID, dataCache: dataCache);
				if (!manualAdjustment)
				{
					var atBatTeamID = state.TopOfInning ? game.Team1ID : game.Team2ID;
					var atBatPlayerID = state.TopOfInning ? state.Team1Player?.ID : state.Team2Player?.ID;
					var battingOrder = GameDataProvider.GetInstance().GetBattingOrder(state.GameID, atBatTeamID);
					if (battingOrder.Count == 0)
						battingOrder.Add(new DTOBattingOrder() { BattingOrder = 1 });
					var currentBatter = battingOrder.First(bo => bo.PlayerID == atBatPlayerID);
					var nextBatter = battingOrder.FirstOrDefault(bo => bo.BattingOrder == currentBatter.BattingOrder + 1);
					if (nextBatter == null)
					{
						nextBatter = battingOrder.First();
					}
					if (state.TopOfInning)
					{
						state.Team1Player = new PlayerBase() { ID = nextBatter.PlayerID };
					}
					else
					{
						state.Team2Player = new PlayerBase() { ID = nextBatter.PlayerID };
					}
					nextBatterResult.PlayerID = nextBatter.PlayerID;
					nextBatterResult.TeamID = atBatTeamID;
				}

				//Change Teams
				if (state.NumberOfOuts == 3)
				{
					state.NumberOfOuts = 0;
					state.TopOfInning = !state.TopOfInning;
					if (state.TopOfInning)
						state.Inning++;
					state.PlayerOnFirst = null;
					state.PlayerOnSecond = null;
					state.PlayerOnThird = null;
					var nextAtBatTeamID = state.TopOfInning ? game.Team1ID : game.Team2ID;

					var battingOrderNextTeam = GameDataProvider.GetInstance().GetBattingOrder(state.GameID, nextAtBatTeamID);
					nextBatterResult.TeamID = nextAtBatTeamID;

					var nextAtBatPlayer = state.TopOfInning ? state.Team1Player : state.Team2Player;
					if (nextAtBatPlayer == null)
					{
						if (battingOrderNextTeam.Any())
							nextBatterResult.PlayerID = battingOrderNextTeam.First().PlayerID;
						else
							nextBatterResult.PlayerID = null;
					}
					else
					{
						nextBatterResult.PlayerID = battingOrderNextTeam.FirstOrDefault(bo => bo.PlayerID == nextAtBatPlayer.ID).PlayerID;
					}
				}

				GameDataProvider.GetInstance().SaveBaseballGameState(ConvertObjects.ConvertType2(state));
			}
			return nextBatterResult;
		}
	}
}